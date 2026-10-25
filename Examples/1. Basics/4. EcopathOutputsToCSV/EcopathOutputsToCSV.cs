// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;
using EwEUtils.Utilities;

/// ---------------------------------------------------------------------------
/// <summary>
/// <para>This example demonstrates how to save results from a loaded EwE model to a
/// CSV files. Two methods are presented: a simple method to illustrate the
/// concept, and a more advanced method that takes internationalization issues
/// and tricky formatting issues into account.</para>
/// <para>This example also demonstrates a few handy EwE utilities from the
/// EwEUtils project.</para>
/// </summary>
/// ---------------------------------------------------------------------------
internal static partial class EcopathOutputsToCSV
{

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Starting point for this demonstration.
    /// </summary>
    /// -----------------------------------------------------------------------
    public static void Main()
    {

        string CSVfileSimple = Path.GetFullPath("Ecopath_out_simple.csv");
        string CSVfileAdvanced = Path.GetFullPath("Ecopath_out_advanced.csv");
        var core = new cCore();

        string modelFile = "tamba_bay.eiixml";

        if (core.LoadModel(modelFile))
        {
            if (core.RunEcopath())
            {

                // -- Write simple CSV file --

                if (SaveToCSVSimple(core, CSVfileSimple))
                {
                    Console.WriteLine("Simple CSV written to " + CSVfileSimple);
                }
                else
                {
                    Console.WriteLine("Simple CSV could not be written to " + CSVfileSimple);
                }
                Console.WriteLine();

                // -- Write advanced CSV file --

                if (SaveToCSVAdvanced(core, CSVfileAdvanced))
                {
                    Console.WriteLine("Advanced CSV written to " + CSVfileAdvanced);
                }
                else
                {
                    Console.WriteLine("Advanced CSV could not be written to " + CSVfileAdvanced);
                }
                Console.WriteLine();

            }

            core.CloseModel();
        }

        Console.WriteLine("Press a key to exit");
        Console.ReadKey();

    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Simple demonstration of how to save Ecopath results to a comma-separated 
    /// (CSV) file. This method just presents the principles of saving the CSV 
    /// file. Potential formatting problems and other disasters are happily 
    /// ignored. The method <see cref="SaveToCSVAdvanced"/> will be more robust.
    /// </summary>
    /// <param name="core">The core with a model that has already ran.</param>
    /// <param name="CSVFileName">The name of the file to write to.</param>
    /// <returns>True if successful.</returns>
    /// -----------------------------------------------------------------------
    private static bool SaveToCSVSimple(cCore core, string CSVFileName)
    {

        var writer = new StreamWriter(CSVFileName);

        // -- Write header --
        writer.WriteLine("Group,B,PB,QB,EE");

        // -- Write data --
        for (int iGroup = 1, loopTo = core.nGroups; iGroup <= loopTo; iGroup++)
        {
            cEcopathGroupOutput @group = core.get_EcopathGroupOutputs(iGroup);
            writer.WriteLine(group.Name + "," + group.Biomass + "," + group.PBOutput + "," + group.QBOutput + "," + group.EEOutput);
        }

        writer.Close();
        return true;

    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Simple demonstration of how to save Ecopath results to a comma-separated 
    /// (CSV) file. This method just presents the principles of saving the CSV 
    /// file. Potential formatting problems and other disasters are happily 
    /// ignored. The method <see cref="SaveToCSVAdvanced"/> will be more robust.
    /// </summary>
    /// <param name="core">The core with a model that has already ran.</param>
    /// <param name="CSVFileName">The name of the file to write to.</param>
    /// <returns>True if successful.</returns>
    /// -----------------------------------------------------------------------
    private static bool SaveToCSVAdvanced(cCore core, string CSVFileName)
    {

        // Test if the Ecopath model has ran. The state monitor can tell us that
        if (!core.StateMonitor.HasEcopathRan())
            return false;

        StreamWriter writer;
        try
        {
            // Try to create the file. It may already exist and be open, in which case .NET will 
            // throw an exception. Here, we exit the routine whenever anything goes wrong. 
            writer = new StreamWriter(CSVFileName);
        }
        catch (Exception ex)
        {
            return false;
        }

        // -- Write header --
        writer.WriteLine("Group,B,PB,QB,EE");

        // -- Write data --

        // Writing CSV files comes with two common challenges:

        // 1) Group names may contains commas, spaces or other characters that may confuse CSV readers. 
        // Since EwE regularly has to deal with this problem, a utility method cStringUtils.ToCSVField 
        // was added. In this method texts that contain problematic characters are encapsulated in 
        // double quotes.

        // 2) Some European languages use commas instead of points for decimal separators. CSV file 
        // readers absolutely love that. To overcome this problem we decided that EwE always should 
        // write CSV files using decimal points. cStringUtils.ToCSVField also does this.

        for (int iGroup = 1, loopTo = core.nGroups; iGroup <= loopTo; iGroup++)
        {
            cEcopathGroupOutput @group = core.get_EcopathGroupOutputs(iGroup);
            writer.WriteLine(cStringUtils.ToCSVField(group.Name) + "," +
                cStringUtils.ToCSVField(group.Biomass) + "," +
                cStringUtils.ToCSVField(group.PBOutput) + "," +
                cStringUtils.ToCSVField(group.QBOutput) + "," +
                cStringUtils.ToCSVField(group.EEOutput));
        }

        writer.Close();
        return true;

    }
}