' =============================================================================
'
' $Log: RunPSD.vb,v $
' Revision 1.26  2009/05/28 12:36:58  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.25  2009/05/12 21:35:11  joeh
' Add titles to graph axis
'
' Revision 1.24  2009/05/11 01:51:01  jeroens
' Renamed command classes
'
' Revision 1.23  2009/04/30 22:33:52  joeh
' V.5 regression analysis is comment in for future comparison
'
' Revision 1.22  2009/04/28 00:28:53  joeh
' Add handling if PSDEnabled is false
'
' Revision 1.21  2009/04/07 20:02:09  jeroens
' Updated to use ZedGraphHelper Attach
'
' Revision 1.20  2009/04/03 18:21:55  jeroens
' Deliberately detached zedgraphhelper
'
' Revision 1.19  2009/04/02 16:32:28  jeroens
' PSD run integrated w Ecopath
' Reinstated use of params variables
'
' Revision 1.18  2009/04/02 01:47:44  joeh
' Pass GroupSelected boolean array to cCore.RunPSD and psdModel.Run
'
' Revision 1.17  2009/03/31 21:36:15  joeh
' Move all PSD computation routines to a new class cPSDModel
'
' Revision 1.16  2009/03/25 00:03:14  joeh
' Add tool strip combo box for the latitude input
'
' Revision 1.15  2009/03/24 14:11:52  jeroens
' Correctly cleans up
' Uses PropertyFormatProviders instead of FormatProviders
' Fixed ecopath <-> graph sync logic
' ZedGraphHelper in charge of formatting graph
'
' Revision 1.14  2009/03/24 01:05:45  joeh
' Add OnCoreExecutionStateChanged event handler
'
' Revision 1.13  2009/03/23 20:45:49  joeh
' Add functionality to the Run button
'
' Revision 1.12  2009/03/21 00:31:19  jeroens
' PSD params exposes nWeightClasses
'
' Revision 1.11  2009/03/20 18:01:02  joeh
' Remove some redundant Imports statements
'
' Revision 1.10  2009/03/19 01:14:04  joeh
' no message
'
' Revision 1.9  2009/03/18 13:32:05  jeroens
' Uses implemented PSD classes
'
' Revision 1.8  2009/03/17 23:37:34  joeh
' Add codes for the Selected Group feature
'
' Revision 1.7  2009/03/17 19:38:08  joeh
' Add latitudes of NW and SE corners of model
'
' Revision 1.6  2009/03/14 18:34:07  joeh
' Change dXValue of double type to sXValue of single type
' Add linear regression of the system PSD
'
' Revision 1.5  2009/03/12 23:51:06  joeh
' Add codes for tabulation of PSD contribution data
'
' Revision 1.4  2009/03/12 01:50:29  joeh
' Add codes for PSD histogram (PSDContributionPlot)
'
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
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Properties
Imports ZedGraph

#End Region 'Imports

Namespace Ecopath.Output

    Public Class RunPSD

