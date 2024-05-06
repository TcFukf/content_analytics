using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics.TextFiles
{
    public class EncodingDetector
    {
        public static Encoding DetectFileEncoding(string fileName)
        {
            byte[] buf = new byte[12000];
            int length;
            using (FileStream fstream = File.OpenRead(fileName))
            {
                length = fstream.Read(buf, 0, buf.Length);
            }

            Ude.CharsetDetector d = new Ude.CharsetDetector();
            d.Feed(buf, 0, length);
            d.DataEnd();
            try
            {
                return Encoding.GetEncoding(d.Charset);
            }
            catch
            {
                Console.WriteLine($"coudnt detect encoding  for {fileName}");
                return Encoding.Default;
            }
        }
    }
}
