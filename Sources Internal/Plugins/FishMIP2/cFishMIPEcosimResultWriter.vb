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
''' Plugin to write aggregated Ecosim results for FishMIP2.
''' </summary>
''' ===========================================================================
Public Class cFishMIPEcosimResultWriter
    Implements IEcosimRunInitializedPlugin
    Implements IEcosimBeginTimestepPlugin
    Implements IEcosimEndTimestepPostPlugin
    Implements IEcosimRunCompletedPlugin
    Implements IAutoSavePlugin

#Region " Private vars "

    ''' <summary>Aggregated data</summary>
    Private m_data As Single(,)
    ''' <summary>Retained state flag</summary>
    Private m_bSaving As Boolean = False

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

#Region " Ecosim integration "

    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) _
        Implements IEcosimRunInitializedPlugin.EcosimRunInitialized

        ' Capture autosave flag for the entire run
        Me.m_bSaving = AutoSave

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Init array for storing aggregated results
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        ReDim Me.m_data(core.nEcosimTimeSteps, [Enum].GetValues(GetType(cConfiguration.eResultTypes)).Length)

    End Sub

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer) _
        Implements IEcosimBeginTimestepPlugin.EcosimBeginTimeStep

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Clear array record - as if needed, but hey
        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            Me.m_data(iTime, result) = 0
        Next

    End Sub

    Public Sub EcosimEndTimeStepPost(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) _
        Implements IEcosimEndTimestepPostPlugin.EcosimEndTimeStepPost

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Aggregate results
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim config As cConfiguration = cFishMIPcore.GetInstance().Configuration
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
            Me.m_data(iTime, result) = val
        Next

    End Sub

    Public Sub EcosimRunCompleted(EcosimDatastructures As Object) _
        Implements IEcosimRunCompletedPlugin.EcosimRunCompleted

        ' Not autosaving? Done
        If Not Me.m_bSaving Then Return

        ' Write output files
        Dim strPath As String = Me.AutoSaveOutputPath
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim w As StreamWriter = Nothing
        Dim sStepsPerYear As Single = cCore.N_MONTHS ' CSng(core.nEcosimTimeSteps / core.nEcosimYears)

        ' Not able to create output path? Abort
        If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then Return

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            w = New StreamWriter(Path.Combine(strPath, result.ToString() & ".csv"))
            Try
                w.WriteLine("year," & result.ToString())
                For i As Integer = 1 To core.nEcosimTimeSteps
                    Dim y As Integer = core.EcosimFirstYear + CInt(Math.Floor((i - 1) / sStepsPerYear))
                    Dim t As Integer = CInt(((i - 1) Mod sStepsPerYear)) + 1
                    w.WriteLine("{0:D4}_{1:D2},{2}", y, t, cStringUtils.FormatNumber(Me.m_data(i, result)))
                Next
            Catch ex As Exception

            End Try
            w.Flush()
            w.Close()
        Next

        ' Notify UI
        Dim msg As New cMessage(String.Format("FishMIP Ecosim results have been saved to {0}", Me.AutoSaveOutputPath),
                                    eMessageType.DataExport, eCoreComponentType.Core, eMessageImportance.Information)
        msg.Hyperlink = Me.AutoSaveOutputPath
        core.Messages.SendMessage(msg)

        ' Free Willy
        Me.m_data = Nothing

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
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP")

    End Function

#End Region ' Autosave

End Class
