// ===============================================================================
// This file is part of Ecopath with Ecosim (EwE)
// 
// EwE is free software: you can redistribute it and/or modify it under the terms
// of the GNU General Public License version 2 as published by the Free Software 
// Foundation.
// 
// EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
// PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with EwE.
// If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
// 
// Copyright 1991- 
// UBC Fisheries Centre, Vancouver BC, Canada, and 
// Ecopath International Initiative, Barcelona, Spain
// ===============================================================================
// 

using EwECore;

namespace BareBasics;


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
