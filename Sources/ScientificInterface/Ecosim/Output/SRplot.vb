'==============================================================================
'
' $Log: SRplot.vb,v $
' Revision 1.3  2009/01/16 18:30:38  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:56:20  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/08/02 03:04:15  jeroens
' Renamed resources
'
' Revision 1.13  2008/02/05 18:26:57  jeroens
' Neatified
'
' Revision 1.12  2007/12/10 00:19:48  jeroens
' * Tweaked and polished even more
'
' Revision 1.11  2007/12/09 22:11:10  jeroens
' * Restyled
'
' Revision 1.10  2007/12/05 03:46:17  jeroens
' - Removed links to specialized core state events; generic core state event suffices
'
' Revision 1.9  2007/09/24 17:57:54  sherman
' Added header log
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ZedGraph

#End Region

Namespace Ecosim

    Public Class SRplot

        Class SRData
            Public Stock As Single
            Public recrt As Single
        End Class

        Class SRLine
            Public pts As List(Of SRData)
            Public title As String
            Public iStanza As Integer
            Public iGrpStart As Integer
            Public iGrpEnd As Integer
            Public isShown As Boolean
            Public isDefault As Boolean
            Public sGrpName As String
            Public eGrpName As String
        End Class

        Private m_Core As cCore
        Private m_GraphPane As GraphPane
        Private m_bEcosimRunning As Boolean = False
        Private WithEvents m_coreStateMonitor As cCoreStateMonitor = Nothing
        Private m_SRResults As List(Of SRLine)
        Private m_SlopeCurve As CurveItem = Nothing

#Region "Constructors"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            m_Core = cCore.GetInstance()
            m_GraphPane = zgSRPlot.GraphPane
            m_coreStateMonitor = Me.m_Core.StateMonitor
            m_SRResults = New List(Of SRLine)

            m_Core.Messages.AddMessageHandler(New cMessageHandler(AddressOf EcosimMessageHandler, eCoreComponentType.EcoSim, eMessageType.Any))

        End Sub

        Public Sub New(ByVal text As String)

            Me.New()

            'Set tab text
            Me.TabText = text
            'Set window text
            Me.Text = text

        End Sub
