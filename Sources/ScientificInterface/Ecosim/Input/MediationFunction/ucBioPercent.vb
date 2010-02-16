#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports ZedGraph

#End Region

Namespace Ecosim

    Public Class ucBioPercent
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_medfn As cMediationFunction = Nothing
        Private m_RdmColor As ColorSymbolRotator = Nothing
        Private m_zgh As cZedGraphHelper = Nothing

        Public Sub New()

            me.InitializeComponent()
            Me.m_RdmColor = New ColorSymbolRotator

        End Sub

        Public Property Shape() As cShapeData
            Get
                Return Me.m_medfn
            End Get
            Set(ByVal value As cShapeData)
                Me.m_medfn = DirectCast(value, cMediationFunction)
                Me.LoadGraphData()
            End Set
        End Property

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.InitGraphPane()
            Me.LoadGraphData()
        End Sub

        Protected Overrides Sub DestroyHandle()
            Me.m_zgh.Detach()
            MyBase.DestroyHandle()
        End Sub

        Private Sub InitGraphPane()

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_zedgraph)
            Me.m_zgh.ConfigurePane("", My.Resources.ECOSIM_DEF_MED_X_AXIS, My.Resources.HEADER_RELATIVEWEIGHT, True)

            'Dim pane As GraphPane = m_zedgraph.GraphPane

            '' Fill the axis background with a color gradient
            'pane.Chart.Fill = New Fill(Color.White, Color.LightGray, 90.0F)
            '' Fill the legend background with a color gradient
            'pane.Legend.Fill = New Fill(Color.White, Color.FromArgb(255, 255, 250), 90.0F)
            '' Fill the pane background with a solid color
            'pane.Fill = New Fill(Color.FromArgb(250, 250, 255))

        End Sub


        Public Sub LoadGraphData()

            ' Sanity check
            If (Me.m_uic Is Nothing) Then Return

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim medGrp As cMediatingGroup = Nothing
            Dim medFlt As cMediatingFleet = Nothing
            Dim list As PointPairList = Nothing
            Dim pane As GraphPane = m_zedgraph.GraphPane
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

                    ' Get the fleet
                    source = Me.m_uic.Core.FleetInputs(medFlt.iFleetIndex)
                    list.Add(i + 1 + m_medfn.CountGroup, medFlt.Weight)

                    clr = m_RdmColor.NextColor
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
                        clr = m_RdmColor.NextColor
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



