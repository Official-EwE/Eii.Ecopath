using EwECore;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.CompilerServices;

namespace EcospaceInputs
{
    static class EcospaceInputs
    {

        #region Private Variables

        private static cCore core;

        private static cCoreStateMonitor _statemonitor;

        private static cCoreStateMonitor statemonitor
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _statemonitor;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_statemonitor != null)
                {
                    _statemonitor.CoreExecutionStateEvent -= onCoreExecutionStateEvent;
                }

                _statemonitor = value;
                if (_statemonitor != null)
                {
                    _statemonitor.CoreExecutionStateEvent += onCoreExecutionStateEvent;
                }
            }
        }

        #endregion

        #region Main

        public static void Main()
        {

            // Initialize a new instance of the core
            core = new cCore();

            // Get the instance of the StateMonitor from the Core
            // This will fire an the Event onCoreExecutionStateEvent(...) when EcoSpace has completed its run
            statemonitor = core.StateMonitor;

            // Get a file name from the user
            string modelfilename = @".\Tampa_Bay.eiixml";

            // Try to load the model in the selected file
            if (core.LoadModel(modelfilename))
            {

                if (core.nEcosimScenarios > 0 & core.nEcospaceScenarios > 0)
                {
                    // Load the first Ecosim and Ecospace scenarios
                    if (core.LoadEcosimScenario(1))
                    {
                        if (core.LoadEcospaceScenario(1))
                        {

                            Console.WriteLine("Loaded first Ecospace scenario");

                            // Set some Model Parameters. i.e. Run length...
                            setEcoSpaceModelParameters();

                            // Set fishing effort
                            setFishingEffort();

                            // Setup a Habitat Foraging Response function
                            setHabitatForagingResponse();

                            // Run Ecopace on this thread(synchronously)
                            // core.RunEcoSpace() will block until the run has completed.
                            // If runAsync = True core.RunEcoSpace() will return before the run has completed(asynchronously)
                            // and onCoreExecutionStateEvent(cCoreStateMonitor) will be called when the run has completed.
                            bool runAsync = false;
                            cCore.EcoSpaceInterfaceDelegate argEcospaceTimeStepHandler = onEcoSpaceTimeStep;
                            core.RunEcospace(ref argEcospaceTimeStepHandler, runAsync);
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
            }

            else
            {
                Console.WriteLine("Model did not load");
            }

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            core.CloseModel();

        }

        #endregion

        #region Set Ecospace Inputs

        private static void setEcoSpaceModelParameters()
        {

            core.EcospaceModelParameters.NumberOfTimeStepsPerYear = 12f;
            // Number of years to run
            core.EcospaceModelParameters.TotalTime = 10f;

            // Have Ecospace distribute the Fishing Effort as a function of, Catch Value, Area Fished and the Fishing Cost Map
            // If PredictEffort = False then Ecospace will use the static Ecopath Effort
            core.EcospaceModelParameters.PredictEffort = true;

            // Use the MultiStanza calculations
            core.EcospaceModelParameters.UseNewMultiStanza = true;
            // Alternativly use the IBM Model for Multistanza species distributions
            // core.EcospaceModelParameters.UseIBM = True

            // Initialize biomass to habitats/capacity
            core.EcospaceModelParameters.AdjustSpace = true;

            // Populate the Capacity map base on Capacity maps
            for (int i = 1, loopTo = core.nGroups; i <= loopTo; i++)
                core.get_EcospaceGroupInputs(i).CapacityCalculationType = eEcospaceCapacityCalType.EnvResponses;

        }

        private static void setFishingEffort()
        {
            float dEffort;
            cFishingRateShape EffortShape;

            // EcoSpace uses the Ecosim Fishing Effort shape for its effort over time input
            // If PredictEffort = True fishing effort is then distributed spatially at each timestep base on Catch Value, Cost and Area Fished 

            dEffort = (float)(2d / core.nEcospaceTimeSteps);
            for (int iflt = 1, loopTo = core.nFleets; iflt <= loopTo; iflt++)
            {
                EffortShape = (cFishingRateShape)core.FishingEffortShapeManager[iflt];
                EffortShape.LockUpdates();
                // Just set Effort to increase over time
                for (int it = 1, loopTo1 = core.nEcospaceTimeSteps; it <= loopTo1; it++)
                    EffortShape.set_ShapeData(it, it * dEffort);
                EffortShape.UnlockUpdates();
            }
        }

        private static void setHabitatForagingResponse()
        {
            var DatabaseID = default(int);
            cEcospaceLayerDriver Layer = null;

            // Habitat base foraging response is used to update the capacity map for each group.
            // During initialization the biomass in the cells is distributed by the capacity map
            // this give the initial distribution of biomass.
            // During a timestep the capacity map effects the dispersal rates and foraging rate for a group in a cell.

            // The habitat base foraging response to enviromental drivers has three main components
            // Two inputs and one Manager
            // 1. Input map layer cCore.EcospaceBasemap.LayerDriver
            // This is the enviromental driving map i.e. Salinity, Temperature...

            // 2. Input foraging response function a cEnviroResponseFunction = cCore.CapacityShapeManager.Item(index) 
            // this is a groups foraging response to an enviromental driver layer

            // 3. The manager core.CapacityMapInteractionManager is used to apply a foraging response function to a group for an input driver layer
            // It does this via a list of cEnviroInputMap objects which joins the LayerDriver to a response function for a group

            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            // First add a Enviromental Driver layer
            // Creates a new layer in the core and gives us the DatabaseID for the layer
            core.AddEcospaceDriverLayer("Example-Layer", "Layer added for example", "Sample units", ref DatabaseID);

            // Get the layer we just created from the core based on the DatabaseID that was set in AddEcospaceDriverLayer(,,DatabaseID)
            for (int iLayer = 1, loopTo = core.nEnvironmentalDriverLayers; iLayer <= loopTo; iLayer++)
            {
                if (DatabaseID == core.EcospaceBasemap.get_LayerDriver(iLayer).DBID)
                {
                    Layer = core.EcospaceBasemap.get_LayerDriver(iLayer);
                    break;
                }
            }

            if (Layer is null)
            {
                // oppss failed to find the layer we just added
                Console.WriteLine("Failed to find the Environmental Driver layer.");
                return;
            }

            // Set some values in the environmental layer
            for (int irow = 1, loopTo1 = core.EcospaceBasemap.InRow; irow <= loopTo1; irow++)
            {
                for (int icol = 1, loopTo2 = core.EcospaceBasemap.InCol; icol <= loopTo2; icol++)
                    Layer.set_Cell(irow, icol, value: irow * icol);
            }
            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            // Next the Response Function
            // this is how a group responses to an enviromental input i.e. Salinity
            var Manager = core.EnviroResponseShapeManager;
            cEnviroResponseFunction ResponseFunction;

            // Create a new response and give it some values
            ResponseFunction = (cEnviroResponseFunction)Manager.CreateNewShape("ResponseShape", null);
            float delta = (float)(1d / ResponseFunction.nPoints * 2d);
            ResponseFunction.LockUpdates();

            for (int ipoint = 1, loopTo3 = ResponseFunction.nPoints; ipoint <= loopTo3; ipoint++)
                // this is the response multiplier that is returned for a value on the x axis
                ResponseFunction.set_ShapeData(ipoint, ipoint * delta);

            // One last step
            // We need to tell the response function what range of data it covers on the x axis/map input values. 
            // It is possible for the response function to just cover part of the input data.
            // In this case we will use the entire range from the input layer, but this can be anything.
            ResponseFunction.ResponseRightLimit = Layer.MaxValue;
            ResponseFunction.ResponseLeftLimit = Layer.MinValue;

            ResponseFunction.UnlockUpdates();
            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            // Last
            // Now we have an enviromental map, the layer we added above
            // and a response function.
            // We need to join these together to tell EwE how a group responds to an enviromental driver (map)

            // CapacityMapInteractionManager contains a list of all the driver layers in the model
            // including the one we just added

            // loop over all the layers/maps in the manager and find the one we just added
            cEnviroInputMap Map;
            for (int imap = 1, loopTo4 = core.CapacityMapInteractionManager.nEnviroData; imap <= loopTo4; imap++)
            {
                Map = (cEnviroInputMap)core.CapacityMapInteractionManager.get_EnviroData(imap);
                // Is this the layer/map we added
                if (Map.Layer.DBID == DatabaseID)
                {

                    // Ok this is the layer we added
                    // Now we have to tell the map which group(s) use which response functions
                    for (int igrp = 1, loopTo5 = core.nGroups; igrp <= loopTo5; igrp++)
                        // for this example we will add this response function to all the groups
                        Map.set_ResponseIndexForGroup(igrp, true, ResponseFunction.Index);

                    break;

                }
            }
            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            // When Ecospace is run it will set the capacity layer(s) for all the groups 
            // using the Example-Layer and Response function we added

        }


        #endregion

        #region Ecospace Events

        private static void onEcoSpaceTimeStep(ref cEcospaceTimestep EcospaceResults)
        {
            // This method will be call at the end of each Ecospace Timestep
            // EcospaceResults will contain results maps for the timestep
            Console.WriteLine("Ecospace Timestep " + EcospaceResults.iTimeStep.ToString());
            var sumSpaceB = default(float);
            var nSpaceB = default(int);
            var sumPathB = default(float);

            for (int igrp = 1, loopTo = core.nGroups; igrp <= loopTo; igrp++)
            {

                for (int irow = 1, loopTo1 = core.EcospaceBasemap.InRow; irow <= loopTo1; irow++)
                {
                    for (int icol = 1, loopTo2 = core.EcospaceBasemap.InRow; icol <= loopTo2; icol++)
                    {
                        // Is this a water cell
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(core.EcospaceBasemap.LayerDepth.get_Cell(irow, icol), 0, false)))
                        {
                            // Sum Ecopace Biomass across all the water cells
                            nSpaceB += 1;
                            sumSpaceB += EcospaceResults.BiomassMap[irow, icol, igrp];
                        }

                    }
                }
                // Sum of Ecopath Biomass
                sumPathB += core.get_EcopathGroupOutputs(igrp).Biomass;

            }

            float deltaB;
            deltaB = sumSpaceB / nSpaceB / (sumPathB / core.nGroups);
            Console.WriteLine("  Change in average b " + deltaB.ToString());

        }

        private static void onCoreExecutionStateEvent(cCoreStateMonitor statemonitor)
        {

            if (statemonitor.CoreExecutionState() == eCoreExecutionState.EcospaceCompleted)
            {
                // Ecospace has completed a run 
                Console.WriteLine("Ecospace run completed");

                cEcospaceGroupOutput SpaceGroup;
                // Ecospace does not stores results over time and space
                // It just stores spatially averaged values for a few varaibles
                for (int igrp = 1, loopTo = core.nGroups; igrp <= loopTo; igrp++)
                {
                    SpaceGroup = core.get_EcospaceGroupOutput(igrp);
                    for (int it = 1, loopTo1 = core.nEcospaceTimeSteps; it <= loopTo1; it++)
                        // other outputs
                        // System.Console.Write(SpaceGroup.RelativeBiomass(it).ToString + ",")

                        // 'Outputs by Group/fleet
                        // For ift As Integer = 1 To core.nFleets
                        // System.Console.Write(SpaceGroup.CatchBiomass(ift, it).ToString + ",")
                        // System.Console.Write(SpaceGroup.Value(ift, it).ToString + ",")
                        // Next ift
                        // System.Console.WriteLine()
                        Console.Write(SpaceGroup.get_Biomass(it).ToString() + ",");

                    Console.WriteLine();
                }

                cEcospaceFleetOutput SpaceFleet;
                for (int ift = 1, loopTo2 = core.nFleets; ift <= loopTo2; ift++)
                {
                    SpaceFleet = core.get_EcospaceFleetOutput(ift);
                    for (int it = 1, loopTo3 = core.nEcospaceTimeSteps; it <= loopTo3; it++)
                        // other outputs
                        // SpaceFleet.Value(it)
                        Console.Write(SpaceFleet.get_CatchBiomass(it).ToString() + ",");
                    Console.WriteLine();
                }

            } // EwEUtils.Core.eCoreExecutionState.EcospaceCompleted 

        }

        #endregion
    }
}