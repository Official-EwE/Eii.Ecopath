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
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

#End Region ' Imports

Public Class cFishMIPEcospaceResultWriterPlugin
    Implements INavigationTreeItemPlugin
    Implements IEcopathPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceBeginTimestepPlugin
    Implements IEcospaceEndTimestepPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements IAutoSavePlugin
    Implements IUIContextPlugin

    Public Const PluginName As String = "ndQ_FishMip_ISIMIP3_Ecospace_writer"

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_ds As cEcospaceDataStructures = Nothing
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
            Return cFishMIPEcospaceResultWriterPlugin.PluginName
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.CAPTION
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

#End Region ' Generic bits

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
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceInput"
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

#Region " Ecospace integration "

    Private m_bWritersInitialized As Boolean = False
    Private m_dNoData As Double = 1.0E+20!

    Private Sub InitWriters(strFile As String)

        If (Me.m_bWritersInitialized) Then Me.CloseWriters()
        If (String.IsNullOrWhiteSpace(strFile)) Then Return

        Try
            For Each var As cOutput In Me.m_config.Outputs
                ' Skip catch summaries if there is no fishing
                If (var.IsBiomass Or Me.m_uic.Core.ActiveTimeSeriesDatasetIndex > 0) Then
                    Dim fo As String = ""
                    If strFile.Contains("[var]") Then
                        fo = strFile.Replace("[var]", var.Name.ToString).ToLower
                    Else
                        fo = strFile & "_" & var.Name.ToString()
                    End If
                    fo = Path.ChangeExtension(fo, ".csv")
                    Me.m_writers(var) = New StreamWriter(Path.Combine(Me.AutoSaveOutputPath, fo))
                    Me.m_writers(var).WriteLine("Time,Latitude,Longitude," & var.Name)
                End If
            Next
            Me.m_bWritersInitialized = True
        Catch ex As Exception
            Me.m_bSaving = False
            ' Clean up failed writers
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

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) _
        Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted

        Me.m_bSaving = Me.AutoSave
        If (Not Me.m_bSaving) Then Return

        Me.m_ds = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)
        Dim core As cCore = Me.m_uic.Core

        Dim strPath As String = Me.AutoSaveOutputPath()
        If cFileUtils.IsDirectoryAvailable(strPath, True) = False Then
            Me.m_bSaving = False
            Return
        End If

        Me.CloseWriters()

    End Sub

    Public Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        ' NOP
    End Sub

    Public Sub EcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return
        If Me.m_ds.bInSpinUp Then Return

        ' Aggregate results
        Dim core As cCore = Me.m_uic.Core
        Dim dt As DateTime = core.EcospaceTimestepToAbsoluteTime(iTime)

        If (dt.Year < Me.m_config.ReportingStartYear) Then Return
        If (dt.Year > Me.m_config.ReportingEndYear) Then Return

        Dim iPeriod As Integer = Me.m_config.GetPeriodNo(dt.Year)
        If (iPeriod <> Me.m_iLastPeriod) Then
            Me.m_iLastPeriod = iPeriod
            Me.CloseWriters()
            If (iPeriod <> -1) Then
                Me.InitWriters(Me.m_config.OutputFileName(iPeriod, iTime))
            End If
        End If

        If (Not Me.m_bWritersInitialized) Then Return

        Dim bm As cEcospaceBasemap = core.EcospaceBasemap

        For Each var As cOutput In Me.m_writers.Keys
            For iRow As Integer = 1 To Me.m_ds.InRow
                For iCol As Integer = 1 To Me.m_ds.InCol
                    Dim bHasData As Boolean = False
                    Dim val As Double = 0
                    If Me.m_ds.Depth(iRow, iCol) > 0 Then
                        For iGrp As Integer = 1 To core.nGroups
                            If var.IsBiomass Then
                                val += Me.m_ds.Bcell(iRow, iCol, iGrp) * var(iGrp)
                                bHasData = True
                            Else
                                val += Me.m_ds.CatchMap(iRow, iCol, iGrp) * var(iGrp)
                                bHasData = True
                            End If
                        Next iGrp
                    End If

                    If Not bHasData Then
                        val = Me.m_dNoData
                    End If

                    Me.m_writers(var).WriteLine("{0},{1},{2},{3}",
                                                dt.ToString("yyyy-MM"),
                                                bm.RowToLat(iRow) - bm.CellSize / 2, bm.ColToLon(iCol) + bm.CellSize / 2,
                                                val)
                Next iCol
            Next iRow
        Next

        ' Flush ever so often
        If (iTime Mod 120 = 0) Then
            For Each wr As StreamWriter In Me.m_writers.Values
                wr.Flush()
            Next
        End If

    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        Dim core As cCore = Me.m_uic.Core

        Me.CloseWriters()

        If Me.m_bSaving Then
            ' Notify UI
            Dim msg As New cMessage(String.Format("{0} results have been saved to {1}", My.Resources.CAPTION, Me.AutoSaveOutputPath),
                                    eMessageType.DataExport, eCoreComponentType.Core, eMessageImportance.Information)
            msg.Hyperlink = Me.AutoSaveOutputPath
            core.Messages.SendMessage(msg)
        End If
    End Sub

#End Region ' Ecospace integration

#Region " Autosave "

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave
        Get
            If (Me.m_config Is Nothing) Then Return False
            Return Me.m_config.SaveWithEcospace
        End Get
        Set(value As Boolean)
            If (Me.m_config Is Nothing) Then Return
            If (value <> Me.m_config.SaveWithEcospace) Then
                Me.m_config.SaveWithEcospace = value
                Me.m_config.SaveChanges()
            End If
        End Set
    End Property

    Public Function AutoSaveType() As eAutosaveTypes _
        Implements IAutoSavePlugin.AutoSaveType

        ' Show for Ecospace results
        Return eAutosaveTypes.EcospaceResults

    End Function

    Public Function AutoSaveOutputPath() As String _
        Implements IAutoSavePlugin.AutoSaveOutputPath

        ' Present complete path to UI
        Dim core As cCore = Me.m_uic.Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP_ISIMIP3b")

    End Function

#End Region ' Autosave

    Public ReadOnly Property Configuration As cConfiguration
        Get
            Return Me.m_config
        End Get
    End Property

End Class
