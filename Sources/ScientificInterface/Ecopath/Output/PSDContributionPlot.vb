' =============================================================================
'
' $Log: PSDContributionPlot.vb,v $
' Revision 1.7  2009/03/17 23:37:34  joeh
' Add codes for the Selected Group feature
'
' Revision 1.6  2009/03/14 18:32:55  joeh
' Change dXValue of double type to sXValue of single type
'
' Revision 1.5  2009/03/13 21:39:14  joeh
' Border only the bars of the selected groups with black border
'
' Revision 1.4  2009/03/12 23:51:06  joeh
' Add codes for tabulation of PSD contribution data
'
' Revision 1.3  2009/03/12 01:50:28  joeh
' Add codes for PSD histogram (PSDContributionPlot)
'
' Revision 1.2  2009/02/21 00:23:06  jeroens
' Added headers
'
' =============================================================================

#Region "Imports"
Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ZedGraph
#End Region 'Imports

Namespace Ecopath.Output

    Public Class PSDContributionPlot

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
        Private Sub PSDContributionPlot_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            PopulateGroupBoxes()

            llbGroups.SelectedIndex = 0
        End Sub

        Private Sub llbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles llbGroups.SelectedIndexChanged
            AddCurves(CreatePane(My.Resources.PSD_PLOTCAPTION_PSDCONTRIB, My.Resources.PSD_XAXISLABEL_WEIGHTCLASS, _
                     My.Resources.PSD_YAXISLABEL_BIOMASS))

            'highlight group contribution in the histogram
            UpdatePlot()
        End Sub
#End Region 'Event handlers

#Region "Helper methods"
        Private Sub PopulateGroupBoxes()
            llbGroups.SuspendLayout()

            llbGroups.Items.Clear()
            'llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(Nothing))
            For i As Integer = 1 To m_core.nLivingGroups
                If m_core.IsGroupSelected(i) Then
                    llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(m_core.EcoPathGroupInputs(i)))
                End If
            Next

            llbGroups.ResumeLayout()
        End Sub

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

            pane.XAxis.Scale.Min = Math.Log10(m_core.FirstWeightClass)
            pane.XAxis.Scale.Max = Math.Log10(m_core.FirstWeightClass * 2 ^ (m_core.nWeightClasses - 1))
            pane.YAxis.Scale.Min = 0

            pane.YAxis.MinorTic.IsAllTics = False
            pane.XAxis.MinorTic.IsAllTics = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves(ByVal pane As GraphPane)
            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim grpOutput As cEcoPathGroupOutput = Nothing
            Dim sSystemPSD(m_core.nWeightClasses) As Single
            Dim sgStyleGuide As StyleGuide = StyleGuide.GetInstance
            Dim curveSelected As BarItem = Nothing

            InitLists(resultLists, m_core.nLivingGroups) '3)

            'Find the system PSD by summing the group PSD
            FindSystemPSD(sSystemPSD)

            For igroup As Integer = 1 To m_core.nLivingGroups
                grpOutput = m_core.EcoPathGroupOutputs(igroup)
                For iWtClass As Integer = 1 To m_core.nWeightClasses
                    sXValue = CSng(m_core.FirstWeightClass * 2 ^ (iWtClass - 1))
                    If sSystemPSD(iWtClass) * 100000 > 0 Then
                        'group contribution to the system PSD is Math.Log10(sSystemPSD(iWtClass) * 100000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass)
                        '* 100000 for plotting purpose
                        resultLists(igroup - 1).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 100000) * grpOutput.PSD(iWtClass) / sSystemPSD(iWtClass))
                    Else
                        resultLists(igroup - 1).Add(Math.Log10(sXValue), 0)
                    End If
                Next
            Next

            ' Clear pane
            pane.CurveList.Clear()

            For iGroup As Integer = 1 To m_core.nLivingGroups
                If iGroup = llbGroups.SelectedIndex + 1 Then
                    curveSelected = AddCurveToGraphPane(pane, "", resultLists(iGroup - 1), sgStyleGuide.GroupColor(Me.m_core, iGroup - 1), Color.Gray)
                Else
                    AddCurveToGraphPane(pane, "", resultLists(iGroup - 1), sgStyleGuide.GroupColor(Me.m_core, iGroup - 1), Color.Gray)
                End If
            Next

            curveSelected.Bar.Border = New Border(Color.Black, 2)
            pane.BarSettings.Type = BarType.Stack
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Function AddCurveToGraphPane(ByVal pane As GraphPane, ByVal legend As String, ByVal list As PointPairList, _
                                        ByVal clrFill As Color, ByVal clrBorder As Color) As BarItem
            Dim brItem As BarItem

            brItem = pane.AddBar(legend, list, clrFill)
            brItem.Bar.Fill = New Fill(clrFill)
            brItem.Bar.Border = New Border(clrBorder, 2)

            Return brItem
        End Function

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
#End Region 'Helper method

    End Class

End Namespace