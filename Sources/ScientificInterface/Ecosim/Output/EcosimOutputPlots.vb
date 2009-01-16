'==============================================================================
'
' $Log: EcosimOutputPlots.vb,v $
' Revision 1.4  2009/01/16 18:30:37  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2008/12/15 15:53:26  jeroens
' no message
'
' Revision 1.2  2008/11/08 23:53:28  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:46  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.37  2008/09/23 16:14:56  jeroens
' TS 'Apply' -> 'Enable'
'
' Revision 1.36  2008/09/09 14:44:51  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.35  2008/08/02 03:04:14  jeroens
' Renamed resources
'
' Revision 1.34  2008/05/16 17:49:05  jeroens
' Removed time series button
' Show all fits only enabled when Sim has applied time series
'
' Revision 1.33  2008/05/07 01:44:30  jeroens
' Fixed bug 387
'
' Revision 1.32  2008/05/07 01:39:03  jeroens
' Fixed bugs 281, 378, 470
'
' Revision 1.31  2008/04/07 02:31:10  jeroens
' Cleaning up resources
'
' Revision 1.30  2008/04/01 15:35:24  jeroens
' Hack-fixed bug 445 (this code was awful but it's too much work to fix this properly)
'
' Revision 1.29  2008/02/13 00:06:09  jeroens
' Uses renamed show all fits form
'
' Revision 1.28  2008/02/11 03:54:57  jeroens
' Years based on Ecosim years, no longer on years stated in first available TS
'
' Revision 1.27  2008/02/08 01:07:51  jeroens
' Fixed issues 364, 376
'
' Revision 1.26  2008/02/05 18:27:34  jeroens
' Fixed bug 411
'
' Revision 1.25  2008/01/25 03:09:46  jeroens
' Woops
'
' Revision 1.23  2008/01/21 04:06:39  jeroens
' Fixed shape max scale issues, once and for all
'
' Revision 1.22  2007/12/28 22:30:14  sherman
' Removed Minor Ticks
'
' Revision 1.21  2007/12/10 00:19:47  jeroens
' * Tweaked and polished even more
'
' Revision 1.20  2007/12/09 22:11:09  jeroens
' * Restyled
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Controls
Imports ScientificInterface.Other
Imports EwEUtils.Commands
Imports ZedGraph
Imports System.IO
Imports System.Text

#End Region

Namespace Ecosim

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class EcosimOutputPlots

