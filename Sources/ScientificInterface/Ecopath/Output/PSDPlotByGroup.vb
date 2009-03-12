' =============================================================================
'
' $Log: PSDPlotByGroup.vb,v $
' Revision 1.6  2009/03/12 01:50:29  joeh
' Add codes for PSD histogram (PSDContributionPlot)
'
' Revision 1.5  2009/03/11 00:14:28  joeh
' Add PSD calculation
'
' Revision 1.4  2009/03/06 17:54:01  joeh
' Minor changes in the computation of Weight, Number and Biomass
'
' Revision 1.3  2009/03/06 00:47:57  joeh
' Add Ecopath output data (Weight, Number, Biomass) over time
'
' Revision 1.2  2009/02/21 00:23:07  jeroens
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

    Public Class PSDPlotByGroup

#Region "Variables"
        Private m_core As cCore = Nothing
        Private m_MasterPane As MasterPane
        Private m_zgh As ZedGraphHelper = Nothing
        Private m_Time() As Single
        Private m_Weight() As Single
        Private m_Number() As Single
        Private m_Biomass() As Single

        Private Enum ePaneTypes As Integer
            Weight = 0
            Number
            Biomass
            PSD
        End Enum
#End Region 'Variables

#Region "Constructor"
        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance()
            Me.m_MasterPane = New MasterPane
            Me.m_zgh = New ZedGraphHelper(Me.zgcZedGraphCntl)
        End Sub
#End Region 'Constructor

#Region "Event handlers"
        Private Sub PSDPlotByGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            PopulateGroupBoxes()
            InitMasterPane()

            CreatePane(ePaneTypes.Weight, My.Resources.HEADER_WEIGHT)
            CreatePane(ePaneTypes.Number, My.Resources.HEADER_NUMBER)
            CreatePane(ePaneTypes.Biomass, My.Resources.HEADER_BIOMASS)
            llbGroups.SelectedIndex = 0
        End Sub

        Private Sub llbGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles llbGroups.SelectedIndexChanged
            AddCurves()
            UpdatePlots()
        End Sub
#End Region 'Event handlers

#Region "Helper methods"
        Private Sub PopulateGroupBoxes()
            llbGroups.SuspendLayout()

            llbGroups.Items.Clear()
            'llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(Nothing))
            For i As Integer = 1 To m_core.nLivingGroups
                llbGroups.Items.Add(New LegendListBox.EcopathGroupItem(m_core.EcoPathGroupInputs(i)))
            Next

            llbGroups.ResumeLayout()
        End Sub

        Private Sub InitMasterPane()
            'Get the master pane
            m_MasterPane = Me.zgcZedGraphCntl.MasterPane

            m_MasterPane.PaneList.Clear()
            'Disable the master pane legend
            m_MasterPane.Legend.IsVisible = False
            'Make the border invisible
            m_MasterPane.Border.IsVisible = False

            m_MasterPane.Title.IsVisible = True
            m_MasterPane.Title.FontSpec.Size = 12
            m_MasterPane.IsFontsScaled = False
        End Sub

        Private Sub CreatePane(ByVal PaneNo As ePaneTypes, ByVal strTitle As String)
            'Define a new graph pane
            Dim pane As New GraphPane

            Debug.Assert(m_MasterPane.PaneList.Count = PaneNo)

            InitGraphPane(strTitle, pane)

            'Add the graphPane to the masterPane
            m_MasterPane.Add(pane)
        End Sub

        Private Sub InitGraphPane(ByVal strTitle As String, ByRef pane As GraphPane)
            pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = True
            pane.Title.FontSpec.Size = 12

            pane.XAxis.Scale.FontSpec.Size = 12
            pane.XAxis.Title.FontSpec.Size = 12

            pane.YAxis.Scale.FontSpec.Size = 12
            pane.YAxis.Title.FontSpec.Size = 12

            pane.XAxis.Scale.Min = 0
            'pane.XAxis.Scale.Max = CDbl(Me.m_core.EcosimFirstYear + (m_core.nEcosimTimeSteps / cCore.N_MONTHS))
            pane.YAxis.Scale.Min = 0

            pane.Border.IsVisible = False
            pane.Legend.IsVisible = False

            pane.Chart.Border.IsVisible = False
            pane.YAxis.MajorTic.IsOpposite = False
            pane.XAxis.MajorTic.IsOpposite = False
            pane.YAxis.MinorTic.IsOpposite = False
            pane.XAxis.MinorTic.IsOpposite = False
            pane.YAxis.MinorTic.IsAllTics = False
            pane.XAxis.MinorTic.IsAllTics = False

            pane.IsFontsScaled = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves()
            'Add single curve into graph first
            'Results data structure
            Dim resultLists As New List(Of PointPairList)
            Dim dXValue As Double = 0
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            grpOutput = m_core.EcoPathGroupOutputs(llbGroups.SelectedIndex + 1)
            InitLists(resultLists, 3)

            For iTimeStep As Integer = 1 To m_core.nEcopathTimeSteps

                dXValue = (iTimeStep - 1) * grpOutput.TmaxOutput / (m_core.nEcopathTimeSteps - 1)

                'Weight plot
                resultLists(0).Add(dXValue, grpOutput.EcopathWeight(iTimeStep))
                'Number plot
                resultLists(1).Add(dXValue, grpOutput.EcopathNumber(iTimeStep))
                'Biomass plot
                resultLists(2).Add(dXValue, grpOutput.EcopathBiomass(iTimeStep))
            Next

            'Set the master pane title
            m_MasterPane.Title.Text = CStr(llbGroups.SelectedItem.ToString)

            ' Clear all panes
            For Each gp As GraphPane In m_MasterPane.PaneList
                gp.CurveList.Clear()
            Next

            AddCurveToGraphPane(ePaneTypes.Weight, resultLists(0), Color.Black)
            AddCurveToGraphPane(ePaneTypes.Number, resultLists(1), Color.Black)
            AddCurveToGraphPane(ePaneTypes.Biomass, resultLists(2), Color.Black)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal paneType As ePaneTypes, ByVal list As PointPairList, ByVal clr As Color)
            Dim gp As GraphPane = m_MasterPane.PaneList(CInt(paneType))
            gp.AddCurve(gp.Title.Text, list, clr, SymbolType.None)
        End Sub

        Private Sub UpdatePlots()
            Me.zgcZedGraphCntl.AxisChange()

            'Tell ZedGraph to auto layout the new GraphPanes
            'Cannot move that part up to the InitMasterPane, Title is dynamic here..??
            Dim g As Graphics = Me.CreateGraphics()
            m_MasterPane.SetLayout(g, PaneLayout.SquareColPreferred)
            g.Dispose()

            Me.zgcZedGraphCntl.Refresh()
        End Sub
#End Region 'Helper methods

    End Class

End Namespace