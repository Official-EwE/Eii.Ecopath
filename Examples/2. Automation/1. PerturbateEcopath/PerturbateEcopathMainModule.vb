Imports EwECore
Imports System.Windows.Forms


Module PerturbateEcopathMainModule

    Private core As cCore
    Private RandNumGenerator As Random

    Sub Main()

        core = New cCore()
        RandNumGenerator = New Random(Environment.TickCount)

        Dim nTrials As Integer = 10
        Dim nPathSearches As Integer = 100
        Dim iSearch As Integer
        Dim FoundBalancedModel As Boolean
        Dim EcopathRan As Boolean
        Dim isBalanced As Boolean
        Dim EcopathGroup As cEcoPathGroupInput


        Dim orgB() As Single
        Dim orgPB() As Single

        'Get a file name from the user
        Dim filename As String = ShowOpenFileDialogue()

        'Try to load the model in the selected file
        If core.LoadModel(filename) Then

            orgB = New Single(core.nGroups) {}
            orgPB = New Single(core.nGroups) {}

            For igrp As Integer = 1 To core.nGroups
                orgB(igrp) = core.EcoPathGroupInputs(igrp).BiomassAreaInput
                orgPB(igrp) = core.EcoPathGroupInputs(igrp).PBInput
            Next

            For itrial As Integer = 1 To nTrials
                System.Console.WriteLine("Ecopath trial " + itrial.ToString)

                FoundBalancedModel = False
                iSearch = 0
                Do Until FoundBalancedModel Or (iSearch > nPathSearches)

                    For igrp As Integer = 1 To core.nGroups
                        EcopathGroup = core.EcoPathGroupInputs(igrp)
                        EcopathGroup.BiomassAreaInput = RandomizeParameter(orgB(igrp), 0.3)
                        EcopathGroup.PBInput = RandomizeParameter(orgPB(igrp), 0.3)
                    Next

                    'RunEcoPath(isModelBalanced) returns True if it found all the missing parameters
                    'this does not mean the model balanced. 
                    'Check the isModelBalanced Argument to see if the model balanced 
                    EcopathRan = core.RunEcoPath(isBalanced)
                    If EcopathRan And isBalanced Then
                        'Yep found all the parameters and the model balanced
                        'No EE > 1
                        FoundBalancedModel = True
                    End If

                    iSearch += 1
                    System.Console.WriteLine("  balanced parameter search " + iSearch.ToString)

                Loop 'FoundBalancedModel Or (iSearch > nPathSearches)

                If FoundBalancedModel Then
                    'Ok we have found a set of balanced Ecopath parameters
                    'Do something 
                    '    DumpEcopathEcosimRun()
                    'End If

                Else
                    System.Console.WriteLine("Failed to find a balanced Ecopath model after " + iSearch.ToString + " tries")
                End If

            Next itrial

        End If 'core.LoadModel(filename)

        'Close up
        core.CloseModel()

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

    Private Function RandomizeParameter(ByVal Mean As Single, cv As Single) As Single
        If Mean < 0 Then Return Mean
        Return Mean * (1 + cv * RandNumGenerator.NextDouble())
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
