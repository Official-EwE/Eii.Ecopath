'==============================================================================
'
' $Log: ucBiomassPlotzgc.vb,v $
' Revision 1.9  2008/11/01 00:13:25  joeh
' Implement Scale option
'
' Revision 1.8  2008/10/31 19:57:03  joeh
' Implement relative catch
'
' Revision 1.7  2008/10/30 22:46:59  joeh
' Implement cumulative catch plot - Take two
'
' Revision 1.6  2008/10/30 00:01:38  joeh
' Implement cumulative catch plot
'
' Revision 1.5  2008/10/29 00:15:13  joeh
' Implement cumulative biomass plot - Take three
'
' Revision 1.4  2008/10/25 00:37:05  joeh
' Implement cumulative biomass plot - Take two
'
' Revision 1.3  2008/10/24 19:36:47  joeh
' Implement cumulative biomass plot - Take one
'
' Revision 1.2  2008/10/02 19:04:07  sherman
' Modified Ecosim plot to start at 1 instead of 0
'
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
            m_ZGPlotter = New ZedGraphPlotter(m_zgc.GraphPane, m_core, "Biomass", "Year", "Cumulative biomass")

            m_ZGPlotter.Overlay = OverlayToolStripMenuItem.Selected

        End Sub
#End Region ' Constructor

