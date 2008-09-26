'==============================================================================
'
' $Log: ucBiomassPlotzgc.vb,v $
' Revision 1.1  2008/09/26 07:31:50  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.22  2008/09/23 16:13:51  jeroens
' TS 'Apply' -> 'Enable'
'
' Revision 1.21  2008/09/19 16:05:00  jeroens
' Fixed issue 542
'
' Revision 1.20  2008/09/02 14:47:30  jeroens
' Simplified ZedGraphHelper wrap interface
'
' Revision 1.19  2008/08/28 21:25:58  sherman
' Moved ZedGraphHelper to SI Shared
'
' Revision 1.18  2008/08/15 00:43:50  sherman
' Fixed plotting with timeseries bug in the x axis
'
' Revision 1.17  2008/08/14 01:56:07  sherman
' Added base year to Yaxis
'
' Revision 1.16  2008/07/31 17:31:10  jeroens
' Smoothened list box populating
'
' Revision 1.15  2008/07/31 17:21:00  jeroens
' Integrated LegendListBox
'
' Revision 1.14  2008/07/31 16:43:08  sherman
' Updated listbox debugging code - tsk sherman
'
' Revision 1.13  2008/07/31 01:46:19  sherman
' Fixed Time Series highlights
' - still need
'
' Revision 1.12  2008/07/30 20:21:54  sherman
' Debugging version.
'
' Revision 1.11  2008/07/30 00:37:55  sherman
' Added Timeseries
'
' Revision 1.10  2008/07/29 19:35:17  sherman
' Bug fixes
' - clear lines when core state changed
' - fixed year change bugs
'
' Revision 1.9  2008/07/29 17:11:46  jeroens
' List boxes no longer integral height
' Coloured lines at foreground
'
' Revision 1.8  2008/07/29 16:32:33  jeroens
' Prettified
'
' Revision 1.7  2008/07/29 16:14:48  sherman
' Added sum of squares
'
' Revision 1.6  2008/07/26 00:01:38  sherman
' Documentation
'
' Revision 1.5  2008/07/25 20:59:32  sherman
' Ported BiomassPlots to zedgraph in RunEcosim
'
' Revision 1.4  2008/07/25 06:44:35  sherman
' Updated Ecosim Biomass plot to zed graph.  Still not perfect.
'
' Revision 1.3  2008/07/25 00:56:09  sherman
' Bug fix - does not draw the very first time
'
' Revision 1.2  2008/07/24 23:05:29  sherman
' Work in progress... able to draw code
'
' Revision 1.1  2008/07/21 19:33:49  sherman
' initial upload
'
'==============================================================================

#Region "Imports Directive"

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ZedGraph

#End Region

Namespace Ecosim

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Ecosim output Biomass plot
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class ucBiomassPlotzgc

        Private m_core As cCore = cCore.GetInstance()
        Private m_ZGHelper As ZedGraphHelper = Nothing
        Private m_ZGPlotter As ZedGraphPlotter = Nothing

#Region " Constructor "
        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

            ' Santa's little helper :)
            m_ZGHelper = New ZedGraphHelper(m_zgc)

            ' 0.5) Call new graph
            m_ZGPlotter = New ZedGraphPlotter(m_zgc.GraphPane, m_core, "Biomass", "Year", "Relative biomass")

            m_ZGPlotter.Overlay = OverlayToolStripMenuItem.Selected

        End Sub
#End Region ' Constructor

