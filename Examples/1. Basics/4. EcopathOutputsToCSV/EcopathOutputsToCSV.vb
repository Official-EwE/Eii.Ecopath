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

Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' <para>This example demonstrates how to save results from a loaded EwE model to a
''' CSV files. Two methods are presented: a simple method to illustrate the
''' concept, and a more advanced method that takes internationalization issues
''' and tricky formatting issues into account.</para>
''' <para>This example also demonstrates a few handy EwE utilities from the
''' EwEUtils project.</para>
''' </summary>
''' ---------------------------------------------------------------------------
Module EcopathOutputsToCSV

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Starting point for this demonstration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub Main()

        Dim CSVfileSimple As String = Path.GetFullPath("Ecopath_out_simple.csv")
        Dim CSVfileAdvanced As String = Path.GetFullPath("Ecopath_out_advanced.csv")
        Dim core As New cCore()

        Dim modelFile As String = BrowseToModel()

        If core.LoadModel(modelFile) Then
            If core.RunEcoPath Then

                ' -- Write simple CSV file --

                If SaveToCSVSimple(core, CSVfileSimple) Then
                    Console.WriteLine("Simple CSV written to " & CSVfileSimple)
                Else
                    Console.WriteLine("Simple CSV could not be written to " & CSVfileSimple)
                End If
                Console.WriteLine()

                ' -- Write advanced CSV file --

                If SaveToCSVAdvanced(core, CSVfileAdvanced) Then
                    Console.WriteLine("Advanced CSV written to " & CSVfileAdvanced)
                Else
                    Console.WriteLine("Advanced CSV could not be written to " & CSVfileAdvanced)
                End If
                Console.WriteLine()

            End If

            core.CloseModel()
        End If

        Console.WriteLine("Press a key to exit")
        Console.ReadKey()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Simple demonstration of how to save Ecopath results to a comma-separated 
    ''' (CSV) file. This method just presents the principles of saving the CSV 
    ''' file. Potential formatting problems and other disasters are happily 
    ''' ignored. The method <see cref="SaveToCSVAdvanced"/> will be more robust.
    ''' </summary>
    ''' <param name="core">The core with a model that has already ran.</param>
    ''' <param name="CSVFileName">The name of the file to write to.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveToCSVSimple(core As cCore, CSVFileName As String) As Boolean

        Dim writer As New StreamWriter(CSVFileName)

        ' -- Write header --
        writer.WriteLine("Group,B,PB,QB,EE")

        ' -- Write data --
        For iGroup As Integer = 1 To core.nGroups
            Dim group As cEcoPathGroupOutput = core.EcoPathGroupOutputs(iGroup)
            writer.WriteLine(group.Name & "," & group.Biomass & "," & group.PBOutput & "," & group.QBOutput & "," & group.EEOutput)
        Next

        writer.Close()
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Simple demonstration of how to save Ecopath results to a comma-separated 
    ''' (CSV) file. This method just presents the principles of saving the CSV 
    ''' file. Potential formatting problems and other disasters are happily 
    ''' ignored. The method <see cref="SaveToCSVAdvanced"/> will be more robust.
    ''' </summary>
    ''' <param name="core">The core with a model that has already ran.</param>
    ''' <param name="CSVFileName">The name of the file to write to.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveToCSVAdvanced(core As cCore, CSVFileName As String) As Boolean

        ' Test if the Ecopath model has ran. The state monitor can tell us that
        If (Not core.StateMonitor.HasEcopathRan) Then Return False

        Dim writer As StreamWriter
        Try
            ' Try to create the file. It may already exist and be open, in which case .NET will 
            ' throw an exception. Here, we exit the routine whenever anything goes wrong. 
            writer = New StreamWriter(CSVFileName)
        Catch ex As Exception
            Return False
        End Try

        ' -- Write header --
        writer.WriteLine("Group,B,PB,QB,EE")

        ' -- Write data --

        ' Writing CSV files comes with two common challenges:

        '  1) Group names may contains commas, spaces or other characters that may confuse CSV readers. 
        '     Since EwE regularly has to deal with this problem, a utility method cStringUtils.ToCSVField 
        '     was added. In this method texts that contain problematic characters are encapsulated in 
        '     double quotes.

        '  2) Some European languages use commas instead of points for decimal separators. CSV file 
        '     readers absolutely love that. To overcome this problem we decided that EwE always should 
        '     write CSV files using decimal points. cStringUtils.ToCSVField also does this.

        For iGroup As Integer = 1 To core.nGroups

            Dim group As cEcoPathGroupOutput = core.EcoPathGroupOutputs(iGroup)
            writer.WriteLine(cStringUtils.ToCSVField(group.Name) & "," & _
                             cStringUtils.ToCSVField(group.Biomass) & "," & _
                             cStringUtils.ToCSVField(group.PBOutput) & "," & _
                             cStringUtils.ToCSVField(group.QBOutput) & "," & _
                             cStringUtils.ToCSVField(group.EEOutput))

        Next

        writer.Close()
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show the 'open file' windows dialog to the user, and return the model that
    ''' the user selected. Note that the user may abort the dialog. In that case
    ''' no model name will be returned.
    ''' </summary>
    ''' <returns>A model name, or an empty string if the user did not pick a model </returns>
    ''' -----------------------------------------------------------------------
    Private Function BrowseToModel() As String

        Dim dialog As New OpenFileDialog()

        dialog.Title = "Select the model file to open"
        dialog.Filter = "EwE models|*.mdb;*.ewemdb;*.accdb;*.eweaccdb"
        dialog.CheckFileExists = True

        If (dialog.ShowDialog = DialogResult.OK) Then
            Return dialog.FileName
        End If

        Return String.Empty

    End Function

End Module
