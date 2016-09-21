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

Public Class cFishMIPEcosimResultWriter
    Implements IEcosimRunInitializedPlugin
    Implements IEcosimBeginTimestepPlugin
    Implements IEcosimEndTimestepPostPlugin
    Implements IEcosimRunCompletedPlugin
    Implements IAutoSavePlugin

    Private m_bSaving As Boolean = False
    Private m_data As Single(,)

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
            Return "hmm"
        End Get
    End Property

    Public Property AutoSave As Boolean Implements IAutoSavePlugin.AutoSave

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        ' NOP
    End Sub

    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) Implements IEcosimRunInitializedPlugin.EcosimRunInitialized
        Me.m_bSaving = AutoSave
        If Not Me.AutoSave Then Return

        Dim core As cCore = cFishMIPcore.GetInstance().Core
        ReDim Me.m_data(core.nEcosimTimeSteps, [Enum].GetValues(GetType(cConfiguration.eResultTypes)).Length)

    End Sub

    Public Sub EcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer) Implements IEcosimBeginTimestepPlugin.EcosimBeginTimeStep
        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            Me.m_data(iTime, result) = 0
        Next
    End Sub

    Public Sub EcosimEndTimeStepPost(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) Implements IEcosimEndTimestepPostPlugin.EcosimEndTimeStepPost

        If Not Me.AutoSave Then Return

        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim config As cConfiguration = cFishMIPcore.GetInstance().Configuration
        Dim simbits As cEcoSimResults = DirectCast(Ecosimresults, cEcoSimResults)

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            Dim val As Single = 0
            For iGrp As Integer = 1 To core.nGroups
                If config(iGrp, result) Then
                    Select Case result
                        Case cConfiguration.eResultTypes.tsb,
                             cConfiguration.eResultTypes.tcb,
                             cConfiguration.eResultTypes.b10cm,
                             cConfiguration.eResultTypes.b30cm
                            val += simbits.Biomass(iGrp)
                        Case cConfiguration.eResultTypes.tc,
                             cConfiguration.eResultTypes.tc10cm,
                             cConfiguration.eResultTypes.tc30cm
                            For iFleet As Integer = 1 To core.nFleets
                                val += simbits.BCatch(iGrp, iFleet)
                            Next
                        Case Else
                            Debug.Assert(False, "Result type not supported")
                    End Select
                End If
            Next
            Me.m_data(iTime, result) = val
        Next
    End Sub

    Public Sub EcosimRunCompleted(EcosimDatastructures As Object) Implements IEcosimRunCompletedPlugin.EcosimRunCompleted

        If Not Me.AutoSave Then Return

        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim w As StreamWriter = Nothing
        Dim sStepsPerYear As Single = CSng(core.nEcosimTimeSteps / core.nEcosimYears)

        Dim strPath As String = Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), Me.AutoSaveOutputPath)
        cFileUtils.IsDirectoryAvailable(strPath, True)

        For Each result As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
            w = New StreamWriter(Path.Combine(strPath, result.ToString() & ".csv"))
            Try
                w.WriteLine("Year,Month," & result.ToString())
                For i As Integer = 1 To core.nEcosimTimeSteps
                    Dim y As Integer = core.EcosimFirstYear + CInt((i - 1) / sStepsPerYear)
                    Dim t As Integer = CInt(((i - 1) Mod sStepsPerYear)) + 1
                    w.WriteLine("{0},{1},{2}", y, t, cStringUtils.FormatNumber(Me.m_data(i, result)))
                Next
            Catch ex As Exception

            End Try
            w.Flush()
            w.Close()
        Next

        Dim msg As New cMessage("FishMIP results have been saved to {0}", eMessageType.DataExport, eCoreComponentType.Core, eMessageImportance.Information)
        msg.Hyperlink = strPath
        core.Messages.SendMessage(msg)

    End Sub

    Public Function AutoSaveName() As String Implements IAutoSavePlugin.AutoSaveName
        Return "FishMip results"
    End Function

    Public Function AutoSaveType() As eAutosaveTypes Implements IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecosim
    End Function

    Public Function AutoSaveOutputPath() As String Implements IAutoSavePlugin.AutoSaveOutputPath
        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Return Path.Combine(core.DefaultOutputPath(Me.AutoSaveType), "FishMIP")
    End Function

End Class
