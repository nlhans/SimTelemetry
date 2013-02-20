using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Simplicit.Net.Lzo;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests
{
    [TestFixture]
    class Sandbox
    {
        [Test]
        public void TestZipWithWhitespaces()
        {
            int sampleSize = 0x4000;
            int samples = 16384;
            int engineRpmOffset = 0x800;
            int speedOffset = 0x300;

            byte[] data = new byte[samples * sampleSize];
            byte[] data2 = new byte[samples * sampleSize];

            var rand = new Random();
            for (int sample = 0; sample < samples; sample++)
            {
                // Add gibberish
                for (int j = 0x1000; j < 0x2000; j += 4)
                {
                    int r = rand.Next(1, int.MaxValue - 1);
                    Array.Copy(BitConverter.GetBytes(r), 0, data, sampleSize * sample + j, 4);
                    Array.Copy(BitConverter.GetBytes(r), 0, data2, sampleSize * sample + j, 4);
                }

                Array.Copy(BitConverter.GetBytes((double)1000 + Math.Sin(sample / 10.0 * 2 * Math.PI)), 0, data, sampleSize * sample + engineRpmOffset, 8);
                Array.Copy(BitConverter.GetBytes((double)1000 + Math.Sin(sample / 10.0 * 2 * Math.PI)), 0, data2, sampleSize * sample + engineRpmOffset, 8);


                Array.Copy(BitConverter.GetBytes((double)12 + Math.Sin(sample / 5.0 * 2 * Math.PI)), 0, data, sampleSize * sample + speedOffset, 8);
                Array.Copy(BitConverter.GetBytes((double)52 + Math.Sin(sample / 5.0 * 2 * Math.PI)), 0, data, sampleSize * sample + speedOffset + 0x100, 8);

            }

            MemoryStream str1 = new MemoryStream(data);
            MemoryStream str2 = new MemoryStream(data2);

            ZipStorer zip = ZipStorer.Create("compression.zip", "");
            zip.AddStream(ZipStorer.Compression.Deflate, "1.bin", str1, DateTime.Now, "");
            zip.AddStream(ZipStorer.Compression.Deflate, "2.bin", str2, DateTime.Now, "");
            zip.Close();
        }

        [Test]
        public void TestLZOWithWhiteSpaces()
        {
            int sampleSize = 0x4000;
            int samples = 16384;
            int engineRpmOffset = 0x800;
            int speedOffset = 0x300;

            byte[] data = new byte[samples * sampleSize];
            byte[] data2 = new byte[samples * sampleSize];

            var rand = new Random();
            for (int sample = 0; sample < samples; sample++)
            {
                // Add gibberish
                for (int j = 0x1000; j < 0x2000; j += 4)
                {
                    int r = rand.Next(1, int.MaxValue - 1);
                    Array.Copy(BitConverter.GetBytes(r), 0, data, sampleSize * sample + j, 4);
                    Array.Copy(BitConverter.GetBytes(r), 0, data2, sampleSize * sample + j, 4);
                }

                Array.Copy(BitConverter.GetBytes((double)1000 + Math.Sin(sample / 10.0 * 2 * Math.PI)), 0, data, sampleSize * sample + engineRpmOffset, 8);
                Array.Copy(BitConverter.GetBytes((double)1000 + Math.Sin(sample / 10.0 * 2 * Math.PI)), 0, data2, sampleSize * sample + engineRpmOffset, 8);


                Array.Copy(BitConverter.GetBytes((double)12 + Math.Sin(sample / 5.0 * 2 * Math.PI)), 0, data, sampleSize * sample + speedOffset, 8);
                Array.Copy(BitConverter.GetBytes((double)52 + Math.Sin(sample / 5.0 * 2 * Math.PI)), 0, data, sampleSize * sample + speedOffset + 0x100, 8);

            }

            LZOCompressor lzo = new LZOCompressor();
            byte[] compressed1 = lzo.Compress(data);
            byte[] compressed2 = lzo.Compress(data2);

            File.WriteAllBytes("1.bin", compressed1);
            File.WriteAllBytes("2.bin", compressed2);
        }

    }

    }
}
