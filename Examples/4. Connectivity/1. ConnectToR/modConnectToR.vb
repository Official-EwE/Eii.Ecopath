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

Imports EwEUtils.Interop
Imports EwECore
Imports System.Windows.Forms
Imports System.Text

Module modConnectToR

    ' Please change this path to point to R.exe on your local system!!!
    Dim PathToR As String = "C:\Program Files\R\R-2.15.0\bin\r.exe"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This example demonstrates how to execute an R script through VB.NET
    ''' </summary>
    ''' <remarks>
    ''' Many thanks to the Ecotroph team and Jerome Guitton for working out the hard bits.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Sub Main()

        ' Create a new R connection
        Dim connection As New cRBridge(PathToR)

        RunSimpleScript(connection)
        RunEwEScript(connection)

        ' Done
        Console.WriteLine("Press any key to exit")
        Console.ReadKey()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Running simple script serves just to demonstrate that .NET code can talk to R at all!
    ''' </summary>
    ''' <param name="connection">The <see cref="cRBridge">EwE-R bridge</see> to use for running the script.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RunSimpleScript(connection As cRBridge)

        Console.WriteLine("Running simple script")
        connection.Execute("getRversion()")
        DumpROutputAndErrors(connection)
        Console.WriteLine("")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Running the EwE script demonstrates how EwE data could be fed to R.
    ''' </summary>
    ''' <param name="connection">The <see cref="cRBridge">EwE-R bridge</see> to use for running the script.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RunEwEScript(connection As cRBridge)

        Console.WriteLine("Running EwE script")

        Dim core As New cCore()
        Dim model As String = PickModel()

        Console.WriteLine(model)

        If core.LoadModel(model) Then
            If core.RunEcoPath() Then

                ' Build R script. A stringbuilder is the most effective utility for 
                ' dynamically constructing and extending texts
                Dim script As New StringBuilder()
                script.Append("biomass<-c(")

                ' Build an R biomass array for all groups
                For iGroup As Integer = 1 To core.nGroups
                    Dim group As cEcoPathGroupOutput = core.EcoPathGroupOutputs(iGroup)
                    If iGroup > 1 Then script.Append(",")
                    script.Append(group.Biomass)
                Next
                script.AppendLine(")")
                ' Add an R script line to return the mean biomass
                script.AppendLine("mean(biomass)")

                connection.Execute(script.ToString)
                DumpROutputAndErrors(connection)

            End If
        Else
            Console.WriteLine("Model not loaded")
        End If

    End Sub

    Private Sub DumpROutputAndErrors(connection As cRBridge)

        Console.WriteLine("Output:")
        If connection.LastRunSuccess Then
            ' #Yes: write results to the console window
            For i As Integer = 0 To connection.Output.Length - 1
                Console.WriteLine(connection.Output(i))
            Next
        Else
            ' #No: write erros to the console window
            Console.WriteLine("Errors:")
            For i As Integer = 0 To connection.Errors.Length - 1
                Console.WriteLine(connection.Errors(i))
            Next
        End If
        Console.WriteLine("")

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Presents the user with a standard Windows interface for selecting a file
    ''' </summary>
    ''' <returns>A user-selected file, or an empty string if the user did not select a file.</returns>
    ''' -----------------------------------------------------------------------
    Private Function PickModel() As String

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
