//' SPDX-License-Identifier: EUPL-1.2
//' This file is part of Ecopath with Ecosim (EwE).
//' Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;

/// <summary>
/// This program demonstrates how to obtain results from an Ecosim run.
/// </summary>
internal static partial class EcosimOutputsMainModule
{

    public static void Main()
    {

        var core = new cCore();

        // Get a file name from the user
        string filename = "Tampa_bay.eiixml";

        // Try to load the model in the selected file
        if (core.LoadModel(filename))
        {

            // Make sure the model contains at least one Ecosim Scenario
            if (core.nEcosimScenarios > 0)
            {

                // Get the first scenario and dump out its name
                cEcoSimScenario EcosimScenario = core.get_EcosimScenarios(1);
                Console.WriteLine("Ecosim scenario name = " + EcosimScenario.Name);

                // Load the scenario
                if (core.LoadEcosimScenario(EcosimScenario))
                {

                    // Set the number of years to run Ecosim for
                    core.EcosimModelParameters.NumberYears = 10;

                    // Run Ecosim and tell it to call onEcosimTimestep() with results at each Ecosim timestep
                    // Note that Ecopath is not explicitly ran; Ecopath will run implicitly when needed
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

    /// <summary>
    /// Callback method for running Ecosim. This Sub will be called by Ecosim at each timestep.
    /// </summary>
    /// <param name="iTime">The time step that Ecosim computed.</param>
    /// <param name="data">The resutls that Ecosim produced.</param>
    private static void onEcosimTimestep(long iTime, cEcoSimResults data)
    {

        // Write the timestep and some results out to the console window
        Console.WriteLine("Biomasses for Ecosim timestep " + iTime + ":");
        for (int iGrp = 1, loopTo = data.nGroups; iGrp <= loopTo; iGrp++)
            Console.WriteLine("  " + iGrp + ": " + data.Biomass[iGrp] + ", ");
        Console.WriteLine();

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
            Console.WriteLine("Group name, " + EcosimGroupOutputs.Name + ", Average Biomass, " + (sumB / core.nEcosimTimeSteps));

            // Results by Fleet for this Group           
            for (int iflt = 1, loopTo2 = core.nFleets; iflt <= loopTo2; iflt++)
            {

                // Get the Fleet definitions from the core
                fleet = core.get_EcopathFleetInputs(iflt);
                sumF = 0f;
                // Is this group fished by this fleet
                if (fleet.Landings[iGrp] + fleet.Discards[iGrp] > 0)
                {
                    // Yes sum F across all the timesteps for this Group/Fleet
                    for (int iTime = 1, loopTo3 = core.nEcosimTimeSteps; iTime <= loopTo3; iTime++)
                        sumF += EcosimGroupOutputs.get_FishingMortByFleet(iflt, iTime);
                    Console.WriteLine("  Fleet name, " + fleet.Name + ", Average fishing mortality, " + (sumF / core.nEcosimTimeSteps));
                }
            }

        }

    }

}