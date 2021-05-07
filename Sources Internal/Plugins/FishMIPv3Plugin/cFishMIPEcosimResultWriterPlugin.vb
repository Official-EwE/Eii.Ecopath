' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin to write aggregated Ecosim results for FishMIP2.
''' </summary>
''' ===========================================================================
Public Class cFishMIPEcosimResultWriterPlugin
    Implements INavigationTreeItemPlugin
    Implements IEcopathPlugin
    Implements IEcosimRunInitializedPlugin
    Implements IEcosimBeginTimestepPlugin
    Implements IEcosimEndTimestepPostPlugin
    Implements IEcosimRunCompletedPlugin
    Implements IAutoSavePlugin
    Implements IUIContextPlugin

    Public Const PluginName As String = "ndQ_FishMip_ISIMIP3_Ecosim_writer"

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_bWritersInitialized As Boolean = False

    Private m_config As cConfiguration = Nothing

    Private m_bAutosaving As Boolean = False
    Private m_bSaving As Boolean = False
    Private m_iLastPeriod As Integer = -1 ' Index of lat output period that was saved

    ''' <summary>Currently open writers</summary>
    Private m_writers As New Dictionary(Of cOutput, StreamWriter)

#End Region ' Private vars

#Region " Generic plug-in integration "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return cFishMIPEcosimResultWriterPlugin.PluginName
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "Ecosim Fish-MIP ISIMIP3b"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ecopathinternational@gmail.com"
        End Get
    End Property

#End Region ' Generic plug-in integration

#Region " UIC "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region ' UIC

#Region " UI integration "

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = frmConfig.GetUI(Me.m_uic, Me.m_config)
    End Sub

    Public ReadOnly Property ControlImage As Image Implements IGUIPlugin.ControlImage
        Get
            Return EcoOceanUtils.My.Resources.ecoocean_768px
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTimeDynamic\ndEcosimInput"
        End Get
    End Property

#End Region ' UI integration

#Region " Ecopath integration "

    Public Function LoadModel(dataSource As Object) As Boolean Implements IEcopathPlugin.LoadModel
        ' Automatically reloads last configuration upon creation
        Me.m_config = cConfiguration.Attach(Me.m_uic.Core)
        Return True
    End Function

    Public Function SaveModel(dataSource As Object) As Boolean Implements IEcopathPlugin.SaveModel
        Return True
    End Function

    Public Function CloseModel() As Boolean Implements IEcopathPlugin.CloseModel
        cConfiguration.Detach()
        Me.m_config = Nothing
        Return True
    End Function

#End Region ' Ecopath integration

#Region " Ecosim integration "

    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) _
        Implements IEcosimRunInitializedPlugin.EcosimRunInitialized

        ' Capture autosave flag for the entire run
        Me.m_bSaving = Me.AutoSave

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        Me.CloseWriters()

    End Sub

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer) _
        Implements IEcosimBeginTimestepPlugin.EcosimBeginTimeStep

        ' NOP

    End Sub

    Public Sub EcosimEndTimeStepPost(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) _
        Implements IEcosimEndTimestepPostPlugin.EcosimEndTimeStepPost

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Aggregate results
        Dim core As cCore = Me.m_uic.Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap

        Dim sStepsPerYear As Single = cCore.N_MONTHS ' CSng(core.nEcosimTimeSteps / core.nEcosimYears)
        Dim y As Integer = core.RunStartYear + CInt(Math.Floor((iTime - 1) / sStepsPerYear))
        Dim m As Integer = CInt(((iTime - 1) Mod sStepsPerYear)) + 1

        If (y < Me.m_config.ReportingStartYear) Then Return
        If (y > Me.m_config.ReportingEndYear) Then Return

        Dim iPeriod As Integer = Me.m_config.GetPeriod(y)
        If (iPeriod <> Me.m_iLastPeriod) Then
            Me.m_iLastPeriod = iPeriod
            Me.CloseWriters()
            If (iPeriod <> -1) Then
                Me.InitWriters(Me.m_config.OutputFileName(iPeriod, iTime))
            End If
        End If

        If (Not Me.m_bWritersInitialized) Then Return

        ' Aggregate results
        Dim simresult As cEcoSimResults = DirectCast(Ecosimresults, cEcoSimResults)
        Dim simdata As cEcosimDatastructures = DirectCast(EcosimDatastructures, cEcosimDatastructures)

        For Each var As cOutput In Me.m_writers.Keys
            Dim val As Single = 0
            For iGrp As Integer = 1 To core.nGroups
                If var.IsBiomass Then
                    ' Use absolute biomasses
                    val += simdata.StartBiomass(iGrp) * simresult.Biomass(iGrp) * var.Group(iGrp)
                Else
                    For iFleet As Integer = 1 To core.nFleets
                        val += simresult.BCatch(iGrp, iFleet) * var.Group(iGrp)
                    Next
                End If
            Next

            Me.m_writers(var).WriteLine("{0:D4}_{1:D2},{2}", y, m, cStringUtils.FormatNumber(val))
        Next

    End Sub

    Public Sub EcosimRunCompleted(EcosimDatastructures As Object) _
        Implements IEcosimRunCompletedPlugin.EcosimRunCompleted

        Dim core As cCore = Me.m_uic.Core

        Me.CloseWriters()

        If Me.m_bSaving Then
            ' Notify UI
            Dim msg As New cMessage(String.Format("FishMIP Ecosim results have been saved to {0}", Me.AutoSaveOutputPath),
                                    eMessageType.DataExport, eCoreComponentType.Core, eMessageImportance.Information)
            msg.Hyperlink = Me.AutoSaveOutputPath
            core.Messages.SendMessage(msg)
        End If

    End Sub

#End Region ' Ecosim integration

#Region " Autosave "

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            If (Me.m_config Is Nothing) Then Return False
            Return Me.m_config.SaveWithEcosim
        End Get
        Set(value As Boolean)
            If (Me.m_config Is Nothing) Then Return
            If (value <> Me.m_config.SaveWithEcosim) Then
                Me.m_config.SaveWithEcosim = value
                Me.m_config.SaveChanges()
            End If
        End Set
    End Property

    Public Function AutoSaveType() As eAutosaveTypes _
        Implements IAutoSavePlugin.AutoSaveType

        ' Show for Ecosim
        Return eAutosaveTypes.Ecosim

    End Function

    Public Function AutoSaveOutputPath() As String _
        Implements IAutoSavePlugin.AutoSaveOutputPath

        ' Present complete path to UI
        Dim core As cCore = Me.m_uic.Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP_ISIMIP3b")

    End Function

#End Region ' Autosave

#Region " Writing results "

    Private Sub InitWriters(strFile As String)

        If (Me.m_bWritersInitialized) Then Me.CloseWriters()
        If (String.IsNullOrWhiteSpace(strFile)) Then Return

        ' Write output files
        Dim strPath As String = Me.AutoSaveOutputPath
        Dim core As cCore = Me.m_uic.Core
        Dim w As StreamWriter = Nothing
        Dim sStepsPerYear As Single = cCore.N_MONTHS ' CSng(core.nEcosimTimeSteps / core.nEcosimYears)

        ' Not able to create output path? Abort
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
            Me.m_config.ReportFailure("FishMip output writer failed to create output path '" & strPath & "'")
            Return
        End If

        Try
            For Each var As cOutput In Me.m_config.OutputVars
                If (var.IsBiomass Or Me.m_uic.Core.ActiveTimeSeriesDatasetIndex > 0) Then
                    Dim fo As String = ""
                    If strFile.Contains("[var]") Then
                        fo = strFile.Replace("[var]", var.Specifier.ToString).ToLower
                    Else
                        fo = strFile & "_" & var.Specifier.ToString()
                    End If
                    fo = Path.ChangeExtension(fo, ".csv")
                    Me.m_writers(var) = New StreamWriter(Path.Combine(Me.AutoSaveOutputPath, fo))
                    Me.m_writers(var).WriteLine("Time," & var.Specifier)
                End If
            Next
            Me.m_bWritersInitialized = True
        Catch ex As Exception
            Me.m_bSaving = False
            Me.m_config.ReportFailure("FishMip output writer error '" & ex.Message & "'")
            cLog.Write(ex, "cFishMIPEcosimResultWriterPlugin::InitWriters(" & strFile & ")")
            ' Todo: Clean up failed writers
        End Try

    End Sub

    Private Sub CloseWriters()

        If Not Me.m_bWritersInitialized Then Return

        For Each wr As StreamWriter In Me.m_writers.Values
            wr.Flush()
            wr.Close()
            wr.Dispose()
        Next
        Me.m_writers.Clear()
        Me.m_bWritersInitialized = False

    End Sub

#End Region ' Writing results

End Class
