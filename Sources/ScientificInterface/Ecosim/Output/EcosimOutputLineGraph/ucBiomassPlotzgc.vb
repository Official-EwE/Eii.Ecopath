'==============================================================================
'
' $Log: ucBiomassPlotzgc.vb,v $
' Revision 1.35  2009/03/30 19:03:20  jeroens
' Fixed memory leak due to dispose/finalize confusion
'
' Revision 1.34  2009/03/27 19:36:25  jeroens
' Overlay -> Run
' Activated group display flags
'
' Revision 1.33  2009/03/23 21:13:04  jeroens
' Fixed issue 598
'
' Revision 1.32  2009/03/23 02:44:09  jeroens
' Uses ZGH option to show data under cursor in tooltip
'
' Revision 1.31  2009/03/19 16:55:57  jeroens
' Renamed LegendListBox to GroupListBox
'
' Revision 1.30  2009/03/17 17:18:22  jeroens
' EcosimCompleteDelegate -> Populate
'
' Revision 1.29  2009/03/12 17:30:08  joeb
' Another shoot at the EcosimCompletedDelegate() It checks statemonitor again.... hope this is the last time...
'
' Revision 1.28  2009/03/11 21:55:33  joeb
' Removed the EcosimCompletedDelegate() bug fix because it stop results from drawing HasEcosimRan was always false????
'
' Revision 1.27  2009/03/11 21:43:04  joeb
' Bug Fix EcosimCompletedDelegate() asserts when trying to get EcosimGroupOutput.Biomass is Ecosim has not been run
'
' Revision 1.26  2008/12/15 16:00:49  jeroens
' no message
'
' Revision 1.25  2008/12/04 06:03:59  sherman
' Fixed Show/Hide refresh bug
' Fixed disposed bug
'
' Revision 1.24  2008/12/04 03:34:44  sherman
' Added show/hide group.
'
' Revision 1.23  2008/12/03 22:29:22  sherman
' Fixed TS off by 1 year (Bug #545)
'
' Revision 1.22  2008/12/03 18:20:00  sherman
' Listened to Color change event
'
' Revision 1.21  2008/12/03 17:54:57  sherman
' Cleaned up the code
'
' Revision 1.20  2008/12/02 20:45:35  sherman
' Fixed autoscale bugs
'
' Revision 1.19  2008/11/29 19:00:11  sherman
' Updated bugs and rescaling in RunEcosim plot
'
' Revision 1.18  2008/11/26 16:00:23  jeroens
' Fixed issue 571
'
' Revision 1.17  2008/11/11 00:52:23  joeh
' Set plot type default to relative and scale default to auto
'
' Revision 1.16  2008/11/06 18:53:20  joeh
' Display Overlay list box only when Overly is selected
'
' Revision 1.15  2008/11/05 23:11:31  joeh
' Move Scale options into Graph options
'
' Revision 1.14  2008/11/05 22:41:05  joeh
' Use gray lines in cumulative plot
'
' Revision 1.13  2008/11/04 02:13:34  joeh
' Implement multiple selects for cumulative plot - Take two
'
' Revision 1.12  2008/11/03 18:40:00  joeh
' Implement multiple selects for cumulative plot
'
' Revision 1.11  2008/11/03 06:35:41  joeh
' Implement multiple selects for relative plot
'
' Revision 1.10  2008/11/03 00:58:21  joeh
' Implement Scale option - Take two
'
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
'==============================================================================

#Region " Imports "

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
        Private m_zgh As ZedGraphHelper = Nothing
        Private m_zgp As ZedGraphBiomassPlotter = Nothing
        Private m_sg As StyleGuide = Nothing

