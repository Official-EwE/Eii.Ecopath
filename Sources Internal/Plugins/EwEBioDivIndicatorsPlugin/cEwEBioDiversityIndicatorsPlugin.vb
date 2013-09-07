' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports EwECore
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class that connects to the core plug-in points. All indicator computations
''' are triggered from within this class.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwEBioDiversityIndicatorsPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IDisposedPlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcopathRunCompleted2Plugin
    Implements EwEPlugin.IEcopathRunInvalidatedPlugin
    Implements EwEPlugin.IEcosimRunCompletedPostPlugin
    Implements EwEPlugin.IEcosimRunInvalidatedPlugin
    Implements EwEPlugin.IEcosimPlugin
    Implements EwEPlugin.IEcospacePlugin
    Implements EwEPlugin.IEcospaceEndTimestepPostPlugin
    Implements EwEPlugin.IEcospaceRunInvalidatedPlugin
    Implements EwEPlugin.IEcospaceInitRunCompletedPlugin
    Implements EwEPlugin.IEcospaceRunCompletedPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.ISearchPlugin
    Implements EwEPlugin.IAutoSavePlugin

#Region " Variables "

    Friend Enum eComponentType As Integer
        Ecopath
        Ecosim
        Ecospace
        MonteCarlo
        Any
    End Enum

    Private m_core As cCore = Nothing
    Private m_uic As cUIContext = Nothing

    Private m_ecopathDS As cEcopathDataStructures = Nothing
    Private m_ecosimDS As cEcosimDatastructures = Nothing
    Private m_ecospaceDS As cEcospaceDataStructures = Nothing
    Private m_stanzaDS As cStanzaDatastructures = Nothing
    Private m_taxonDS As cTaxonDataStructures = Nothing
    Private m_searchDS As cSearchDatastructures = Nothing

    ''' <summary>Indicators for Ecopath.</summary>
    Friend m_indEcopath As cEcopathIndicators = Nothing
    ''' <summary>Indicators for each Ecosim time step.</summary>
    Friend m_lIndEcosim As List(Of cEcosimIndicators) = Nothing
    ''' <summary>Indicators for each MC trial and time step.</summary>
    Friend m_lIndMC As List(Of List(Of cMCIndicators)) = Nothing
    ''' <summary>Indicators for each Ecospace cell.</summary>
    Friend m_dtIndEcospace As Dictionary(Of Point, cEcospaceIndicators)
    ''' <summary>Indicators grouping.</summary>
    Friend m_settings As New cIndicatorSettings()

    Private m_frm As frmMain = Nothing

    Private m_msgStatus As cMessage = Nothing

#End Region ' Variables

#Region " Plug-in points "

#Region " Generic "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the author of this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Marta Coll Montón, Audrey Valls, Jeroen Steenbeek"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the contact information for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:martacoll@yahoo.com"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the assembly description for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for EwE6 that computes additional biodiversity indocators"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the internal name for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwEBiomassIndicatorsPlugin"
        End Get
    End Property

#End Region ' Generic

