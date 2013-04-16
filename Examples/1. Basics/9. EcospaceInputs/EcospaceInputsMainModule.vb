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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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

                        setFishingEffort()


                        'Run Ecopace on this thread(synchronously)
                        'core.RunEcoSpace() will block until the run has completed.
                        'If runAsync = True core.RunEcoSpace() will return before the run has completed(asynchronously)
                        'and onCoreExecutionStateEvent(cCoreStateMonitor) will be fire when the run has completed.
                        Dim runAsync As Boolean = False
                        core.RunEcoSpace(AddressOf onEcoSpaceTimeStep, runAsync)

                    Else
                        Console.WriteLine("Failed to load first Ecospace scenario")
                    End If
                Else
                    Console.WriteLine("This model does not contain any Ecospace scenarios")
                End If
            Else
                Console.WriteLine("This model does not contain any Ecosim scenarios")
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
        core.EcospaceModelParameters.TotalTime = 10

        'Have Ecospace distribute the Fishing Effort as a function of, Catch Value, Area Fished and the Fishing Cost Map
        'If PredictEffort = False then Ecospace will use the static Ecopath Effort
        core.EcospaceModelParameters.PredictEffort = True

        'Use the MultiStanza calculations
        core.EcospaceModelParameters.UseNewMultiStanza = True
        'Alternativly use the IBM Model for Multistanza species distributions
        'core.EcospaceModelParameters.UseIBM = True


    End Sub

    Private Sub setFishingEffort()
        Dim dEffort As Single
        Dim EffortShape As cFishingRateShape

        dEffort = 2 / core.nEcospaceTimeSteps
        For iflt As Integer = 1 To core.nFleets
            'EcoSpace uses the Ecosim Fishing Effort shape for its Effort over time input
            EffortShape = core.FishingEffortShapeManager(iflt)
            EffortShape.LockUpdates()
            'Just set Effort to increase over time
            For it As Integer = 1 To core.nEcospaceTimeSteps
                EffortShape.ShapeData(it) = it * dEffort
            Next
            EffortShape.UnlockUpdates()
        Next
    End Sub

#End Region

#Region "Ecospace Events"

    Private Sub onEcoSpaceTimeStep(ByRef EcospaceResults As cEcospaceTimestep)
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

        End If

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
