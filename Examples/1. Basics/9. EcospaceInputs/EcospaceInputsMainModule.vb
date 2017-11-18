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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports System.IO
Imports System.Windows.Forms
Imports EwECore

Module EcospaceInputs

#Region "Private Variables"

    Private core As cCore

    Private WithEvents statemonitor As cCoreStateMonitor

#End Region

#Region "Main"

    Sub Main()

        'Initialize a new instance of the core
        core = New cCore

        'Get the instance of the StateMonitor from the Core
        'This will fire an the Event onCoreExecutionStateEvent(...) when EcoSpace has completed its run
        statemonitor = core.StateMonitor

        'Get a file name from the user
        Dim modelfilename As String = ShowOpenFileDialogue()

        'Try to load the model in the selected file
        If core.LoadModel(modelfilename) Then

            If core.nEcosimScenarios > 0 And core.nEcospaceScenarios > 0 Then
                'Load the first Ecosim and Ecospace scenarios
                If core.LoadEcosimScenario(1) Then
                    If core.LoadEcospaceScenario(1) Then

                        Console.WriteLine("Loaded first Ecospace scenario")

                        'Set some Model Parameters. i.e. Run length...
                        setEcoSpaceModelParameters()

                        'Set fishing effort
                        setFishingEffort()

                        'Setup a Habitat Foraging Response function
                        setHabitatForagingResponse()

                        'Run Ecopace on this thread(synchronously)
                        'core.RunEcoSpace() will block until the run has completed.
                        'If runAsync = True core.RunEcoSpace() will return before the run has completed(asynchronously)
                        'and onCoreExecutionStateEvent(cCoreStateMonitor) will be called when the run has completed.
                        Dim runAsync As Boolean = False
                        core.RunEcoSpace(AddressOf onEcoSpaceTimeStep, runAsync)

                    Else
                        Console.WriteLine("Failed to load first Ecospace scenario")
                    End If
                Else
                    Console.WriteLine("Failed to load first Ecosim scenario")
                End If
            Else
                Console.WriteLine("This model is missing an Ecosim or Ecospace scenario")
            End If

        Else
            Console.WriteLine("Model did not load")
        End If

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

        core.CloseModel()

    End Sub

#End Region

