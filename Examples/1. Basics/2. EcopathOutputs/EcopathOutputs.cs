// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;

/// <summary>
/// This code provides an example how to load an EwE model, run Ecopath, and extract a few computed results.
/// </summary>
internal static partial class EcopathOutputs
{
    public static void Main()
    {

        var core = new cCore();
        bool bModelBalanced = false;

        // Able to load model?
        if (core.LoadModel("Tampa_Bay.eiixml"))
        {
            Console.WriteLine("Model loaded");

            // Able to run Ecopath?
            if (core.RunEcopath(ref bModelBalanced))
            {
                Console.WriteLine("Ecopath ran successfully");

                if (bModelBalanced)
                {
                    Console.WriteLine("Ecopath balanced");
                }
                else
                {
                    Console.WriteLine("Ecopath did not balance");
                }
            }
            else
            {
                Console.WriteLine("Ecopath did not run successfully");
            }

            // Write values that have been computed by Ecopath
            for (int i = 1, loopTo = core.nGroups; i <= loopTo; i++)
            {

                // Obtain Ecopath output information for a single group from the core
                cEcopathGroupOutput @group = core.get_EcopathGroupOutputs(i);
                Console.WriteLine("Group " + i + ": B = " + group.Biomass + ", EE = " + group.EEOutput);

                // Note that .NET provides many ways to make outputs look pretty. 
                // Feel free to comment-out the versions below to see what they do:

                // 1. The same as above, just more compactly written
                // Console.WriteLine("Group {0}: B = {1}, EE = {2}", i, group.Biomass, group.EEOutput)

                // 2. Again the same, but with additional formatting to make results look more neat:
                // Value 0, group number, is written to two characters;
                // Value 1, computed group Biomass, is written to 9 characters with 5 decimals;
                // Value 2, computed group EE, is also written to 9 characters with 7 decimals;
                // Console.WriteLine("Group {0,2}: B = {1,9:F5}, EE = {2,9:F7}", i, group.Biomass, group.EEOutput)

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

}