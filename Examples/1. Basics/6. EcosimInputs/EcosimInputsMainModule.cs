using EwECore;
using System.Diagnostics;

namespace EcosimInputs
{

    /// <summary>
    /// This program demonstrates how to tweak Ecosim Vulnerabilities and Fishing Effort inputs.
    /// </summary>
    static class EcosimInputsMainModule
    {

        private static cCore? core;

        public static void Main()
        {
            core = new cCore();

            // Get a file name from the user
            string filename = @".\Tampa_Bay.eiixml";

            // Try to load the model in the selected file
            if (core.LoadModel(filename))
            {
                // Make sure the model contains at least one Ecosim Scenario
                if (core.nEcosimScenarios > 0)
                {
                    // Dump out all the scenario names
                    for (int iScen = 1, loopTo = core.nEcosimScenarios; iScen <= loopTo; iScen++)
                        Console.WriteLine("Ecosim scenario name = " + core.get_EcosimScenarios(iScen).Name);

                    // Load the first scenario
                    if (core.LoadEcosimScenario(1))
                    {

                        // Set the number of years to run Ecosim for
                        core.EcosimModelParameters.NumberYears = 10;

                        VaryVulnerabilities();
                        VaryFishingEffort();

                        // Run Ecosim and tell it to call onEcosimTimestep() with results at each Ecosim timestep
                        if (core.RunEcosim(onEcosimTimestep))
                        {
                            DumpEcosimResults(core);
                        }

                    } // core.LoadEcosimScenario(EcosimScenario)
                }
                else
                {
                    Console.WriteLine("This model does not contain any scenarios");
                } // core.EcosimScenarioCount > 0

                // Close up
                core.CloseModel();
            }
            else
            {
                Console.WriteLine("Model did not load");
            }

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();
        }


        private static void VaryVulnerabilities()
        {
            var rand = new Random();
            float factor = 0.5f;

            // Vulnerabilities are stored in a Prey/Pred matrix, vulnerability multiplier of a group(prey) to predation 
            // cCore.EcoSimGroupInputs(iprey).VulMult(ipred)

            // DietComp is stored in a Pred/Prey matrix, percentage of a groups diet made up by its prey
            // core.EcoPathGroupInputs(ipred).DietComp(iprey)

            // loop over the the groups matrix
            for (int iprey = 1, loopTo = core.nLivingGroups; iprey <= loopTo; iprey++)
            {
                for (int ipred = 1, loopTo1 = core.nLivingGroups; ipred <= loopTo1; ipred++)
                {
                    // is this prey group vulnerable to predation by this pred
                    if (core.get_EcopathGroupInputs(ipred).get_DietComp(iprey) > 0f)
                    {
                        // Yes this ipred consumes this iprey
                        // Randomly vary the VulMult of this group(iprey) to predation by group(ipred)
                        core.get_EcosimGroupInputs(iprey).set_VulMult(ipred, (float)((double)core.get_EcosimGroupInputs(iprey).get_VulMult(ipred) * (1d + (double)factor * (0.5d - rand.NextDouble()))));
                    }
                }
            }
        }


