using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ViridisTableToArray
{
    class Program
    {
        // Utility to convert Villy's R dump of Viridis colour ramps to a VB array
        static void Main(string[] args)
        {
            using (StreamWriter sw = new StreamWriter("ramparrays.txt"))
            {
                sw.Write("Private m_ramps(,) As Integer = {");
                int k = 0;
                foreach (string fin in Directory.GetFiles(".", "viridis*.txt"))
                {
                    sw.WriteLine(k == 0 ? "" : ",");
                    k += 1;
                    using (StreamReader sr = new StreamReader(fin))
                    {
                        sr.ReadLine();
                        sw.Write("   {");
                        int n = 0;
                        while (!sr.EndOfStream)
                        {
                            String val = sr.ReadLine().Split(',')[1].Replace("\"", "").Trim().Substring(1, 6);
                            if (n > 0) sw.Write(", ");
                            n += 1;
                            sw.Write("&h" + val);
                        }
                        sw.Write("}");
                    }
                }
                sw.WriteLine("}");
            }
        }
    }
}