#Region " Public Interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary> Called when the time stem is finished </summary>
        ''' -------------------------------------------------------------------
        Public Sub EcosimCompleteDelegate()
            Dim list1 As New PointPairList()

            ' 1) Parepare dataset
            m_ZGPlotter.PrepareDataset()

            ' todo: change to groups that listed in group box
            For i As Integer = 1 To m_core.nGroups
                list1 = New PointPairList
                For t As Integer = 0 To m_core.nEcosimTimeSteps
                    If AnnualOutputToolStripMenuItem.Checked Then
                        If t Mod cCore.N_MONTHS = 0 Then
                            list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) / m_core.EcoPathGroupOutputs(i).Biomass()))
                        End If
                    Else
                        ' 2) Add a single point to temp list
                        list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) / m_core.EcoPathGroupOutputs(i).Biomass()))
                    End If
                Next t
                ' 3) Store the line
                m_ZGPlotter.AddSingleData(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphPlotter.eLineType.Biomass, list1)
            Next i

            ' 4) Tell the plotter it's finished
            Me.m_ZGPlotter.StoreDataset()

            ' Draw the boxes, but should this be here or external?
            DrawTimeSeries()

            ' Make sure the group boxes say the correct items.
            PopulateGroupBoxes()

            ' Calculate the Axis Scale Ranges
            Me.m_ZGHelper.RescaleAndRedraw()
            Me.UpdateControls()

        End Sub

        Public Sub DrawTimeSeries()

            'ReDim m_abHasTSData(m_core.nGroups)
            'If Not m_bShowTSData Then Return Nothing

            'Dim ret(m_core.nGroups, m_EcosimModelParams.NumberYears) As Single
            Dim styGuide As StyleGuide = StyleGuide.GetInstance()
            Dim list1 As New PointPairList()

            Dim ts As cTimeSeries = Nothing

            For i As Integer = 1 To m_core.nTimeSeries
                ts = m_core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        Dim gts As cGroupTimeSeries = CType(ts, cGroupTimeSeries)
                        If gts.Enabled() Then
                            'm_abHasTSData(gts.GroupIndex) = True
                            Dim da() As Single = gts.ShapeData()
                            list1 = New PointPairList

                            ' This is hard coded for number of years only
                            For j As Integer = 1 To m_core.EcoSimModelParameters.NumberYears
                                If j < da.Length Then
                                    If da(j) > 0 Then
                                        list1.Add(j + m_core.EcosimFirstYear, (da(j) / CSng(Math.Exp(gts.DataQ))) / m_core.StartBiomass(gts.GroupIndex))

                                        'If j < 5 Then Console.WriteLine(String.Format("da(j)={0}, gts.DataQ={1}, SB={2}, gIndex={3}", da(j), gts.DataQ, m_Core.StartBiomass(gts.GroupIndex), gts.GroupIndex))
                                    End If
                                    'Else
                                    '    'Assign a NULL value
                                    '    points(j) = Single.NaN
                                End If
                            Next

                            ' Add line to graph.

                            m_ZGPlotter.AddSingleData(ts.Name, gts.GroupIndex, ZedGraphPlotter.eLineType.TimeSeries, list1)

                        End If

                    Else
                        Debug.Assert(True, "Relative Biomass TS should be cGroupTimeSeries object, check for import")
                    End If
                End If

            Next
        End Sub

        Public Sub OnCoreExecutionStateChanged()
            Me.m_ZGPlotter.PrepareDataset(True)
            Me.PopulateGroupBoxes()
        End Sub

        Public WriteOnly Property SSValue() As Single
            Set(ByVal value As Single)
                Me.tslblSSValue.Text = StyleGuide.GetInstance().FormatNumber(value)
            End Set
        End Property

#End Region

        ''' -------------------------------------------------------------------
        ''' <summary> When any of the indexes are changed </summary>
        ''' -------------------------------------------------------------------
        Private Sub lb_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbOverlay.SelectedIndexChanged, lbGroups.SelectedIndexChanged
            ' The plotter will set the highlight for this item.
            m_ZGPlotter.SetHighlight(lbGroups.SelectedIndex, lbOverlay.SelectedIndex - 1)
            Me.m_zgc.Invalidate()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Draws all the names </summary>
        ''' -------------------------------------------------------------------
        Private Sub PopulateGroupBoxes()

            lbOverlay.SuspendLayout()
            lbGroups.SuspendLayout()

            lbOverlay.Items.Clear()
            lbOverlay.Items.Add("All")
            For i As Integer = 1 To m_ZGPlotter.NumOverlays
                lbOverlay.Items.Add("Overlay " & (i).ToString)
            Next

            lbGroups.Items.Clear()
            lbGroups.Items.Add(New LegendListBox.EcopathGroupItem(Nothing))
            For i As Integer = 1 To m_core.nGroups
                lbGroups.Items.Add(New LegendListBox.EcopathGroupItem(m_core.EcoPathGroupInputs(i)))
            Next

            lbOverlay.ResumeLayout()
            lbGroups.ResumeLayout()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub OverlayToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OverlayToolStripMenuItem.Click
            OverlayToolStripMenuItem.Checked = Not OverlayToolStripMenuItem.Checked
            m_ZGPlotter.Overlay = OverlayToolStripMenuItem.Checked
            PopulateGroupBoxes()
            m_ZGHelper.RescaleAndRedraw()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub AnnualOutputToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnnualOutputToolStripMenuItem.Click
            AnnualOutputToolStripMenuItem.Checked = Not AnnualOutputToolStripMenuItem.Checked
            m_ZGPlotter.PrepareDataset(True)
            PopulateGroupBoxes()
            m_ZGHelper.RescaleAndRedraw()
        End Sub

        Private Sub ShowLegendToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowLegendToolStripMenuItem.Click
            ShowLegendToolStripMenuItem.Checked = Not ShowLegendToolStripMenuItem.Checked
            m_ZGPlotter.ShowLegend = ShowLegendToolStripMenuItem.Checked
            m_ZGHelper.RescaleAndRedraw()
        End Sub

        Private Sub OnAutoscale(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbAutoscale.Click
            Me.m_ZGHelper.AutoscalePane = True
            Me.UpdateControls()
        End Sub

        Private Sub OnCustomScale(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbCustomScale.Click
            Me.m_ZGHelper.AutoscalePane = False
            Me.UpdateControls()
        End Sub

        Private Sub OnScaleMinValidating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles m_tstbScaleMin.Validating

            Try
                Dim dTest As Double
                Double.TryParse(Me.m_tstbScaleMin.Text, dTest)
            Catch ex As Exception
                e.Cancel = True
            End Try

        End Sub

        Private Sub OnScaleMaxValidating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles m_tstbScaleMin.Validating

            Try
                Dim dTest As Double
                Double.TryParse(Me.m_tstbScaleMax.Text, dTest)
            Catch ex As Exception
                e.Cancel = True
            End Try

        End Sub

        Private Sub OnScaleMinValidated(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tstbScaleMin.Validated

            Double.TryParse(Me.m_tstbScaleMin.Text, Me.m_ZGHelper.YScaleMin)
            Me.m_ZGHelper.AutoscalePane = False
            Me.UpdateControls()

        End Sub

        Private Sub OnScaleMaxValidated(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tstbScaleMax.Validated

            Double.TryParse(Me.m_tstbScaleMax.Text, Me.m_ZGHelper.YScaleMax)
            Me.m_ZGHelper.AutoscalePane = False
            Me.UpdateControls()

        End Sub

        Private Sub UpdateControls()
            Me.m_tsbAutoscale.Checked = Me.m_ZGHelper.AutoscalePane
            Me.m_tsbCustomScale.Checked = Not Me.m_ZGHelper.AutoscalePane
            Me.m_tstbScaleMin.Text = CStr(Me.m_ZGHelper.YScaleMin)
            Me.m_tstbScaleMax.Text = CStr(Me.m_ZGHelper.YScaleMax)
        End Sub

    End Class
    
End Namespace



