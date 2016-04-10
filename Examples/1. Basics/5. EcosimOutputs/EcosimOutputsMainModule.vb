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

Imports EwECore
Imports System.Windows.Forms

''' <summary>
''' This program demonstrates how to obtain results from an Ecosim run.
''' </summary>
Module EcosimOutputsMainModule

    Sub Main()

        Dim core As New cCore()

        'Get a file name from the user
        Dim filename As String = ShowOpenFileDialogue()

        'Try to load the model in the selected file
        If core.LoadModel(filename) Then

            'Make sure the model contains at least one Ecosim Scenario
            If core.nEcosimScenarios > 0 Then

                'Get the first scenario and dump out its name
                Dim EcosimScenario As cEcoSimScenario = core.EcosimScenarios(1)
                System.Console.WriteLine("Ecosim scenario name = " + EcosimScenario.Name)

                'Load the scenario
                If core.LoadEcosimScenario(EcosimScenario) Then

                    'Set the number of years to run Ecosim for
                    core.EcoSimModelParameters.NumberYears = 10

                    'Run Ecosim and tell it to call onEcosimTimestep() with results at each Ecosim timestep
                    'Note that Ecopath is not explicitly ran; Ecopath will run implicitly when needed
                    If core.RunEcoSim(AddressOf onEcosimTimestep) Then
                        DumpEcosimResults(core)
                    End If

                End If 'core.LoadEcosimScenario(EcosimScenario)
            Else
                Console.WriteLine("This model does not contain any scenarios")
            End If 'core.EcosimScenarioCount > 0

            'Close up
            core.CloseModel()
        Else
            Console.WriteLine("Model did not load")
        End If

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

    ''' <summary>
    ''' Presents the user with a standard Windows interface for selecting a file
    ''' </summary>
    ''' <returns>A user-selected file, or an empty string if the user did not select a file.</returns>
    Private Function ShowOpenFileDialogue() As String

        'Create a new open file dialogue
        Dim openFileDialogue As New OpenFileDialog()

        'Set the file filters
        openFileDialogue.Filter = "EwE models|*.EwEmdb|All files|*.*"

        'Show the dialogue box and get the user-selected filename
        If (openFileDialogue.ShowDialog() = DialogResult.OK) Then
            Return openFileDialogue.FileName
        End If

        'The user did not select a file. Return an empty string
        Return String.Empty

    End Function

    ''' <summary>
    ''' Callback method for running Ecosim. This Sub will be called by Ecosim at each timestep.
    ''' </summary>
    ''' <param name="iTime">The time step that Ecosim computed.</param>
    ''' <param name="data">The resutls that Ecosim produced.</param>
    Private Sub onEcosimTimestep(ByVal iTime As Long, ByVal data As cEcoSimResults)

        'Write the timestep and some results out to the console window
        System.Console.WriteLine("On Ecosim timestep = " + iTime.ToString)
        For iGrp As Integer = 1 To data.nGroups
            System.Console.Write(data.Biomass(iGrp).ToString + ", ")
        Next iGrp
        System.Console.WriteLine("--------------------------------------")

    End Sub

    ''' <summary>
    ''' Write Ecosim results to the console after the end of a run.
    ''' </summary>
    ''' <param name="core"></param>
    Private Sub DumpEcosimResults(core As cCore)

        'Once an Ecosim run has been completed 
        'cCore.EcoSimGroupOutputs(GroupIndex) will contain the results for a group by timestep
        Dim EcosimGroupOutputs As cEcosimGroupOutput
        Dim sumB As Single
        Dim sumF As Single
        'Fleet definitions
        Dim fleet As cFleetInput

        Console.WriteLine("Ecosim results over time")
        Console.WriteLine()

        'Loop over all the groups
        For iGrp As Integer = 1 To core.nGroups

            'Get the results for this group
            'Results by group Biomass(), Yield()...
            EcosimGroupOutputs = core.EcoSimGroupOutputs(iGrp)

            'Results are stored by Timestep index starting at One
            sumB = 0
            For iTime As Integer = 1 To core.nEcosimTimeSteps
                sumB += EcosimGroupOutputs.Biomass(iTime)
            Next iTime
            'Dump something to the console window 
            System.Console.WriteLine("Group name, " + EcosimGroupOutputs.Name + ", Average Biomass, " + (sumB / core.nEcosimTimeSteps).ToString)

            'Results by Fleet for this Group           
            For iflt As Integer = 1 To core.nFleets

                'Get the Fleet definitions from the core
                fleet = core.FleetInputs(iflt)
                sumF = 0
                'Is this group fished by this fleet
                If fleet.Landings(iGrp) + fleet.Discards(iGrp) > 0 Then
                    'Yes sum F across all the timesteps for this Group/Fleet
                    For iTime As Integer = 1 To core.nEcosimTimeSteps
                        sumF += EcosimGroupOutputs.FishingMortByFleet(iflt, iTime)
                    Next iTime
                    System.Console.WriteLine("  Fleet name, " + fleet.Name + ", Average fishing mortality, " + (sumF / core.nEcosimTimeSteps).ToString)
                End If
            Next iflt

        Next iGrp

    End Sub

End Module
