using System.Security.Cryptography;
using System.Text.Json;

class Program
{
    static async Task<int> Main(string[] args)
    {
        string vtApiKey = "6202df9c8dfc19c314a6a8bb037f6191fee52f3a5d8e0899c0aa41c65ff4d322";
        string root = @".\Output";

#if DEBUG
        root = "D:\\Sources\\Ecopath6\\Sources Internal\\Deployment\\Installer\\Output";
#endif
        foreach (string installerPath in Directory.GetFiles(root, "ewe_*-bit_setup.exe")) 
        {
            string sha256 = ComputeSHA256(installerPath);
            string hashFile = Path.ChangeExtension(installerPath, ".sha256.txt");
            File.WriteAllText(hashFile, "SHA-256: " + sha256);
            Console.WriteLine("✔ SHA-256 written to " + hashFile);

            if (!string.IsNullOrWhiteSpace(vtApiKey))
            {
                try
                {
                    string vtUrl = await UploadToVirusTotal(installerPath, vtApiKey);
                    string vtFile = installerPath + ".vtlink.txt";
                    File.WriteAllText(vtFile, vtUrl);
                    Console.WriteLine("✔ VirusTotal link saved to " + vtFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠ VirusTotal upload failed: " + ex.Message);
                }
            }
        }

        return 0;
    }

    static string ComputeSHA256(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        byte[] hash = sha256.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    static async Task<string> UploadToVirusTotal(string filePath, string apiKey)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-apikey", apiKey);
        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);
        var streamContent = new StreamContent(fileStream);
        form.Add(streamContent, "file", Path.GetFileName(filePath));

        var response = await client.PostAsync("https://www.virustotal.com/api/v3/files", form);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        string id = doc.RootElement.GetProperty("data").GetProperty("id").GetString();
        return $"https://www.virustotal.com/gui/file/{id}/detection";
    }
}
