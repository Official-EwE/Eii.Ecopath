'==============================================================================
'
' $Log: ucBioPercent.vb,v $
' Revision 1.2  2008/12/15 16:00:49  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:38  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/09/19 14:14:38  jeroens
' Fixed issue 496
'
' Revision 1.15  2008/08/02 03:04:21  jeroens
' Renamed resources
'
' Revision 1.14  2008/05/07 01:39:07  jeroens
' Fixed bugs 281, 378, 470
'
' Revision 1.13  2008/05/05 08:35:50  jeroens
' Uses Styleguide group colors instead of group PoolColorArgb
'
' Revision 1.12  2008/04/07 02:31:21  jeroens
' Cleaning up resources
'
' Revision 1.11  2007/11/15 15:03:15  jeroens
' * Changed Load interface to use proper core objects
'
' Revision 1.10  2007/10/03 01:54:28  jeroens
' * Reworked styleguide, colormanager
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports ZedGraph

#End Region

Namespace Ecosim

    Public Class ucBioPercent

        Private m_core As cCore = Nothing
        Private m_MedFunc As cMediationFunction = Nothing
        Private m_RdmColor As ColorSymbolRotator = Nothing
        Private m_zgh As ZedGraphHelper = Nothing

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            ' Get the only core reference
            m_core = cCore.GetInstance()

            m_RdmColor = New ColorSymbolRotator

        End Sub

        Public Property Shape() As cShapeData
            Get
                Return Me.m_MedFunc
            End Get
            Set(ByVal value As cShapeData)

                If (TypeOf value Is cMediationFunction) Then
                    Me.m_MedFunc = DirectCast(value, cMediationFunction)
                End If
                LoadGraphData()

            End Set
        End Property

        Private Sub ucBiomassPercentage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.Dock = DockStyle.Fill

            InitGraphPane()
            LoadGraphData()

        End Sub

        Private Sub InitGraphPane()

            Dim myPane As GraphPane = zgBP.GraphPane

            Me.m_zgh = New ZedGraphHelper(Me.zgBP)
            Me.m_zgh.ConfigurePane("", My.Resources.ECOSIM_DEF_MED_X_AXIS, My.Resources.HEADER_RELATIVEWEIGHT, True)

            'myPane.Border.IsVisible = False
            'myPane.Chart.Border.IsVisible = False
            'myPane.YAxis.MajorTic.IsOpposite = False
            'myPane.XAxis.MajorTic.IsOpposite = False
            'myPane.YAxis.MinorTic.IsOpposite = False
            'myPane.XAxis.MinorTic.IsOpposite = False
            'myPane.XAxis.Scale.IsVisible = False

            ' Fill the axis background with a color gradient
            myPane.Chart.Fill = New Fill(Color.White, _
               Color.LightGray, 90.0F)
            ' Fill the legend background with a color gradient
            myPane.Legend.Fill = New Fill(Color.White, _
               Color.FromArgb(255, 255, 250), 90.0F)
            ' Fill the pane background with a solid color
            myPane.Fill = New Fill(Color.FromArgb(250, 250, 255))

        End Sub


        Public Sub LoadGraphData()

            Dim medGrp As cMediatingGroup = Nothing
            Dim medFlt As cMediatingFleet = Nothing
            Dim list As PointPairList = Nothing
            Dim pane As GraphPane = zgBP.GraphPane
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim clr As Color = Color.Transparent
            Dim myCurve As BarItem = Nothing

            pane.CurveList.Clear()

            If (Me.m_MedFunc IsNot Nothing) Then

                For i As Integer = 0 To m_MedFunc.CountGroup - 1
                    list = New PointPairList()
                    medGrp = m_MedFunc.Group(i)
                    list.Add(i + 1, medGrp.Weight)

                    ' Get the group
                    source = Me.m_core.EcoPathGroupInputs(medGrp.iGroupIndex)
                    clr = sg.GroupColor(Me.m_core, medGrp.iGroupIndex)

                    myCurve = pane.AddBar(source.Name, list, clr)
                    myCurve.Bar.Fill = New Fill(clr)

                Next

                For i As Integer = 0 To m_MedFunc.CountFleet - 1
                    list = New PointPairList()
                    medFlt = m_MedFunc.Fleet(i)

                    ' Get the fleet
                    source = m_core.FleetInputs(medFlt.iFleetIndex)
                    list.Add(i + 1 + m_MedFunc.CountGroup, medFlt.Weight)

                    clr = m_RdmColor.NextColor
                    myCurve = pane.AddBar(source.Name, list, clr)
                    myCurve.Bar.Fill = New Fill(clr)
                Next

            End If

            ' Calculate the Axis Scale Ranges
            zgBP.AxisChange()
            zgBP.Refresh()

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        Public Sub LoadGraphData(ByVal data As Dictionary(Of cCoreInputOutputBase, Single))

            Dim myPane As GraphPane = zgBP.GraphPane
            Dim sg As StyleGuide = StyleGuide.GetInstance()
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
                        clr = StyleGuide.GetInstance().GroupColor(Me.m_core, source.Index)
                        myCurve = myPane.AddBar(source.Name, list, clr)
                        myCurve.Bar.Fill = New Fill(clr)
                    End If
                Next

            End If

            zgBP.AxisChange()
            zgBP.Refresh()

        End Sub

    End Class

End Namespace