#Region " Constructor/Destructor "

        Public Sub New()
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = Nothing

            Me.m_sg = StyleGuide.GetInstance()

            Me.InitializeComponent()
            Me.SplitContainer1.Panel1Collapsed = True

            ' Show/Hide Groups
            cmd = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.AddControl(Me.tsbtnShowHideGroups)
            End If

            ' Style guide
            AddHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End Sub

        ''' <summary>
        ''' Clean up!
        ''' </summary>
        Private Sub ucBiomassPlotzgc_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

            ' Style guide
            RemoveHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.m_sg = Nothing

            Me.m_zgp.Clear()
            Me.m_zgp = Nothing
            Me.m_zgh = Nothing

            ' Show/Hide Groups
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.RemoveControl(Me.tsbtnShowHideGroups)
            End If

        End Sub

#End Region ' Constructor/Destructor

#Region " Public Interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the graph.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub PopulateGraph()
            Dim list1 As New PointPairList()
            Dim listSum As New PointPairList

            ' Double check
            If Me.m_zgh Is Nothing Or Me.m_zgp Is Nothing Then Return

            'jb added if ecosim has not run then the Ecosim EcoSimGroupOutputs will not be populated and can not be plotted
            If Not Me.m_core.StateMonitor.HasEcosimRan Then
                Return
            End If

            ' 1) Parepare new run
            m_zgp.PrepareNewRun()

            'Cumulative plot
            If CumulativeToolStripMenuItem.Checked Then
                If BiomassToolStripMenuItem.Checked Then
                    'Biomass
                    m_zgp.Title = My.Resources.HEADER_BIOMASS_CUMULATIVE
                    m_zgp.YaxisTitle = My.Resources.HEADER_BIOMASS_CUMULATIVE
                Else
                    'Catch
                    m_zgp.Title = My.Resources.HEADER_CATCH_CUMULATIVE
                    m_zgp.YaxisTitle = My.Resources.HEADER_CATCH_CUMULATIVE
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
                For iLBItem As Integer = 1 To Me.m_lbGroups.Items.Count - 1

                    Dim i As Integer = DirectCast(Me.m_lbGroups.Items(iLBItem), GroupListBox.GroupItem).Group.Index

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
                        If CumulativeToolStripMenuItem.Checked Then
                            'Cumulative highlight
                            m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphBiomassPlotter.eLineType.CumulativeBiomass, list1)
                        Else
                            'Cumulative selected
                            m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphBiomassPlotter.eLineType.CumulativeSelectedBiomass, list1)
                        End If
                    Else
                        'Catch
                        m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphBiomassPlotter.eLineType.CumulativeCatch, list1)
                    End If

                Next
            End If

            'Relative plot
            If RelativeToolStripMenuItem.Checked Then
                If BiomassToolStripMenuItem.Checked Then
                    'Biomass
                    m_zgp.Title = My.Resources.HEADER_RELATIVEBIOMASS
                    m_zgp.YaxisTitle = My.Resources.HEADER_RELATIVEBIOMASS
                Else
                    'Catch
                    m_zgp.Title = My.Resources.HEADER_RELATIVE_CATCH
                    m_zgp.YaxisTitle = My.Resources.HEADER_RELATIVE_CATCH
                End If

                ' todo: change to groups that listed in group box
                For j As Integer = 1 To Me.m_lbGroups.Items.Count - 1

                    Dim i As Integer = DirectCast(Me.m_lbGroups.Items(j), GroupListBox.GroupItem).Group.Index

                    ' No need to check; already done in populating group box

                    ''Catch
                    'If CatchToolStripMenuItem.Checked Then
                    '    'Find the sum of discard and landing of the group
                    '    Dim dblSumDiscardsLandings As Double = 0.0
                    '    For f As Integer = 1 To m_core.nFleets
                    '        dblSumDiscardsLandings = dblSumDiscardsLandings + m_core.FleetInputs(f).Discards(i) + m_core.FleetInputs(f).Landings(i)
                    '    Next f
                    '    'If sum=0 then skip this group
                    '    If Not dblSumDiscardsLandings > 0 Then Continue For
                    'End If

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
                        m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphBiomassPlotter.eLineType.RelativeBiomass, list1)
                    Else
                        'Catch
                        m_zgp.AddLine(m_core.EcoSimGroupInputs(i).Name, i, ZedGraphBiomassPlotter.eLineType.RelativeCatch, list1)
                    End If

                Next j
            End If


            ' 4) Tell the plotter it's finished
            Me.m_zgp.StoreRun()

            ' JS: I'm too impatient to figure out how to make the plotter do this
            Me.m_zgc.GraphPane.XAxis.Scale.Min = m_core.EcosimFirstYear
            Me.m_zgc.GraphPane.XAxis.Scale.Max = m_core.EcoSimModelParameters.NumberYears + m_core.EcosimFirstYear

            ' Draw timeseries 
            If BiomassToolStripMenuItem.Checked And RelativeToolStripMenuItem.Checked Then DrawTimeSeries()

            ' Calculate the Axis Scale Ranges
            Me.m_zgh.RescaleAndRedraw()
            Me.UpdateControls()
            Me.InvalidateGraph()

        End Sub

        Public Sub DrawTimeSeries()

            'ReDim m_abHasTSData(m_core.nGroups)
            'If Not m_bShowTSData Then Return Nothing

            'Dim ret(m_core.nGroups, m_EcosimModelParams.NumberYears) As Single
            Dim styGuide As StyleGuide = StyleGuide.GetInstance()
            Dim list1 As New PointPairList()

            Dim ts As cTimeSeries = Nothing

            ' Don't draw if it's biomass relative
            For i As Integer = 1 To m_core.nTimeSeries
                ts = m_core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        Dim gts As cGroupTimeSeries = DirectCast(ts, cGroupTimeSeries)
                        If gts.Enabled() Then
                            'm_abHasTSData(gts.GroupIndex) = True
                            Dim da() As Single = gts.ShapeData()
                            list1 = New PointPairList

                            For j As Integer = 1 To m_core.EcoSimModelParameters.NumberYears
                                If j < da.Length Then
                                    If da(j) > 0 Then
                                        ' Minus 1 because it should start with the first year
                                        list1.Add(j + m_core.EcosimFirstYear - 1, (da(j) / CSng(Math.Exp(gts.DataQ))) / m_core.StartBiomass(gts.GroupIndex))
                                    End If
                                End If
                            Next

                            ' Add line to graph.

                            m_zgp.AddLine(ts.Name, gts.GroupIndex, ZedGraphBiomassPlotter.eLineType.TimeSeries, list1)

                        End If

                    Else
                        Debug.Assert(True, "Relative Biomass TS should be cGroupTimeSeries object, check for import")
                    End If
                End If

            Next
        End Sub

        Public Sub OnCoreExecutionStateChanged()
            Me.m_zgp.PrepareNewRun()
            Me.PopulateRunsBox()
            Me.PopulateGroupBox()
        End Sub

        Public WriteOnly Property SSValue() As Single
            Set(ByVal value As Single)
                Me.tslblSSValue.Text = StyleGuide.GetInstance().FormatNumber(value)
            End Set
        End Property
#End Region

#Region " Events "

#Region " Menu Item Clicks "
        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_tsmShowMultipleRuns_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmShowMultipleRuns.Click
            m_tsmShowMultipleRuns.Checked = Not m_tsmShowMultipleRuns.Checked
            m_zgp.ShowMultipleRuns = m_tsmShowMultipleRuns.Checked
            Me.PopulateRunsBox()
            m_zgh.RescaleAndRedraw()
            SplitContainer1.Panel1Collapsed = Not m_tsmShowMultipleRuns.Checked
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub AnnualOutputToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnnualOutputToolStripMenuItem.Click
            AnnualOutputToolStripMenuItem.Checked = Not AnnualOutputToolStripMenuItem.Checked
            m_zgp.PrepareNewRun(True)
            Me.PopulateGroupBox()
            m_zgh.RescaleAndRedraw()
        End Sub

        Private Sub ShowLegendToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowLegendToolStripMenuItem.Click
            ShowLegendToolStripMenuItem.Checked = Not ShowLegendToolStripMenuItem.Checked
            m_zgp.ShowLegend = ShowLegendToolStripMenuItem.Checked
            m_zgh.RescaleAndRedraw()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub CumulativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CumulativeToolStripMenuItem.Click, CumulativeToolStripMenuItem.CheckedChanged
            RelativeToolStripMenuItem.Checked = Not CumulativeToolStripMenuItem.Checked
            Me.PopulateGraph()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub RelativeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RelativeToolStripMenuItem.Click, RelativeToolStripMenuItem.CheckedChanged
            CumulativeToolStripMenuItem.Checked = Not RelativeToolStripMenuItem.Checked
            Me.PopulateGraph()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub BiomassToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BiomassToolStripMenuItem.Click
            CatchToolStripMenuItem.Checked = Not BiomassToolStripMenuItem.Checked
            'Set default plot type to relative
            RelativeToolStripMenuItem.Checked = True
            Me.PopulateGraph()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub CatchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CatchToolStripMenuItem.Click
            BiomassToolStripMenuItem.Checked = Not CatchToolStripMenuItem.Checked
            'Set default plot type to relative
            RelativeToolStripMenuItem.Checked = True
            Me.PopulateGraph()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> Upon toggleing of menu item </summary>
        ''' -------------------------------------------------------------------
        Private Sub AutoScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tlsAutoScaleToolStripMenuItem.Click
            Me.m_zgh.AutoScaleOption = ZedGraphHelper.ScaleOptions.MaxOnly
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tstbxSetMin.Click, m_tstbxSetMax.Click
            m_tlsAutoScaleToolStripMenuItem.Checked = False
            m_tlsCustomScaleToolStripMenuItem.Checked = True
        End Sub

        '''' -------------------------------------------------------------------
        '''' <summary> Upon toggleing of menu item </summary>
        '''' -------------------------------------------------------------------
        Private Sub CustomScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tlsCustomScaleToolStripMenuItem.Click
            Double.TryParse(Me.m_tstbxSetMax.Text, Me.m_zgh.YScaleMax)
            Double.TryParse(Me.m_tstbxSetMin.Text, Me.m_zgh.YScaleMin)
            Me.m_zgh.AutoScaleOption = ZedGraphHelper.ScaleOptions.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSetMax_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tstbxSetMax.LostFocus ' m_tstbxSetMax.KeyPress, 
            Double.TryParse(TryCast(sender, ToolStripTextBox).Text, Me.m_zgh.YScaleMax)
            Me.m_zgh.AutoScaleOption = ZedGraphHelper.ScaleOptions.None
            Me.UpdateControls()
        End Sub

        Private Sub m_tstbxSetMin_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tstbxSetMin.LostFocus
            Double.TryParse(TryCast(sender, ToolStripTextBox).Text, Me.m_zgh.YScaleMin)
            Me.m_zgh.AutoScaleOption = ZedGraphHelper.ScaleOptions.None
            Me.UpdateControls()
        End Sub
#End Region
        ' Menu Items Clicks

        Private Sub ucBiomassPlotzgc_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' Design-time bail out
            If (Me.m_core Is Nothing) Then Return
            If (Me.m_core.EcoSimModelParameters Is Nothing) Then Return

            ' Santa's little helper :)
            Me.m_zgh = New ZedGraphHelper(Me.m_zgc)

            Me.m_zgh.AutoScaleOption = ZedGraphHelper.ScaleOptions.MaxOnly
            Me.m_zgh.YScaleMin = 0.0!
            Me.m_zgh.ShowPointValue = True

            Me.m_zgp = New ZedGraphBiomassPlotter(Me.m_zgc.GraphPane, Me.m_core, _
                    My.Resources.HEADER_RELATIVEBIOMASS, My.Resources.HEADER_YEAR, My.Resources.HEADER_RELATIVEBIOMASS)
            Me.m_zgp.ShowMultipleRuns = m_tsmShowMultipleRuns.Selected

            ' Set the axis
            Me.m_zgc.GraphPane.XAxis.Scale.Min = m_core.EcosimFirstYear
            Me.m_zgc.GraphPane.XAxis.Scale.Max = m_core.EcoSimModelParameters.NumberYears + m_core.EcosimFirstYear
            Me.m_zgc.AxisChange()
        End Sub

        Private Sub OnStyleGuideChanged(ByVal ct As StyleGuide.eChangeType)

            Me.InvalidateGraph()
            Me.PopulateGroupBox()
            Me.PopulateGraph()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary> When any of the indexes are changed </summary>
        ''' -------------------------------------------------------------------
        Private Sub lb_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_lbRuns.SelectedIndexChanged, m_lbGroups.SelectedIndexChanged
            Me.InvalidateGraph()
        End Sub

