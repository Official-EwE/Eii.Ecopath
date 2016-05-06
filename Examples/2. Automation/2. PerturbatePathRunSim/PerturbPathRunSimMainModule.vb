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

Imports EwECore
Imports System.Windows.Forms


Module PerturbPathRunSimMainModule

    Private Core As cCore
    Private RandNumGenerator As Random

    Sub Main()
        'Number of Trials to run
        Dim nTrials As Integer = 10
        Dim nScenario As Integer = 1
        'Number of attemps at finding a balanced Ecopath model
        Dim nBalanceAttempts As Integer = 100
        'Current attempt at finding a balanced Ecopath model
        Dim iAttempt As Integer
        Dim FoundBalancedModel As Boolean
        'Original Ecopath B and PB

        Dim orgB As Single() = New Single(Core.nGroups) {}
        Dim orgPB As Single() = New Single(Core.nGroups) {}
        Dim bestSS As Single = Single.MaxValue

        'Init the objects needed Core and .NET Random
        Core = New cCore() ' new instance of cCore
        Core.OutputPath = "D:\temp"
        Core.Autosave(EwEUtils.Core.eAutosaveTypes.Ecosim) = True

        RandNumGenerator = New Random(Environment.TickCount) 'New instance of .NET random number generator

        'Get a file name from the user
        Dim filename As String = ShowOpenFileDialogue()

        'Try to load the model in the selected file
        If Core.LoadModel(filename) Then

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Get baseline values
            'Makes a copy of the original parameters used to restore the model during the trials 
            SetBaselineParameters(orgB, orgPB)
            'You may want to run EcoPath/Ecosim and save the results
            'So you have a baseline state to compare against
            'For Example
            'Core.LoadEcosimScenario(1)
            'Core.Autosave(EwEUtils.Core.eAutosaveTypes.Ecosim) = True
            'Core.RunEcoPath()
            'Core.RunEcoSim()
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Run the trials
            For itrial As Integer = 1 To nTrials
                System.Console.WriteLine("Ecopath trial " + itrial.ToString)

                FoundBalancedModel = False
                iAttempt = 0

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Loop over Ecopath varying parameters and running 
                'until it finds a balanced set of parameters or run out of attempts
                Do Until FoundBalancedModel Or (iAttempt >= nBalanceAttempts)
                    iAttempt += 1
                    System.Console.WriteLine("  balance attempt " + iAttempt.ToString)

                    'Vary some Ecopath input parameters relative to the baseline
                    PerturbEcopathParameters(orgB, orgPB)

                    'Run Ecopath and see if the new parameters balanced
                    FoundBalancedModel = isEcoPathBalanced()

                Loop 'FoundBalancedModel Or (iSearch > nPathSearches)
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Do something with the balanced Ecopath model
                If FoundBalancedModel Then
                    'Ok we have found a set of balanced Ecopath parameters
                    'Do something
                    'You could run Ecosim and compare outputs to the baseline or some timeseries data
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    Core.LoadEcosimScenario(nScenario)
                    Core.LoadTimeSeries(1, True)
                    'Core.Autosave(EwEUtils.Core.eAutosaveTypes.Ecosim) = True

                    Core.EcoSimModelParameters.NumberYears = 10
                    If Core.RunEcoSim() = True Then
                        If Core.EcosimStats.SS < bestSS Then
                            bestSS = Core.EcosimStats.SS
                            'PreserveTrialParameters()
                            Console.WriteLine("Better SS: " & bestSS)
                        End If
                    Else
                        Console.WriteLine("Ecosim failed to run")
                    End If
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                Else
                    System.Console.WriteLine("Failed to find a balanced Ecopath model after " + iAttempt.ToString + " attempts")
                End If

            Next itrial

        End If 'core.LoadModel(filename)

        'Close up
        Core.CloseModel()

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

    Private Function PerturbParameter(ByVal Mean As Single, cv As Single) As Single
        If Mean < 0 Then Return Mean
        'You will have to do better than this
        'Each parameter should have its own distribution to sample from
        Return Mean * (1 + cv * RandNumGenerator.NextDouble())
    End Function


    Private Sub SetBaselineParameters(ByVal orgB() As Single, ByVal orgPB() As Single)

        For igrp As Integer = 1 To Core.nGroups
            orgB(igrp) = Core.EcoPathGroupInputs(igrp).BiomassAreaInput
            orgPB(igrp) = Core.EcoPathGroupInputs(igrp).PBInput
        Next

    End Sub

    Private Sub PerturbEcopathParameters(orgB() As Single, orgPB() As Single)


        'Core.SetBatchLockStop(Type) stops the Core from updating variable in response to input changes
        Core.SetBatchLock(cCore.eBatchLockType.Update)
        For igrp As Integer = 1 To Core.nGroups
            'In this case just B and PB
            Core.EcoPathGroupInputs(igrp).BiomassAreaInput = PerturbParameter(orgB(igrp), 0.3)
            Core.EcoPathGroupInputs(igrp).PBInput = PerturbParameter(orgPB(igrp), 0.3)
        Next
        'Core.ReleaseBatchLock() The Core will now update any variable that need to change in response to the edit
        Core.ReleaseBatchLock(cCore.eBatchLockType.Update)

    End Sub

    Private Function isEcoPathBalanced() As Boolean
        Dim EcopathRan As Boolean
        Dim isBalanced As Boolean

        'RunEcoPath(isModelBalanced) returns True if it found all the missing parameters
        'this does not mean the model balanced. 
        'Check the isModelBalanced Argument to see if the model balanced.
        'Alternatively you could check the outputs and decide if this is acceptable
        EcopathRan = Core.RunEcoPath(isBalanced)
        If EcopathRan And isBalanced Then
            'Yep found all the parameters and the model balanced. No EE > 1
            Return True
        End If

        Return False

    End Function


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

End Module
