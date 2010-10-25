#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ZedGraph
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' =======================================================================
    Public Class SRplot

#Region " Helper classes "

        ''' <summary>
        ''' 
        ''' </summary>
        Private Class SRData
            Public Stock As Single
            Public recrt As Single
        End Class

        ''' <summary>
        ''' 
        ''' </summary>
        Private Class SRLine

            Public StanzaGroup As cStanzaGroup
            Public GroupStart As cEcoPathGroupInput
            Public GroupEnd As cEcoPathGroupInput

            Public SRDataList As List(Of SRData)
            Public Title As String
            Public IsVisible As Boolean
            Public IsDefault As Boolean

        End Class

#End Region ' Helper classes

#Region " Private vars "

        Private m_graphpane As GraphPane = Nothing
        Private m_bEcosimRunning As Boolean = False
        Private m_coreStateMonitor As cCoreStateMonitor = Nothing
        Private m_curveSlope As CurveItem = Nothing
        Private m_mhEcosim As cMessageHandler = Nothing
        Private m_SRResults As List(Of SRLine)
        Private m_zgh As cZedGraphHelper = Nothing

#End Region ' Private vars

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Me.m_coreStateMonitor = Me.Core.StateMonitor
            Me.m_graphpane = Me.m_plot.GraphPane
            Me.m_SRResults = New List(Of SRLine)
            Me.m_zgh = New cZedGraphHelper()

            Me.LoadGroups()
            Me.m_zgh.Attach(Me.UIContext, Me.m_plot)
            Me.m_zgh.ConfigurePane(My.Resources.SR_PLOT_TITLE, _
                                   String.Format(My.Resources.SR_PLOT_X_AXIS, String.Empty), _
                                   String.Format(My.Resources.HEADER_RECRUITMENT_UNIT, String.Empty), _
                                   False)

            Dim m_SyncObj As System.Threading.SynchronizationContext = System.Threading.SynchronizationContext.Current
            'if there is no current context then create a new one on this thread. 
            If (m_SyncObj Is Nothing) Then m_SyncObj = New System.Threading.SynchronizationContext()

            ' Start listening for core messages
            Me.m_mhEcosim = New cMessageHandler(AddressOf EcosimMessageHandler, eCoreComponentType.EcoSim, eMessageType.Any, m_SyncObj)
            Me.Core.Messages.AddMessageHandler(Me.m_mhEcosim)

            AddHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcosim)
            Me.m_mhEcosim = Nothing
            Me.m_zgh.Detach()

            RemoveHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            MyBase.OnFormClosed(e)

        End Sub

        ''' <summary>
        ''' Keep me open, please!
        ''' </summary>
        Public Overrides ReadOnly Property IsRunForm() As Boolean
            Get
                Return True
            End Get
        End Property

        Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnRun.Click
            If Not m_bEcosimRunning Then

                For i As Integer = 0 To m_SRResults.Count - 1
                    m_SRResults(i).SRDataList.Clear()
                Next
                Me.Core.RunEcoSim(AddressOf TimeStepFromEcoSim_handler)
            Else
                Me.Core.StopEcoSim()
            End If

        End Sub

        Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            Dim stanza As cStanzaGroup = Nothing
            Dim group As cEcoPathGroupInput = Nothing
            Dim tmpSR As SRData = Nothing

            If results.hasSRData Then

                For i As Integer = 1 To results.nStanza
                    stanza = Me.Core.StanzaGroups(i - 1)

                    For j As Integer = 1 To stanza.NStanzas - 1

                        group = Me.Core.EcoPathGroupInputs(stanza.iGroups(j))
                        tmpSR = New SRData()
                        If results.hasSRData(i, j) Then

                            tmpSR.Stock = results.BStock(i, j)
                            tmpSR.recrt = results.BRecruitment(i, j)

                            For k As Integer = 0 To m_SRResults.Count - 1
                                If (Object.ReferenceEquals(stanza, m_SRResults(k).StanzaGroup)) And _
                                   (Object.ReferenceEquals(group, m_SRResults(k).GroupStart)) Then

                                    m_SRResults(k).SRDataList.Add(tmpSR)
                                    Exit For

                                End If
                            Next
                        End If

                    Next
                Next

                Me.AddCurves(Me.m_graphpane)
            End If

        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)

            ' Check whether ecosim is running
            Dim bEcosimRunning As Boolean = (csm.IsEcosimRunning)

            ' Is this a state change?
            If (bEcosimRunning <> Me.m_bEcosimRunning) Then
                ' #Yes: update to new state
                Me.m_bEcosimRunning = bEcosimRunning

                ' Configure run/stop button
                Me.m_btnRun.Text = CStr(IIf(Me.m_bEcosimRunning, My.Resources.LABEL_STOP, My.Resources.LABEL_RUN))
                Me.m_btnRun.Enabled = Me.m_coreStateMonitor.HasEcosimLoaded
                ' Reflect change immediately
                Me.m_btnRun.Update()

            End If

        End Sub

        Private Sub OnStyleGuideChanged(ByVal change As cStyleGuide.eChangeType)
            If (change And cStyleGuide.eChangeType.Colours) > 0 Then
                ' Add the curves again
                Me.AddCurves(Me.m_graphpane, False)
            End If
        End Sub

        Private Sub EcosimMessageHandler(ByRef msg As cMessage)

            Try
                Select Case msg.Type
                    Case eMessageType.EcosimRunCompleted

                        If Not m_SRResults Is Nothing Then
                            Me.AddCurves(m_graphpane, True)
                        End If
                End Select

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

        Private Sub tvGroups_AfterSelect(ByVal sender As System.Object, ByVal e As TreeViewEventArgs) _
            Handles m_tvGroups.AfterSelect

            Dim iLevel As Integer = e.Node.Level
            Select Case iLevel

                Case 0
                    Me.SetDefaultDisplay()

                Case 1
                    Me.SetAllStanzaGrpsDisplay(DirectCast(e.Node.Tag, cStanzaGroup))

                Case 2
                    ' JS 15apr09: split by spaces instead of the potential dangerous comma 
                    '             for this can conflict with locale number settings
                    Dim astrTmp() As String = CStr(e.Node.Tag).Split(New [Char]() {" "c})
                    Dim iStart As Integer = CInt(astrTmp(0))
                    Dim iEnd As Integer = CInt(astrTmp(1))
                    Me.SetOneGroupDisplay(iStart, iEnd)

            End Select

        End Sub

        Private Function zgSRPlot_MouseDownEvent(ByVal sender As ZedGraph.ZedGraphControl, ByVal e As MouseEventArgs) As System.Boolean _
            Handles m_plot.MouseDownEvent

            Dim ptMouse As New PointF(e.X, e.Y)
            Dim pane As GraphPane = sender.MasterPane.FindChartRect(ptMouse)
            Dim x, y As Double
            Dim item As CurveItem = Nothing
            Dim sg As cStyleGuide = Me.UIContext.StyleGuide

            If Not pane Is Nothing Then

                pane.ReverseTransform(ptMouse, x, y)
                item = pane.AddCurve("", New Double() {0.0, x}, New Double() {0.0, y}, Color.Black, SymbolType.None)
                m_lblPt.Text = String.Format(My.Resources.ECOSIM_SR_SLOPELABEL, _
                                           sg.FormatNumber(CSng(x)), sg.FormatNumber(CSng(y)), _
                                           sg.FormatNumber(CSng(y / x)))
                RemoveSlopeCurve(pane, item)
            End If

            Return False
        End Function

        Private Function zgSRPlot_MouseMoveEvent(ByVal sender As ZedGraph.ZedGraphControl, ByVal e As MouseEventArgs) As System.Boolean _
            Handles m_plot.MouseMoveEvent

            Dim mousePt As New PointF(e.X, e.Y)
            Dim pane As GraphPane = sender.MasterPane.FindChartRect(mousePt)

            If pane Is Nothing Then
                m_lblPt.Text = String.Empty
                RemoveSlopeCurve(m_graphpane, Nothing)
            End If

        End Function

#End Region ' Events

#Region " Internals "

        Private Sub LoadGroups()

            Dim strTitle As String = ""
            Dim stanza As cStanzaGroup = Nothing
            Dim groupStart As cEcoPathGroupInput = Nothing
            Dim groupEnd As cEcoPathGroupInput = Nothing
            Dim node As TreeNode = Nothing
            Dim iGroupLast As Integer = 0
            Dim iGroup As Integer = 0
            Dim srl As SRLine = Nothing

            m_tvGroups.BeginUpdate()
            m_tvGroups.Nodes.Clear()

            m_SRResults.Clear()

            If Me.Core.nStanzas > 0 Then
                m_tvGroups.Nodes.Add(My.Resources.HEADER_SHOWALL)

                'Stanza group index is Zero-based.
                For i As Integer = 0 To Me.Core.nStanzas - 1
                    ' Get stanza group
                    stanza = Me.Core.StanzaGroups(i)
                    ' Add stanza node
                    node = New TreeNode(stanza.Name)
                    node.Tag = stanza
                    m_tvGroups.Nodes(0).Nodes.Add(node)

                    ' Add subnodes for life stages
                    iGroupLast = stanza.iGroups(stanza.NStanzas)
                    groupEnd = Me.Core.EcoPathGroupInputs(iGroupLast)

                    For j As Integer = 1 To stanza.NStanzas - 1

                        iGroup = stanza.iGroups(j)
                        groupStart = Me.Core.EcoPathGroupInputs(iGroup)

                        strTitle = String.Format(SharedResources.GENERIC_LABEL_DETAILEDLABEL, groupStart.Name, groupEnd.Name)
                        srl = New SRLine()
                        srl.Title = strTitle
                        srl.StanzaGroup = stanza
                        srl.GroupStart = groupStart
                        srl.GroupEnd = groupEnd
                        srl.IsDefault = (j = 1)
                        srl.IsVisible = srl.IsDefault

                        'srl.StanzaIndex = i
                        'srl.GrpStart = iGroup
                        'srl.NumLifeStages = iNumLifeStages
                        'srl.GroupStartName = group.Name
                        'srl.GroupEndName = strName

                        node = New TreeNode(strTitle)
                        node.Tag = String.Format("{0} {1}", iGroup, iGroupLast)
                        m_tvGroups.Nodes(0).Nodes(i).Nodes.Add(node) ' Wow, here's to having some good faith....

                        srl.SRDataList = New List(Of SRData)
                        m_SRResults.Add(srl)

                    Next
                Next
                m_btnRun.Enabled = True
            Else
                m_tvGroups.Nodes.Add(My.Resources.SR_PLOT_NO_STANZA_GROUP)
                m_btnRun.Enabled = False
            End If

            m_tvGroups.EndUpdate()
            m_tvGroups.ExpandAll()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add the curves to the pane.
        ''' </summary>
        ''' <param name="pane"></param>
        ''' <param name="bRescale"></param>
        ''' -------------------------------------------------------------------
        Private Sub AddCurves(ByRef pane As GraphPane, Optional ByVal bRescale As Boolean = False)

            pane.CurveList.Clear()

            Dim curve As CurveItem = Nothing
            Dim ppl As PointPairList = Nothing
            Dim srl As SRLine = Nothing

            For i As Integer = 0 To m_SRResults.Count - 1

                srl = m_SRResults(i)
                ppl = New PointPairList()

                For j As Integer = 0 To m_SRResults(i).SRDataList.Count - 1
                    ppl.Add(srl.SRDataList(j).Stock, srl.SRDataList(j).recrt)
                Next

                curve = pane.AddCurve(srl.Title, ppl, _
                                      Me.StyleGuide.GroupColor(Me.Core, srl.GroupStart.Index), _
                                      SymbolType.Circle)

                curve.IsVisible = srl.IsVisible

            Next

            If bRescale Then Me.m_plot.AxisChange()
            Me.m_plot.Refresh()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pane"></param>
        ''' <param name="strTitleX"></param>
        ''' <param name="strTitleY"></param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateCurves(ByRef pane As GraphPane, ByVal strTitleX As String, ByVal strTitleY As String)

            Dim line As SRLine = Nothing
            Dim curve As CurveItem = Nothing

            For i As Integer = 0 To m_SRResults.Count - 1
                line = m_SRResults(i)
                curve = pane.CurveList(line.Title)

                If (curve IsNot Nothing) Then
                    curve.IsVisible = line.IsVisible
                End If
            Next

            Me.m_graphpane.XAxis.Title.Text = String.Format(My.Resources.SR_PLOT_X_AXIS, strTitleX)
            Me.m_graphpane.YAxis.Title.Text = String.Format(My.Resources.HEADER_RECRUITMENT_UNIT, strTitleY)

            Me.m_plot.AxisChange()
            Me.m_plot.Refresh()

        End Sub

        Private Sub SetDefaultDisplay()
            For i As Integer = 0 To m_SRResults.Count - 1
                If Me.m_SRResults(i).IsDefault Then
                    Me.m_SRResults(i).IsVisible = True
                Else
                    Me.m_SRResults(i).IsVisible = False
                End If
            Next
            Me.UpdateCurves(m_graphpane, "", "")
        End Sub

        Private Sub SetAllStanzaGrpsDisplay(ByVal stanzaGroup As cStanzaGroup)
            For i As Integer = 0 To m_SRResults.Count - 1
                m_SRResults(i).IsVisible = Object.ReferenceEquals(m_SRResults(i).StanzaGroup, stanzaGroup)
            Next
            Me.UpdateCurves(Me.m_graphpane, "", "")
        End Sub

        Private Sub SetOneGroupDisplay(ByVal iStart As Integer, ByVal iEnd As Integer)
            Dim strTitleX As String = ""
            Dim strTitleY As String = ""

            For i As Integer = 0 To m_SRResults.Count - 1
                If m_SRResults(i).GroupStart.Index = iStart And m_SRResults(i).GroupEnd.Index = iEnd Then
                    m_SRResults(i).IsVisible = True
                    strTitleX = Me.m_SRResults(i).GroupEnd.Name
                    strTitleY = Me.m_SRResults(i).GroupStart.Name
                Else
                    Me.m_SRResults(i).IsVisible = False
                End If
            Next
            Me.UpdateCurves(Me.m_graphpane, strTitleX, strTitleY)
        End Sub

        Private Sub RemoveSlopeCurve(ByRef pane As GraphPane, ByRef item As CurveItem)

            If Not m_curveSlope Is Nothing Then
                pane.CurveList.Remove(Me.m_curveSlope)
            End If
            Me.m_curveSlope = item
            Me.m_plot.Refresh()

        End Sub

#End Region 'Internals

    End Class

End Namespace
