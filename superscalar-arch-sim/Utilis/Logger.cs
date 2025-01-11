using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.Utilis
{
    internal static class Logger
    {
        private static string DefaultDumpFile => (@"../../../logs/" + DateTime.Now.ToLongTimeString().Replace(":", "_") + "-cdump.txt");
        public static void DumpConsoleToFile(string path=null)
        {
            FileStream ostrm = null;
            StreamWriter writer = null;
            path = path ?? DefaultDumpFile;
            try
            {
                ostrm = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
                writer.Write(Console.Out.ToString());
            } 
            catch (Exception e)
            {
                Console.WriteLine($"Failed to dump console output to {path} : {e.Message}");
            } 
            finally
            {
                writer?.Close();
                ostrm?.Close();
            }
        }
    }
}
