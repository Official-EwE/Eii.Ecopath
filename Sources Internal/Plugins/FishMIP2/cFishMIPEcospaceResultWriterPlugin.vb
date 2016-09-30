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

#Region " Private vars "

    ''' <summary>Retained state flag</summary>
    Private m_bSaving As Boolean = False
    ''' <summary>Currently open writers</summary>
    Private m_writers() As StreamWriter = Nothing
    Private m_ds As cEcospaceDataStructures = Nothing

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

        ReDim Me.m_writers([Enum].GetValues(GetType(cConfiguration.eResultTypes)).Length)

        Try
            For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
                Me.m_writers(result) = New StreamWriter(Path.Combine(Me.AutoSaveOutputPath, result.ToString & ".txt"))
                Me.m_writers(result).WriteLine("Time,Latitude,Longitude," & result.ToString())
            Next
        Catch ex As Exception
            Me.m_bSaving = False
            ' Clean up failed writers
        End Try
    End Sub

    Public Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        ' NOP
    End Sub

    Public Sub EcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Aggregate results
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim config As cConfiguration = cFishMIPcore.GetInstance().Configuration

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            For iRow As Integer = 1 To Me.m_ds.InRow
                For iCol As Integer = 1 To Me.m_ds.InCol
                    If Me.m_ds.Depth(iRow, iCol) > 0 Then
                        Dim val As Single = 0
                        For iGrp As Integer = 1 To core.nGroups
                            If config(iGrp, result) Then
                                Select Case result
                                    Case cConfiguration.eResultTypes.tsb,
                                         cConfiguration.eResultTypes.tcb,
                                         cConfiguration.eResultTypes.b10cm,
                                         cConfiguration.eResultTypes.b30cm,
                                         cConfiguration.eResultTypes.bcom
                                        val += Me.m_ds.Bcell(iRow, iCol, iGrp)
                                    Case cConfiguration.eResultTypes.tc,
                                         cConfiguration.eResultTypes.tc10cm,
                                         cConfiguration.eResultTypes.tc30cm
                                        val += Me.m_ds.CatchMap(iRow, iCol, iGrp)
                                    Case Else
                                        Debug.Assert(False, "Result type not supported")
                                End Select
                            End If
                        Next iGrp
                        Me.m_writers(result).WriteLine("{0},{1},{2},{3}",
                                                   iTime - 1,
                                                   bm.RowToLat(iRow), bm.ColToLon(iCol),
                                                   val)
                    End If
                Next iCol
            Next iRow
        Next

    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        Dim core As cCore = cFishMIPcore.GetInstance().Core

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            If (Me.m_writers IsNot Nothing) Then
                If (Me.m_writers(result) IsNot Nothing) Then
                    Me.m_writers(result).Flush()
                    Me.m_writers(result).Close()
                    Me.m_writers(result) = Nothing
                End If
            End If
        Next

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
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP")

    End Function

#End Region ' Autosave

End Class