#Region "Set Ecospace Inputs"

    Private Sub setEcoSpaceModelParameters()

        core.EcospaceModelParameters.NumberOfTimeStepsPerYear = 12
        'Number of years to run
        core.EcospaceModelParameters.TotalTime = 10

        'Have Ecospace distribute the Fishing Effort as a function of, Catch Value, Area Fished and the Fishing Cost Map
        'If PredictEffort = False then Ecospace will use the static Ecopath Effort
        core.EcospaceModelParameters.PredictEffort = True

        'Use the MultiStanza calculations
        core.EcospaceModelParameters.UseNewMultiStanza = True
        'Alternativly use the IBM Model for Multistanza species distributions
        'core.EcospaceModelParameters.UseIBM = True

        'Initialize biomass to habitats/capacity
        core.EcospaceModelParameters.AdjustSpace = True

        'Populate the Capacity map base on Capacity maps
        For i As Integer = 1 To core.nGroups
            core.EcospaceGroups(i).CapacityCalculationType = EwEUtils.Core.eEcospaceCapacityCalType.EnvResponses
        Next

    End Sub

    Private Sub setFishingEffort()
        Dim dEffort As Single
        Dim EffortShape As cFishingRateShape

        'EcoSpace uses the Ecosim Fishing Effort shape for its effort over time input
        'If PredictEffort = True fishing effort is then distributed spatially at each timestep base on Catch Value, Cost and Area Fished 

        dEffort = 2 / core.nEcospaceTimeSteps
        For iflt As Integer = 1 To core.nFleets
            EffortShape = core.FishingEffortShapeManager(iflt)
            EffortShape.LockUpdates()
            'Just set Effort to increase over time
            For it As Integer = 1 To core.nEcospaceTimeSteps
                EffortShape.ShapeData(it) = it * dEffort
            Next
            EffortShape.UnlockUpdates()
        Next iflt
    End Sub

    Private Sub setHabitatForagingResponse()
        Dim DatabaseID As Integer
        Dim Layer As cEcospaceLayerDriver = Nothing

        'Habitat base foraging response is used to update the capacity map for each group.
        'During initialization the biomass in the cells is distributed by the capacity map
        'this give the initial distribution of biomass.
        'During a timestep the capacity map effects the dispersal rates and foraging rate for a group in a cell.

        'The habitat base foraging response to enviromental drivers has three main components
        'Two inputs and one Manager
        '1. Input map layer cCore.EcospaceBasemap.LayerDriver
        '       This is the enviromental driving map i.e. Salinity, Temperature...

        '2. Input foraging response function a cEnviroResponseFunction = cCore.CapacityShapeManager.Item(index) 
        '       this is a groups foraging response to an enviromental driver layer

        '3. The manager core.CapacityMapInteractionManager is used to apply a foraging response function to a group for an input driver layer
        '       It does this via a list of cEnviroInputMap objects which joins the LayerDriver to a response function for a group

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'First add a Enviromental Driver layer
        'Creates a new layer in the core and gives us the DatabaseID for the layer
        core.AddEcospaceDriverLayer("Example-Layer", "Layer added for example", "Sample units", DatabaseID)

        'Get the layer we just created from the core based on the DatabaseID that was set in AddEcospaceDriverLayer(,,DatabaseID)
        For iLayer As Integer = 1 To core.nEnvironmentalDriverLayers
            If DatabaseID = core.EcospaceBasemap.LayerDriver(iLayer).DBID Then
                Layer = core.EcospaceBasemap.LayerDriver(iLayer)
                Exit For
            End If
        Next

        If Layer Is Nothing Then
            'oppss failed to find the layer we just added
            System.Console.WriteLine("Failed to find the Environmental Driver layer.")
            Exit Sub
        End If

        'Set some values in the environmental layer
        For irow As Integer = 1 To core.EcospaceBasemap.InRow
            For icol As Integer = 1 To core.EcospaceBasemap.InCol
                Layer.Cell(irow, icol) = irow * icol
            Next icol
        Next irow
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Next the Response Function
        'this is how a group responses to an enviromental input i.e. Salinity
        Dim Manager As cEnviroResponseShapeManager = core.EnviroResponseShapeManager
        Dim ResponseFunction As cEnviroResponseFunction

        'Create a new response and give it some values
        ResponseFunction = Manager.CreateNewShape("ResponseShape", Nothing)
        Dim delta As Single = 1 / ResponseFunction.nPoints * 2
        ResponseFunction.LockUpdates()

        For ipoint As Integer = 1 To ResponseFunction.nPoints
            'this is the response multiplier that is returned for a value on the x axis
            ResponseFunction.ShapeData(ipoint) = ipoint * delta
        Next

        'One last step
        'We need to tell the response function what range of data it covers on the x axis/map input values. 
        'It is possible for the response function to just cover part of the input data.
        'In this case we will use the entire range from the input layer, but this can be anything.
        ResponseFunction.ResponseRightLimit = Layer.MaxValue
        ResponseFunction.ResponseLeftLimit = Layer.MinValue

        ResponseFunction.UnlockUpdates()
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Last
        'Now we have an enviromental map, the layer we added above
        'and a response function.
        'We need to join these together to tell EwE how a group responds to an enviromental driver (map)

        'CapacityMapInteractionManager contains a list of all the driver layers in the model
        'including the one we just added

        'loop over all the layers/maps in the manager and find the one we just added
        Dim Map As cEnviroInputMap
        For imap As Integer = 1 To core.CapacityMapInteractionManager.nEnviroData
            Map = core.CapacityMapInteractionManager.EnviroData(imap)
            'Is this the layer/map we added
            If Map.Layer.DBID = DatabaseID Then

                'Ok this is the layer we added
                'Now we have to tell the map which group(s) use which response functions
                For igrp As Integer = 1 To core.nGroups
                    'for this example we will add this response function to all the groups
                    Map.ResponseIndexForGroup(igrp) = ResponseFunction.Index
                Next

                Exit For

            End If
        Next
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        'When Ecospace is run it will set the capacity layer(s) for all the groups 
        'using the Example-Layer and Response function we added

    End Sub


