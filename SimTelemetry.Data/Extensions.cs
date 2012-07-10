using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimTelemetry.Data
{
    public static partial class Extensions
    {

        public static void WriteString(this FileStream fs, string f)
        {
            byte[] b = ASCIIEncoding.ASCII.GetBytes(f);

            fs.Write(b, 0, b.Length);

        }
        public static void WriteLine(this FileStream fs, string f)
        {
            try
            {
                byte[] b = ASCIIEncoding.ASCII.GetBytes(f + "\r\n");

                fs.Write(b, 0, b.Length);
            }catch(Exception e)
            {
                //throw e;
            }
        }

        public static string ReadLine(this FileStream fs)
        {
            List<byte> b = new List<byte>();

            while(b.Count == 0 ||  b[b.Count-1] != 10)
            {
                if (fs.Position == fs.Length) break;
                b.Add((byte)fs.ReadByte());
            }

            return ASCIIEncoding.ASCII.GetString(b.ToArray());
        }
    }
}