#Region " Life span "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that is called when the core has initialized and is
    ''' ready to be used by plug-ins.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore">core</see> that this plug-in
    ''' can connect to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' Grab and remember core when it is provided via the plug-in mechanism
        Me.m_core = DirectCast(core, cCore)
        ' Prepare data
        Me.m_indEcopath = Nothing
        Me.m_lIndEcosim = New List(Of cEcosimIndicators)
        Me.m_lIndMC = New List(Of List(Of cMCIndicators))
        Me.m_dtIndEcospace = New Dictionary(Of Point, cEcospaceIndicators)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that delivers ecopath, ecosim and ecospace models!
    ''' </summary>
    ''' <param name="objEcoPath"></param>
    ''' <param name="objEcoSim"></param>
    ''' <param name="objEcoSpace"></param>
    ''' -----------------------------------------------------------------------
    Public Sub CoreInitialized(ByRef objEcoPath As Object, _
                               ByRef objEcoSim As Object, _
                               ByRef objEcoSpace As Object) _
                           Implements EwEPlugin.ICorePlugin.CoreInitialized
        ' Not needed at this moment
    End Sub

    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose

        If Me.HasUI Then Me.m_frm.Close()
        If Me.m_frm IsNot Nothing Then Me.m_frm.Dispose()
        Me.m_frm = Nothing

        Me.m_indEcopath = Nothing
        Me.m_dtIndEcospace.Clear()
        Me.m_dtIndEcospace = Nothing

        Me.m_ecopathDS = Nothing
        Me.m_ecosimDS = Nothing
        Me.m_ecospaceDS = Nothing
        Me.m_searchDS = Nothing
        Me.m_taxonDS = Nothing
        Me.m_stanzaDS = Nothing

    End Sub

    Public Function LoadModel(ByVal dataSource As Object) As Boolean _
        Implements EwEPlugin.IEcopathPlugin.LoadModel

    End Function

    Public Function SaveModel(ByVal dataSource As Object) As Boolean _
        Implements EwEPlugin.IEcopathPlugin.SaveModel

    End Function

    Public Function Closemodel() As Boolean _
        Implements EwEPlugin.IEcopathPlugin.CloseModel

        ' Clear previous results
        Me.m_indEcopath = Nothing
        Me.m_lIndEcosim.Clear()
        Me.m_dtIndEcospace.Clear()

    End Function

#End Region ' Life span

#Region " Ecopath "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that delivers ecopath, taxon and stanza data structures when
    ''' Ecopath has finished a run.
    ''' </summary>
    ''' <param name="EcopathDataStructures">The <see cref="cEcopathDataStructures">ecopath data</see> with results.</param>
    ''' <param name="TaxonDataStructures">The <see cref="cTaxonDataStructures">taxonomy data</see> with supporting information for Ecopath.</param>
    ''' <param name="StanzaDataStructures">The <see cref="cStanzaDatastructures">stanza data</see> with supporting information for Ecopath.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object, _
                                   ByRef TaxonDataStructures As Object, _
                                   ByRef StanzaDataStructures As Object) Implements EwEPlugin.IEcopathRunCompleted2Plugin.EcopathRunCompleted

        ' Grab and remember ecopath data structures when provided via the plug-in mechanism
        Me.m_ecopathDS = DirectCast(EcopathDataStructures, cEcopathDataStructures)
        Me.m_taxonDS = DirectCast(TaxonDataStructures, cTaxonDataStructures)
        Me.m_stanzaDS = DirectCast(StanzaDataStructures, cStanzaDatastructures)

        ' Do not calculate if not supposed to run with Ecospath
        If (Not My.Settings.RunWithEcopath) Then Return
        ' Do not calculate when Ecopath is running as part of a searches
        If (Me.m_core.StateMonitor.IsSearching()) Then Return

        ' Compute
        Me.m_indEcopath = New cEcopathIndicators(Me.m_core, Me.m_ecopathDS, Me.m_stanzaDS, Me.m_taxonDS)
        Me.m_indEcopath.Compute()

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.Ecopath)
        End If

        If (Me.AutoSave And My.Settings.RunWithEcopath) Then
            If Me.BeginSave(eComponentType.Ecopath) Then
                Me.PerformSave(eComponentType.Ecopath)
                Me.EndSave()
            End If
        End If

    End Sub

    Public Sub EcopathRunInvalidated() Implements EwEPlugin.IEcopathRunInvalidatedPlugin.EcopathRunInvalidated

        ' Do not calculate if not supposed to run with Ecospath
        If (Not My.Settings.RunWithEcopath) Then Return
        ' Clear
        Me.ClearEcopathIndicators()

    End Sub

#End Region ' Ecopath

