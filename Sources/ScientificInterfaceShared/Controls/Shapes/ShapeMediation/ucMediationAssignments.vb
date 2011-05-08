#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph
Imports System.ComponentModel
Imports ScientificInterfaceShared.Style

#End Region

Namespace Controls

    ''' <summary>
    ''' User control for showing the percentages for mediation effects.
    ''' </summary>
    Public Class ucMediationAssignments
        Implements IUIElement

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_medfn As cMediationBaseFunction = Nothing
        Private m_zgh As cZedGraphHelper = Nothing
        Private m_strXAxisLabel As String = ""
        Private m_strYAxisLabel As String = ""
        Private m_data As cBioPercentData = Nothing
        Private m_viewmode As eViewModeTypes = eViewModeTypes.Pie

#End Region ' Private vars

#Region " Framework overrides "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub DestroyHandle()
            Me.UIContext = Nothing
            MyBase.DestroyHandle()
        End Sub

#End Region ' Framework overrides

#Region " Events "

        Protected Overridable Sub OnStyleGuideChanged(ByVal change As cStyleGuide.eChangeType)
            ' Refresh regardless of the type of change
            Me.LoadGraphData(Me.m_data)
        End Sub

#End Region ' Events

#Region " Public interfaces "

        Public Enum eViewModeTypes As Integer
            Pie = 0
            Bar
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Data structure for showing data in this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cBioPercentData
            Public Groups() As cMediatingGroup
            Public Fleets() As cMediatingFleet
        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the content of this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub RefreshContent()

            If (Me.m_medfn IsNot Nothing) Then
                Me.m_data = Me.ExtractData(Me.m_medfn)
            End If
            Me.LoadGraphData(Me.m_data)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cMediationBaseFunction"/> to display in this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Public Property Shape() As cMediationBaseFunction
            Get
                Return Me.m_medfn
            End Get
            Set(ByVal value As cMediationBaseFunction)
                ' Store function ref
                Me.m_medfn = DirectCast(value, cMediationBaseFunction)
                ' Reload content
                Me.RefreshContent()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cBioPercentData"/> to display in this control.
        ''' </summary>
        ''' <remarks>
        ''' This data is automatically extracted when a <see cref="Shape"/> is provided.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
         Public Property Data() As cBioPercentData
            Get
                Return Me.m_data
            End Get
            Set(ByVal value As cBioPercentData)
                Me.m_data = value
                Me.RefreshContent()
            End Set
        End Property

        <Browsable(False)> _
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)

                If m_uic IsNot Nothing Then
                    RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                    Me.m_zgh.Detach()
                    Me.m_zgh = Nothing
                End If

                Me.m_uic = value

                If Me.m_uic IsNot Nothing Then
                    Me.m_zgh = New cZedGraphHelper()
                    Me.m_zgh.Attach(Me.UIContext, Me.m_zedgraph, 1)
                    Me.LoadGraphData(Me.m_data)
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the X-axis label for the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Category("Mediation"), _
         Description("Label to display on the Y axis")> _
        Public Property XAxisLabel() As String
            Get
                Return Me.m_strXAxisLabel
            End Get
            Set(ByVal value As String)
                Me.m_strXAxisLabel = value
                If Me.m_zgh IsNot Nothing Then
                    Me.m_zgh.GetPane(1).XAxis.Title.Text = Me.m_strXAxisLabel
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the Y-axis label for the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Category("Mediation"), _
         Description("Label to display on the X axis")> _
        Public Property YAxisLabel() As String
            Get
                Return Me.m_strYAxisLabel
            End Get
            Set(ByVal value As String)
                Me.m_strYAxisLabel = value
                If Me.m_zgh IsNot Nothing Then
                    Me.m_zgh.GetPane(1).YAxis.Title.Text = Me.m_strYAxisLabel
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the X-axis label for the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Category("Mediation"), _
         Description("Data view mode")> _
        Public Property ViewMode() As eViewModeTypes
            Get
                Return Me.m_viewmode
            End Get
            Set(ByVal value As eViewModeTypes)
                Me.m_viewmode = value
                ' Whoah
                Me.UIContext = Me.UIContext
            End Set
        End Property

