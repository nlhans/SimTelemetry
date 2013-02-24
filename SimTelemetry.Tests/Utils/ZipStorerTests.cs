using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests.Utils
{
    [TestFixture]
    public class ZipStorerTests
    {
        [Test]
        public void ZipUnzipTextFile()
        {
            string zipFileComment = "This is a test comment.";
            string fileComment = "This is a file comment.";

            string[] myFile = new string[10000];
            for (int i = 0; i < myFile.Length; i++)
                myFile[i] = "My file contains a new line, namely line : " + i;
            File.WriteAllLines("tmp.txt", myFile);

            var filesizeOfInput = new FileInfo("tmp.txt").Length;

            ZipStorer writeZipFile = ZipStorer.Create("ziptmp2.zip", zipFileComment);
            writeZipFile.AddFile(ZipStorer.Compression.Deflate, "tmp.txt", "tmp.txt",fileComment);
            writeZipFile.AddFile(ZipStorer.Compression.Deflate, "tmp.txt", "tmp2.txt",fileComment);
            writeZipFile.Close();

            ZipStorer readZipFile = ZipStorer.Open("ziptmp2.zip", FileAccess.Read);
            Assert.AreEqual(zipFileComment, readZipFile.Comment);

            var files = readZipFile.ReadCentralDir();
            Assert.AreEqual(2, files.Count);

            Assert.AreEqual("tmp.txt", files[0].FilenameInZip);
            Assert.AreEqual("tmp2.txt", files[1].FilenameInZip);

            Assert.AreEqual(filesizeOfInput, files[0].FileSize);
            Assert.AreEqual(filesizeOfInput, files[1].FileSize);

            Assert.LessOrEqual(files[0].CompressedSize, filesizeOfInput);
            Assert.LessOrEqual(files[1].CompressedSize, filesizeOfInput);
            Debug.WriteLine("Compression ratio: " + Math.Round(files[0].CompressedSize*100.0/filesizeOfInput,3)+"%");

            readZipFile.ExtractFile(files[0], "./tmpout.txt");
            readZipFile.ExtractFile(files[0], "./tmp2out.txt");

            string[] file1 = File.ReadAllLines("./tmpout.txt");
            string[] file2 = File.ReadAllLines("./tmp2out.txt");
            Assert.AreEqual(myFile.Length, file1.Length);
            Assert.AreEqual(myFile.Length, file2.Length);

            for (int i = 0; i < myFile.Length; i++)
            {
                Assert.AreEqual(myFile[i], file1[i]);
                Assert.AreEqual(myFile[i], file2[i]);
            }

            // All is well!
            readZipFile.Close();

            readZipFile = null;
            writeZipFile = null;
            File.Delete("tmp.txt");
            File.Delete("ziptmp2.zip");
            File.Delete("tmpout.txt");
            File.Delete("tmp2out.txt");
        }


        [Test]
        public void ZipUnzipBinaryFile()
        {
            string zipFileComment = "This is a test comment.";
            string fileComment = "This is a file comment.";

            byte[] myFile = new byte[1024*1024];
            Random rand = new Random(102019123);
            for (int i = 0; i < myFile.Length; i++)
                myFile[i] = (byte) (rand.Next(1, 256));

            File.WriteAllBytes("tmp.bin", myFile);

            var filesizeOfInput = new FileInfo("tmp.bin").Length;

            ZipStorer writeZipFile = ZipStorer.Create("ziptmp.zip", zipFileComment);
            writeZipFile.AddFile(ZipStorer.Compression.Deflate, "tmp.bin", "tmp.bin", fileComment);
            writeZipFile.AddFile(ZipStorer.Compression.Deflate, "tmp.bin", "tmp2.bin", fileComment);
            writeZipFile.Close();

            ZipStorer readZipFile = ZipStorer.Open("ziptmp.zip", FileAccess.Read);
            Assert.AreEqual(zipFileComment, readZipFile.Comment);

            var files = readZipFile.ReadCentralDir();
            Assert.AreEqual(2, files.Count);

            Assert.AreEqual("tmp.bin", files[0].FilenameInZip);
            Assert.AreEqual("tmp2.bin", files[1].FilenameInZip);

            Assert.AreEqual(filesizeOfInput, files[0].FileSize);
            Assert.AreEqual(filesizeOfInput, files[1].FileSize);

            Assert.LessOrEqual(files[0].CompressedSize, filesizeOfInput);
            Assert.LessOrEqual(files[1].CompressedSize, filesizeOfInput);
            var compressionRate = files[0].CompressedSize*100.0/filesizeOfInput;
            Assert.AreEqual(100.0, compressionRate, 0.1); // we use completely random data, so it's a very bad compression ratio
            Debug.WriteLine("Compression ratio: " + compressionRate + "%");

            readZipFile.ExtractFile(files[0], "./tmpout.bin");
            readZipFile.ExtractFile(files[0], "./tmp2out.bin");

            byte[] file1 = File.ReadAllBytes("./tmpout.bin");
            byte[] file2 = File.ReadAllBytes("./tmp2out.bin");
            Assert.AreEqual(myFile.Length, file1.Length);
            Assert.AreEqual(myFile.Length, file2.Length);

            for (int i = 0; i < myFile.Length; i++)
            {
                Assert.AreEqual(myFile[i], file1[i]);
                Assert.AreEqual(myFile[i], file2[i]);
            }

            // All is well!
            readZipFile.Close();

            readZipFile = null;
            writeZipFile = null;
            File.Delete("tmp.txt");
            File.Delete("tmp.bin");
            File.Delete("tmpout.bin");
            File.Delete("tmp2out.bin");
            File.Delete("ziptmp.zip");
            File.Delete("tmpout.txt");
            File.Delete("tmp2out.txt");
        }
    }
}