#Region " Public Interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary> Called when the time stem is finished </summary>
        ''' -------------------------------------------------------------------
        Public Sub EcosimCompleteDelegate()
            Dim list1 As New PointPairList()
            Dim listSum As New PointPairList

            ' 1) Parepare dataset
            m_ZGPlotter.PrepareDataset()

            'Cumulative plot
            If CumulativeToolStripMenuItem.Checked = True Then
                If BiomassToolStripMenuItem.Checked Then
                    'Biomass
                    m_ZGPlotter.Title = "Cumulative biomass"
                    m_ZGPlotter.YaxisTitle = "Cumulative biomass"
                Else
                    'Catch
                    m_ZGPlotter.Title = "Cumulative catch"
                    m_ZGPlotter.YaxisTitle = "Cumulative catch"
                End If

                'Initialize listSum.Y=0
                listSum.Add(0, 0)
                For t As Integer = 1 To m_core.nEcosimTimeSteps
                    If AnnualOutputToolStripMenuItem.Checked Then
                        If t Mod cCore.N_MONTHS = 0 Then
                            listSum.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, 0.0)
                        End If
                    Else
                        ' 2) Add a single point to temp list
                        listSum.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, 0.0)
                    End If
                Next t

                ' todo: change to groups that listed in group box
                For i As Integer = 1 To m_core.nGroups
                    'Catch
                    If CatchToolStripMenuItem.Checked Then
                        'Find the sum of discard and landing of the group
                        Dim dblSumDiscardsLandings As Double = 0.0
                        For f As Integer = 1 To m_core.nFleets
                            dblSumDiscardsLandings = dblSumDiscardsLandings + m_core.FleetInputs(f).Discards(i) + m_core.FleetInputs(f).Landings(i)
                        Next f
                        'If sum=0 then skip this group
                        If Not dblSumDiscardsLandings > 0 Then Continue For
                    End If


                    list1 = New PointPairList
                    If BiomassToolStripMenuItem.Checked Then
                        'Biomass
                        list1.Add(0, m_core.EcoPathGroupOutputs(i).Biomass())
                    Else
                        'Catch
                        list1.Add(0, m_core.EcoPathGroupOutputs(i).Biomass() * m_core.EcoPathGroupOutputs(i).MortCoFishRate())
                    End If
                    For t As Integer = 1 To m_core.nEcosimTimeSteps
                        If AnnualOutputToolStripMenuItem.Checked Then
                            If t Mod cCore.N_MONTHS = 0 Then
                                If BiomassToolStripMenuItem.Checked Then
                                    'Biomass
                                    list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t)))
                                Else
                                    'Catch
                                    list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) * _
                                      (m_core.EcoSimGroupOutputs(i).FishMort(t) - m_core.EcoSimGroupOutputs(i).PredMort(t))))
                                End If
                            End If
                        Else
                            ' 2) Add a single point to temp list
                            If BiomassToolStripMenuItem.Checked Then
                                'Biomass
                                list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t)))
                            Else
                                'Catch
                                list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) * _
                                  (m_core.EcoSimGroupOutputs(i).FishMort(t) - m_core.EcoSimGroupOutputs(i).PredMort(t))))
                            End If
                        End If
                    Next t

                    'listSum=listSum+list1
                    For j As Integer = 0 To listSum.Count - 1
                        listSum(j).Y = listSum(j).Y + list1(j).Y
                    Next

                    For j As Integer = 0 To listSum.Count - 1
                        list1(j).Y = listSum(j).Y
                    Next

                    ' 3) Store the line
                    If BiomassToolStripMenuItem.Checked Then
                        'Biomass
                        m_ZGPlotter.AddSingleData(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphPlotter.eLineType.CumulativeBiomass, list1)
                    Else
                        'Catch
                        m_ZGPlotter.AddSingleData(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphPlotter.eLineType.CumulativeCatch, list1)
                    End If

                Next i
            End If

            'Relative plot
            If RelativeToolStripMenuItem.Checked = True Then
                If BiomassToolStripMenuItem.Checked Then
                    'Biomass
                    m_ZGPlotter.Title = "Relative biomass"
                    m_ZGPlotter.YaxisTitle = "Relative biomass"
                Else
                    'Catch
                    m_ZGPlotter.Title = "Relative catch"
                    m_ZGPlotter.YaxisTitle = "Relative catch"
                End If

                ' todo: change to groups that listed in group box
                For i As Integer = 1 To m_core.nGroups
                    'Catch
                    If CatchToolStripMenuItem.Checked Then
                        'Find the sum of discard and landing of the group
                        Dim dblSumDiscardsLandings As Double = 0.0
                        For f As Integer = 1 To m_core.nFleets
                            dblSumDiscardsLandings = dblSumDiscardsLandings + m_core.FleetInputs(f).Discards(i) + m_core.FleetInputs(f).Landings(i)
                        Next f
                        'If sum=0 then skip this group
                        If Not dblSumDiscardsLandings > 0 Then Continue For
                    End If

                    list1 = New PointPairList
                    list1.Add(0, 1) ' Brute force to make 0 TS 1
                    For t As Integer = 1 To m_core.nEcosimTimeSteps
                        If AnnualOutputToolStripMenuItem.Checked Then
                            If t Mod cCore.N_MONTHS = 0 Then
                                If BiomassToolStripMenuItem.Checked Then
                                    'Biomass
                                    list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t)) / m_core.EcoPathGroupOutputs(i).Biomass())
                                Else
                                    'Catch
                                    list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) * _
                                      (m_core.EcoSimGroupOutputs(i).FishMort(t) - m_core.EcoSimGroupOutputs(i).PredMort(t)) / (m_core.EcoPathGroupOutputs(i).Biomass() * m_core.EcoPathGroupOutputs(i).MortCoFishRate())))
                                End If
                            End If
                        Else
                            ' 2) Add a single point to temp list
                            If BiomassToolStripMenuItem.Checked Then
                                'Biomass
                                list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t)) / m_core.EcoPathGroupOutputs(i).Biomass())
                            Else
                                'Catch
                                'Console.WriteLine(m_core.EcoPathGroupInputs(i).Name)
                                list1.Add(CDbl(t / cCore.N_MONTHS) + m_core.EcosimFirstYear, CDbl(m_core.EcoSimGroupOutputs(i).Biomass(t) * _
                                  (m_core.EcoSimGroupOutputs(i).FishMort(t) - m_core.EcoSimGroupOutputs(i).PredMort(t)) / (m_core.EcoPathGroupOutputs(i).Biomass() * m_core.EcoPathGroupOutputs(i).MortCoFishRate())))
                            End If
                        End If
                    Next t

                    ' 3) Store the line
                    If BiomassToolStripMenuItem.Checked Then
                        'Biomass
                        m_ZGPlotter.AddSingleData(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphPlotter.eLineType.RelativeBiomass, list1)
                    Else
                        'Catch
                        m_ZGPlotter.AddSingleData(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphPlotter.eLineType.RelativeCatch, list1)
                    End If

                Next i
            End If


            ' 4) Tell the plotter it's finished
            Me.m_ZGPlotter.StoreDataset()

            ' Draw the boxes, but should this be here or external?
            DrawTimeSeries()

            ' Make sure the group boxes say the correct items.
            PopulateGroupBoxes()

            ' Calculate the Axis Scale Ranges
            Me.m_ZGHelper.RescaleAndRedraw()
            Me.UpdateControls()
            Me.newUpdateControls()

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
                If CatchToolStripMenuItem.Checked Then
                    Dim dblSumDiscardsLandings As Double = 0.0
                    For f As Integer = 1 To m_core.nFleets
                        dblSumDiscardsLandings = dblSumDiscardsLandings + m_core.FleetInputs(f).Discards(i) + m_core.FleetInputs(f).Landings(i)
                    Next f
                    If Not dblSumDiscardsLandings > 0 Then
                        Continue For
                    Else
                        lbGroups.Items.Add(New LegendListBox.EcopathGroupItem(m_core.EcoPathGroupInputs(i)))
                    End If
                Else
                    lbGroups.Items.Add(New LegendListBox.EcopathGroupItem(m_core.EcoPathGroupInputs(i)))
                End If
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

        Private Sub m_tstbxSetMin_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles m_tstbxSetMin.Validating
            Try
                Dim dTest As Double
                Double.TryParse(Me.m_tstbxSetMin.Text, dTest)
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

        Private Sub m_tstbxSetMax_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles m_tstbxSetMax.Validating
            Try
                Dim dTest As Double
                Double.TryParse(Me.m_tstbxSetMax.Text, dTest)
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

        Private Sub m_tstbxSetMin_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbxSetMin.Validated
            Double.TryParse(Me.m_tstbxSetMin.Text, Me.m_ZGHelper.YScaleMin)
            Me.m_ZGHelper.AutoscalePane = False
            Me.newUpdateControls()
        End Sub

        Private Sub OnScaleMaxValidated(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tstbScaleMax.Validated

            Double.TryParse(Me.m_tstbScaleMax.Text, Me.m_ZGHelper.YScaleMax)
            Me.m_ZGHelper.AutoscalePane = False
            Me.UpdateControls()

        End Sub

        Private Sub m_tstbxSetMax_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tstbxSetMax.Validated
            Double.TryParse(Me.m_tstbxSetMax.Text, Me.m_ZGHelper.YScaleMax)
            Me.m_ZGHelper.AutoscalePane = False
            Me.newUpdateControls()
        End Sub

        Private Sub UpdateControls()
            Me.m_tsbAutoscale.Checked = Me.m_ZGHelper.AutoscalePane
            Me.m_tsbCustomScale.Checked = Not Me.m_ZGHelper.AutoscalePane
            Me.m_tstbScaleMin.Text = CStr(Me.m_ZGHelper.YScaleMin)
            Me.m_tstbScaleMax.Text = CStr(Me.m_ZGHelper.YScaleMax)
        End Sub

        Private Sub newUpdateControls()
            AutoScaleToolStripMenuItem.Checked = Me.m_ZGHelper.AutoscalePane
            CustomScaleToolStripMenuItem.Checked = Not AutoScaleToolStripMenuItem.Checked
            Me.m_tstbxSetMax.Text = CStr(Me.m_ZGHelper.YScaleMax)
            Me.m_tstbxSetMin.Text = CStr(Me.m_ZGHelper.YScaleMin)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub CumulativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CumulativeToolStripMenuItem.Click
            RelativeToolStripMenuItem.Checked = Not CumulativeToolStripMenuItem.Checked
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub RelativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RelativeToolStripMenuItem.Click
            CumulativeToolStripMenuItem.Checked = Not RelativeToolStripMenuItem.Checked
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub BiomassToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BiomassToolStripMenuItem.Click
            CatchToolStripMenuItem.Checked = Not BiomassToolStripMenuItem.Checked
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub CatchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CatchToolStripMenuItem.Click
            BiomassToolStripMenuItem.Checked = Not CatchToolStripMenuItem.Checked
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub AutoScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoScaleToolStripMenuItem.Click
            Me.m_ZGHelper.AutoscalePane = True
            Me.newUpdateControls()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub CustomScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomScaleToolStripMenuItem.Click
            Me.m_ZGHelper.AutoscalePane = False
            Me.newUpdateControls()
        End Sub

        '''' -------------------------------------------------------------------
        '''' <summary> Upon toggleing of menu item </summary>
        '''' -------------------------------------------------------------------
        'Private Sub SetMaxToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SetMaxToolStripMenuItem.Click
        '    AutoScaleToolStripMenuItem.Checked = Not SetMaxToolStripMenuItem.Checked
        'End Sub

        '''' -------------------------------------------------------------------
        '''' <summary> Upon toggleing of menu item </summary>
        '''' -------------------------------------------------------------------
        'Private Sub SetMinToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SetMinToolStripMenuItem.Click
        '    SetMaxToolStripMenuItem.Checked = True
        '    AutoScaleToolStripMenuItem.Checked = Not SetMaxToolStripMenuItem.Checked
        'End Sub

        '''' -------------------------------------------------------------------
        '''' <summary> Upon toggleing of menu item </summary>
        '''' -------------------------------------------------------------------
        'Private Sub m_tstbxSetMax_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tstbxSetMax.TextChanged
        '    AutoScaleToolStripMenuItem.Checked = Not SetMaxToolStripMenuItem.Checked
        'End Sub

        '''' -------------------------------------------------------------------
        '''' <summary> Upon toggleing of menu item </summary>
        '''' -------------------------------------------------------------------
        'Private Sub m_tstbxSetMin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tstbxSetMin.TextChanged
        '    AutoScaleToolStripMenuItem.Checked = Not SetMaxToolStripMenuItem.Checked
        'End Sub
    End Class
    
End Namespace