        private static void VaryFishingEffort()
        {
            cEcopathFleetInput FleetInput;
            var EffortManager = core.FishingEffortShapeManager;

            // Ecosim stores Fishing Effort and Fishing Mortality input data in cShapeData objects 
            // Each type of cShapeData has its own implementation
            // Fishing effort = cFishingRateShape
            // Fishing mortality = cFishingMortShape

            // Fishing effort shapes are stored in the FishingEffortShapeManager list
            foreach (cFishingRateShape EffortShape in core.FishingEffortShapeManager)
            {

                if (EffortShape.Index > core.nFleets)
                {
                    // FishingEffortShapeManager stores the "All Fleets" fleet in the last index cCore.nFleets + 1
                    // It is used by the core to update all the fleets to the same effort  
                    // This fleet does not have an FleetInput object so don't try to get it from the core
                    break;
                }

                // Get the Fleet Input data for this Fleet
                // cShape objects keeps their array index to core data in the .Index property
                FleetInput = core.get_EcopathFleetInputs(EffortShape.Index);

                Debug.Assert((EffortShape.Name ?? "") == (FleetInput.Name ?? ""));
                Console.WriteLine("Setting fishing effort * 2 for Fleet " + EffortShape.Name);

                // Lock all core updates of this data  
                EffortShape.LockUpdates();

                // Loop over all the Ecosim timesteps
                for (int it = 1, loopTo = core.nEcosimTimeSteps; it <= loopTo; it++)
                    // Effort is stored by cumulative month
                    EffortShape.set_ShapeData(it, EffortShape.get_ShapeData(it) * 2f);
                // Unlock the core updates for this fleet
                // this will update all the core data related to effort i.e. fishing mortality
                EffortShape.UnlockUpdates();

                // Dump out the update fishing mortality
                // Fishing Mortality shapes are stored in the cCore.FishMortShapeManager
                foreach (cFishingMortShape FishingMort in core.FishMortShapeManager)
                {
                    // The core array index is stored in the cShapeData.Index property
                    if (FleetInput.get_Landings(FishingMort.Index) > 0f)
                    {
                        // Fishing mortality shapes stored their data by time the same way as the EffortShapes
                        // FishingMort.ShapeData(TimeIndex)
                        Console.WriteLine(" F for group " + FishingMort.Name + " changed");
                    }
                }
            }
        }


        /// <summary>
        /// Callback method for running Ecosim. This Sub will be called by Ecosim at each timestep.
        /// </summary>
        /// <param name="iTime">The time step that Ecosim computed.</param>
        /// <param name="data">The resutls that Ecosim produced.</param>
        private static void onEcosimTimestep(long iTime, cEcoSimResults data)
        {

            // Write the timestep and some results out to the console window
            Console.WriteLine("On Ecosim timestep = " + iTime.ToString());
            for (int iGrp = 1, loopTo = data.nGroups; iGrp <= loopTo; iGrp++)
                Console.Write(data.Biomass[iGrp].ToString() + ", ");
            Console.WriteLine("--------------------------------------");

        }

        /// <summary>
        /// Write Ecosim results to the console after the end of a run.
        /// </summary>
        /// <param name="core"></param>
        private static void DumpEcosimResults(cCore core)
        {

            // Once an Ecosim run has been completed 
            // cCore.EcoSimGroupOutputs(GroupIndex) will contain the results for a group by timestep
            cEcosimGroupOutput EcosimGroupOutputs;
            float sumB;
            float sumF;
            // Fleet definitions
            cEcopathFleetInput fleet;

            Console.WriteLine("Ecosim results over time");
            Console.WriteLine();

            // Loop over all the groups
            for (int iGrp = 1, loopTo = core.nGroups; iGrp <= loopTo; iGrp++)
            {

                // Get the results for this group
                // Results by group Biomass(), Yield()...
                EcosimGroupOutputs = core.get_EcosimGroupOutputs(iGrp);

                // Results are stored by Timestep index starting at One
                sumB = 0f;
                for (int iTime = 1, loopTo1 = core.nEcosimTimeSteps; iTime <= loopTo1; iTime++)
                    sumB += EcosimGroupOutputs.get_Biomass(iTime);
                // Dump something to the console window 
                Console.WriteLine("Group name, " + EcosimGroupOutputs.Name + ", Average Biomass, " + (sumB / core.nEcosimTimeSteps).ToString());

                // Results by Fleet for this Group           
                for (int iflt = 1, loopTo2 = core.nFleets; iflt <= loopTo2; iflt++)
                {

                    // Get the Fleet definitions from the core
                    fleet = core.get_EcopathFleetInputs(iflt);
                    sumF = 0f;
                    // Is this group fished by this fleet
                    if (fleet.get_Landings(iGrp) + fleet.get_Discards(iGrp) > 0f)
                    {
                        // Yes sum F across all the timesteps for this Group/Fleet
                        for (int iTime = 1, loopTo3 = core.nEcosimTimeSteps; iTime <= loopTo3; iTime++)
                            sumF += EcosimGroupOutputs.get_FishingMortByFleet(iflt, iTime);
                        Console.WriteLine("  Fleet name, " + fleet.Name + ", Average fishing mortality, " + (sumF / core.nEcosimTimeSteps).ToString());
                    }
                }

            }

        }
    }
}