#Region " Variables "

        Private m_core As cCore
        Private m_EcosimModelParams As cEcoSimModelParameters
        Private m_MasterPane As New MasterPane
        Private m_sg As StyleGuide = Nothing
        Private m_zgh As ZedGraphHelper = Nothing

        Private Enum ePaneTypes As Integer
            Biomass = 0
            ConsumptionBiomass = 1
            PredationMortality
            Mortality
            FeedingTime
            Prey
            Yield
            AvgWeightOrProdCons
        End Enum

        Private Enum eCurveTypes As Integer
            EcosimOutput
            TimeSeries
        End Enum

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            ' Get the core reference
            Me.m_core = cCore.GetInstance()
            Me.m_EcosimModelParams = m_core.EcoSimModelParameters()
            Me.m_sg = StyleGuide.GetInstance()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        Private Sub EcosimOutputPlots_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Add group names into listbox
            For i As Integer = 1 To m_core.nGroups
                Dim item As String = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, i, m_core.EcoSimGroupInputs(i).Name)
                lbGroups.Items.Add(item)
            Next

            InitMasterPane()

            CreatePane(ePaneTypes.Biomass, My.Resources.HEADER_BIOMASS)
            CreatePane(ePaneTypes.ConsumptionBiomass, My.Resources.HEADER_CONSUMPTIONBIOMASS)
            CreatePane(ePaneTypes.PredationMortality, My.Resources.ECOSIM_PLOT_CAPTION_PREDMORT)
            CreatePane(ePaneTypes.Mortality, My.Resources.ECOSIM_PLOT_CAPTION_MORT)
            CreatePane(ePaneTypes.FeedingTime, My.Resources.HEADER_FEEDINGTIME)
            CreatePane(ePaneTypes.Prey, My.Resources.ECOSIM_PLOT_CAPTION_PREYPERC)
            CreatePane(ePaneTypes.Yield, My.Resources.HEADER_YIELD)
            ' Need to test StanZaGroup..Sometimes displayed as Average weight
            ' update it in the actual rendering.
            CreatePane(ePaneTypes.AvgWeightOrProdCons, My.Resources.ECOSIM_PLOT_CAPTION_PRODCONS)

            'Me.m_zgh = New ZedGraphHelper(Me.zgcPlots, 8)
            lbGroups.SelectedIndex = 0

            Me.UpdateControls()
            Me.UpdateColors()

            Me.MessageSources = New eCoreComponentType() {eCoreComponentType.TimeSeries}
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Private Sub EcosimOutputPlots_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.MessageSources = Nothing
        End Sub

        Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType)
            If ((changeType And StyleGuide.eChangeType.Colours) = StyleGuide.eChangeType.Colours) Then
                Me.UpdateColors()
            End If
        End Sub

        ''' <summary>
        ''' Event hander to display results for another group
        ''' </summary>
        Private Sub lbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbGroups.SelectedIndexChanged
            AddCurves()
            AddTSData()
            UpdatePlots()

            'Display pred and prey ranks
            DisplayRanks()
        End Sub

        ''' <summary>
        ''' Event handler for closing the form
        ''' </summary>
        Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        ''' <summary>
        ''' Event handler for saving the value to .csv file
        ''' </summary>
        Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click

            Dim bSaveAnnual As Boolean = False 'Save each time steps
            Dim fbDlg As New FolderBrowserDialog

            With fbDlg
                .SelectedPath = My.Settings.LastSelectedDirectory
                .ShowNewFolderButton = True
                .Description = My.Resources.ECOSIM_PROMPT_SAVEDESTINATION
            End With

            If (fbDlg.ShowDialog() <> Windows.Forms.DialogResult.OK) Then Return
            If (String.IsNullOrEmpty(fbDlg.SelectedPath)) Then Return

            Select Case MsgBox(My.Resources.ECOSIM_PROMPT_SAVEANNUAL, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    bSaveAnnual = True
                Case MsgBoxResult.No
                    bSaveAnnual = False
                Case MsgBoxResult.Cancel
                    Return
            End Select

            'Plot 0 - 5 
            '"Biomass, Mortality, Yield, Cons/biom, Feeding time, Weight, ";
            'Plot 6 - Predation
            'Plot 7 - Prey
            For i As Integer = 0 To 7
                Me.SaveOutputToFile(fbDlg.SelectedPath, bSaveAnnual, i)
            Next

        End Sub

        ''' <summary>
        ''' Custom draw method to render a group name in a particualr color.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub lb_DrawItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles lbPredRanks.DrawItem, lbPreyRanks.DrawItem

            ' get the sender of this event
            Dim lb As ListBox = CType(sender, ListBox)
            Dim group As cEcoPathGroupInput = Nothing
            Dim sg As StyleGuide = StyleGuide.GetInstance()

            If lb Is Nothing Then Return
            If e.Index = -1 Then Return

            If (TypeOf lb.Items(e.Index) Is cEcoPathGroupInput) Then
                group = DirectCast(lb.Items(e.Index), cEcoPathGroupInput)
                Me.DrawCustomText(e, group.Name, sg.GroupColor(Me.m_core, group.Index), e.Bounds)
            End If

        End Sub

        Private Sub btnTimeSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("LoadTimeSeries")
            If (cmd IsNot Nothing) Then cmd.Invoke()
        End Sub

        Private Sub btnShowAllFits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowAllFits.Click
            Dim showAllFitsDlg As New frmShowAllFits
            showAllFitsDlg.ShowDialog()
        End Sub

#End Region ' Event handlers

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If (msg.Source = eCoreComponentType.TimeSeries) Then
                Me.UpdateControls()
            End If
        End Sub

#End Region ' Overrides

#Region " Helper methods "

        ''' <summary>
        ''' Initialize the configuration for Zedgraph's master pane.
        ''' </summary>
        Private Sub InitMasterPane()

            'Get the master pane
            m_MasterPane = zgcPlots.MasterPane

            m_MasterPane.PaneList.Clear()
            'Disable the master pane legend
            m_MasterPane.Legend.IsVisible = False
            'Make the border invisible
            m_MasterPane.Border.IsVisible = False

            m_MasterPane.Title.IsVisible = True
            m_MasterPane.Title.FontSpec.Size = 12
            m_MasterPane.IsFontsScaled = False

        End Sub

        ''' <summary>
        ''' Initialize the configuration for individual graph plot
        ''' </summary>
        Private Sub InitGraphPane(ByVal strTitle As String, ByRef pane As GraphPane)

            pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = True
            pane.Title.FontSpec.Size = 12

            pane.XAxis.Scale.FontSpec.Size = 12
            pane.XAxis.Title.FontSpec.Size = 12

            pane.YAxis.Scale.FontSpec.Size = 12
            pane.YAxis.Title.FontSpec.Size = 12

            pane.XAxis.Scale.Min = CDbl(Me.m_core.EcosimFirstYear)
            pane.XAxis.Scale.Max = CDbl(Me.m_core.EcosimFirstYear + (m_core.nEcosimTimeSteps / cCore.N_MONTHS))
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

            Me.UpdateColors()

        End Sub

        ''' <summary>
        ''' To add one plot into the main graph
        ''' </summary>
        Private Sub CreatePane(ByVal PaneNo As ePaneTypes, ByVal strTitle As String)
            'Define a new graph pane
            Dim pane As New GraphPane

            Debug.Assert(m_MasterPane.PaneList.Count = PaneNo)

            ' Yo!
            InitGraphPane(strTitle, pane)

            'Add the graphPane to the masterPane
            m_MasterPane.Add(pane)
        End Sub

        ''' <summary>
        ''' Init the data structure for storing the results to feed into graph
        ''' </summary>
        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)

            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next

        End Sub

        ''' <summary>
        ''' Get the values from core and add them into graph
        ''' </summary>
        Private Sub AddCurves()

            ' ToDo_JS: Find way to update colours in existing curves [CurveList.Item(x).Color = ...]

            'Add single curve into graph first
            'Results data structure
            Dim resultLists As New List(Of PointPairList)
            Dim mortResultLists As New List(Of PointPairList)
            Dim dXValue As Double = 0
            Dim iGroup As Integer = lbGroups.SelectedIndex + 1
            Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(iGroup)

            InitLists(resultLists, 5)
            InitLists(mortResultLists, 3)

            For i As Integer = 1 To m_core.nEcosimTimeSteps

                dXValue = Me.m_core.EcosimFirstYear + (i / cCore.N_MONTHS)

                'Biomass plot
                resultLists(0).Add(dXValue, grpOutput.Biomass(i))
                'Consumption/biomass plot
                resultLists(1).Add(dXValue, grpOutput.ConsumpBiomass(i))
                'Feeding time plot
                resultLists(2).Add(dXValue, grpOutput.FeedingTime(i))
                'Yield plot
                resultLists(3).Add(dXValue, grpOutput.Yield(i))
                If grpOutput.isMultiStanza() Then
                    'Average weight
                    resultLists(4).Add(dXValue, grpOutput.AvgWeight(i))
                Else
                    resultLists(4).Add(dXValue, grpOutput.ProdConsump(i))
                End If
                'Mortality plot
                mortResultLists(0).Add(dXValue, grpOutput.TotalMort(i))
                mortResultLists(1).Add(dXValue, grpOutput.PredMort(i))
                mortResultLists(2).Add(dXValue, grpOutput.FishMort(i))
            Next

            'Set the master pane title
            m_MasterPane.Title.Text = CStr(lbGroups.SelectedItem)

            ' Clear all panes
            For Each gp As GraphPane In m_MasterPane.PaneList
                gp.CurveList.Clear()
            Next

            AddCurveToGraphPane(ePaneTypes.Biomass, resultLists(0), Color.Black)
            AddCurveToGraphPane(ePaneTypes.ConsumptionBiomass, resultLists(1), Color.Black)
            AddCurveToGraphPane(ePaneTypes.FeedingTime, resultLists(2), Color.Black)
            AddCurveToGraphPane(ePaneTypes.Yield, resultLists(3), Color.Black)

            If grpOutput.isMultiStanza() Then
                UpdateGraphPaneTitle(ePaneTypes.AvgWeightOrProdCons, My.Resources.HEADER_AVGERAGEWEIGHT)
                AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, resultLists(4), Color.Black)
            Else
                UpdateGraphPaneTitle(ePaneTypes.AvgWeightOrProdCons, My.Resources.ECOSIM_PLOT_CAPTION_PRODCONS)
                AddCurveToGraphPane(ePaneTypes.AvgWeightOrProdCons, resultLists(4), Color.Black)
            End If

            Dim colors As Color() = {Color.Black, Color.Red, Color.Blue}
            AddCurvesToGraphPane(ePaneTypes.Mortality, mortResultLists, eCurveTypes.EcosimOutput, colors)

            'Dynamic size depending on the Prey number
            Dim predResultsLists As New List(Of PointPairList)
            Dim cnt As Integer = 0
            Dim predColor As New List(Of Color)
            Dim sg As StyleGuide = StyleGuide.GetInstance()

            'Predation mortality plot
            For i As Integer = 1 To m_core.nLivingGroups
                If grpOutput.isPred(i) Then
                    Dim list As New PointPairList
                    predResultsLists.Add(list)
                    predColor.Add(sg.GroupColor(Me.m_core, i))
                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                        dXValue = Me.m_core.EcosimFirstYear + (j / cCore.N_MONTHS)
                        predResultsLists(cnt).Add(dXValue, grpOutput.Predation(i, j))
                    Next
                    cnt += 1
                End If
            Next

            AddCurvesToGraphPane(ePaneTypes.PredationMortality, predResultsLists, eCurveTypes.EcosimOutput, predColor.ToArray())

            'Dynamic size depending on the Predator number
            Dim preyPercResultsLists As New List(Of PointPairList)
            cnt = 0
            Dim preyColor As New List(Of Color)

            'Prey %
            For i As Integer = 1 To m_core.nLivingGroups
                If grpOutput.isPrey(i) Then
                    Dim list As New PointPairList
                    preyPercResultsLists.Add(list)
                    preyColor.Add(sg.GroupColor(Me.m_core, i))
                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                        dXValue = Me.m_core.EcosimFirstYear + (j / cCore.N_MONTHS)
                        preyPercResultsLists(cnt).Add(dXValue, grpOutput.PreyPercentage(i, j) * 100)
                    Next
                    cnt += 1
                End If
            Next

            'For the prey percentage
            AddCurvesToGraphPane(ePaneTypes.Prey, preyPercResultsLists, eCurveTypes.EcosimOutput, preyColor.ToArray())

        End Sub

        Private Sub AddTSData()

            Dim listsTS As List(Of PointPairList)

            listsTS = GetTSData(eTimeSeriesType.BiomassRel)
            AddCurvesToGraphPane(ePaneTypes.Biomass, listsTS, eCurveTypes.TimeSeries)

            listsTS = GetTSData(eTimeSeriesType.TotalMortality)
            AddCurvesToGraphPane(ePaneTypes.Mortality, listsTS, eCurveTypes.TimeSeries, New Color() {Color.Green})

            listsTS = GetTSData(eTimeSeriesType.FishingMortality)
            AddCurvesToGraphPane(ePaneTypes.Mortality, listsTS, eCurveTypes.TimeSeries, New Color() {Color.Red})

            Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(lbGroups.SelectedIndex + 1)
            If grpOutput.isMultiStanza() Then
                listsTS = GetTSData(eTimeSeriesType.AverageWeight)
                AddCurvesToGraphPane(ePaneTypes.AvgWeightOrProdCons, listsTS, eCurveTypes.TimeSeries)
            End If

            listsTS = GetTSData(eTimeSeriesType.FishingEffort)
            listsTS.AddRange(GetTSData(eTimeSeriesType.Catches))
            listsTS.AddRange(GetTSData(eTimeSeriesType.CatchesForcing))
            AddCurvesToGraphPane(ePaneTypes.Yield, listsTS, eCurveTypes.TimeSeries)

        End Sub

        Private Function GetTSData(ByVal TSType As eTimeSeriesType) As List(Of PointPairList)

            Dim listTS As New List(Of PointPairList)
            Dim iGroup As Integer = lbGroups.SelectedIndex + 1

            Dim ts As cTimeSeries = Nothing

            For i As Integer = 1 To m_core.nTimeSeries
                ts = m_core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = TSType Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        Dim gts As cGroupTimeSeries = CType(ts, cGroupTimeSeries)
                        If gts.GroupIndex = iGroup Then
                            If gts.Enabled() Then
                                listTS.Add(TStoPointPair(ts))
                            End If
                        End If
                    End If
                End If
            Next

            Return listTS

        End Function

        Private Function TStoPointPair(ByRef ts As cTimeSeries) As PointPairList

            Dim list As New PointPairList
            Dim dScale As Single = 1.0F

            If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or _
                 ts.TimeSeriesType = eTimeSeriesType.TotalMortality Or _
                    ts.TimeSeriesType = eTimeSeriesType.AverageWeight Then
                If ts.DataQ > 0 Then
                    dScale = 1.0F / CSng(Math.Exp(ts.DataQ))
                End If
            End If

            Dim da() As Single = ts.ShapeData()
            For j As Integer = 1 To da.Length - 1
                If da(j) <> 0 Then
                    list.Add(Me.m_core.EcosimFirstYear + j - 1, da(j) * dScale)
                End If
            Next
            Return list
        End Function

        Private Sub UpdatePlots()
            zgcPlots.AxisChange()

            'Tell ZedGraph to auto layout the new GraphPanes
            'Cannot move that part up to the InitMasterPane, Title is dynamic here..??
            Dim g As Graphics = Me.CreateGraphics()
            m_MasterPane.SetLayout(g, PaneLayout.SquareColPreferred)
            g.Dispose()

            zgcPlots.Refresh()

        End Sub

        Private Sub DisplayRanks()

            Dim iGroup As Integer = lbGroups.SelectedIndex + 1
            Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(iGroup)

            Dim avgPredConsumption As New List(Of Single)
            Dim avgPredIndex As New List(Of Integer)

            Dim avgPreyConsumption As New List(Of Single)
            Dim avgpreyIndex As New List(Of Integer)

            For i As Integer = 1 To m_core.nLivingGroups

                If grpOutput.isPred(i) Then
                    avgPredConsumption.Add(grpOutput.AvgPredConsumption(i))
                    avgPredIndex.Add(i)
                End If

                If grpOutput.isPrey(i) Then
                    avgPreyConsumption.Add(grpOutput.AvgPreyConsumption(i))
                    avgpreyIndex.Add(i)
                End If

            Next

            Dim iAvgPred() As Integer = avgPredIndex.ToArray

            SortRanks(avgPredConsumption.ToArray, iAvgPred)

            AddItemsToList(lbPredRanks, iAvgPred)

            Dim iAvgPrey() As Integer = avgpreyIndex.ToArray
            SortRanks(avgPreyConsumption.ToArray, iAvgPrey)

            AddItemsToList(lbPreyRanks, iAvgPrey)

        End Sub

        Private Sub SortRanks(ByRef a() As Single, ByRef b() As Integer)
            'Use a simple bubblesort algorithm here

            For i As Integer = 0 To a.Length - 2
                For j As Integer = a.Length - 1 To i + 1 Step -1
                    If a(j) > a(j - 1) Then
                        ' swap 
                        Dim sTmp As Single = a(j)
                        a(j) = a(j - 1)
                        a(j - 1) = sTmp
                        Dim iTmp As Integer = b(j)
                        b(j) = b(j - 1)
                        b(j - 1) = iTmp
                    End If
                Next
            Next

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="l">Listbox to add items to.</param>
        ''' <param name="aiGroupIndex">Array of group index values.</param>
        ''' <remarks></remarks>
        Private Sub AddItemsToList(ByRef l As ListBox, ByRef aiGroupIndex() As Integer)

            Dim group As cEcoPathGroupInput = Nothing

            l.Items.Clear()

            For i As Integer = 0 To aiGroupIndex.Length - 1

                'Dim item As cColorItem = m_PoolColor(items(i))
                group = Me.m_core.EcoPathGroupInputs(aiGroupIndex(i))
                l.Items.Add(group)
            Next

        End Sub

        Private Sub UpdateGraphPaneTitle(ByVal paneType As ePaneTypes, ByVal strTitle As String)
            Dim gp As GraphPane = m_MasterPane.PaneList(CInt(paneType))
            gp.Title.Text = strTitle
        End Sub

        ''' <summary>
        ''' Add one curve into the graph pane
        ''' </summary>
        ''' <param name="paneType">Index of the graph pane</param>
        ''' <param name="list">The data points for the curve</param>
        Private Sub AddCurveToGraphPane(ByVal paneType As ePaneTypes, ByVal list As PointPairList, ByVal clr As Color)
            Dim gp As GraphPane = m_MasterPane.PaneList(CInt(paneType))
            gp.AddCurve(gp.Title.Text, list, clr, SymbolType.None)
        End Sub

        ''' <summary>
        ''' Add multiple curves into the graph pane
        ''' </summary>
        ''' <param name="paneType">The idnex of the graph pane</param>
        ''' <param name="lists">The lists of data points for the multiple curves</param>
        ''' <param name="aclr">Optional lists of colors user wants to render for the curves</param>
        ''' <remarks>Overloaded method with different color options</remarks>
        Private Sub AddCurvesToGraphPane(ByVal paneType As ePaneTypes, ByRef lists As List(Of PointPairList), _
                ByVal curveType As eCurveTypes, Optional ByRef aclr As Color() = Nothing)

            Dim gp As GraphPane = m_MasterPane.PaneList(CInt(paneType))
            Dim clr As Color = Nothing
            Dim rotator As New ColorSymbolRotator

            For i As Integer = 0 To lists.Count - 1

                If Object.ReferenceEquals(aclr, Nothing) Then
                    clr = rotator.NextColor
                Else
                    If (i >= aclr.Length) Then
                        clr = rotator.NextColor
                    Else
                        clr = aclr(i)
                    End If
                End If

                Select Case curveType
                    Case eCurveTypes.EcosimOutput
                        gp.AddCurve(gp.Title.Text, lists(i), clr, SymbolType.None)

                    Case eCurveTypes.TimeSeries
                        Dim curve As LineItem = gp.AddCurve(gp.Title.Text, lists(i), clr, SymbolType.Circle)
                        curve.Line.IsVisible = False
                        curve.Symbol.Size = 4.0
                End Select
            Next

        End Sub

        ''' <summary>
        ''' Helper methods to draw a custom listcontrol item 
        ''' </summary>
        ''' <param name="e">DrawItemEventArgs sent by DrawItem event handler</param>
        ''' <param name="txt">The text beside the colorbox</param>
        ''' <remarks>This method is called by both Listbox drawItem event handlers</remarks>
        Private Sub DrawCustomText(ByVal e As System.Windows.Forms.DrawItemEventArgs, ByRef txt As String, ByVal c As Color, ByRef rect As Rectangle)
            ' Do nothing if there is no data
            If e.Index = -1 Then Return

            'If the item is selected, draw the correct background color
            e.DrawBackground()
            e.DrawFocusRectangle()

            'Get the listbox's graphics object
            Dim g As Graphics = e.Graphics
            'Draw text 
            g.DrawString(txt, e.Font, New SolidBrush(c), rect)

        End Sub

        Private Sub SaveOutputToFile(ByVal strPath As String, ByVal bSaveAnnual As Boolean, ByVal iPlot As Integer)

            Dim strFile As String = String.Empty
            If bSaveAnnual Then
                Select Case iPlot
                    Case 0
                        strFile = "EwE6-Simplot_annual_biomass.csv"
                    Case 1
                        strFile = "EwE6-Simplot_annual_mortality.csv"
                    Case 2
                        strFile = "EwE6-Simplot_annual_yield.csv"
                    Case 3
                        strFile = "EwE6-Simplot_annual_cons_biom.csv"
                    Case 4
                        strFile = "EwE6-Simplot_annual_feedingtime.csv"
                    Case 5
                        strFile = "EwE6-Simplot_annual_weight.csv"
                    Case 6
                        strFile = "EwE6-Simplot_annual_predation.csv"
                    Case 7
                        strFile = "EwE6-Simplot_annual_prey.csv"
                End Select
            Else
                Select Case iPlot
                    Case 0
                        strFile = "EwE6-Simplot_biomass.csv"
                    Case 1
                        strFile = "EwE6-Simplot_mortality.csv"
                    Case 2
                        strFile = "EwE6-Simplot_yield.csv"
                    Case 3
                        strFile = "EwE6-Simplot_cons_biom.csv"
                    Case 4
                        strFile = "EwE6-Simplot_feedingtime.csv"
                    Case 5
                        strFile = "EwE6-Simplot_weight.csv"
                    Case 6
                        strFile = "EwE6-Simplot_predation.csv"
                    Case 7
                        strFile = "EwE6-Simplot_prey.csv"
                End Select
            End If


            Dim strFileName As String = Path.Combine(strPath, strFile)
            Dim strPrompt As String = String.Empty
            Dim strCaption As String = My.Resources.GENERIC_PROMPT_SAVEDATATOCSV_CAPTION

            If File.Exists(strFileName) Then

                strPrompt = String.Format(My.Resources.GENERIC_PROMPT_OVERWRITEFILE, strFileName)
                Dim bValid As DialogResult = MessageBox.Show(strPrompt, strCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                If bValid = Windows.Forms.DialogResult.Cancel Then
                    Return
                ElseIf bValid = Windows.Forms.DialogResult.No Then
                    Dim cmdh As CommandHandler = CommandHandler.GetInstance()
                    Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

                    cmdFS.Invoke(My.Resources.FILEFILTER_CSV)
                    If cmdFS.Result = Windows.Forms.DialogResult.OK Then
                        strFileName = cmdFS.FileName
                    Else
                        Return
                    End If
                End If
            End If

            Dim strModelDetails As String = GetModelDetails()

            If iPlot <= 5 Then

                Dim data(m_core.nGroups, m_core.nEcosimTimeSteps) As Single
                For i As Integer = 1 To m_core.nGroups
                    Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(i)
                    For j As Integer = 1 To m_core.nEcosimTimeSteps
                        Select Case iPlot
                            Case 0
                                data(i, j) = grpOutput.Biomass(j)
                            Case 1
                                data(i, j) = grpOutput.TotalMort(j)
                            Case 2
                                data(i, j) = grpOutput.Yield(j)
                            Case 3
                                data(i, j) = grpOutput.ConsumpBiomass(j)
                            Case 4
                                data(i, j) = grpOutput.FeedingTime(j)
                            Case 5
                                If grpOutput.isMultiStanza Then
                                    data(i, j) = grpOutput.AvgWeight(j)
                                Else
                                    data(i, j) = grpOutput.ProdConsump(j)
                                End If

                        End Select
                    Next

                Next

                Dim astrGroupNames As String = GetAllGroupNames()
                Me.SaveDataToFile(strFileName, bSaveAnnual, data, strModelDetails, astrGroupNames)

            ElseIf iPlot = 6 Then
                'For predation and prey, the saving scheme is different
                Dim iGroup As Integer = lbGroups.SelectedIndex + 1
                Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(iGroup)

                Dim cntPred As Integer = 0
                Dim predNames As New StringBuilder
                For i As Integer = 1 To m_core.nLivingGroups
                    If grpOutput.isPred(i) Then
                        cntPred += 1
                        predNames.Append("""" & m_core.EcoSimGroupOutputs(i).Name & """")
                        predNames.Append(",")
                    End If
                Next

                Dim predData(cntPred, m_core.nEcosimTimeSteps) As Single
                cntPred = 1

                For i As Integer = 1 To m_core.nLivingGroups
                    If grpOutput.isPred(i) Then
                        For j As Integer = 1 To m_core.nEcosimTimeSteps
                            predData(cntPred, j) = grpOutput.Predation(i, j)
                        Next
                        cntPred += 1
                    End If
                Next

                Dim predMdlDetails As String = String.Format("{0},Prey:,{1}, Gives predation mortality rates for this group", strModelDetails, grpOutput.Name)
                SaveDataToFile(strFileName, bSaveAnnual, predData, predMdlDetails, predNames.ToString)

            ElseIf iPlot = 7 Then 'Plot 7 - Prey (For the selected group)

                Dim iGroup As Integer = lbGroups.SelectedIndex + 1
                Dim grpOutput As cEcosimGroupOutput = m_core.EcoSimGroupOutputs(iGroup)

                Dim cntPrey As Integer = 0
                Dim preyNames As New StringBuilder

                For i As Integer = 1 To m_core.nLivingGroups
                    If grpOutput.isPrey(i) Then
                        cntPrey += 1
                        preyNames.Append("""" & m_core.EcoSimGroupOutputs(i).Name & """")
                        preyNames.Append(",")
                    End If
                Next

                Dim preyData(cntPrey, m_core.nEcosimTimeSteps) As Single
                cntPrey = 1

                For i As Integer = 1 To m_core.nLivingGroups

                    If grpOutput.isPrey(i) Then
                        For j As Integer = 1 To m_core.nEcosimTimeSteps
                            preyData(cntPrey, j) = grpOutput.PreyPercentage(i, j)
                        Next
                        cntPrey += 1
                    End If
                Next
                Dim preyMdlDetails As String = String.Format("{0},Predator:,{1}, Shows diets as proportions", strModelDetails, grpOutput.Name)
                SaveDataToFile(strFileName, bSaveAnnual, preyData, preyMdlDetails, preyNames.ToString)

            End If

        End Sub

        Private Function SaveDataToFile(ByVal strFileName As String, ByVal bSaveYearly As Boolean, ByRef data As Single(,), ByRef strModelDetails As String, ByRef strGroupNames As String) As Boolean

            Try
                'Overwritten the file
                Using sw As StreamWriter = New StreamWriter(strFileName, False)
                    sw.WriteLine(strModelDetails)
                    sw.WriteLine(strGroupNames)

                    If bSaveYearly Then
                        Dim simYears As Integer = CInt((data.GetLength(1) - 1) / cCore.N_MONTHS)
                        Dim nGroups As Integer = data.GetLength(0) - 1
                        Dim sum(nGroups) As Single
                        For j As Integer = 1 To simYears
                            ReDim sum(nGroups)
                            For i As Integer = 1 To nGroups
                                For k As Integer = 1 To cCore.N_MONTHS
                                    sum(i) = sum(i) + data(i, (j - 1) * cCore.N_MONTHS + k)
                                Next
                                sw.Write(sum(i) / cCore.N_MONTHS)
                                sw.Write(",")
                            Next
                            sw.WriteLine()
                        Next
                    Else
                        'Each time steps
                        For j As Integer = 1 To data.GetLength(1) - 1
                            'For every group
                            For i As Integer = 1 To data.GetLength(0) - 1
                                sw.Write(data(i, j))
                                sw.Write(",")
                            Next
                            sw.WriteLine()
                        Next
                    End If
                    sw.Close()

                End Using

            Catch ex As Exception
                Return False
            End Try
            Return True

        End Function

        Private Function GetAllGroupNames() As String

            Dim str As New StringBuilder
            For i As Integer = 1 To m_core.nGroups
                str.Append("""" & m_core.EcoSimGroupOutputs(i).Name & """")
                If i <> m_core.nGroups Then str.Append(",")
            Next

            Return str.ToString()

        End Function

        ''' <summary>
        ''' This saving format is based on EwE5 code
        ''' </summary>
        Private Function GetModelDetails() As String

            Dim str As New StringBuilder
            'Add the database name

            Dim dbName As String = My.Settings.MdbRecentlyUsedList(0).ToString
            Dim iIndex As Integer = dbName.IndexOf(",")
            str.Append(dbName.Substring(0, iIndex))
            str.Append(",")
            'Add the model name
            str.Append(m_core.EwEModel.Name)
            str.Append(",")
            'Add the active scenario name
            str.Append(m_core.EcosimScenarios(m_core.ActiveEcosimScenarioIndex).Name)

            Return str.ToString()

        End Function

        Private Sub UpdateColors()
            m_MasterPane.Fill = New Fill(Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            For Each p As GraphPane In Me.m_MasterPane.PaneList
                p.Chart.Fill = New Fill(Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND))
            Next
        End Sub

        Private Sub UpdateControls()
            Me.btnShowAllFits.Enabled = Me.m_core.HasAppliedTimeSeries()
        End Sub

#End Region ' Helper methods

    End Class

End Namespace
