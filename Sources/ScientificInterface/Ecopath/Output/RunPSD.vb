' =============================================================================
'
' $Log: RunPSD.vb,v $
' Revision 1.3  2009/03/11 00:14:29  joeh
' Add PSD calculation
'
' Revision 1.2  2009/02/21 00:24:14  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ZedGraph

#End Region

Namespace Ecopath.Output

    Public Class RunPSD

#Region "Variables"
        Private m_core As cCore = Nothing
        Private m_zgh As ZedGraphHelper = Nothing
#End Region 'Variables

#Region "Constructor"
        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance()
            Me.m_zgh = New ZedGraphHelper(Me.zgcZedGraphCntl)
        End Sub
#End Region 'Constructor

#Region "Event handlers"
        Private Sub RunPSD_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddCurves(CreatePane("PSD", My.Resources.PSD_AXISLABEL_WEIGHTCLASS, My.Resources.PSD_AXISLABEL_BIOMASS))
            UpdatePlot()
        End Sub

        Private Sub mnuItmGroupPB_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuItmGroupPB.CheckedChanged
            mnuItmLorenzen.Checked = Not mnuItmGroupPB.Checked
        End Sub

        Private Sub mnuItmLorenzen_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuItmLorenzen.CheckedChanged
            mnuItmGroupPB.Checked = Not mnuItmLorenzen.Checked
        End Sub

        Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
            'm_core.RunEcoPath()
        End Sub
#End Region 'Event handlers

#Region "Helper methods"
        Private Function CreatePane(ByVal strTitle As String, ByVal strXAxisTitle As String, _
                                    ByVal strYAxisTitle As String) As GraphPane
            Dim pane As GraphPane = Me.zgcZedGraphCntl.GraphPane

            InitGraphPane(strTitle, strXAxisTitle, strYAxisTitle, pane)
            Return pane
        End Function

        Private Sub InitGraphPane(ByVal strTitle As String, ByVal strXAxisTitle As String, _
                                    ByVal strYAxisTitle As String, ByVal pane As GraphPane)
            Pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = False
            pane.Title.FontSpec.Size = 15

            pane.XAxis.Scale.FontSpec.Size = 15
            pane.XAxis.Title.Text = strXAxisTitle
            pane.XAxis.Title.FontSpec.Size = 15

            pane.YAxis.Scale.FontSpec.Size = 15
            pane.YAxis.Title.Text = strYAxisTitle
            pane.YAxis.Title.FontSpec.Size = 15

            pane.XAxis.Scale.Min = Math.Log10(m_core.FirstWeightClass)
            pane.XAxis.Scale.Max = Math.Log10(m_core.FirstWeightClass * 2 ^ (m_core.nWeightClasses - 1))
            'pane.YAxis.Scale.Min = 0

            pane.Border.IsVisible = True
            pane.Legend.IsVisible = True

            pane.Chart.Border.IsVisible = True
            Pane.YAxis.MajorTic.IsOpposite = False
            Pane.XAxis.MajorTic.IsOpposite = False
            Pane.YAxis.MinorTic.IsOpposite = False
            Pane.XAxis.MinorTic.IsOpposite = False
            Pane.YAxis.MinorTic.IsAllTics = False
            Pane.XAxis.MinorTic.IsAllTics = False

            Pane.IsFontsScaled = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves(ByVal pane As GraphPane)
            Dim resultLists As New List(Of PointPairList)
            Dim dXValue As Double = 0
            Dim sSystemPSD(m_core.nWeightClasses) As Single

            InitLists(resultLists, 1)

            'Find the system PSD by summing the group PSD
            FindSystemPSD(sSystemPSD)

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) > 0 Then
                    dXValue = m_core.FirstWeightClass * 2 ^ (iWtClass - 1)

                    'PSD plot
                    resultLists(0).Add(Math.Log10(dXValue), Math.Log10(sSystemPSD(iWtClass)))
                    'PSD fit plot
                    'resultLists(1).Add(dXValue, grpOutput.EcopathNumber(iTimeStep))
                End If
            Next

            ' Clear pane
            pane.CurveList.Clear()

            AddCurveToGraphPane(pane, resultLists(0), Color.Black)
            'AddCurveToGraphPane(pane, resultLists(1), Color.Black)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal pane As GraphPane, ByVal list As PointPairList, ByVal clr As Color)
            pane.AddCurve("", list, clr, SymbolType.Circle)
        End Sub

        Private Sub UpdatePlot()
            Me.zgcZedGraphCntl.AxisChange()
            Me.zgcZedGraphCntl.Refresh()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To m_core.nLivingGroups
                grpOutput = m_core.EcoPathGroupOutputs(iGroup)
                For iWtClass As Integer = 1 To m_core.nWeightClasses
                    sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                Next
            Next
        End Sub
#End Region 'Helper methods

    End Class

End Namespace