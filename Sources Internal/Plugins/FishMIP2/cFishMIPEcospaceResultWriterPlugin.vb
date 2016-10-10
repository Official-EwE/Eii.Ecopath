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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin to write aggregated Ecospace results for FishMIP2.
''' </summary>
''' ===========================================================================
Public Class cFishMIPEcospaceResultWriterPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceBeginTimestepPlugin
    Implements IEcospaceEndTimestepPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements IAutoSavePlugin
    Implements IUIContextPlugin

#Region " Private vars "

    ''' <summary>Retained state flag</summary>
    Private m_bSaving As Boolean = False
    ''' <summary>Currently open writers</summary>
    Private m_writers() As StreamWriter = Nothing
    Private m_ds As cEcospaceDataStructures = Nothing
    Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " Generic bits "

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

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "FishMipSpaceWriter"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP; rely on cFishMipCore instead
    End Sub

#End Region ' Generic bits

#Region " Ecospace integration "

    Private m_strRunHist As String = ""
    Private m_iYearHist As Integer = 1971
    Private m_strRunFore As String = ""
    Private m_iYearFore As Integer = 2006
    Private m_bHasWriters As Boolean = False
    Private m_dNoData As Double = 1.0E+20!

    Private Sub InitWriters(strFile As String)

        If (Me.m_bHasWriters) Then CloseWriters()

        Try
            For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
                Dim fo As String = ""
                If strFile.Contains("{0}") Then
                    fo = String.Format(strFile, result.ToString).ToLower
                Else
                    fo = strFile & "_" & result.ToString()
                End If
                fo = Path.ChangeExtension(fo, ".csv")
                Me.m_writers(result) = New StreamWriter(Path.Combine(Me.AutoSaveOutputPath, fo))
                Me.m_writers(result).WriteLine("Time,Latitude,Longitude," & result.ToString())
            Next
            Me.m_bHasWriters = True
        Catch ex As Exception
            Me.m_bSaving = False
            ' Clean up failed writers
        End Try

    End Sub

    Private Sub CloseWriters()

        If Not Me.m_bHasWriters Then Return

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            If (Me.m_writers IsNot Nothing) Then
                If (Me.m_writers(result) IsNot Nothing) Then
                    Me.m_writers(result).Flush()
                    Me.m_writers(result).Close()
                    Me.m_writers(result) = Nothing
                End If
            End If
        Next
        Me.m_bHasWriters = False

    End Sub

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) _
        Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted

        Me.m_bSaving = Me.AutoSave
        If (Not Me.m_bSaving) Then Return

        Dim strPath As String = Me.AutoSaveOutputPath()
        If cFileUtils.IsDirectoryAvailable(strPath, True) = False Then
            Me.m_bSaving = False
            Return
        End If

        Me.m_ds = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)

        ' Capture autosave flag for the entire run
        Me.m_bSaving = AutoSave

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ReDim Me.m_writers([Enum].GetValues(GetType(cConfiguration.eResultTypes)).Length)

        Dim dlg As New dlgSpaceRun(Me.m_uic.Core, Me.m_strRunHist, Me.m_iYearHist, Me.m_strRunFore, Me.m_iYearFore, Me.m_dNoData)
        If dlg.ShowDialog(Me.m_uic.FormMain) = Windows.Forms.DialogResult.Cancel Then
            Me.m_uic.Core.StopEcospace()
            Return
        End If

        Me.m_strRunHist = dlg.RunHistorical
        Me.m_iYearHist = dlg.YearHist
        Me.m_strRunFore = dlg.RunForecast
        Me.m_iYearFore = dlg.YearForecast
        Me.m_dNoData = dlg.NoData

    End Sub

    Public Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        ' NOP
    End Sub

    Public Sub EcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Aggregate results
        Dim core As cCore = cFishMIPPlugin.GetInstance().Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim config As cConfiguration = cFishMIPPlugin.GetInstance().Configuration

        Dim dt As DateTime = core.EcospaceTimestepToAbsoluteTime(iTime)
        If (dt.Year < Me.m_iYearHist) Then Return
        If (dt.Month = 1) Then
            If (dt.Year = m_iYearHist) Then
                Me.InitWriters(Me.m_strRunHist)
            ElseIf (dt.Year = Me.m_iYearFore) Then
                Me.InitWriters(Me.m_strRunFore)
            End If
        End If

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            For iRow As Integer = 1 To Me.m_ds.InRow
                For iCol As Integer = 1 To Me.m_ds.InCol
                    Dim bHasData As Boolean = False
                    Dim val As Double = 0
                    If Me.m_ds.Depth(iRow, iCol) > 0 Then
                        For iGrp As Integer = 1 To core.nGroups
                            If config(iGrp, result) Then
                                Select Case result
                                    Case cConfiguration.eResultTypes.tsb,
                                         cConfiguration.eResultTypes.tcb,
                                         cConfiguration.eResultTypes.b10cm,
                                         cConfiguration.eResultTypes.b30cm,
                                         cConfiguration.eResultTypes.bcom
                                        val += Me.m_ds.Bcell(iRow, iCol, iGrp) / 10 ' Unit conversion
                                        bHasData = True
                                    Case cConfiguration.eResultTypes.tc,
                                         cConfiguration.eResultTypes.tc10cm,
                                         cConfiguration.eResultTypes.tc30cm
                                        val += Me.m_ds.CatchMap(iRow, iCol, iGrp)
                                        bHasData = True
                                    Case Else
                                        Debug.Assert(False, "Result type not supported")
                                End Select
                            End If
                        Next iGrp
                    End If

                    If Not bHasData Then val = Me.m_dNoData
                    Me.m_writers(result).WriteLine("{0},{1},{2},{3}",
                                                   iTime - 1,
                                                   bm.RowToLat(iRow) - bm.CellSize / 2, bm.ColToLon(iCol) + bm.CellSize / 2,
                                                   val)
                Next iCol
            Next iRow
        Next
        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            Me.m_writers(result).Flush()
        Next

    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        Dim core As cCore = cFishMIPPlugin.GetInstance().Core

        Me.CloseWriters()

        If Me.m_bSaving Then
            ' Notify UI
            Dim msg As New cMessage(String.Format("FishMIP Ecospace results have been saved to {0}", Me.AutoSaveOutputPath),
                                    eMessageType.DataExport, eCoreComponentType.Core, eMessageImportance.Information)
            msg.Hyperlink = Me.AutoSaveOutputPath
            core.Messages.SendMessage(msg)
        End If
    End Sub

#End Region ' Ecospace integration

#Region " Autosave "

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave

    Public Function AutoSaveName() As String _
        Implements IAutoSavePlugin.AutoSaveName

        ' For the UI
        Return "FishMip results"

    End Function

    Public Function AutoSaveType() As eAutosaveTypes _
        Implements IAutoSavePlugin.AutoSaveType

        ' Show for Ecospace
        Return eAutosaveTypes.Ecospace

    End Function

    Public Function AutoSaveOutputPath() As String _
        Implements IAutoSavePlugin.AutoSaveOutputPath

        ' Present complete path to UI
        Dim core As cCore = cFishMIPPlugin.GetInstance().Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP")

    End Function

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

#End Region ' Autosave

End Class