#End Region ' Public interfaces

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract <see cref="cBioPercentData"/> from the current attached <see cref="Shape"/>.
        ''' </summary>
        ''' <param name="fn"></param>
        ''' -------------------------------------------------------------------
        Private Function ExtractData(ByVal fn As cMediationBaseFunction) As cBioPercentData

            Dim d As New cBioPercentData()

            Dim lGroups As New List(Of cMediatingGroup)
            For iIndex As Integer = 0 To fn.NumGroups - 1
                lGroups.Add(fn.Group(iIndex))
            Next
            d.Groups = lGroups.ToArray()

            Dim lFleets As New List(Of cMediatingFleet)
            For iIndex As Integer = 0 To fn.NumFleet - 1
                lFleets.Add(fn.Fleet(iIndex))
            Next
            d.Fleets = lFleets.ToArray()

            Return d

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the graph with external data, for UI preview purposes.
        ''' </summary>
        ''' <param name="data"></param>
        ''' -------------------------------------------------------------------
        Public Sub LoadGraphData(ByVal data As cBioPercentData)

            Me.m_data = data

            If (data IsNot Nothing) Then
                Select Case Me.m_viewmode
                    Case eViewModeTypes.Pie
                        Me.LoadAsPie()
                    Case eViewModeTypes.Bar
                        Me.LoadAsBar()
                End Select
                m_zedgraph.Visible = True
            Else
                m_zedgraph.Visible = False
            End If

            ' Calculate the Axis Scale Ranges
            m_zedgraph.AxisChange()
            m_zedgraph.Refresh()

        End Sub

        Public Sub LoadAsBar()

            ' Sanity checks
            If (Me.m_uic Is Nothing) Then Return
            If (Me.IsDisposed) Then Return

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim medGrp As cMediatingGroup = Nothing
            Dim medFlt As cMediatingFleet = Nothing
            Dim list As PointPairList = Nothing
            Dim pane As GraphPane = Nothing
            Dim source As cCoreInputOutputBase = Nothing
            Dim strLabel As String = ""
            Dim fmt As New cCoreInputOutputBaseFormatter()
            Dim clr As Color = Color.Transparent
            Dim myCurve As BarItem = Nothing

            Me.m_zgh.ConfigurePane("", Me.m_strXAxisLabel, Me.m_strYAxisLabel, True)
            pane = Me.m_zgh.GetPane(1)

            pane.XAxis.Scale.IsVisible = False
            pane.CurveList.Clear()

            For i As Integer = 0 To data.Groups.Length - 1
                list = New PointPairList()
                medGrp = data.Groups(i)
                list.Add(i + 1, medGrp.Weight)

                ' Get the group
                source = Me.m_uic.Core.EcoPathGroupInputs(medGrp.iGroupIndex)
                clr = sg.GroupColor(Me.m_uic.Core, medGrp.iGroupIndex)

                If (TypeOf medGrp Is cLandingsMediatingGroup) Then
                    Dim medLandings As cLandingsMediatingGroup = DirectCast(medGrp, cLandingsMediatingGroup)
                    ' Is a landings interaction?
                    If (medLandings.iFleetIndex > 0) Then
                        Dim sourceSec As cCoreInputOutputBase = Me.m_uic.Core.FleetInputs(medLandings.iFleetIndex)
                        strLabel = String.Format(My.Resources.GENERIC_LABEL_DETAILED, _
                                                 fmt.GetDescriptor(source), _
                                                 fmt.GetDescriptor(sourceSec))
                    Else
                        strLabel = String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                                 fmt.GetDescriptor(source), _
                                                 My.Resources.GENERIC_VALUE_ALL)
                    End If
                Else
                    strLabel = fmt.GetDescriptor(source)
                End If
                myCurve = pane.AddBar(strLabel, list, clr)
                myCurve.Bar.Fill = New Fill(clr)

            Next

            For i As Integer = 0 To data.Fleets.Length - 1
                list = New PointPairList()
                medFlt = data.Fleets(i)
                list.Add(i + 1 + data.Groups.Length, medFlt.Weight)

                ' Get the fleet
                source = Me.m_uic.Core.FleetInputs(medFlt.iFleetIndex)
                clr = sg.FleetColor(Me.m_uic.Core, medFlt.iFleetIndex)
                strLabel = fmt.GetDescriptor(source)

                myCurve = pane.AddBar(strLabel, list, clr)
                myCurve.Bar.Fill = New Fill(clr)
            Next

            ' Configure pane
            pane.Title.Text = ""
            pane.Title.IsVisible = False

        End Sub

        Private Sub LoadAsPie()

            ' ToDo_JS: globalize this

            ' Sanity checks
            If (Me.m_uic Is Nothing) Then Return
            If (Me.IsDisposed) Then Return

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim medGrp As cMediatingGroup = Nothing
            Dim medFlt As cMediatingFleet = Nothing
            Dim list As PointPairList = Nothing
            Dim pane As GraphPane = Nothing
            Dim valSource As cCoreInputOutputBase = Nothing
            Dim strLabel As String = ""
            Dim fmt As New cCoreInputOutputBaseFormatter()
            Dim clr As Color = Color.Transparent
            Dim myCurve As BarItem = Nothing
            Dim varname As EwEUtils.Core.eVarNameFlags
            Dim iGroup As Integer
            Dim strTitle As String = ""

            Me.m_zgh.ConfigurePane("", "", "", True)
            pane = Me.m_zgh.GetPane(1)

            pane.XAxis.Scale.IsVisible = False
            pane.CurveList.Clear()


            For i As Integer = 0 To Data.Groups.Length - 1

                ' Get the group
                medGrp = Data.Groups(i)
                valSource = Me.m_uic.Core.EcoPathGroupOutputs(medGrp.iGroupIndex)

                ' Is a landings interaction?
                If (TypeOf medGrp Is cLandingsMediatingGroup) Then
                    ' #Yes
                    Dim medLandings As cLandingsMediatingGroup = DirectCast(medGrp, cLandingsMediatingGroup)
                    'set the title
                    strTitle = "Weighted percentage of landings"

                    ' Group index and VarName for the landings of this group by this fleet
                    iGroup = medLandings.iGroupIndex
                    varname = EwEUtils.Core.eVarNameFlags.Landings

                    If (medLandings.iFleetIndex > 0) Then
                        Dim FleetSource As cCoreInputOutputBase = Me.m_uic.Core.FleetInputs(medLandings.iFleetIndex)
                        strLabel = String.Format(My.Resources.GENERIC_LABEL_DETAILED, _
                                                 fmt.GetDescriptor(valSource), _
                                                 fmt.GetDescriptor(FleetSource))

                        'Ok this is a little strange
                        'set the source of the values to the FleetInput 
                        valSource = FleetSource

                    Else
                        strLabel = String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                                 fmt.GetDescriptor(valSource), _
                                                 My.Resources.GENERIC_VALUE_ALL)
                    End If
                Else
                    'Biomass 
                    strLabel = fmt.GetDescriptor(valSource)
                    iGroup = cCore.NULL_VALUE
                    varname = EwEUtils.Core.eVarNameFlags.Biomass

                    strTitle = "Weighted percentage of biomass"
                End If

                clr = sg.GroupColor(Me.m_uic.Core, medGrp.iGroupIndex)

                Dim sliceVal As Double = CDbl(valSource.GetVariable(varname, iGroup)) * medGrp.Weight
                Dim slice As PieItem = pane.AddPieSlice(sliceVal, clr, 0.05, strLabel)
                slice.ValueDecimalDigits = sg.NumDigits
                slice.LabelType = PieLabelType.Name_Value_Percent

            Next

            ' Configure pane
            pane.Legend.Position = LegendPos.Right
            pane.Legend.IsHStack = False

            pane.Title.Text = strTitle
            pane.Title.IsVisible = True
            pane.Rect = Me.ClientRectangle

        End Sub

#End Region ' Internals

    End Class

End Namespace



