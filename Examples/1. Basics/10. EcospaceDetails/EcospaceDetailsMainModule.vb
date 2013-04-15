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
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.Drawing

''' <summary>
''' An example application that extracts various Ecospace input data to a text file.
''' </summary>
''' 
Module EcospaceDetailsMainModule

    Private core As cCore

    Sub Main()

        core = New cCore()

        'Get a file name from the user
        Dim modelfilename As String = ShowOpenFileDialogue()
        Dim outputfilename As String = Path.GetFullPath("ecospace_details.txt")
        Dim writer As New StreamWriter(outputfilename)

        'Try to load the model in the selected file
        If core.LoadModel(modelfilename) Then

            If core.nEcosimScenarios > 0 And core.nEcospaceScenarios > 0 Then
                If core.LoadEcosimScenario(1) Then
                    If core.LoadEcospaceScenario(3) Then

                        Console.WriteLine("Loaded first Ecospace scenario")

                        WriteMapDetails(core, writer)
                        WriteHabitats(core, writer)
                        WriteMPAs(core, writer)
                        WriteImportanceLayers(core, writer)
                        WriteEnvironmentalDriverLayers(core, writer)

                        writer.Close()
                        Process.Start(outputfilename)

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
    ''' Write out Ecospace map details.
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteMapDetails(core As cCore, writer As StreamWriter)

        Dim map As cEcospaceBasemap = core.EcospaceBasemap
        writer.WriteLine("# rows     : " & map.InRow)
        writer.WriteLine("# columns  : " & map.InCol)
        writer.WriteLine("Spatial ref: " & map.PosTopLeft.Y & "N, " & map.PosBottomRight.X & "W, " & map.PosBottomRight.Y & "S, " & map.PosTopLeft.X & "E")
        writer.WriteLine("Cell size  : " & map.CellSize)
        writer.WriteLine("Cell length: " & map.CellLength)
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all Ecospace habitats, if any.
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteHabitats(core As cCore, writer As StreamWriter)

        writer.WriteLine("# habitats: " & core.nHabitats)

        ' Different behaviour: nHabitats includes the 'all' habitat at index 0
        For iHabitat As Integer = 0 To core.nHabitats - 1

            Dim habitat As cEcospaceHabitat = core.EcospaceHabitats(iHabitat)
            writer.WriteLine("   " & iHabitat & ": " & habitat.Name)

            For iGroup As Integer = 1 To core.nGroups
                ' Note that Ecospace does not have cEcospaceGroupInput classes. This is a historical
                ' 'mistake', ecospace did not have group-based outputs for many years, and therefore
                ' initially the input / output distinction was not implemented.
                Dim group As cEcospaceGroup = core.EcospaceGroups(iGroup)

                If group.PreferredHabitat(iHabitat) > 0 Then
                    writer.WriteLine("      " & iGroup & ": " & group.Name & " uses " & CInt(group.PreferredHabitat(iHabitat) * 100) & "%")
                End If
            Next
        Next iHabitat
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all Ecospace MPAs.
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteMPAs(core As cCore, writer As StreamWriter)

        writer.WriteLine("# MPAs: " & core.nMPAs)
        For iMPA As Integer = 1 To core.nMPAs

            Dim mpa As cEcospaceMPA = core.EcospaceMPAs(iMPA)
            Dim isopen As Boolean = False

            writer.Write("   " & iMPA & ": " & mpa.Name & ", ")

            For iMonth As Integer = 1 To cCore.N_MONTHS
                If mpa.MPAMonth(iMonth) Then
                    If isopen Then
                        writer.Write(", ")
                    Else
                        writer.Write("open ")
                    End If
                    writer.Write(cDateUtils.GetMonthName(iMonth, False))
                    isopen = True
                End If
            Next

            If Not isopen Then
                writer.Write("closed all year")
            End If

            writer.WriteLine()
        Next
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all importance layers
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteImportanceLayers(core As cCore, writer As StreamWriter)

        Dim map As cEcospaceBasemap = core.EcospaceBasemap

        writer.WriteLine("# importance layers: " & core.nImportanceLayers)
        For iLayer As Integer = 1 To core.nImportanceLayers

            ' Layers are distributed by the basemap
            Dim layer As cEcospaceLayerImportance = map.LayerImportance(iLayer)
            writer.WriteLine("   " & iLayer & ": ")
        Next
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all environmental driver layers
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteEnvironmentalDriverLayers(core As cCore, writer As StreamWriter)

        Dim basemap As cEcospaceBasemap = core.EcospaceBasemap
        Dim interactionmanager As cMapResponseInteractionManager = core.CapacityMapInteractionManager
        Dim shapemanager As cCapMapResponseManager = core.CapacityShapeManager

        writer.WriteLine("# environmental driver layers: " & core.nEnvironmentalDriverLayers)
        For iLayer As Integer = 1 To core.nEnvironmentalDriverLayers

            ' Layers are distributed by the basemap
            Dim layer As cEcospaceLayerDriver = basemap.LayerDriver(iLayer)
            writer.WriteLine("   " & iLayer & ": " & layer.Name)

            ' Find all interactions for this current layer
            For iMap As Integer = 1 To interactionmanager.nMaps

                Dim mapTest As IEnviroInputMap = interactionmanager.Map(iMap)
                If Object.ReferenceEquals(mapTest.Layer, layer) Then

                    ' Find all the groups that are driven by this map, and via which functions
                    For iGroup As Integer = 1 To core.nGroups
                        Dim iShape As Integer = mapTest.ResponseIndexForGroup(iGroup)
                        If (iShape > 0) Then

                            ' Get the shape object from the manager based on the core index
                            Dim shape As cEnviroResponseFunction = shapemanager.CoreItem(iShape)
                            Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)

                            writer.WriteLine("      " & group.Name & " mediated via response function " & shape.Name)
                        End If
                    Next

                End If
            Next iMap

        Next iLayer

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