#End Region


        Private Sub SRplot_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            LoadGrps()
            InitGraphPane(m_GraphPane)
        End Sub

        Private Sub LoadGrps()

            tvGroups.BeginUpdate()

            tvGroups.Nodes.Clear()

            m_SRResults.Clear()

            If m_Core.nStanzas > 0 Then
                tvGroups.Nodes.Add(My.Resources.HEADER_SHOWALL)

                Dim sGrp As cStanzaGroup = Nothing
                Dim source As cCoreGroupBase = Nothing

                'Stanza group Index is Zero-based.
                For i As Integer = 0 To m_Core.nStanzas - 1
                    sGrp = m_Core.StanzaGroups(i)

                    tvGroups.Nodes(0).Nodes.Add(sGrp.Name)
                    Dim ilGrp As Integer = sGrp.iGroups(sGrp.NStanzas)
                    Dim lGrpName As String = m_Core.EcoPathGroupInputs(ilGrp).Name
                    For j As Integer = 1 To sGrp.NStanzas - 1
                        Dim isGrp As Integer = sGrp.iGroups(j)
                        source = m_Core.EcoPathGroupInputs(isGrp)
                        Dim srl As New SRLine
                        srl.title = String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, source.Name, lGrpName)
                        srl.iStanza = i
                        srl.iGrpStart = isGrp
                        srl.iGrpEnd = ilGrp
                        srl.sGrpName = source.Name
                        srl.eGrpName = lGrpName
                        If j = 1 Then srl.isDefault = True Else srl.isDefault = False
                        If srl.isDefault Then srl.isShown = True Else srl.isShown = False
                        Dim ndTmp As New TreeNode(srl.title)
                        ndTmp.Tag = String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, isGrp, ilGrp)
                        tvGroups.Nodes(0).Nodes(i).Nodes.Add(ndTmp)
                        srl.pts = New List(Of SRData)
                        m_SRResults.Add(srl)
                    Next
                Next
                btnRun.Enabled = True
            Else
                tvGroups.Nodes.Add(My.Resources.SR_PLOT_NO_STANZA_GROUP)
                btnRun.Enabled = False
            End If

            tvGroups.EndUpdate()
            tvGroups.ExpandAll()

        End Sub

        Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
            If Not m_bEcosimRunning Then

                For i As Integer = 0 To m_SRResults.Count - 1
                    m_SRResults(i).pts.Clear()
                Next
                m_Core.RunEcoSim(AddressOf TimeStepFromEcoSim_handler)
            Else
                m_Core.StopEcoSim()
            End If

        End Sub

        Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            If results.hasSRData Then

                Dim sGrp As cStanzaGroup = Nothing

                For i As Integer = 1 To results.nStanza

                    sGrp = m_Core.StanzaGroups(i - 1)

                    For j As Integer = 1 To sGrp.NStanzas - 1

                        If results.hasSRData(i, j) Then
                            Dim tmpSR As New SRData

                            tmpSR.Stock = results.BStock(i, j)
                            tmpSR.recrt = results.BRecruitment(i, j)
                            For k As Integer = 0 To m_SRResults.Count - 1
                                If (i - 1) = m_SRResults(k).iStanza And sGrp.iGroups(j) = m_SRResults(k).iGrpStart Then
                                    m_SRResults(k).pts.Add(tmpSR)
                                    Exit For
                                End If
                            Next
                        End If

                    Next
                Next
                AddCurves(m_GraphPane)
            End If

        End Sub

        Private Sub InitGraphPane(ByRef pane As GraphPane)
            pane.Title.Text = My.Resources.SR_PLOT_TITLE
            pane.Title.FontSpec.Size = 16

            pane.XAxis.Title.Text = String.Format(My.Resources.SR_PLOT_X_AXIS, String.Empty)
            pane.XAxis.Title.FontSpec.Size = 12
            pane.XAxis.Scale.FontSpec.Size = 12

            pane.YAxis.Title.Text = String.Format(My.Resources.HEADER_RECRUITMENT_UNIT, String.Empty)
            pane.YAxis.Title.FontSpec.Size = 12
            pane.YAxis.Scale.FontSpec.Size = 12

            pane.Legend.IsVisible = False
            pane.IsFontsScaled = False
        End Sub

        Private Sub AddCurves(ByRef pane As GraphPane, Optional ByVal bChangeAxis As Boolean = False)

            pane.CurveList.Clear()

            Dim rotator As New ColorSymbolRotator
            For i As Integer = 0 To m_SRResults.Count - 1
                Dim srl As SRLine = m_SRResults(i)
                Dim data As New PointPairList
                For j As Integer = 0 To m_SRResults(i).pts.Count - 1
                    data.Add(srl.pts(j).Stock, srl.pts(j).recrt)
                Next
                Dim item As CurveItem = pane.AddCurve(srl.title, data, rotator.NextColor, SymbolType.Circle)
                If srl.isShown Then
                    item.IsVisible = True
                Else
                    item.IsVisible = False
                End If
            Next
            If bChangeAxis Then zgSRPlot.AxisChange()
            zgSRPlot.Refresh()

        End Sub

        Private Sub UpdateCurves(ByRef pane As GraphPane, ByVal strTitleX As String, ByVal strTitleY As String)

            For i As Integer = 0 To m_SRResults.Count - 1
                Dim tmp As SRLine = m_SRResults(i)
                Dim item As CurveItem = pane.CurveList(tmp.title)
                If Not item Is Nothing Then
                    item.IsVisible = tmp.isShown
                End If
            Next
            m_GraphPane.XAxis.Title.Text = String.Format(My.Resources.SR_PLOT_X_AXIS, strTitleX)
            m_GraphPane.YAxis.Title.Text = String.Format(My.Resources.HEADER_RECRUITMENT_UNIT, strTitleY)

            zgSRPlot.AxisChange()
            zgSRPlot.Refresh()
        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal core As EwECore.cCore, ByVal iState As eCoreExecutionState) Handles m_coreStateMonitor.CoreExecutionStateEvent

            ' Check whether ecosim is running
            Dim bEcosimRunning As Boolean = (iState = eCoreExecutionState.EcosimRunning)
            ' Is this a state change?
            If (bEcosimRunning <> Me.m_bEcosimRunning) Then
                ' #Yes: update to new state
                Me.m_bEcosimRunning = bEcosimRunning

                ' Configure run/stop button
                ' ToDo_JS: Use two different buttons
                Me.btnRun.Text = CStr(IIf(Me.m_bEcosimRunning, "&Stop", "&Run"))
                Me.btnRun.Enabled = Me.m_coreStateMonitor.HasEcosimLoaded
                ' Reflect change immediately
                Me.btnRun.Update()

            End If

            If iState = eCoreExecutionState.EcosimLoaded Then
                ' Config x-axis labels
            End If

        End Sub

        Private Sub EcosimMessageHandler(ByRef msg As cMessage)

            Try
                Select Case msg.Type
                    Case eMessageType.EcosimRunCompleted

                        If Not m_SRResults Is Nothing Then
                            AddCurves(m_GraphPane, True)
                        End If
                End Select

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

        Private Sub tvGroups_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvGroups.AfterSelect
            Dim iLevel As Integer = e.Node.Level
            Select Case iLevel
                Case 0
                    SetDefaultDisplay()
                Case 1
                    SetAllStanzaGrpsDisplay(e.Node.Index)
                Case 2
                    Dim tmp() As String = CStr(e.Node.Tag).Split(New [Char]() {","c})
                    Dim iStart As Integer = CInt(tmp(0))
                    Dim iEnd As Integer = CInt(tmp(1))
                    SetOneGrpDisplay(iStart, iEnd)
            End Select

        End Sub

        Private Sub SetDefaultDisplay()
            For i As Integer = 0 To m_SRResults.Count - 1
                If m_SRResults(i).isDefault Then
                    m_SRResults(i).isShown = True
                Else
                    m_SRResults(i).isShown = False
                End If
            Next

            UpdateCurves(m_GraphPane, String.Empty, String.Empty)

        End Sub

        Private Sub SetAllStanzaGrpsDisplay(ByVal iStanza As Integer)
            For i As Integer = 0 To m_SRResults.Count - 1
                If m_SRResults(i).iStanza = iStanza Then
                    m_SRResults(i).isShown = True
                Else
                    m_SRResults(i).isShown = False
                End If
            Next

            UpdateCurves(m_GraphPane, String.Empty, String.Empty)

        End Sub

        Private Sub SetOneGrpDisplay(ByVal iStart As Integer, ByVal iEnd As Integer)
            Dim xTitle As String = String.Empty
            Dim yTitle As String = String.Empty
            For i As Integer = 0 To m_SRResults.Count - 1
                If m_SRResults(i).iGrpStart = iStart And m_SRResults(i).iGrpEnd = iEnd Then
                    m_SRResults(i).isShown = True
                    xTitle = m_SRResults(i).eGrpName
                    yTitle = m_SRResults(i).sGrpName
                Else
                    m_SRResults(i).isShown = False
                End If
            Next
            UpdateCurves(m_GraphPane, xTitle, yTitle)
        End Sub

        Private Function zgSRPlot_MouseDownEvent(ByVal sender As ZedGraph.ZedGraphControl, ByVal e As System.Windows.Forms.MouseEventArgs) As System.Boolean Handles zgSRPlot.MouseDownEvent

            Dim mousePt As New PointF(e.X, e.Y)
            Dim pane As GraphPane = sender.MasterPane.FindChartRect(mousePt)

            If Not pane Is Nothing Then

                Dim x, y As Double
                pane.ReverseTransform(mousePt, x, y)
                Dim item As CurveItem = pane.AddCurve(String.Empty, New Double() {0.0, x}, New Double() {0.0, y}, Color.Black, SymbolType.None)
                lblPt.Text = String.Format("({0} , {1}) slope of line is: {2} ", x.ToString("f2"), y.ToString("f2"), (y / x).ToString("f2"))
                RemoveSlopeCurve(pane, item)
                'Else
                '    lblPt.Text = String.Empty
                '    RemoveSlopeCurve(m_GraphPane, Nothing)
            End If

            Return False
        End Function

        Private Sub RemoveSlopeCurve(ByRef pane As GraphPane, ByRef item As CurveItem)

            If Not m_SlopeCurve Is Nothing Then
                pane.CurveList.Remove(m_SlopeCurve)
            End If
            m_SlopeCurve = item
            zgSRPlot.Refresh()

        End Sub

        Private Function zgSRPlot_MouseMoveEvent(ByVal sender As ZedGraph.ZedGraphControl, ByVal e As System.Windows.Forms.MouseEventArgs) As System.Boolean Handles zgSRPlot.MouseMoveEvent
            Dim mousePt As New PointF(e.X, e.Y)
            Dim pane As GraphPane = sender.MasterPane.FindChartRect(mousePt)

            If pane Is Nothing Then
                lblPt.Text = String.Empty
                RemoveSlopeCurve(m_GraphPane, Nothing)
            End If
        End Function
    End Class

End Namespace