#Region " Ecosim "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point that delivers ecosim data structures when Ecosim has finished a run.
    ''' </summary>
    ''' <param name="EcosimDatastructures">The <see cref="cEcosimDatastructures">Ecosim data</see> with results.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcosimRunCompletedPost(ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunCompletedPostPlugin.EcosimRunCompletedPost

        ' Grab and remember ecosim data structures when provided via the plug-in mechanism
        Me.m_ecosimDS = DirectCast(EcosimDatastructures, cEcosimDatastructures)

        ' Do not calculate if not supposed to run with Ecosim
        If (Not My.Settings.RunWithEcosim) Then Return
        ' Do not calculate when Ecosim is running as part of a searches
        If (Me.m_core.StateMonitor.IsSearching()) Then Return

        ' Get ready to calculate
        Me.m_lIndEcosim.Clear()
        For iTime As Integer = 1 To Me.m_ecosimDS.NTimes
            Dim ind As New cEcosimIndicators(Me.m_core, Me.m_ecopathDS, Me.m_ecosimDS, iTime, Me.m_stanzaDS, Me.m_taxonDS)
            Me.m_lIndEcosim.Add(ind)
            ind.Compute()
        Next

        ' Need to save?
        If (My.Settings.AutoSaveCSV) Then
            ' #Yes: Save
            If Me.BeginSave(eComponentType.Ecosim) Then
                Me.PerformSave(eComponentType.Ecosim)
                Me.EndSave()
            End If
        End If

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.Ecosim)
        End If

    End Sub

    Public Sub EcosimRunInvalidated() Implements EwEPlugin.IEcosimRunInvalidatedPlugin.EcosimRunInvalidated
        ' Only clear when supposed to run with Ecosim
        If (Not My.Settings.RunWithEcosim) Then Return
        ' Clear
        Me.ClearEcosimIndicators()
    End Sub

    Public Sub CloseEcosimScenario() Implements EwEPlugin.IEcosimPlugin.CloseEcosimScenario
        If (My.Settings.RunWithEcosim) Then
            Me.ClearEcosimIndicators()
            Me.ClearMCIndicators()
        End If
    End Sub

    Public Sub LoadEcosimScenario(dataSource As Object) Implements EwEPlugin.IEcosimPlugin.LoadEcosimScenario
        If (My.Settings.RunWithEcosim) Then
            Me.ClearEcosimIndicators()
        End If
    End Sub

    Public Sub SaveEcosimScenario(dataSource As Object) Implements EwEPlugin.IEcosimPlugin.SaveEcosimScenario
        ' NOP
    End Sub

#End Region ' Ecosim

#Region " Monte Carlo "

    Public Sub SearchInitialized(SearchDatastructures As Object) _
        Implements EwEPlugin.ISearchPlugin.SearchInitialized
        Me.m_searchDS = DirectCast(SearchDatastructures, cSearchDatastructures)
    End Sub

    Public Sub SearchIterationsStarting() _
        Implements EwEPlugin.ISearchPlugin.SearchIterationsStarting
        Me.ClearMCIndicators()
    End Sub

    Public Sub PostRunSearchResults(SearchDatastructures As Object) _
        Implements EwEPlugin.ISearchPlugin.PostRunSearchResults

        Dim man As cMonteCarloManager = Me.m_core.EcosimMonteCarlo
        Dim lIter As New List(Of cMCIndicators)

        ' Calculate only if supposed to run with MC
        If (My.Settings.RunWithMC = False) Then Return
        ' Calculate only if running MC
        If (Me.m_searchDS.SearchMode <> eSearchModes.MonteCarlo) Then Return

        ' Get ready to calculate
        For iTime As Integer = 1 To Me.m_ecosimDS.NTimes
            Dim ind As New cMCIndicators(Me.m_core, Me.m_ecopathDS, Me.m_ecosimDS, CInt(man.nTrialIterations), iTime, Me.m_stanzaDS, Me.m_taxonDS)
            ind.Compute()
            lIter.Add(ind)
        Next
        Me.m_lIndMC.Add(lIter)

        ' Need to save?
        If (My.Settings.AutoSaveCSV) And (CInt(man.nTrialIterations) = man.nTrials) Then
            ' #Yes: Save
            If Me.BeginSave(eComponentType.MonteCarlo) Then
                Me.PerformSave(eComponentType.MonteCarlo)
                Me.EndSave()
            End If
        End If

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.MonteCarlo)
        End If

    End Sub

#End Region ' Monte Carlo

