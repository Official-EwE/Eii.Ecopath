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

'Imports EwEUtils.Core
'Imports EwEUtils.Utilities
'Imports System.Drawing

Module EcospaceInputs

    Private core As cCore

    Sub Main()

        core = New cCore

        'Get a file name from the user
        Dim modelfilename As String = ShowOpenFileDialogue()

        'Try to load the model in the selected file
        If core.LoadModel(modelfilename) Then

            If core.nEcosimScenarios > 0 And core.nEcospaceScenarios > 0 Then
                If core.LoadEcosimScenario(1) Then
                    If core.LoadEcospaceScenario(1) Then

                        Console.WriteLine("Loaded first Ecospace scenario")

                        'Change some stuff
                        'Run the beatch...

                        core.EcospaceModelParameters.NumberOfTimeStepsPerYear = 12
                        core.EcospaceModelParameters.TotalTime = 2

                        core.RunEcoSpace(AddressOf onEcoSpaceTimeStep, True)



                    Else
                        Console.WriteLine("Failed to load first Ecospace scenario")
                    End If
                Else
                    Console.WriteLine("This model does not contain any Ecospace scenarios")
                End If
            Else
                Console.WriteLine("This model does not contain any Ecosim scenarios")
            End If

            core.CloseModel()
        Else
            Console.WriteLine("Model did not load")
        End If

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub


    Private Sub onEcoSpaceTimeStep(ByRef EcospaceResults As cEcospaceTimestep)

    End Sub


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


End Module
