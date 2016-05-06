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
Imports EwEUtils.Core
Imports EwEUtils.Utilities

''' <summary>
''' An example application that extracts various Ecosim input data to a text file.
''' </summary>
Module EcosimDetailsMainModule

    Dim core As cCore

    Sub Main()

        core = New cCore()

        'Get a file name from the user
        Dim filename As String = ShowOpenFileDialogue()
        Dim writer As New StreamWriter("ecosim_details.txt")

        'Try to load the model in the selected file
        If core.LoadModel(filename) Then

            If core.nEcosimScenarios > 0 Then
                If core.LoadEcosimScenario(1) Then

                    Console.WriteLine("Loaded first scenario")

                    ' Load first time series dataset (if available)
                    If core.nTimeSeriesDatasets > 0 Then
                        If core.LoadTimeSeries(1, True) Then
                            Console.WriteLine("Loaded first time series data set")
                        Else
                            Console.WriteLine("Failed to load first time series data set")
                        End If
                    End If

                    WriteScenarioDetails(core, writer)
                    WriteForcingFunctions(core, writer)
                    WriteEggProdFunctions(core, writer)
                    WriteMediationFunctions(core, writer)
                    WriteTimeSeries(core, writer)
                    WriteFishingEffort(core, writer)
                    WriteFishingMortality(core, writer)

                    writer.Close()
                    Process.Start("ecosim_details.txt")

                Else
                    Console.WriteLine("Failed to load first scenario")
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
    Private Sub WriteScenarioDetails(core As cCore, writer As StreamWriter)

        Dim scenario As cEwEScenario = core.EcosimScenarios(core.ActiveEcosimScenarioIndex)

        writer.WriteLine("Scenario   : " & scenario.Name)
        writer.WriteLine("Author     : " & scenario.Author)
        writer.WriteLine("Description: " & scenario.Description)

        Dim modelDate As Date = cDateUtils.JulianToDate(scenario.LastSaved)
        writer.WriteLine("Last saved : " & modelDate.ToLongDateString)
        writer.WriteLine()

        writer.WriteLine("Start year : " & core.EcosimFirstYear)
        writer.WriteLine("# years    : " & core.nEcosimYears)
        writer.WriteLine()

        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all forcing functions in Ecosim and their allocation, if any.
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteForcingFunctions(core As cCore, writer As StreamWriter)

        Dim manager As cForcingFunctionManager = core.ForcingShapeManager
        Dim parameters As cEcoSimModelParameters = core.EcoSimModelParameters

        writer.WriteLine("# forcing functions: " & manager.Count)
        For i As Integer = 1 To manager.Count

            ' Forcing functions are zero-based indexed
            Dim shape As cForcingFunction = manager(i - 1)
            writer.Write("   " & i & ": " & shape.Name)
            If shape.IsSeasonal Then
                writer.Write(", seasonal")
            Else
                writer.Write(", annual")
            End If

            If parameters.NutForceFunctionNumber = i Then
                writer.Write(", forces Nutrients")
            End If
            If parameters.SalinityForceFunctionNumber = i Then
                writer.Write(", forces Salinity")
            End If
            If parameters.TemperatureForceFunctionNumber = i Then
                writer.Write(", forces Temperature")
            End If
            writer.WriteLine()
        Next
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all egg production functions in Ecosim and their allocation, if any.
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteEggProdFunctions(core As cCore, writer As StreamWriter)

        Dim manager As cEggProductionManager = core.EggProdShapeManager

        writer.WriteLine("# egg production functions: " & manager.Count)
        For i As Integer = 0 To manager.Count - 1

            ' Egg production functions are zero-based indexed
            Dim shape As cForcingFunction = manager(i)
            writer.Write("   " & i & ": " & shape.Name)

            ' Check all shape assignments
            For j = 1 To manager.GroupShapeList.Count
                Dim assignment As cGroupShapePair = manager.GroupShapeList(j - 1)
                If (assignment.ShapeID = i) Then

                    ' Remember? Stanza groups are also zero-based indexed
                    Dim stanza As cStanzaGroup = core.StanzaGroups(j - 1)
                    writer.Write(", used by " & stanza.Name)

                End If
            Next
            writer.WriteLine()
        Next
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all mediation functions in Ecosim and their allocation, if any.
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteMediationFunctions(core As cCore, writer As StreamWriter)

        Dim shapemanager As cMediationManager = core.MediationShapeManager
        Dim interactionmanager As cMediatedInteractionManager = core.MediatedInteractionManager

        writer.WriteLine("# mediation functions: " & shapemanager.Count)

        For i As Integer = 0 To shapemanager.Count - 1

            ' Mediation functions are zero-based indexed
            Dim shape As cMediationFunction = shapemanager(i)
            writer.WriteLine("   " & i & ": " & shape.Name)

            For iPred As Integer = 1 To core.nLivingGroups
                For iPrey As Integer = 1 To core.nGroups

                    ' Is there a mediated predation interaction?
                    If interactionmanager.isPredPrey(iPred, iPrey) Then

                        Dim interaction As cMediatedInteraction = interactionmanager.PredPreyInteraction(iPred, iPrey)
                        Dim predator As cEcoPathGroupInput = core.EcoPathGroupInputs(iPred)
                        Dim prey As cEcoPathGroupInput = core.EcoPathGroupInputs(iPrey)
                        Dim application As eForcingFunctionApplication
                        Dim shapetest As cForcingFunction = Nothing

                        For iApplication As Integer = 1 To interaction.nAppliedShapes

                            If interaction.getShape(iApplication, shapetest, application) Then
                                If (Object.ReferenceEquals(shape, shapetest)) Then
                                    writer.WriteLine("      Mediates of " & predator.Name & " onto " & prey.Name & ", affecting " & application.ToString)
                                End If
                            End If

                        Next
                    End If

                Next iPrey
            Next iPred

            For iFleet As Integer = 1 To core.nFleets
                For iGroup As Integer = 1 To core.nGroups

                    ' Is there a fishing mediation interaction?
                    If interactionmanager.isLandings(iFleet, iGroup) Then

                        Dim interaction As cMediatedInteraction = interactionmanager.LandingInteraction(iFleet, iGroup)
                        Dim Fleet As cFleetInput = core.FleetInputs(iFleet)
                        Dim Group As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
                        Dim Application As eForcingFunctionApplication
                        Dim ShapeTest As cForcingFunction = Nothing

                        For iApplication As Integer = 1 To interaction.nAppliedShapes
                            If interaction.getShape(iApplication, ShapeTest, Application) Then
                                If (Object.ReferenceEquals(shape, ShapeTest)) Then
                                    writer.WriteLine("      Mediates of " & Fleet.Name & ", " & Group.Name & ", affecting " & Application.ToString)
                                End If
                            End If
                        Next

                    End If
                Next
            Next
            writer.WriteLine()
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write out the names of all time series in Ecosim..
    ''' </summary>
    ''' <param name="core">The core to obtain details from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteTimeSeries(core As cCore, writer As StreamWriter)

        writer.WriteLine("# time series: " & core.nTimeSeries)

        ' No dataset loaded? Abort
        If core.ActiveTimeSeriesDatasetIndex <= 0 Then Return

        Dim dataset As cTimeSeriesDataset = core.TimeSeriesDataset(core.ActiveTimeSeriesDatasetIndex)
        writer.WriteLine("Data set: " & dataset.Name & ", " & dataset.FirstYear)

        For iTimeSeries As Integer = 1 To core.nTimeSeries
            Dim timeseries As cTimeSeries = dataset.TimeSeries(iTimeSeries)
            writer.Write("   " & timeseries.Name & ", " & timeseries.TimeSeriesType.ToString & ", ")

            If TypeOf timeseries Is cGroupTimeSeries Then
                ' Time series targets a group
                Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(timeseries.DatPool)
                writer.WriteLine("group " & group.Name)
            Else
                ' Time series targets a fleet (perhaps should we test for this?)
                Dim fleet As cFleetInput = core.FleetInputs(timeseries.DatPool)
                writer.WriteLine("fleet " & fleet.Name)
            End If

        Next
        writer.WriteLine()

    End Sub

    Private Sub WriteFishingEffort(core As cCore, writer As StreamWriter)
        'FishingEffortShapeManager contains a fleet for the "All Fleet" in the last position
        writer.WriteLine("# fishing effort shapes: " + (core.nFleets + 1).ToString)

        For Each EffortShape As cFishingRateShape In core.FishingEffortShapeManager
            writer.Write("   " + EffortShape.Name)
            'dump out one years worth of fishing effort points
            For it As Integer = 1 To 12 ' EffortShape.nPoints
                writer.Write(", " + EffortShape.ShapeData(it).ToString)
            Next
            writer.WriteLine()
        Next EffortShape

        writer.WriteLine()

    End Sub

    Private Sub WriteFishingMortality(core As cCore, writer As StreamWriter)
        writer.WriteLine("# fishing mortality shapes: " + core.nGroups.ToString)

        For Each MortalityShape As cFishingMortShape In core.FishMortShapeManager

            writer.Write("   " + MortalityShape.Name)

            If core.EcoPathGroupInputs(MortalityShape.Index).IsFished Then
                'dump out one years worth of fishing mortality points
                For it As Integer = 1 To 12 ' EffortShape.nPoints
                    writer.Write(", " + MortalityShape.ShapeData(it).ToString)
                Next
                writer.WriteLine()
            Else
                writer.WriteLine(", No fishing mortality on this group.")
            End If

        Next MortalityShape

        writer.WriteLine()

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