#End Region

#Region "Ecospace Events"

    Private Sub onEcoSpaceTimeStep(ByRef EcospaceResults As cEcospaceTimestep)
        'This method will be call at the end of each Ecospace Timestep
        'EcospaceResults will contain results maps for the timestep
        System.Console.WriteLine("Ecospace Timestep " + EcospaceResults.iTimeStep.ToString)
        Dim sumSpaceB As Single
        Dim nSpaceB As Integer
        Dim sumPathB As Single

        For igrp As Integer = 1 To core.nGroups

            For irow As Integer = 1 To core.EcospaceBasemap.InRow
                For icol As Integer = 1 To core.EcospaceBasemap.InRow
                    'Is this a water cell
                    If core.EcospaceBasemap.LayerDepth.Cell(irow, icol) > 0 Then
                        'Sum Ecopace Biomass across all the water cells
                        nSpaceB += 1
                        sumSpaceB += EcospaceResults.BiomassMap(irow, icol, igrp)
                    End If

                Next icol
            Next irow
            'Sum of Ecopath Biomass
            sumPathB += core.EcoPathGroupOutputs(igrp).Biomass

        Next igrp

        Dim deltaB As Single
        deltaB = (sumSpaceB / nSpaceB) / (sumPathB / core.nGroups)
        System.Console.WriteLine("  Change in average b " + deltaB.ToString)

    End Sub

    Private Sub onCoreExecutionStateEvent(statemonitor As EwECore.cCoreStateMonitor) Handles statemonitor.CoreExecutionStateEvent

        If statemonitor.CoreExecutionState = EwEUtils.Core.eCoreExecutionState.EcospaceCompleted Then
            'Ecospace has completed a run 
            System.Console.WriteLine("Ecospace run completed")

            Dim SpaceGroup As cEcospaceGroupOutput
            'Ecospace does not stores results over time and space
            'It just stores spatially averaged values for a few varaibles
            For igrp As Integer = 1 To core.nGroups
                SpaceGroup = core.EcospaceGroupOutput(igrp)
                For it As Integer = 1 To core.nEcospaceTimeSteps
                    System.Console.Write(SpaceGroup.Biomass(it).ToString + ",")
                    'other outputs
                    'System.Console.Write(SpaceGroup.RelativeBiomass(it).ToString + ",")

                    ''Outputs by Group/fleet
                    'For ift As Integer = 1 To core.nFleets
                    '    System.Console.Write(SpaceGroup.CatchBiomass(ift, it).ToString + ",")
                    '    System.Console.Write(SpaceGroup.Value(ift, it).ToString + ",")
                    'Next ift
                    'System.Console.WriteLine()
                Next it

                System.Console.WriteLine()
            Next (igrp)

            Dim SpaceFleet As cEcospaceFleetOutput
            For ift As Integer = 1 To core.nFleets
                SpaceFleet = core.EcospaceFleetOutput(ift)
                For it As Integer = 1 To core.nEcospaceTimeSteps
                    System.Console.Write(SpaceFleet.CatchBiomass(it).ToString + ",")
                    'other outputs
                    'SpaceFleet.Value(it)
                Next it
                System.Console.WriteLine()
            Next ift

        End If 'EwEUtils.Core.eCoreExecutionState.EcospaceCompleted 

    End Sub

#End Region

#Region "Private support methods"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Presents the user with a standard Windows interface for selecting a file
    ''' </summary>
    ''' <returns>A user-selected file, or an empty string if the user did not select a file.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ShowOpenFileDialogue() As String

        'Create a new open file dialogue
        Dim openFileDialogue As New OpenFileDialog()

        'Set the file filters
        openFileDialogue.Filter = "EwE models|*.ewemdb;*.mdb;*.eweaccdb;*.accdb"

        'Show the dialogue box and get the user-selected filename
        If (openFileDialogue.ShowDialog() = DialogResult.OK) Then
            Return openFileDialogue.FileName
        End If

        'The user did not select a file. Return an empty string
        Return String.Empty

    End Function

#End Region

End Module
