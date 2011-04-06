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
    ''' User control for showing the percentages for mediation effects, per group.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ucBioPercent
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_medfn As cMediationFunction = Nothing
        Private m_zgh As cZedGraphHelper = Nothing
        Private m_strXAxisLabel As String = ""
        Private m_strYAxisLabel As String = ""

        Public Sub New()

            Me.InitializeComponent()

        End Sub

        <Browsable(False)> _
        Public Property Shape() As cShapeData
            Get
                Return Me.m_medfn
            End Get
            Set(ByVal value As cShapeData)
                Me.m_medfn = DirectCast(value, cMediationFunction)
                Me.LoadGraphData()
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
                    Me.m_zgh.Attach(Me.UIContext, Me.m_zedgraph)
                    Me.m_zgh.ConfigurePane("", Me.m_strXAxisLabel, Me.m_strYAxisLabel, True)
                    Me.LoadGraphData()
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                End If
            End Set
        End Property

        ''' <summary>
        ''' Get/set the X-axis label for the control.
        ''' </summary>
        <Browsable(True), _
         Category("Mediation"), _
         Description("Label to display on the Y axis")> _
        Public Property XAxisLabel() As String
            Get
                Return Me.m_strXAxisLabel
            End Get
            Set(ByVal value As String)
                Me.m_strXAxisLabel = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the Y-axis label for the control.
        ''' </summary>
        <Browsable(True), _
         Category("Mediation"), _
         Description("Label to display on the X axis")> _
        Public Property YAxisLabel() As String
            Get
                Return Me.m_strYAxisLabel
            End Get
            Set(ByVal value As String)
                Me.m_strYAxisLabel = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Sub DestroyHandle()
            Me.UIContext = Nothing
            MyBase.DestroyHandle()
        End Sub

        Protected Overridable Sub OnStyleGuideChanged(ByVal change As cStyleGuide.eChangeType)
            If (change And cStyleGuide.eChangeType.Colours) > 0 Then
                Me.LoadGraphData()
            End If
        End Sub

        Public Sub LoadGraphData()

            ' Sanity checks
            If (Me.m_uic Is Nothing) Then Return
            If (Me.m_medfn Is Nothing) Then Return
            If (Me.IsDisposed) Then Return

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim medGrp As cMediatingGroup = Nothing
            Dim medFlt As cMediatingFleet = Nothing
            Dim list As PointPairList = Nothing
            Dim pane As GraphPane = Me.m_zgh.GetPane(1)
            Dim source As cCoreInputOutputBase = Nothing
            Dim clr As Color = Color.Transparent
            Dim myCurve As BarItem = Nothing

            pane.CurveList.Clear()

            If (Me.m_medfn IsNot Nothing) Then

                For i As Integer = 0 To m_medfn.CountGroup - 1
                    list = New PointPairList()
                    medGrp = m_medfn.Group(i)
                    list.Add(i + 1, medGrp.Weight)

                    ' Get the group
                    source = Me.m_uic.Core.EcoPathGroupInputs(medGrp.iGroupIndex)
                    clr = sg.GroupColor(Me.m_uic.Core, medGrp.iGroupIndex)

                    myCurve = pane.AddBar(source.Name, list, clr)
                    myCurve.Bar.Fill = New Fill(clr)

                Next

                For i As Integer = 0 To m_medfn.CountFleet - 1
                    list = New PointPairList()
                    medFlt = m_medfn.Fleet(i)
                    list.Add(i + 1 + m_medfn.CountGroup, medFlt.Weight)

                    ' Get the fleet
                    source = Me.m_uic.Core.FleetInputs(medFlt.iFleetIndex)
                    clr = sg.FleetColor(Me.m_uic.Core, medFlt.iFleetIndex)

                    myCurve = pane.AddBar(source.Name, list, clr)
                    myCurve.Bar.Fill = New Fill(clr)
                Next

                m_zedgraph.Visible = True
            Else
                m_zedgraph.Visible = False
            End If

            ' Calculate the Axis Scale Ranges
            m_zedgraph.AxisChange()
            m_zedgraph.Refresh()

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        Public Sub LoadGraphData(ByVal data As Dictionary(Of cCoreInputOutputBase, Single))

            Dim myPane As GraphPane = m_zedgraph.GraphPane
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim source As cCoreInputOutputBase = Nothing
            Dim clr As Color = Color.Transparent
            Dim myCurve As BarItem = Nothing

            myPane.CurveList.Clear()

            If data.Count > 0 Then
                Dim cnt As Integer = 1
                For Each source In data.Keys
                    Dim list As New PointPairList()
                    list.Add(cnt, data(source))
                    cnt += 1
                    'list.Add(i, data(i))

                    ' Is fleet?
                    If (TypeOf source Is cFleetInput) Then
                        ' #Yes: get the fleet
                        clr = Me.m_uic.StyleGuide.FleetColor(Me.m_uic.Core, source.Index)
                        myCurve = myPane.AddBar(source.Name, list, clr)
                        myCurve.Bar.Fill = New Fill(clr)
                    Else
                        ' #No: get the group
                        clr = Me.m_uic.StyleGuide.GroupColor(Me.m_uic.Core, source.Index)
                        myCurve = myPane.AddBar(source.Name, list, clr)
                        myCurve.Bar.Fill = New Fill(clr)
                    End If
                Next

            End If

            m_zedgraph.AxisChange()
            m_zedgraph.Refresh()

        End Sub

    End Class

End Namespace