#Region " Ecospace "

    Public Sub LoadEcospaceScenario(ByVal dataSource As Object) _
        Implements EwEPlugin.IEcospacePlugin.LoadEcospaceScenario
        If (My.Settings.RunWithEcospace) Then
            Me.ClearEcospaceIndicators()
        End If
    End Sub

    Public Sub SaveEcospaceScenario(ByVal dataSource As Object) _
        Implements EwEPlugin.IEcospacePlugin.SaveEcospaceScenario
        ' NOP
    End Sub

    Public Sub CloseEcospaceScenario() _
        Implements EwEPlugin.IEcospacePlugin.CloseEcospaceScenario
        If (My.Settings.RunWithEcospace) Then
            Me.ClearEcospaceIndicators()
        End If
    End Sub

    Private Property PreserveCalcTL As Boolean

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted

        ' Calculate only if supposed to run with Ecospace
        If (My.Settings.RunWithEcospace = False) Then Return
        ' Do not calculate when Ecospace is running as part of a searches
        If (Me.m_core.StateMonitor.IsSearching()) Then Return

        ' Grab and remember ecosim data structures when provided via the plug-in mechanism
        Me.m_ecospaceDS = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)

        ' Preserve old TL calc setting
        Me.PreserveCalcTL = Me.m_ecospaceDS.bCalTrophicLevel
        ' Enable trophic level calculations when plugin is configured to run with Ecospace
        Me.m_ecospaceDS.bCalTrophicLevel = My.Settings.RunWithEcospace

        If My.Settings.AutoSaveCSV Then Me.BeginSave(eComponentType.Ecospace)

    End Sub

    Public Sub EcospaceEndTimeStepPost(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceEndTimestepPostPlugin.EcospaceEndTimeStepPost

        ' Calculate only if supposed to run with Ecospace
        If (My.Settings.RunWithMC = False) Then Return
        ' Do not calculate when Ecospace is running as part of a searches
        If (Me.m_core.StateMonitor.IsSearching()) Then Return

        ' Create indicators for each water cell if necessary
        If (Me.m_dtIndEcospace.Count = 0) Then

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth
            Dim ptCell As Point = Nothing

            ' Create only indicators for water cells
            For iRow As Integer = 1 To bm.InRow
                For iCol As Integer = 1 To bm.InCol
                    If (depth.IsWaterCell(iRow, iCol)) Then
                        ptCell = New Point(iCol, iRow)
                        Me.m_dtIndEcospace(ptCell) = New cEcospaceIndicators(Me.m_core, Me.m_ecopathDS, Me.m_ecospaceDS, New Point(iCol, iRow), Me.m_stanzaDS, Me.m_taxonDS)
                    End If
                Next iCol
            Next iRow
        End If

        If (iTime <> Me.m_core.nEcospaceTimeSteps) Then Return

        cApplicationStatusNotifier.StartProgress(Me.m_core, "Calculating Ecospace indicators...")
        Try
            ' Compute
            For Each ind As cIndicators In Me.m_dtIndEcospace.Values
                ind.Compute()
            Next
        Catch ex As Exception

        End Try
        cApplicationStatusNotifier.EndProgress(Me.m_core)

        Me.PerformSave(eComponentType.Ecospace)
        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.Ecospace)
        End If

    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        ' Calculate only if supposed to run with Ecospace
        If (My.Settings.RunWithEcospace = False) Then Return
        ' Do not calculate when Ecospace is running as part of a searches
        If (Me.m_core.StateMonitor.IsSearching()) Then Return

        ' Restore old TL calc setting
        Me.m_ecospaceDS.bCalTrophicLevel = Me.PreserveCalcTL

        Me.EndSave()

    End Sub

    Public Sub EcospaceRunInvalidated() _
        Implements EwEPlugin.IEcospaceRunInvalidatedPlugin.EcospaceRunInvalidated
        ' Clear
        Me.ClearEcospaceIndicators()
    End Sub

#End Region ' Ecospace

