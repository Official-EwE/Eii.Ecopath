
Imports System.IO

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Imports System.Windows.Forms


Module EcospaceInputs

    Private core As cCore

    Sub Main()

        core = New cCore()

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
