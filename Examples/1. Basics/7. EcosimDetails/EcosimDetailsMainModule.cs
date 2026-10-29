using EwECore;
using EwEUtils.Utilities;
using System.Diagnostics;

namespace EcosimDetails
{
    /// <summary>
    /// An example application that extracts various Ecosim input data to a text file.
    /// </summary>
    static class EcosimDetailsMainModule
    {

        private static cCore? core;

        public static void Main()
        {
            core = new cCore();

            // Get a file name from the user
            string filename = @".\Tampa_Bay.eiixml";
            var writer = new StreamWriter("ecosim_details.txt");

            // Try to load the model in the selected file
            if (core.LoadModel(filename))
            {
                if (core.nEcosimScenarios > 0)
                {
                    if (core.LoadEcosimScenario(1))
                    {
                        Console.WriteLine("Loaded first scenario");

                        // Load first time series dataset (if available)
                        if (core.nTimeSeriesDatasets > 0)
                        {
                            if (core.LoadTimeSeries(1, true))
                            {
                                Console.WriteLine("Loaded first time series data set");
                            }
                            else
                            {
                                Console.WriteLine("Failed to load first time series data set");
                            }
                        }

                        WriteScenarioDetails(core, writer);
                        WriteForcingFunctions(core, writer);
                        WriteEggProdFunctions(core, writer);
                        WriteMediationFunctions(core, writer);
                        WriteTimeSeries(core, writer);
                        WriteFishingEffort(core, writer);
                        WriteFishingMortality(core, writer);

                        writer.Close();

                        new Process
                        {
                            StartInfo = new ProcessStartInfo("ecosim_details.txt")
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                    }
                    else
                    {
                        Console.WriteLine("Failed to load first scenario");
                    }
                }
                else
                {
                    Console.WriteLine("This model does not contain any Ecosim scenarios");
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

            cEwEScenario scenario = core.get_EcosimScenarios(core.ActiveEcosimScenarioIndex);

            writer.WriteLine("Scenario   : " + scenario.Name);
            writer.WriteLine("Author     : " + scenario.Author);
            writer.WriteLine("Description: " + scenario.Description);

            var modelDate = cDateUtils.JulianToDate(scenario.LastSaved);
            writer.WriteLine("Last saved : " + modelDate.ToLongDateString());
            writer.WriteLine();

            writer.WriteLine("Start year : " + core.EcosimFirstYear());
            writer.WriteLine("# years    : " + core.nEcosimYears);
            writer.WriteLine();

            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all forcing functions in Ecosim and their allocation, if any.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteForcingFunctions(cCore core, StreamWriter writer)
        {
            var manager = core.ForcingShapeManager;
            var parameters = core.EcosimModelParameters;

            writer.WriteLine("# forcing functions: " + manager.Count);
            for (int i = 1, loopTo = manager.Count; i <= loopTo; i++)
            {
                // Forcing functions are zero-based indexed
                var shape = manager[i - 1];
                writer.Write("   " + i + ": " + shape.Name);
                if (shape.IsSeasonal)
                {
                    writer.Write(", seasonal");
                }
                else
                {
                    writer.Write(", annual");
                }

                if (parameters.NutForceFunctionNumber == i)
                {
                    writer.Write(", forces Nutrients");
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all egg production functions in Ecosim and their allocation, if any.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteEggProdFunctions(cCore core, StreamWriter writer)
        {
            var manager = core.EggProdShapeManager;

            writer.WriteLine("# egg production functions: " + manager.Count);
            for (int i = 0, loopTo = manager.Count - 1; i <= loopTo; i++)
            {
                // Egg production functions are zero-based indexed
                var shape = manager[i];
                writer.Write("   " + i + ": " + shape.Name);

                // Check all shape assignments
                for (int j = 1, loopTo1 = manager.GroupShapeList.Count(); j <= loopTo1; j++)
                {
                    var assignment = manager.GroupShapeList[j - 1];
                    if (assignment.ShapeID == i)
                    {

                        // Remember? Stanza groups are also zero-based indexed
                        var stanza = core.get_StanzaGroups(j - 1);
                        writer.Write(", used by " + stanza.Name);

                    }
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all mediation functions in Ecosim and their allocation, if any.
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteMediationFunctions(cCore core, StreamWriter writer)
        {
            var shapemanager = core.MediationShapeManager;
            var interactionmanager = core.MediatedInteractionManager;

            writer.WriteLine("# mediation functions: " + shapemanager.Count);

            var application = default(eForcingFunctionApplication);
            var Application = default(eForcingFunctionApplication);
            for (int i = 0, loopTo = shapemanager.Count - 1; i <= loopTo; i++)
            {
                // Mediation functions are zero-based indexed
                cMediationFunction shape = (cMediationFunction)shapemanager[i];
                writer.WriteLine("   " + i + ": " + shape.Name);

                for (int iPred = 1, loopTo1 = core.nLivingGroups; iPred <= loopTo1; iPred++)
                {
                    for (int iPrey = 1, loopTo2 = core.nGroups; iPrey <= loopTo2; iPrey++)
                    {
                        // Is there a mediated predation interaction?
                        if (interactionmanager.get_isPredPrey(iPred, iPrey))
                        {
                            cMediatedInteraction interaction = interactionmanager.get_PredPreyInteraction(iPred, iPrey);
                            var predator = core.get_EcopathGroupInputs(iPred);
                            var prey = core.get_EcopathGroupInputs(iPrey);
                            cForcingFunction shapetest = null;

                            for (int iApplication = 1, loopTo3 = interaction.nAppliedShapes; iApplication <= loopTo3; iApplication++)
                            {
                                if (interaction.getShape(iApplication, ref shapetest, ref application))
                                {
                                    if (ReferenceEquals(shape, shapetest))
                                    {
                                        writer.WriteLine("      Mediates of " + predator.Name + " onto " + prey.Name + ", affecting " + application.ToString());
                                    }
                                }
                            }
                        }
                    }
                }

                for (int iFleet = 1, loopTo4 = core.nFleets; iFleet <= loopTo4; iFleet++)
                {
                    for (int iGroup = 1, loopTo5 = core.nGroups; iGroup <= loopTo5; iGroup++)
                    {
                        // Is there a fishing mediation interaction?
                        if (interactionmanager.get_isLandings(iFleet, iGroup))
                        {
                            cMediatedInteraction interaction = interactionmanager.get_LandingInteraction(iFleet, iGroup);
                            var Fleet = core.get_EcopathFleetInputs(iFleet);
                            var Group = core.get_EcopathGroupInputs(iGroup);
                            cForcingFunction ShapeTest = null;

                            for (int iApplication = 1, loopTo6 = interaction.nAppliedShapes; iApplication <= loopTo6; iApplication++)
                            {
                                if (interaction.getShape(iApplication, ref ShapeTest, ref Application))
                                {
                                    if (ReferenceEquals(shape, ShapeTest))
                                    {
                                        writer.WriteLine("      Mediates of " + Fleet.Name + ", " + Group.Name + ", affecting " + Application.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                writer.WriteLine();
            }
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        /// Write out the names of all time series in Ecosim..
        /// </summary>
        /// <param name="core">The core to obtain details from.</param>
        /// <param name="writer">The text file writer to write to.</param>
        /// -----------------------------------------------------------------------
        private static void WriteTimeSeries(cCore core, StreamWriter writer)
        {
            writer.WriteLine("# time series: " + core.nTimeSeries);

            // No dataset loaded? Abort
            if (core.ActiveTimeSeriesDatasetIndex <= 0)
                return;

            var dataset = core.TimeSeriesDataset(core.ActiveTimeSeriesDatasetIndex);
            writer.WriteLine("Data set: " + dataset.Name + ", " + dataset.FirstYear);

            for (int iTimeSeries = 1, loopTo = core.nTimeSeries; iTimeSeries <= loopTo; iTimeSeries++)
            {
                var timeseries = dataset.get_TimeSeries(iTimeSeries);
                writer.Write("   " + timeseries.Name + ", " + timeseries.TimeSeriesType.ToString() + ", ");

                if (timeseries is cGroupTimeSeries)
                {
                    // Time series targets a group
                    var @group = core.get_EcopathGroupInputs(timeseries.DatPool);
                    writer.WriteLine("group " + group.Name);
                }
                else
                {
                    // Time series targets a fleet (perhaps should we test for this?)
                    var fleet = core.get_EcopathFleetInputs(timeseries.DatPool);
                    writer.WriteLine("fleet " + fleet.Name);
                }
            }
            writer.WriteLine();
        }

        private static void WriteFishingEffort(cCore core, StreamWriter writer)
        {
            // FishingEffortShapeManager contains a fleet for the "All Fleet" in the last position
            writer.WriteLine("# fishing effort shapes: " + (core.nFleets + 1).ToString());

            foreach (cFishingRateShape EffortShape in core.FishingEffortShapeManager)
            {
                writer.Write("   " + EffortShape.Name);
                // dump out one years worth of fishing effort points
                for (int it = 1; it <= 12; it++) // EffortShape.nPoints
                    writer.Write(", " + EffortShape.get_ShapeData(it).ToString());
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        private static void WriteFishingMortality(cCore core, StreamWriter writer)
        {
            writer.WriteLine("# fishing mortality shapes: " + core.nGroups.ToString());

            foreach (cFishingMortShape MortalityShape in core.FishMortShapeManager)
            {
                writer.Write("   " + MortalityShape.Name);

                if (core.get_EcopathGroupInputs(MortalityShape.Index).IsFished)
                {
                    // dump out one years worth of fishing mortality points
                    for (int it = 1; it <= 12; it++) // EffortShape.nPoints
                        writer.Write(", " + MortalityShape.get_ShapeData(it).ToString());
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine(", No fishing mortality on this group.");
                }
            }
            writer.WriteLine();
        }
    }
}