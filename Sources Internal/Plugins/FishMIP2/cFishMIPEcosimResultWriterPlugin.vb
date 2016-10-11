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
''' Plugin to write aggregated Ecosim results for FishMIP2.
''' </summary>
''' ===========================================================================
Public Class cFishMIPEcosimResultWriterPlugin
    Implements IEcosimRunInitializedPlugin
    Implements IEcosimBeginTimestepPlugin
    Implements IEcosimEndTimestepPostPlugin
    Implements IEcosimRunCompletedPlugin
    Implements IAutoSavePlugin
    Implements IUIContextPlugin

#Region " Private vars "

    ''' <summary>Retained state flag</summary>
    Private m_bSaving As Boolean = False

    Private m_uic As cUIContext = Nothing
    Private m_strRunHist As String = ""
    Private m_iYearHist As Integer = 1971
    Private m_strRunFore As String = ""
    Private m_iYearFore As Integer = 2006
    Private m_bHasWriters As Boolean = False
    Private m_dNoData As Double = 1.0E+20!

#End Region ' Private vars

#Region " General bits "

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
            Return "FishMipSimWriter"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP; rely on cFishMipCore instead
    End Sub

#End Region ' General bits

#Region " UIC "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region ' UIC

#Region " Writing results "

    ''' <summary>Currently open writers</summary>
    Private m_writers() As StreamWriter = Nothing

    Private Sub InitWriters(strFile As String)

        If (Me.m_bHasWriters) Then CloseWriters()

        ' Write output files
        Dim strPath As String = Me.AutoSaveOutputPath
        Dim core As cCore = cFishMIPPlugin.GetInstance().Core
        Dim w As StreamWriter = Nothing
        Dim sStepsPerYear As Single = cCore.N_MONTHS ' CSng(core.nEcosimTimeSteps / core.nEcosimYears)

        ' Not able to create output path? Abort
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return

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
                Me.m_writers(result).WriteLine("Time," & result.ToString())
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

#End Region ' Writing results

#Region " Ecosim integration "

    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) _
        Implements IEcosimRunInitializedPlugin.EcosimRunInitialized

        ' Capture autosave flag for the entire run
        Me.m_bSaving = AutoSave

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        Dim dlg As New dlgSimRun(Me.m_uic.Core, Me.m_strRunHist, Me.m_iYearHist, Me.m_strRunFore, Me.m_iYearFore)
        dlg.ShowDialog(Me.m_uic.FormMain)

        Me.m_strRunHist = dlg.RunHistorical
        Me.m_iYearHist = dlg.YearHist
        Me.m_strRunFore = dlg.RunForecast
        Me.m_iYearFore = dlg.YearForecast

        ReDim Me.m_writers([Enum].GetValues(GetType(cConfiguration.eResultTypes)).Length)

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
        Dim core As cCore = cFishMIPPlugin.GetInstance().Core
        Dim bm As cEcospaceBasemap = core.EcospaceBasemap
        Dim config As cConfiguration = cFishMIPPlugin.GetInstance().Configuration

        Dim sStepsPerYear As Single = cCore.N_MONTHS ' CSng(core.nEcosimTimeSteps / core.nEcosimYears)
        Dim y As Integer = core.EcosimFirstYear + CInt(Math.Floor((iTime - 1) / sStepsPerYear))
        Dim m As Integer = CInt(((iTime - 1) Mod sStepsPerYear)) + 1

        If (y < Me.m_iYearHist) Then Return
        If (m = 1) Then
            If (y = m_iYearHist) Then
                Me.InitWriters(Me.m_strRunHist)
            ElseIf (y = Me.m_iYearFore) Then
                Me.InitWriters(Me.m_strRunFore)
            End If
        End If

        If Not Me.m_bHasWriters Then Return

        ' Aggregate results
        Dim simresult As cEcoSimResults = DirectCast(Ecosimresults, cEcoSimResults)
        Dim simdata As cEcosimDatastructures = DirectCast(EcosimDatastructures, cEcosimDatastructures)

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            Dim val As Single = 0
            For iGrp As Integer = 1 To core.nGroups
                If config(iGrp, result) Then
                    Select Case result
                        Case cConfiguration.eResultTypes.tsb,
                             cConfiguration.eResultTypes.tcb,
                             cConfiguration.eResultTypes.b10cm,
                             cConfiguration.eResultTypes.b30cm,
                             cConfiguration.eResultTypes.bcom
                            ' Use absolute biomasses
                            val += simdata.StartBiomass(iGrp) * simresult.Biomass(iGrp) / 10 ' Unit conversion
                        Case cConfiguration.eResultTypes.tc,
                             cConfiguration.eResultTypes.tc10cm,
                             cConfiguration.eResultTypes.tc30cm
                            For iFleet As Integer = 1 To core.nFleets
                                val += simresult.BCatch(iGrp, iFleet)
                            Next
                        Case Else
                            Debug.Assert(False, "Result type not supported")
                    End Select
                End If
            Next
            Me.m_writers(result).WriteLine("{0:D4}_{1:D2},{2}", y, m, cStringUtils.FormatNumber(val))
        Next

    End Sub

    Public Sub EcosimRunCompleted(EcosimDatastructures As Object) _
        Implements IEcosimRunCompletedPlugin.EcosimRunCompleted

        Dim core As cCore = cFishMIPPlugin.GetInstance().Core

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

    Public Function AutoSaveName() As String _
        Implements IAutoSavePlugin.AutoSaveName

        ' For the UI
        Return "FishMip results"

    End Function

    Public Function AutoSaveType() As eAutosaveTypes _
        Implements IAutoSavePlugin.AutoSaveType

        ' Show for Ecosim
        Return eAutosaveTypes.Ecosim

    End Function

    Public Function AutoSaveOutputPath() As String _
        Implements IAutoSavePlugin.AutoSaveOutputPath

        ' Present complete path to UI
        Dim core As cCore = cFishMIPPlugin.GetInstance().Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP")

    End Function

#End Region ' Autosave

End Class
