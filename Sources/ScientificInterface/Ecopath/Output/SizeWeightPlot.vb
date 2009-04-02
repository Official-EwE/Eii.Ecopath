' =============================================================================
'
' $Log: SizeWeightPlot.vb,v $
' Revision 1.6  2009/04/02 16:24:54  jeroens
' PSD run integrated w Ecopath
'
' Revision 1.5  2009/04/02 01:47:44  joeh
' Pass GroupSelected boolean array to cCore.RunPSD and psdModel.Run
'
' Revision 1.4  2009/04/01 15:21:49  joeh
' Call core.RunPSD() in the Constructor
'
' Revision 1.3  2009/03/20 18:06:18  joeh
' Add codes to plot Size/Weight plot
'
' Revision 1.2  2009/02/21 00:23:07  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph

#End Region 'Imports

Namespace Ecopath.Output

    Public Class SizeWeightPlot

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

            'Don't manually run! The core execution states take care of this!
            'm_core.RunPSD(IsGroupSelected)
        End Sub
#End Region 'Constructor

#Region "Event handlers"
        Private Sub SizeWeightPlot_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddCurves(CreatePane(My.Resources.PSD_PLOTCAPTION_SIZEWT, My.Resources.PSD_XAXISLABEL_SIZECLASS, ""))

            UpdatePlot()
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
            pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = False
            pane.Title.FontSpec.Size = 16

            pane.XAxis.Scale.IsVisible = True 'False
            pane.XAxis.Title.Text = strXAxisTitle
            pane.XAxis.Title.FontSpec.Size = 14

            pane.YAxis.Scale.IsVisible = True 'False
            pane.YAxis.Title.Text = strYAxisTitle
            pane.YAxis.Title.FontSpec.Size = 14

            pane.XAxis.Scale.Min = 1
            pane.XAxis.Scale.Max = m_core.nLivingGroups
            pane.YAxis.Scale.Min = 0

            pane.YAxis.MinorTic.IsAllTics = False
            pane.XAxis.MinorTic.IsAllTics = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves(ByVal pane As GraphPane)
            Dim resultLists As New List(Of PointPairList)
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            InitLists(resultLists, 2)

            For iGroup As Integer = 1 To m_core.nLivingGroups
                grpOutput = m_core.EcoPathGroupOutputs(iGroup)
                resultLists(0).Add(iGroup, grpOutput.BiomassAvgSzWt)
                resultLists(1).Add(iGroup, grpOutput.BiomassSzWt)
            Next

            ' Clear pane
            pane.CurveList.Clear()

            AddCurveToGraphPane(pane, My.Resources.PSD_LINELEGEND_BYNUM, resultLists(0), Color.Blue)
            AddCurveToGraphPane(pane, My.Resources.PSD_LINELEGEND_BYBIOMASS, resultLists(1), Color.Brown)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal pane As GraphPane, ByVal legend As String, ByVal list As PointPairList, _
                                        ByVal clr As Color)
            pane.AddCurve(legend, list, clr, SymbolType.None)
        End Sub

        Private Sub UpdatePlot()
            Me.zgcZedGraphCntl.AxisChange()
            Me.zgcZedGraphCntl.Refresh()
        End Sub

        Private Function IsGroupSelected() As Boolean()
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim bGroupSelected(m_core.nLivingGroups) As Boolean

            For i As Integer = 1 To m_core.nLivingGroups
                bGroupSelected(i) = sg.GroupVisible(i)
            Next
            Return bGroupSelected
        End Function
#End Region 'Helper methods

    End Class

End Namespace