#End Region ' Events

#Region " Private Helpers "

        Private Sub InvalidateGraph()
            'Check the SelectedIndices collection
            For i As Integer = 0 To m_lbGroups.SelectedIndices.Count - 1
                'If "All" is in the SelectedIndices and it is not the only selected index
                If m_lbGroups.SelectedIndices(i) = 0 And m_lbGroups.SelectedIndices.Count > 1 Then
                    'Make sure it cannot be selected
                    m_lbGroups.SetSelected(i, False)
                    Exit For
                End If
            Next

            For i As Integer = m_lbGroups.SelectedIndices.Count - 1 To 0 Step -1
                m_zgp.Highlight(i, m_lbGroups.SelectedIndices.Count, m_lbGroups.SelectedIndices(i), m_lbRuns.SelectedIndex - 1)
            Next
            Me.m_zgc.Invalidate()
        End Sub

        Private Sub PopulateRunsBox()

            m_lbRuns.SuspendLayout()

            m_lbRuns.Items.Clear()
            m_lbRuns.Items.Add(My.Resources.GENERIC_VALUE_ALL)
            For i As Integer = 1 To m_zgp.NumRuns
                m_lbRuns.Items.Add(String.Format(My.Resources.ECOSIM_LABEL_RUN, i))
            Next
            m_lbRuns.SelectedIndex = 0
            m_lbRuns.ResumeLayout()

        End Sub

        Private Sub PopulateGroupBox()

            Dim sSumDiscardsLandings As Double = 0.0
            Dim group As cCoreGroupBase = Nothing
            Dim gi As GroupListBox.GroupItem = Nothing
            Dim groupSelected As cCoreGroupBase = Nothing
            Dim bIncludeGroup As Boolean = False

            If (Me.m_lbGroups.SelectedIndex > 0) Then
                groupSelected = DirectCast(Me.m_lbGroups.SelectedItem, GroupListBox.GroupItem).Group
            End If

            Me.m_lbGroups.SuspendLayout()
            Me.m_lbGroups.Items.Clear()
            Me.m_lbGroups.Items.Add(New GroupListBox.GroupItem(Nothing))

            For iGroup As Integer = 1 To m_core.nGroups

                ' Include visible groups only
                bIncludeGroup = Me.m_sg.GroupVisible(iGroup)

                ' Displaying catch and discards?
                If CatchToolStripMenuItem.Checked Then

                    ' Get sum of landings and discards for this group
                    sSumDiscardsLandings = 0
                    For f As Integer = 1 To m_core.nFleets
                        sSumDiscardsLandings += (Me.m_core.FleetInputs(f).Discards(iGroup) + Me.m_core.FleetInputs(f).Landings(iGroup))
                    Next f

                    ' Include when group has landings and/or discards
                    bIncludeGroup = bIncludeGroup And (sSumDiscardsLandings > 0)
                End If

                ' Include group?
                If bIncludeGroup Then
                    ' #Yes: add group to the list of options
                    group = Me.m_core.EcoPathGroupInputs(iGroup)
                    gi = New GroupListBox.GroupItem(group)
                    Me.m_lbGroups.Items.Add(gi)

                    If Object.ReferenceEquals(group, groupSelected) Then
                        Me.m_lbGroups.SelectedItem = gi
                    End If
                End If

            Next

            If Me.m_lbGroups.SelectedItem Is Nothing Then
                Me.m_lbGroups.SelectedIndex = 0
            End If

            m_lbGroups.ResumeLayout()
        End Sub

        Private Sub UpdateControls()
            m_tlsAutoScaleToolStripMenuItem.Checked = Me.m_zgh.AutoScaleOption = ZedGraphHelper.ScaleOptions.MaxOnly
            m_tlsCustomScaleToolStripMenuItem.Checked = Not m_tlsAutoScaleToolStripMenuItem.Checked
            Me.m_tstbxSetMax.Text = CStr(Me.m_zgh.YScaleMax)
            Me.m_tstbxSetMin.Text = CStr(Me.m_zgh.YScaleMin)
        End Sub

#End Region ' Private helpers

    End Class
    
End Namespace



