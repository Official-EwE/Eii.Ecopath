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

Imports EwEUtils.Interop
Imports EwECore
Imports System.Windows.Forms
Imports System.Text

Module modConnectToR

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This example demonstrates how to execute an R script through VB.NET
    ''' </summary>
    ''' <remarks>
    ''' Many thanks to the Ecotroph team and Jerome Guitton for working out the hard bits.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Sub Main()

        Dim PathToR As String = PickRLocation()
        If Not String.IsNullOrWhiteSpace(PathToR) Then

            Dim connection As New cRBridge(PathToR)

            RunSimpleScript(connection)
            RunEwEScript(connection)

        Else
            Console.WriteLine("R path not selected, aborting")
        End If

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
        Dim model As String = PickModelLocation()
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
                ' Make R report the content of the biomass array
                script.AppendLine("biomass")
                ' Do some magical math
                script.AppendLine("mean(biomass)")

                connection.Execute(script.ToString)
                DumpROutputAndErrors(connection)

            End If
        Else
            Console.WriteLine("Model not loaded")
        End If

    End Sub

    Private Sub DumpROutputAndErrors(connection As cRBridge)

        ' Did R run successfully?
        If connection.LastRunSuccess Then
            ' #Yes: write R ouptut to the console window
            Console.WriteLine("Output:")
            For i As Integer = 0 To connection.Output.Length - 1
                Console.WriteLine(connection.Output(i))
            Next
        Else
            ' #No: write R errors to the console window
            Console.WriteLine("Errors:")
            For i As Integer = 0 To connection.Errors.Length - 1
                Console.WriteLine(connection.Errors(i))
            Next
        End If
        Console.WriteLine("")

    End Sub

#Region " Selecting files "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Presents the user with a standard Windows interface for selecting a file
    ''' </summary>
    ''' <param name="strCaption">Caption for the file open form.</param>
    ''' <param name="strFileFilter">File filter to use for the file open form.</param>
    ''' <param name="strLastPickedFile">The location that the user picked last time.</param>
    ''' <returns>A user-selected file, or an empty string if the user did not select a file.</returns>
    ''' -----------------------------------------------------------------------
    Private Function PickFile(ByVal strCaption As String, _
                              ByVal strFileFilter As String, _
                              ByVal strLastPickedFile As String) As String

        'Create a new open file dialogue
        Dim openFileDialogue As New OpenFileDialog()

        openFileDialogue.Title = strCaption
        openFileDialogue.Filter = strFileFilter
        openFileDialogue.FileName = strLastPickedFile

        'Show the dialogue box and get the user-selected filename
        If (openFileDialogue.ShowDialog() = DialogResult.OK) Then
            Return openFileDialogue.FileName
        End If

        'The user did not select a file. Return an empty string
        Return String.Empty

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Pick the location of the R application on your system
    ''' </summary>
    ''' <returns>The location of the R application, or an empty string if the 
    ''' user did not complete the selection process.</returns>
    ''' -----------------------------------------------------------------------
    Private Function PickRLocation() As String

        ' Retrieve last used path to R from the persistent application settings
        Dim PathToR As String = PickFile("Select R location", "R application|R.ewe", My.Settings.PathToR)

        If Not String.IsNullOrWhiteSpace(PathToR) Then
            ' Store path to R in the persistent application settings for the next time you run this application
            My.Settings.PathToR = PathToR
            ' Just to be sure
            My.Settings.Save()
        End If
        Return PathToR

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Pick the location of the EwE model to use
    ''' </summary>
    ''' <returns>The location of a model, or an empty string if the user did not 
    ''' complete the selection process.</returns>
    ''' -----------------------------------------------------------------------
    Private Function PickModelLocation() As String

        ' Retrieve last used model path from the persistent application settings
        Dim PathToModel As String = PickFile("Select EwE model", "EwE models|*.ewemdb;*.mdb;*.eweaccdb;*.accdb", My.Settings.PathToModel)

        If Not String.IsNullOrWhiteSpace(PathToModel) Then
            ' Store path to model in the persistent application settings for the next time you run this application
            My.Settings.PathToModel = PathToModel
            ' Just to be sure
            My.Settings.Save()
        End If
        Return PathToModel

    End Function
#End Region ' Selecting files

End Module
