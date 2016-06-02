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

''' ---------------------------------------------------------------------------
''' <summary>
''' This example demonstrates how to obtain details from a loaded EwE model.
''' Details are written to a text file.
''' </summary>
''' ---------------------------------------------------------------------------
Module EcopathDetails

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Starting point for this demonstration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub Main()

        Dim modelFile As String = BrowseToModel()

        ' Use EwE File utilities to generate a temporary text file
        ' The Ecopath model details will be written to this file
        Dim outputFile As String = cFileUtils.MakeTempFile(".txt")

        If Not String.IsNullOrWhiteSpace(modelFile) Then

            Dim core As New cCore()

            If core.LoadModel(modelFile) Then
                Console.WriteLine("Model " & modelFile & " loaded")

                Dim writer As New StreamWriter(outputFile)

                WriteAttributes(core, writer)
                WriteGroups(core, writer)
                WriteMultiStanza(core, writer)
                WriteDiets(core, writer)
                WriteFleets(core, writer)
                WriteTaxonomy(core, writer)
                WritePedigree(core, writer)
                WriteEcosimScenarios(core, writer)
                WriteEcosimTimeSeries(core, writer)
                WriteEcospaceScenarios(core, writer)
                WriteEcotracerScenarios(core, writer)

                writer.Close()
                core.CloseModel()

                Console.WriteLine("Output file written to " & outputFile)

                ' Launch the file via Windows
                Process.Start(outputFile)

            Else
                Console.WriteLine("Model did not load")
            End If

            ' Wait!!
            Console.WriteLine("Press a key to exit")
            Console.ReadKey()

        End If

    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write basic attributes for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get model  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteAttributes(core As cCore, writer As StreamWriter)

        Dim model As cEwEModel = core.EwEModel

        writer.WriteLine("Model      : " & model.Name)
        writer.WriteLine("Author     : " & model.Author)
        writer.WriteLine("Description: " & model.Description)

        ' Dates in the EwE model are stored in the Julian calendar which is more commonly used than the .NET date
        ' Use EwE Date utilities to perform this conversion
        Dim modelDate As Date = cDateUtils.JulianToDate(model.LastSaved)
        writer.WriteLine("Last saved : " & modelDate.ToLongDateString)
        writer.WriteLine()

        writer.WriteLine("Country    : " & model.Country)
        writer.WriteLine("Spatial ref: " & model.North & "N, " & model.West & "W, " & model.South & "S, " & model.East & "E")
        writer.WriteLine("Area size  : " & model.Area & " km2")
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write group  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteGroups(core As cCore, writer As StreamWriter)

        writer.WriteLine("# groups: " & core.nGroups)
        For iGroup As Integer = 1 To core.nGroups

            Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
            writer.Write("   " & iGroup & ": " & group.Name & " (")
            If group.IsDetritus Then
                writer.Write("detritus")
            ElseIf group.IsProducer Then
                writer.Write("producer")
            ElseIf group.IsConsumer Then
                writer.Write("consumer")
            Else
                writer.Write("?")
            End If
            writer.WriteLine(")")

        Next iGroup
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write group  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteMultiStanza(core As cCore, writer As StreamWriter)

        writer.WriteLine("# stanza: " & core.nStanzas)
        For iStanza As Integer = 1 To core.nStanzas

            ' Beware! Stanza groups are found by zero-based indices, not one-based!!

            Dim stanza As cStanzaGroup = core.StanzaGroups(iStanza - 1)
            writer.WriteLine("   " & iStanza & ": " & stanza.Name & " with " & stanza.nLifeStages & " life stages:")
            For iLifeStage As Integer = 1 To stanza.nLifeStages

                ' Get the index of the group that this life stage represents
                Dim iGroup As Integer = stanza.iGroups(iLifeStage)
                Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)

                writer.Write("      " & iGroup & ": " & group.Name & ", start age: " & stanza.StartAge(iLifeStage))
                If stanza.LeadingB = (iLifeStage - 1) Then
                    writer.Write(", leading B")
                End If
                If stanza.LeadingCB = (iLifeStage - 1) Then
                    writer.Write(", leading CB")
                End If
                writer.WriteLine()

            Next iLifeStage
        Next iStanza
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write diet  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteDiets(core As cCore, writer As StreamWriter)

        writer.WriteLine("Diet")
        For iPredator As Integer = 1 To core.nLivingGroups

            Dim predator As cEcoPathGroupInput = core.EcoPathGroupInputs(iPredator)
            If (predator.IsConsumer) Then

                writer.WriteLine("  Predator: " & predator.Name)

                For iPrey As Integer = 1 To core.nGroups

                    Dim prey As cEcoPathGroupInput = core.EcoPathGroupInputs(iPrey)
                    Dim diet As Single = predator.DietComp(iPrey)

                    ' Does predator eat this prey?
                    If (diet > 0) Then
                        writer.WriteLine("      " & iPrey & ": " & prey.Name & ", amount: " & diet)
                    End If

                Next iPrey
            End If
        Next iPredator
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write fleet  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteFleets(core As cCore, writer As StreamWriter)

        writer.WriteLine("# fleets: " & core.nFleets)
        For iFleet As Integer = 1 To core.nFleets

            Dim fleet As cFleetInput = core.FleetInputs(iFleet)
            writer.WriteLine("   " & iFleet & ": " & fleet.Name)

            For iGroup As Integer = 1 To core.nGroups
                If fleet.Landings(iGroup) > 0 Then
                    Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
                    writer.WriteLine("      Lands " & group.Name)
                End If

                If fleet.Discards(iGroup) > 0 Then
                    Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
                    writer.WriteLine("      Discards " & group.Name)
                End If
            Next

        Next iFleet
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write taxonomy  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteTaxonomy(core As cCore, writer As StreamWriter)

        writer.WriteLine("# taxa: " & core.nTaxon)
        For iTaxon As Integer = 1 To core.nTaxon

            Dim taxon As cTaxon = core.Taxon(iTaxon)
            writer.Write("   " & iTaxon & ": " & taxon.Common & " (" & taxon.Genus & " " & taxon.Species & ")")
            If (taxon.iGroup > 0) Then
                Dim group As cEcoPathGroupInput = core.EcoPathGroupInputs(taxon.iGroup)
                writer.WriteLine(", group: " & group.Name & ", prop. B: " & taxon.Proportion & ", prop. catch: " & taxon.ProportionCatch)
            ElseIf ((taxon.iStanza) >= 0) Then
                Dim stanza As cStanzaGroup = core.StanzaGroups(taxon.iStanza)
                writer.WriteLine(", stanza: " & stanza.Name & ", prop. B: " & taxon.Proportion & ", prop. catch: " & taxon.ProportionCatch)
            Else
                writer.WriteLine()
            End If

        Next iTaxon
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write pedigree  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WritePedigree(core As cCore, writer As StreamWriter)

        writer.WriteLine("# pedigree categories: " & core.nPedigreeVariables)
        For iVariable As Integer = 1 To core.nPedigreeVariables

            Dim variable As eVarNameFlags = core.PedigreeVariable(iVariable)
            Dim manager As cPedigreeManager = core.GetPedigreeManager(variable)

            writer.WriteLine("   " & iVariable & ": " & variable.ToString())
            For iLevel As Integer = 1 To manager.NumLevels

                Dim level As cPedigreeLevel = manager.Level(iLevel)
                writer.WriteLine("      " & iLevel & ": " & level.Name & ", conf: " & level.ConfidenceInterval & "%")

            Next
        Next iVariable
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write basic ecosim scenario  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteEcosimScenarios(core As cCore, writer As StreamWriter)

        writer.WriteLine("# Ecosim scenarios: " & core.nEcosimScenarios)
        For iScenario As Integer = 1 To core.nEcosimScenarios

            Dim scenario As cEcoSimScenario = core.EcosimScenarios(iScenario)
            Dim modelDate As Date = cDateUtils.JulianToDate(scenario.LastSaved)
            writer.WriteLine("   " & iScenario & ": " & scenario.Name)
            writer.WriteLine("      Author     : " & scenario.Author)
            writer.WriteLine("      Description: " & scenario.Description)
            writer.WriteLine("      Last saved : " & modelDate.ToLongDateString)

        Next iScenario
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write basic ecosim time series  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteEcosimTimeSeries(core As cCore, writer As StreamWriter)

        writer.WriteLine("# Ecosim time series datasets: " & core.nTimeSeriesDatasets)
        For iDataset As Integer = 1 To core.nTimeSeriesDatasets

            Dim dataset As cTimeSeriesDataset = core.TimeSeriesDataset(iDataset)
            writer.WriteLine("   " & iDataset & ": " & dataset.Name & ", # time series: " & dataset.nTimeSeries)

            ' Note that actual time series are not loaded yet. They only become available 
            ' if first an Ecosim scenario is loaded, and then a then the time series dataset.

            ' This Ecosim exercise will not be demonstrated here.

        Next iDataset
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write basic ecospace scenario  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteEcospaceScenarios(core As cCore, writer As StreamWriter)

        writer.WriteLine("# Ecospace scenarios: " & core.nEcospaceScenarios)
        For iScenario As Integer = 1 To core.nEcospaceScenarios

            Dim scenario As cEcospaceScenario = core.EcospaceScenarios(iScenario)
            Dim modelDate As Date = cDateUtils.JulianToDate(scenario.LastSaved)
            writer.WriteLine("   " & iScenario & ": " & scenario.Name)
            writer.WriteLine("      Author     : " & scenario.Author)
            writer.WriteLine("      Description: " & scenario.Description)
            writer.WriteLine("      Last saved : " & modelDate.ToLongDateString)

        Next iScenario
        writer.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write basic ecotracer scenario  for a model to a text file.
    ''' </summary>
    ''' <param name="core">The core to get  from.</param>
    ''' <param name="writer">The text file writer to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteEcotracerScenarios(core As cCore, writer As StreamWriter)

        writer.WriteLine("# Ecotracer scenarios: " & core.nEcotracerScenarios)
        For iScenario As Integer = 1 To core.nEcotracerScenarios

            Dim scenario As cEcotracerScenario = core.EcotracerScenarios(iScenario)
            writer.WriteLine("   " & iScenario & ": " & scenario.Name & ", author: " & scenario.Author)

        Next iScenario
        writer.WriteLine()

    End Sub

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