#Region " UI "

    Public Sub UIContext(ByVal uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Biodiversity indicators"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ControlText
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        If (Not Me.HasUI) Then
            Me.m_frm = New frmMain(Me.m_uic, Me)
            Me.m_frm.Text = Me.ControlText
        End If
        frmPlugin = Me.m_frm

    End Sub

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' UI

#End Region ' Plug-in points

#Region " Friend interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the default output folder.
    ''' </summary>
    ''' <returns>The default output folder, as specified in the EwE application options.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function DefaultFolder() As String
        Return Me.m_core.OutputPath
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the output folder that the user selected.
    ''' </summary>
    ''' <returns>The output folder that the user selected.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function OutputFolder(component As eComponentType) As String
        If My.Settings.SaveToDefault Then
            Select Case component
                Case eComponentType.Ecopath : Return Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecopath)
                Case eComponentType.Ecosim : Return Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecosim)
                Case eComponentType.Ecospace : Return Me.m_core.DefaultOutputPath(eAutosaveTypes.EcospaceMaps)
                Case eComponentType.MonteCarlo : Return Me.m_core.DefaultOutputPath(eAutosaveTypes.MonteCarlo)
            End Select
            Debug.Assert(False)
        End If
        Return My.Settings.CustomFolder
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Central point to save computed indicators to a CSV file, as initiated 
    ''' by the user.
    ''' </summary>
    ''' <param name="component">The <see cref="eComponentType"/> to save indicators for.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub SaveToCSVManual(component As eComponentType)

        ' Start CSV save process
        cApplicationStatusNotifier.StartProgress(Me.m_core)
        Try
            If Me.BeginSave(component) Then
                Me.PerformSave(component)
                Me.EndSave()
            End If
        Catch ex As Exception

        End Try
        ' End CSV save process
        cApplicationStatusNotifier.EndProgress(Me.m_core)

    End Sub

    Friend Sub ClearEcopathIndicators()

        ' Eradicate computed Ecopath indicators
        Me.m_indEcopath = Nothing

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.Ecopath)
        End If

    End Sub

    Friend Sub ClearEcosimIndicators()

        ' Eradicate computed Ecosim indicators
        Me.m_lIndEcosim.Clear()

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.Ecosim)
        End If

    End Sub

    Friend Sub ClearEcospaceIndicators()

        ' Eradicate computed Ecospace indicators
        Me.m_dtIndEcospace.Clear()

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.Ecospace)
        End If

    End Sub

    Friend Sub ClearMCIndicators()

        ' Eradicate computed MC indicators
        Me.m_lIndMC.Clear()

        ' Has UI?
        If (Me.HasUI) Then
            ' #Yes: Update UI
            Me.m_frm.UpdateIndicators(eComponentType.MonteCarlo)
        End If

    End Sub

#End Region ' Friend interfaces

#Region " Internal helpers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the plug-in has an active user interface.
    ''' </summary>
    ''' <returns>True if the plug-in has an active user interface.</returns>
    ''' -----------------------------------------------------------------------
    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        If (Me.m_frm.IsDisposed) Then Return False
        Return True
    End Function

    Private Function FileName(ByVal component As eComponentType, ByVal strStep As String) As String
        Return cFileUtils.ToValidFileName(String.Format("biodiv_ind_{0}{1}.csv", Me.ComponentName(component), strStep), False)
    End Function

    Private Function ComponentName(component As eComponentType) As String
        Select Case component
            Case eComponentType.Ecopath : Return SharedResources.HEADER_ECOPATH
            Case eComponentType.Ecosim : Return SharedResources.HEADER_ECOSIM
            Case eComponentType.Ecospace : Return SharedResources.HEADER_ECOSPACE
            Case eComponentType.MonteCarlo : Return SharedResources.HEADER_MONTECARLO
        End Select
        Return ""
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save calculated Ecopath indicators to a CSV file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub SaveEcopathCSV()

        If (Me.m_msgStatus Is Nothing) Then Return

        ' Sanity check
        Debug.Assert(Me.m_indEcopath.IsComputed, "Application flow error, ecopath indicators not calculated yet")

        Dim strPath As String = Me.OutputFolder(eComponentType.Ecopath)
        Dim strFile As String = Path.Combine(strPath, Me.FileName(eComponentType.Ecopath, ""))
        Dim sw As New StreamWriter(strFile)

        ' Write header line
        sw.WriteLine("{0},{1}", cStringUtils.ToCSVField(SharedResources.HEADER_INDICATOR), cStringUtils.ToCSVField(SharedResources.HEADER_VALUE))

        ' Write a line for each indicator
        For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
            Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
            For iInfo As Integer = 0 To grp.NumIndicators - 1
                Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                sw.WriteLine("{0},{1}", cStringUtils.ToCSVField(info.Name), cStringUtils.FormatSingle(info.GetValue(Me.m_indEcopath)))
            Next
        Next

        ' Done
        sw.Flush()
        sw.Close()

        Me.ReportStatus(String.Format(My.Resources.STATUS_SAVE_SUCCESS, Me.ComponentName(eComponentType.Ecopath), strFile), eStatusFlags.OK)

    End Sub

    Private Sub SaveEcosimCSV()

        Dim strPath As String = Me.OutputFolder(eComponentType.Ecosim)
        Dim strFile As String = Path.Combine(strPath, Me.FileName(eComponentType.Ecosim, ""))
        Dim sw As New StreamWriter(strFile)
        Dim sb As New StringBuilder()

        ' Write header line
        sb.Append(SharedResources.HEADER_TIME)
        For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
            Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
            For iInfo As Integer = 0 To grp.NumIndicators - 1
                Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                sb.Append(",")
                sb.Append(cStringUtils.ToCSVField(info.Name))
            Next
        Next
        sw.WriteLine(sb.ToString())

        ' Write a line for each time step
        For Each ind As cEcosimIndicators In Me.m_lIndEcosim

            ' Sanity check
            Debug.Assert(ind.IsComputed, "Application flow error, ecosim indicators not calculated yet")

            sb.Length = 0
            sb.Append(ind.Time)
            For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
                Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
                For iInfo As Integer = 0 To grp.NumIndicators - 1
                    Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                    sb.Append(",")
                    sb.Append(cStringUtils.FormatSingle(info.GetValue(ind)))
                Next iInfo
            Next iGrp
            sw.WriteLine(sb.ToString())
        Next ind

        ' Done
        sw.Flush()
        sw.Close()

        Me.ReportStatus(String.Format(My.Resources.STATUS_SAVE_SUCCESS, Me.ComponentName(eComponentType.Ecosim), strFile), eStatusFlags.OK)

    End Sub

    Private Sub SaveMCCSV()

        Dim core As cCore = Me.m_uic.Core
        Dim strTS As String = core.TimeSeriesDataset(core.ActiveTimeSeriesDatasetIndex).Name
        Dim strPath As String = Me.OutputFolder(eComponentType.MonteCarlo)
        Dim strFile As String = Path.Combine(strPath, Me.FileName(eComponentType.MonteCarlo, ""))
        Dim sw As New StreamWriter(strFile)
        Dim sb As New StringBuilder()

        ' Write header line
        sb.Append(My.Resources.HEADER_TRIAL)
        sb.Append(",")
        sb.Append(SharedResources.HEADER_TIME)
        For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
            Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
            For iInfo As Integer = 0 To grp.NumIndicators - 1
                Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                sb.Append(",")
                sb.Append(cStringUtils.ToCSVField(info.Name))
            Next
        Next
        sw.WriteLine(sb.ToString())

        ' Write a line for each trial + time step
        For iTrial As Integer = 0 To Me.m_lIndMC.Count - 1
            Dim lInd As List(Of cMCIndicators) = Me.m_lIndMC(iTrial)
            For Each ind As cMCIndicators In lInd

                ' Sanity check
                Debug.Assert(ind.IsComputed, "Application flow error, MC indicators not calculated yet")

                sb.Length = 0
                sb.Append(iTrial + 1)
                sb.Append(",")
                sb.Append(ind.Time)
                For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
                    Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
                    For iInfo As Integer = 0 To grp.NumIndicators - 1
                        Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                        sb.Append(",")
                        sb.Append(cStringUtils.FormatSingle(info.GetValue(ind)))
                    Next iInfo
                Next iGrp
                sw.WriteLine(sb.ToString())

            Next ind
        Next iTrial

        ' Done
        sw.Flush()
        sw.Close()

        Me.ReportStatus(String.Format(My.Resources.STATUS_SAVE_SUCCESS, Me.ComponentName(eComponentType.MonteCarlo), strFile), eStatusFlags.OK)

    End Sub

    Private Sub SaveEcospaceCSV()

        Dim strPath As String = Me.OutputFolder(eComponentType.Ecospace)
        Dim strFile As String = Path.Combine(strPath, Me.FileName(eComponentType.Ecospace, CStr(Me.m_ecospaceDS.TimeNow)))

        Dim sw As New StreamWriter(strFile)
        Dim sb As New StringBuilder()
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

        ' Write header line
        sb.Append(String.Format("{0},{1},{2},{3}",
                                cStringUtils.ToCSVField(SharedResources.HEADER_ROW), _
                                cStringUtils.ToCSVField(SharedResources.HEADER_COL), _
                                cStringUtils.ToCSVField(SharedResources.HEADER_LATITUDE), _
                                cStringUtils.ToCSVField(SharedResources.HEADER_LONGITUDE)))

        For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
            Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
            For iInfo As Integer = 0 To grp.NumIndicators - 1
                Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                sb.Append(",")
                sb.Append(cStringUtils.ToCSVField(info.Name))
            Next
        Next
        sw.WriteLine(sb.ToString())

        ' Write line for cell
        For Each ind As cEcospaceIndicators In Me.m_dtIndEcospace.Values

            ' Sanity check
            Debug.Assert(ind.IsComputed, "Application flow error, ecospace indicators not calculated yet")

            sb.Length = 0
            sb.Append(String.Format("{0},{1},{2},{3}", ind.Location.Y, ind.Location.X, bm.RowToLat(ind.Location.Y), bm.ColToLon(ind.Location.X)))

            For iGrp As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
                Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(iGrp)
                For iInfo As Integer = 0 To grp.NumIndicators - 1
                    Dim info As cIndicatorSettings.cIndicatorInfo = grp.Indicator(iInfo)
                    sb.Append(",")
                    sb.Append(cStringUtils.FormatSingle(info.GetValue(ind)))
                Next iInfo
            Next iGrp

            sw.WriteLine(sb.ToString())
        Next ind

        ' Done
        sw.Flush()
        sw.Close()

        Me.ReportStatus(String.Format(My.Resources.STATUS_SAVE_SUCCESS, Me.ComponentName(eComponentType.Ecospace), strFile), eStatusFlags.OK)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Append a status to the status message.
    ''' </summary>
    ''' <param name="strMessage">The message to send.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ReportStatus(strMessage As String, status As eStatusFlags)

        If (Me.m_msgStatus Is Nothing) Then Return

        Dim vs As New cVariableStatus(status, strMessage, eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.External, 0)
        Me.m_msgStatus.AddVariable(vs)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Begin the (auto)save progress
    ''' </summary>
    ''' <param name="component"></param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function BeginSave(component As eComponentType) As Boolean

        Dim strPath As String = Me.OutputFolder(component)

        Me.m_msgStatus = Nothing

        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            Dim msg As New cMessage(String.Format(My.Resources.STATUS_INVALID_FOLDER, strPath), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
            Me.m_core.Messages.SendMessage(msg)
            Return False
        End If

        Me.m_msgStatus = New cMessage(String.Format(SharedResources.GENERIC_FILESAVE_SUCCES, My.Resources.CAPTION, strPath), _
                                        eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        Me.m_msgStatus.Hyperlink = strPath

        Return True

    End Function

    Private Function PerformSave(component As eComponentType) As Boolean

        If (Me.m_msgStatus Is Nothing) Then Return False

        ' Safely encase file access logic to make sure that this method will not get interrupted
        Try
            Select Case component
                Case eComponentType.Ecopath
                    Me.SaveEcopathCSV()
                Case eComponentType.Ecosim
                    Me.SaveEcosimCSV()
                Case eComponentType.Ecospace
                    Me.SaveEcospaceCSV()
                Case eComponentType.MonteCarlo
                    Me.SaveMCCSV()
            End Select

        Catch ex As Exception
            Me.ReportStatus(String.Format(My.Resources.STATUS_SAVE_FAILED, Me.ComponentName(component), ex.Message), eStatusFlags.ErrorEncountered)
        End Try

        Return True

    End Function

    Private Function EndSave() As Boolean

        If (Me.m_msgStatus Is Nothing) Then Return False
        Me.m_core.Messages.SendMessage(Me.m_msgStatus)
        Me.m_msgStatus = Nothing

    End Function

#End Region ' Internal helpers

#Region " Autosave "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IAutoSavePlugin.AutoSave"/>
    ''' -----------------------------------------------------------------------
    Public Property AutoSave As Boolean _
        Implements EwEPlugin.IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.AutoSaveCSV
        End Get
        Set(value As Boolean)
            My.Settings.AutoSaveCSV = value
            My.Settings.Save()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IAutoSavePlugin.AutoSaveName"/>
    ''' -----------------------------------------------------------------------
    Public Function AutoSaveName() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveName
        Return My.Resources.CAPTION
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IAutoSavePlugin.AutoSaveSubPath"/>
    ''' -----------------------------------------------------------------------
    Public Function AutoSaveSubPath() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveSubPath
        ' Not used
        Return ""
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IAutoSavePlugin.AutoSaveType"/>
    ''' -----------------------------------------------------------------------
    Public Function AutoSaveType() As EwEUtils.Core.eAutosaveTypes _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.NotSet
    End Function

#End Region ' Autosave

End Class
