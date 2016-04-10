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
''' This program demonstrates how to tweak Ecosim Vulnerabilities and Fishing Effort inputs.
''' </summary>
Module EcosimInputsMainModule

    Private core As cCore

    Sub Main()

        core = New cCore()

        'Get a file name from the user
        Dim filename As String = ShowOpenFileDialogue()

        'Try to load the model in the selected file
        If core.LoadModel(filename) Then

            'Make sure the model contains at least one Ecosim Scenario
            If core.nEcosimScenarios > 0 Then

                'Dump out all the scenario names
                For iScen As Integer = 1 To core.nEcosimScenarios
                    System.Console.WriteLine("Ecosim scenario name = " + core.EcosimScenarios(iScen).Name)
                Next

                'Load the first scenario
                If core.LoadEcosimScenario(1) Then

                    'Set the number of years to run Ecosim for
                    core.EcoSimModelParameters.NumberYears = 10

                    VaryVulnerabilities()

                    VaryFishingEffort()

                    'Run Ecosim and tell it to call onEcosimTimestep() with results at each Ecosim timestep
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


    Private Sub VaryVulnerabilities()
        Dim rand As New Random
        Dim factor As Single = 0.5

        'Vulnerabilities are store in a Prey/Pred matrix, vulnerability multiplier of a group(prey) to predation 
        'cCore.EcoSimGroupInputs(iprey).VulMult(ipred)

        'DietComp is stored in a Pred/Prey matrix, percentage of a groups diet made up by its prey
        'core.EcoPathGroupInputs(ipred).DietComp(iprey)

        'loop over the the groups matrix
        For iprey As Integer = 1 To core.nLivingGroups
            For ipred As Integer = 1 To core.nLivingGroups
                'is this prey group vulnerable to predation by this pred
                If core.EcoPathGroupInputs(ipred).DietComp(iprey) > 0 Then
                    'Yes this ipred consumes this iprey
                    'Randomly vary the VulMult of this group(iprey) to predation by group(ipred)
                    core.EcoSimGroupInputs(iprey).VulMult(ipred) = core.EcoSimGroupInputs(iprey).VulMult(ipred) * (1 + factor * (0.5 - rand.NextDouble()))
                End If
            Next
        Next

    End Sub


    Private Sub VaryFishingEffort()
        Dim FleetInput As cFleetInput
        Dim EffortManager As cFishingEffortManger = core.FishingEffortShapeManager

        'Ecosim stores Fishing Effort and Fishing Mortality input data in cShapeData objects 
        'Each type of cShapeData has its own implementation
        'Fishing effort = cFishingRateShape
        'Fishing mortality = cFishingMortShape

        'Fishing effort shapes are stored in the FishingEffortShapeManager list
        For Each EffortShape As cFishingRateShape In core.FishingEffortShapeManager

            If EffortShape.Index > core.nFleets Then
                'FishingEffortShapeManager stores the "All Fleets" fleet in the last index cCore.nFleets + 1
                'It is used by the core to update all the fleets to the same effort  
                'This fleet does not have an FleetInput object so don't try to get it from the core
                Exit For
            End If

            'Get the Fleet Input data for this Fleet
            'cShape objects keeps their array index to core data in the .Index property
            FleetInput = core.FleetInputs(EffortShape.Index)

            Debug.Assert(EffortShape.Name = FleetInput.Name)
            System.Console.WriteLine("Setting fishing effort * 2 for Fleet " + EffortShape.Name)

            'Lock all core updates of this data  
            EffortShape.LockUpdates()

            'Loop over all the Ecosim timesteps
            For it As Integer = 1 To core.nEcosimTimeSteps
                'Effort is stored by cumulative month
                EffortShape.ShapeData(it) = EffortShape.ShapeData(it) * 2
            Next it
            'Unlock the core updates for this fleet
            'this will update all the core data related to effort i.e. fishing mortality
            EffortShape.UnlockUpdates()

            'Dump out the update fishing mortality
            'Fishing Mortality shapes are stored in the cCore.FishMortShapeManager
            For Each FishingMort As cFishingMortShape In core.FishMortShapeManager
                'The core array index is stored in the cShapeData.Index property
                If FleetInput.Landings(FishingMort.Index) > 0 Then
                    'Fishing mortality shapes stored their data by time the same way as the EffortShapes
                    'FishingMort.ShapeData(TimeIndex)
                    System.Console.WriteLine(" F for group " + FishingMort.Name + " changed")
                End If
            Next FishingMort

        Next EffortShape

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
