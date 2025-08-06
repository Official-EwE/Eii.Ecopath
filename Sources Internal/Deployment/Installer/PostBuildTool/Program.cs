using System.Security.Cryptography;
using System.Text.Json;

class Program
{
    static async Task<int> Main(string[] args)
    {
        string root = @".\Output";

#if DEBUG
        root = "D:\\Sources\\Ecopath6\\Sources Internal\\Deployment\\Installer\\Output";
#endif
        foreach (string installerPath in Directory.GetFiles(root, "ewe_*-bit_setup.exe")) 
        {
            await this.ProcessFile(installerPath);
        }

        return 0;
    }

    public async Task<bool> ProcessFile(string? installerPath)
    {
        if (string.IsNullOrWhiteSpace(installerPath)) 
            return false;

        ComputeSHA sha = new();
        string hashFile = Path.ChangeExtension(installerPath, ".sha256.txt");
        if (!sha.Compute(installerPath, hashFile))
        {
            // Aargh
            return false;
        }
        Console.WriteLine("✔ SHA-256 written to " + hashFile);

        RunVirusTotal run = new();
        return await run.Run(installerPath, sha.SHA);
    }
}
