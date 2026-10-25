// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;

/// <summary>
/// Bare basics program that loads an EwE model and checks whether Ecopath can run.
/// </summary>
static class BareBasics
{

    public static void Main()
    {

        // Create a new core
        var core = new cCore();

        // Can we load a model into the core?
        if (core.LoadModel("Tampa_Bay.eiixml"))
        {
            Console.WriteLine("Model loaded");

            // Able to run Ecopath?
            if (core.RunEcopath())
            {
                Console.WriteLine("Ecopath ran successfully");
            }
            else
            {
                Console.WriteLine("Ecopath failed to run");
            }

            // Done
            core.CloseModel();
        }
        else
        {
            Console.WriteLine("Model did not load");
        }

        // Wait for the user to press a key before closing this program
        Console.WriteLine("Press a key to exit");
        Console.ReadKey();

    }

}
