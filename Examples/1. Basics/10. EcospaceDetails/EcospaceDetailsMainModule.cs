using EwECore;
using EwEUtils.Utilities;
using System.Diagnostics;

namespace EcospaecDetails
{
    /// <summary>
    /// An example application that extracts various Ecospace input data to a text file.
    /// </summary>
    static class EcospaceDetailsMainModule
    {

        private static cCore? core;

        public static void Main()
        {
            core = new cCore();

            // Get a file name from the user
            string modelfilename = @".\Anchovy Bay Spatial.eiixml";
            string outputfilename = @".\ecospace_details.txt";
            var writer = new StreamWriter(outputfilename);

            Console.WriteLine("Your output file is going to: " + outputfilename);

            // Try to load the model in the selected file
            if (core.LoadModel(modelfilename))
            {
                if (core.nEcosimScenarios > 0 & core.nEcospaceScenarios > 0)
                {
                    if (core.LoadEcosimScenario(1))
                    {
                        if (core.LoadEcospaceScenario(1))
                        {
                            Console.WriteLine("Loaded first Ecospace scenario");

                            WriteScenarioDetails(core, writer);
                            WriteGroups(core, writer);
                            WriteFleets(core, writer);
                            WriteHabitats(core, writer);
                            WriteMPAs(core, writer);
                            WriteImportanceLayers(core, writer);
                            WriteEnvironmentalDriverLayers(core, writer);

                            writer.Close();

                            var pi = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                FileName = outputfilename
                            };
                            Process.Start(pi);
                        }
                        else
                        {
                            Console.WriteLine("Failed to load first Ecospace scenario");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to load first Ecosim scenario");
                    }
                }
                else
                {
                    Console.WriteLine("This model is missing an Ecosim or Ecospace scenario");
                }
                core.CloseModel();
            }
            else
            {
                Console.WriteLine("Model did not load");
            }

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out Ecospace map details.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteScenarioDetails(cCore core, StreamWriter writer)
        {
            cEwEScenario scenario = core.get_EcospaceScenarios(core.ActiveEcospaceScenarioIndex);

            writer.WriteLine("Scenario   : " + scenario.Name);
            writer.WriteLine("Author     : " + scenario.Author);
            writer.WriteLine("Description: " + scenario.Description);

            var modelDate = cDateUtils.JulianToDate(scenario.LastSaved);
            writer.WriteLine("Last saved : " + modelDate.ToLongDateString());

            var map = core.EcospaceBasemap;
            writer.WriteLine("# rows     : " + map.InRow);
            writer.WriteLine("# columns  : " + map.InCol);
            writer.WriteLine("Spatial ref: " + map.PosTopLeft.Y + "N, " + map.PosBottomRight.X + "W, " + map.PosBottomRight.Y + "S, " + map.PosTopLeft.X + "E");
            writer.WriteLine("Cell size  : " + map.CellSize);
            writer.WriteLine("Cell length: " + map.CellLength);
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out basic Ecospace group details.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteGroups(cCore core, StreamWriter writer)
        {
            writer.WriteLine("# groups: " + core.nGroups);

            for (int iGroup = 1, loopTo = core.nGroups; iGroup <= loopTo; iGroup++)
            {
                // Note that Ecospace groups are not called 'EcospaceGroupInputs': when we started
                // coding EwE6 there was no Ecospace group output data, so we (naively) thought it
                // not necessary to create explicit 'input' groups. An inconsistent oversight...
                var @group = core.get_EcospaceGroupInputs(iGroup);
                writer.WriteLine("   " + iGroup + ": " + group.Name);

                // Display basic habitat usage info
                // Different behaviour: nHabitats includes the 'all' habitat at index 0. Not at the end like Ecosim fleets. Great.
                for (int iHabitat = 0, loopTo1 = core.nHabitats - 1; iHabitat <= loopTo1; iHabitat++)
                {
                    if (group.get_PreferredHabitat(iHabitat) > 0f)
                    {
                        var habitat = core.get_EcospaceHabitats(iHabitat);
                        writer.WriteLine("      Uses habitat " + habitat.Name + " " + (int)Math.Round(group.get_PreferredHabitat(iHabitat) * 100f) + "%");
                    }
                }
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out basic Ecospace fleet details.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteFleets(cCore core, StreamWriter writer)
        {
            writer.WriteLine("# fleets: " + core.nFleets);

            for (int iFleet = 1, loopTo = core.nFleets; iFleet <= loopTo; iFleet++)
            {
                // Note that Ecospace fleets are also not explicitly labelled as 'inputs', for the
                // same reason as te groups above.
                var fleet = core.get_EcospaceFleetInputs(iFleet);
                writer.WriteLine("   " + iFleet + ": " + fleet.Name);

                for (int iHabitat = 0, loopTo1 = core.nHabitats - 1; iHabitat <= loopTo1; iHabitat++)
                {
                    if (fleet.get_HabitatFishery(iHabitat) == true) // Thanks Cam
                    {
                        var habitat = core.get_EcospaceHabitats(iHabitat);
                        writer.WriteLine("      Fishes in habitat " + habitat.Name);
                    }
                }

                for (int iMPA = 1, loopTo2 = core.nMPAs; iMPA <= loopTo2; iMPA++)
                {

                    if (fleet.get_MPAFishery(iMPA) == true) // Thanks Cam
                    {
                        var mpa = core.get_EcospaceMPAs(iMPA);
                        writer.WriteLine("      Fishes in MPA " + mpa.Name);
                    }
                }
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all Ecospace habitats, if any.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteHabitats(cCore core, StreamWriter writer)
        {
            writer.WriteLine("# habitats: " + core.nHabitats);

            // Different behaviour: nHabitats includes the 'all' habitat at index 0
            for (int iHabitat = 0, loopTo = core.nHabitats - 1; iHabitat <= loopTo; iHabitat++)
            {
                var habitat = core.get_EcospaceHabitats(iHabitat);
                writer.WriteLine("   " + iHabitat + ": " + habitat.Name);

                for (int iGroup = 1, loopTo1 = core.nGroups; iGroup <= loopTo1; iGroup++)
                {
                    // Note that Ecospace does not have cEcospaceGroupInput classes. This is a historical
                    // 'mistake', ecospace did not have group-based outputs for many years, and therefore
                    // initially the input / output distinction was not implemented.
                    var @group = core.get_EcospaceGroupInputs(iGroup);

                    if (group.get_PreferredHabitat(iHabitat) > 0f)
                    {
                        writer.WriteLine("      " + iGroup + ": " + group.Name + " uses " + (int)Math.Round(group.get_PreferredHabitat(iHabitat) * 100f) + "%");
                    }
                }
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all Ecospace MPAs.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteMPAs(cCore core, StreamWriter writer)
        {
            writer.WriteLine("# MPAs: " + core.nMPAs);
            for (int iMPA = 1, loopTo = core.nMPAs; iMPA <= loopTo; iMPA++)
            {
                var mpa = core.get_EcospaceMPAs(iMPA);

                writer.WriteLine("   " + iMPA + ": " + mpa.Name);

                for (int iMonth = 1; iMonth <= cCore.N_MONTHS; iMonth++)
                {
                    writer.Write("      " + cDateUtils.GetMonthName(iMonth) + ": ");
                    if (mpa.get_MPAMonth(iMonth))
                    {
                        writer.WriteLine("open");
                    }
                    else
                    {
                        writer.WriteLine("closed");
                    }
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all importance layers
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteImportanceLayers(cCore core, StreamWriter writer)
        {
            var map = core.EcospaceBasemap;

            writer.WriteLine("# importance layers: " + core.nImportanceLayers);
            for (int iLayer = 1, loopTo = core.nImportanceLayers; iLayer <= loopTo; iLayer++)
            {

                // Layers are distributed by the basemap
                var layer = map.get_LayerImportance(iLayer);
                writer.WriteLine("   " + iLayer + ": ");
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all environmental driver layers
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteEnvironmentalDriverLayers(cCore core, StreamWriter writer)
        {
            var basemap = core.EcospaceBasemap;
            var interactionmanager = core.CapacityMapInteractionManager;
            var shapemanager = core.EnviroResponseShapeManager;

            writer.WriteLine("# environmental driver layers: " + core.nEnvironmentalDriverLayers);
            for (int iLayer = 1, loopTo = core.nEnvironmentalDriverLayers; iLayer <= loopTo; iLayer++)
            {

                // Layers are distributed by the basemap
                var layer = basemap.get_LayerDriver(iLayer);
                writer.WriteLine("   " + iLayer + ": " + layer.Name);

                // Find all interactions for this current layer
                for (int iMap = 1, loopTo1 = interactionmanager.nEnviroData; iMap <= loopTo1; iMap++)
                {

                    var mapTest = interactionmanager.get_EnviroData(iMap);
                    if (ReferenceEquals(((cEnviroInputMap)mapTest).Layer, layer))
                    {

                        // Find all the groups that are driven by this map, and via which functions
                        for (int iGroup = 1, loopTo2 = core.nGroups; iGroup <= loopTo2; iGroup++)
                        {
                            int iShape = mapTest.get_ResponseIndexForGroup(iGroup);
                            if (iShape > 0)
                            {

                                // Get the shape object from the manager based on the core index
                                cEnviroResponseFunction shape = (cEnviroResponseFunction)shapemanager.get_CoreItem(iShape);
                                var @group = core.get_EcopathGroupInputs(iGroup);

                                writer.WriteLine("      " + group.Name + " mediated via response function " + shape.Name);
                            }
                        }
                    }
                }
            }
        }
    }
}