#Region "Variables"

        ' -- Core connection
        Private m_coreStateMonitor As cCoreStateMonitor = Nothing
        Private m_core As cCore = Nothing

        ' -- To make life easier and a more fun place to be
        Private m_zgh As cZedGraphHelper = Nothing

        ' -- Format providers --
        Private m_fpNoOfPointsPSD As cEwEFormatProvider = Nothing
        Private m_fpMinWeight As cEwEFormatProvider = Nothing
        Private m_fpNoOfPointsMovAvg As cEwEFormatProvider = Nothing

        ' -- Internal admin --
        ''' <summary>Flag stating whether the current Ecopath results have been plotted.</summary>
        Private m_bEcopathResultsPlotted As Boolean = False

#End Region 'Variables

#Region " Constructor/Destructor "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructor/Destructor

#Region " Event handlers "

        Private Sub RunPSD_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load
            Dim parms As cPSDParameters = Nothing
            Dim str As String = ""
            Dim msg As cMessage = Nothing
            Dim cmdh As cCommandHandler = Nothing
            Dim cmd As cCommand = Nothing
            Dim sg As cStyleGuide = Nothing

            Me.m_core = cCore.GetInstance()
            Me.m_coreStateMonitor = Me.m_core.StateMonitor
            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.m_core, Me.m_zedgraph)

            ' Connect to show/hide groups command
            cmdh = cCommandHandler.GetInstance()
            cmd = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.AddControl(Me.m_tsbnShowHideGroups)
            End If

            ' Style guide
            sg = cStyleGuide.GetInstance()
            AddHandler sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

            ' Connect format providers
            parms = Me.m_core.ParticleSizeDistributionParameters
            Me.m_fpNoOfPointsPSD = New cPropertyFormatProvider(Me.m_tstbxNoOfPointsPSD.Control, parms, eVarNameFlags.PSDNumWeightClasses)
            Me.m_fpMinWeight = New cPropertyFormatProvider(Me.m_tstbxMinWeight.Control, parms, eVarNameFlags.PSDFirstWeightClass)
            Me.m_fpNoOfPointsMovAvg = New cPropertyFormatProvider(Me.m_tstbxNoOfPointsMovAvg.Control, parms, eVarNameFlags.NumPtsMovAvg)

            ' Connect to core state monitor events
            AddHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            ' Sync controls
            Me.UpdateToolstrip()
            ' Neatify
            cToolstripUtils.HideRepeatingSeparators(Me.m_tsRunPSD)

            ' Synchronize plot with Ecopath results
            Me.SynchronizePlot()

            If parms.PSDEnabled = False Then
                str = My.Resources.PSD_MSG_PSDDISABLED
                msg = New cMessage(str, eMessageType.TooManyMissingParameters, eCoreComponentType.EcoPath, eMessageImportance.Warning)
                Me.m_core.Messages.SendMessage(msg)
            End If
        End Sub

        Private Sub RunPSD_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
            Dim parms As cPSDParameters = Nothing

            parms = Me.m_core.ParticleSizeDistributionParameters
            If parms.PSDEnabled = False Then Me.Close()
        End Sub

        Private Sub RunPSD_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
            Handles Me.FormClosing

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmd As cCommand = Nothing

            ' Detach format providers
            Me.m_fpNoOfPointsPSD.Release()
            Me.m_fpMinWeight.Release()
            Me.m_fpNoOfPointsMovAvg.Release()

            Me.m_zgh.Detach()
            Me.m_zgh = Nothing

            ' Detach from show/hide groups command
            cmd = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.RemoveControl(Me.m_tsbnShowHideGroups)
            End If

            ' Detach from core state monitor events
            RemoveHandler Me.m_coreStateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

            'Style guide
            RemoveHandler sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Private Sub MenuItmGroupPB_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tsmiGroupPB.Click
            ' Make sure one checkbox is checked exclusively
            Me.m_tsmiGroupPB.Checked = True
            Me.m_tsmiLorenzen.Checked = Not Me.m_tsmiGroupPB.Checked
            'Disable MeanLat label and combo box
            m_tsmiMeanLat.Enabled = Me.m_tsmiLorenzen.Checked
            m_tscbxMeanLat.Enabled = Me.m_tsmiLorenzen.Checked
        End Sub

        Private Sub MenuItmLorenzen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tsmiLorenzen.Click
            ' Make sure one checkbox is checked exclusively
            Me.m_tsmiLorenzen.Checked = True
            Me.m_tsmiGroupPB.Checked = Not Me.m_tsmiLorenzen.Checked
            'Enable MeanLat label and combo box
            m_tsmiMeanLat.Enabled = Me.m_tsmiLorenzen.Checked
            m_tscbxMeanLat.Enabled = Me.m_tsmiLorenzen.Checked
        End Sub

        Private Sub BtnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbtnRun.Click

            ' Grab PSD settings from the GUI and stick them in the core
            Me.UpdateVariables()

            Me.m_core.RunEcoPath()
            Me.PlotCurves()
        End Sub

        Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)
            Me.SynchronizePlot()
        End Sub

        Private Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            Me.UpdateVariables()
            Me.m_core.RunEcoPath()
        End Sub

#End Region ' Event handlers

#Region "Helper methods"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Synchronize the plot area with Ecopath results.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SynchronizePlot()

            ' This code is optimized to only plot when new results are available
            ' Are Ecopath results available?
            If Me.m_coreStateMonitor.HasEcopathRan Then
                ' #Yes: are these results not plotted yet?
                If Me.m_bEcopathResultsPlotted = False Then
                    ' #Yes: Plot the curves
                    Me.PlotCurves()
                    ' Set flag to remind ourselves that these results are plotted
                    Me.m_bEcopathResultsPlotted = True
                End If
            Else
                '#No: Ecopath results have disappeared (or are not yet available)
                'Is the plot populated?
                If Me.m_bEcopathResultsPlotted = True Then
                    ' #Yes: clear the plot
                    Me.InitializePane()
                    ' Set local flag to remind ourselves that the plot is empty
                    Me.m_bEcopathResultsPlotted = False
                End If
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure the graph pane to look pretty and dandy.
        ''' </summary>
        ''' <returns>
        ''' The graph pane that was initialized.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function InitializePane() As GraphPane

            Dim pane As GraphPane = Me.m_zedgraph.GraphPane
            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters

            pane.CurveList.Clear()

            ' JS 23Mar09: Zedgraph helper performs standardized label, axis styling
            Me.m_zgh.ConfigurePane(My.Resources.PSD_PLOTCAPTION_PSD, _
                                   My.Resources.PSD_XAXISLABEL_BODYWEIGHT, _
                                   My.Resources.PSD_YAXISLABEL_BIOMASS, _
                                   True)

            pane.Title.FontSpec.Size = 16
            pane.Legend.FontSpec.Size = 14
            pane.XAxis.Title.FontSpec.Size = 14
            pane.YAxis.Title.FontSpec.Size = 14

            pane.XAxis.Scale.Min = Int(Math.Log10(parms.FirstWeightClass))
            pane.XAxis.Scale.Max = Math.Round(Math.Log10(parms.FirstWeightClass * 2 ^ (Me.m_core.nWeightClasses - 1)) + 0.4, 0, MidpointRounding.AwayFromZero)
            pane.YAxis.Scale.Min = 0
            Return pane

        End Function

        Private Sub AddCurves(ByVal pane As GraphPane)

            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim sSystemPSD(m_core.nWeightClasses) As Single
            Dim sSlope As Single
            Dim sIntercept As Single
            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()

            Me.InitLists(resultLists, 2)

            'Find system PSD by summing the group PSD
            Me.FindSystemPSD(sSystemPSD)

            'Find regression of the system PSD
            Me.FindRegression(sSlope, sIntercept, sSystemPSD)

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    'PSD plot
                    resultLists(0).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 100000)) '* 100000 for plotting purpose
                    'PSD regression plot
                    resultLists(1).Add(Math.Log10(sXValue), sSlope * Math.Log10(sXValue) + sIntercept)

                End If
            Next

            Me.AddCurveToGraphPane(pane, resultLists(0), "", Color.Transparent)
            Me.AddCurveToGraphPane(pane, resultLists(1), _
                    String.Format(My.Resources.PSD_GRAPH_REGRESSION_LABEL, sg.FormatNumber(sSlope), sg.FormatNumber(sIntercept)), _
                    Color.Black)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal pane As GraphPane, ByVal list As PointPairList, _
                                        ByVal strLabel As String, ByVal lineClr As Color)
            Dim lnItem As LineItem

            lnItem = pane.AddCurve(strLabel, list, lineClr)

            If lineClr = Color.Transparent Then
                lnItem.Line.IsVisible = False
                lnItem.Symbol.Type = SymbolType.Circle
                lnItem.Symbol.Border.IsVisible = False
                lnItem.Symbol.Fill.IsVisible = True
                lnItem.Symbol.Fill.Brush = Brushes.Black
            Else
                lnItem.Line.IsVisible = True
                lnItem.Symbol.Type = SymbolType.None
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update PSD variables from user settings.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateVariables()

            Dim grpInput As cEcoPathGroupInput = Nothing
            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()

            'Mortality type
            If m_tsmiGroupPB.Checked Then
                parms.MortalityType = ePSDMortalityTypes.GroupZ
            ElseIf m_tsmiLorenzen.Checked Then
                parms.MortalityType = ePSDMortalityTypes.Lorenzen
            End If

            'Climate type
            If m_tsmiLorenzen.Checked Then
                parms.ClimateType = CType(m_tscbxMeanLat.SelectedIndex, eClimateTypes)
            End If

            'Group included in PSD 
            For iGroup As Integer = 1 To Me.m_core.nLivingGroups
                grpInput = m_core.EcoPathGroupInputs(iGroup)
                parms.GroupIncluded(iGroup) = sg.GroupVisible(iGroup)
            Next

            ' JS: the variable values are automatically updated by the property format providers
            'parms.NumWeightClasses = CInt(m_fpNoOfPointsPSD.Value)
            'parms.FirstWeightClass = CSng(m_fpMinWeight.Value)
        End Sub

        Private Sub UpdatePlot()
            Me.m_zedgraph.AxisChange()
            Me.m_zedgraph.Refresh()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update toolstrip item states.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateToolstrip()

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim grpInput As cEcoPathGroupInput = Nothing
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()

            'Mortality type
            Select Case parms.MortalityType
                Case ePSDMortalityTypes.GroupZ
                    Me.m_tsmiGroupPB.Checked = True
                Case ePSDMortalityTypes.Lorenzen
                    Me.m_tsmiLorenzen.Checked = True
            End Select

            'Climate type
            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                Me.m_tscbxMeanLat.Enabled = True
                Me.m_tscbxMeanLat.SelectedIndex = parms.ClimateType
            End If

            'Group included in PSD
            ' ToDo_JS or ToDo_JH: The groupvisible flags are meant solely for showing/hiding groups in graphs.
            ' The PSD group inclusion settings is separate behaviour, and should perhaps not alter the EwE group 
            ' visibility settings. 
            '
            ' We can solve this by either:
            '    1. building a different show/hide group interface solely for use with PSD, 
            '    2. reuse the current show/hide group interface but make it operate on different data,
            '    3. reuse show/hide groups but do NOT store PSD group inclusion settings in the database.
            '
            ' 1) is quite a bit of work
            ' 2) is possible by passing an array of show/hide flags into the command.Tag, which we could make
            '    the show/hide dialog operate on. This is pretty hack. Additionally, the hijacked dialog needs to 
            '    display an alternate caption to distinguish its behaviour from regular show/hide group behaviour.
            ' 3) is probably the best option
            'For iGroup As Integer = 1 To m_core.nLivingGroups
            '    grpInput = m_core.EcoPathGroupInputs(iGroup)
            '    sg.GroupVisible(iGroup) = grpInput.PSDIncluded
            'Next

        End Sub

        Private Sub PlotCurves()
            Me.AddCurves(Me.InitializePane())
            Me.UpdatePlot()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To m_core.nLivingGroups
                If parms.GroupIncluded(iGroup) Then
                    grpOutput = m_core.EcoPathGroupOutputs(iGroup)
                    For iWtClass As Integer = 1 To m_core.nWeightClasses
                        sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                    Next
                End If
            Next

        End Sub

        Private Sub FindRegression(ByRef sSlope As Single, ByRef sIntercept As Single, _
                                   ByVal sSystemPSD() As Single)

            Dim sXValue As Single = 0
            Dim dSumX As Double = 0
            Dim dSumY As Double = 0
            Dim iNum As Integer = 0
            Dim dXMean As Double
            Dim dYMean As Double
            Dim dSumXdevYdev As Double = 0
            Dim dSumXdevSq As Double = 0
            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    dSumX = dSumX + Math.Log10(sXValue)
                    dSumY = dSumY + Math.Log10(sSystemPSD(iWtClass) * 100000.0!)

                    'v.5
                    'sXValue = iWtClass

                    'dSumX = dSumX + sXValue
                    'dSumY = dSumY + Math.Log10(sSystemPSD(iWtClass))
                    'End v.5
                    iNum = iNum + 1
                End If
            Next
            dXMean = dSumX / iNum
            dYMean = dSumY / iNum

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    dSumXdevYdev = dSumXdevYdev + (Math.Log10(sXValue) - dXMean) * (Math.Log10(sSystemPSD(iWtClass) * 100000) - dYMean)
                    dSumXdevSq = dSumXdevSq + (Math.Log10(sXValue) - dXMean) ^ 2

                    'v.5
                    'sXValue = iWtClass

                    'dSumXdevYdev = dSumXdevYdev + ((sXValue) - dXMean) * (Math.Log10(sSystemPSD(iWtClass)) - dYMean)
                    'dSumXdevSq = dSumXdevSq + ((sXValue) - dXMean) ^ 2
                    'End v.5
                End If
            Next

            sSlope = CSng(dSumXdevYdev / dSumXdevSq)
            sIntercept = CSng(dYMean - sSlope * dXMean)

        End Sub

#End Region ' Helper methods

    End Class

End Namespace