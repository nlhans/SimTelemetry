using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace SimTelemetry.Game.rFactor2
{
    public struct MAS2File
    {
        public string Filename;
        public string Filename_Path;

        public MAS2Reader Master;

        public bool IsCompressed { get { return (CompressedSize != UncompressedSize); } }

        public uint Index;
        public uint CompressedSize;
        public uint UncompressedSize;

        public uint FileOffset;
    }

    /// <summary>
    /// This class reads .MAS files for rFactor 2.
    /// Made in-house of course :-)
    /// </summary>
    public class MAS2Reader
    {
        private string mas2_file;
        private BinaryReader reader;
        private byte[] salt;
        private uint saltkey;
        private uint saltkey2;

        private byte[] file_header;

        public List<MAS2File> Files;

        private byte[] FileTypeKeys = new byte[16]
                             {
                                 0x42, 0xF8, 0x95, 0x20, 0xDE, 0x5F, 0xC1, 0x10, 0xD9, 0xC8, 0xAE, 0xD0, 0x0F, 0x0D,
                                 0x70, 0xAB
                             };
        private byte[] FileHeaderKeys = new byte[256]
                                      {
                                          0xB8, 0xA8, 0x8B, 0x07, 0x8A, 0x0E, 0xF2, 0x11, 0x68, 0xFB, 0xBC, 0xDB, 0x12,
                                          0xD0, 0xB6, 0xB3, 0x9F, 0x69, 0x55, 0x5F, 0xC7, 0xCA, 0x61, 0xAD, 0x3C, 0x56,
                                          0xC1, 0xDF, 0x46, 0x13, 0x28, 0x1C, 0x4B, 0x20, 0x3B, 0x75, 0xAC, 0xE7, 0x3E,
                                          0x9A, 0xB1, 0x8D, 0xE1, 0x6E, 0x6F, 0x14, 0xC0, 0x88, 0x97, 0x95, 0x6B, 0xB0,
                                          0xD7, 0x64, 0x17, 0x30, 0xA4, 0xCE, 0x66, 0x9E, 0x70, 0x03, 0x1E, 0xEC, 0xE9,
                                          0xDE, 0x5A, 0xB2, 0x90, 0xA2, 0xBA, 0x4A, 0x2A, 0xF8, 0x6D, 0xE6, 0x23, 0x59,
                                          0x7A, 0xEE, 0xF6, 0xDA, 0x3A, 0x60, 0xB5, 0xFE, 0x06, 0x2B, 0xEA, 0xD4, 0xE3,
                                          0x72, 0x62, 0x27, 0xA1, 0x4C, 0x0B, 0x3D, 0x8F, 0x34, 0xEB, 0x15, 0x18, 0x39,
                                          0xC5, 0x22, 0x58, 0x94, 0xF5, 0x42, 0xA3, 0xC2, 0x25, 0x87, 0x81, 0x48, 0x93,
                                          0x02, 0xD6, 0xC9, 0x71, 0x7D, 0x35, 0xBD, 0xD3, 0x09, 0x9D, 0x7F, 0x53, 0xCD,
                                          0xAE, 0x21, 0x4E, 0x77, 0xE8, 0x43, 0xD1, 0xB9, 0xFF, 0x33, 0xBB, 0xF7, 0x8C,
                                          0xB4, 0xBF, 0xD9, 0xAB, 0x4F, 0x4D, 0xE0, 0x6A, 0xD5, 0xF3, 0xD8, 0xC6, 0x08,
                                          0x83, 0x1D, 0xFA, 0x05, 0x41, 0xCB, 0xD2, 0xF9, 0xC3, 0x84, 0x65, 0x40, 0x49,
                                          0x54, 0xCC, 0xEF, 0x0F, 0xA6, 0xA9, 0xAF, 0x5C, 0x91, 0x1A, 0x36, 0xBE, 0x01,
                                          0x2F, 0x16, 0x1F, 0xA5, 0xFD, 0x45, 0x96, 0x52, 0x79, 0xE5, 0x10, 0x1B, 0x67,
                                          0x63, 0x24, 0x32, 0x80, 0x9C, 0x2E, 0x47, 0x57, 0x2D, 0x8E, 0x7E, 0xE4, 0x78,
                                          0xA0, 0x99, 0xDC, 0xB7, 0xFC, 0x76, 0xF4, 0x19, 0x3F, 0xAA, 0x5D, 0x04, 0x26,
                                          0x7C, 0xF0, 0xF1, 0x37, 0xA7, 0xED, 0x86, 0xDD, 0x98, 0xC4, 0x82, 0x89, 0x31,
                                          0x5B, 0x74, 0x0C, 0x92, 0x0D, 0x6C, 0x38, 0xCF, 0x51, 0x2C, 0x7B, 0x44, 0x50,
                                          0x0A, 0x9B, 0x5E, 0, 0x73, 0x29, 0x85, 0xC8, 0xE2  };


        public string File { get { return mas2_file; } }
        public int Count { get { return Files.Count; } }

        private string DecodeHeaderString(byte[] mas_header)
        {
            // The header is encoded with a XOR-like compression with 16-byte key.
            byte[] pkg_type = mas_header;

            for (int i = 0; i < 16; i++)
                pkg_type[i] = (byte)(pkg_type[i] ^ (FileTypeKeys[i] >> 1));

            string type = ASCIIEncoding.ASCII.GetString(pkg_type);
            return type;
        }

        private void ReadHeader()
        {
            byte[] file_type = reader.ReadBytes(16);
            string file_type_s = DecodeHeaderString(file_type).Replace('\0',' ').Trim();
            if (file_type_s == "GMOTOR_MAS_2.90")
            {
                salt = reader.ReadBytes(8);
                saltkey = BitConverter.ToUInt32(salt, 0);
                saltkey2 = BitConverter.ToUInt32(salt, 4);
                if (saltkey < 64) saltkey += 64;
                if (saltkey2 < 64) saltkey2 += 64;

                byte[] garbage = reader.ReadBytes(120 - 16 - 8);
                int bf_size = reader.ReadInt32();
                byte[] bf = reader.ReadBytes(bf_size);

                file_header = DecodeFileHeader(bf);
            }
        }

        private byte[] DecodeFileHeader(byte[] bf)
        {
            // The files header is encoded with a XOR-like compression with 256-byte key.
            // The specific decoding algorithm is in here.
            // The MAS header itself is not parsed

            byte[] output = new byte[bf.Length];

            if (bf.Length > 0)
            {
                uint gigabyte_index = 0;
                for (int byte_index = 0; byte_index < bf.Length; byte_index++)
                {
                    byte ind = (byte)((byte_index + byte_index / 256) % 256);
                    byte c = (byte)(byte_index & 0x3F);

                    ulong value = ((ulong)FileHeaderKeys[ind]) << c;

                    ulong value_h = value & 0xFFFFFFFF00000000;
                    value_h = value_h >> 32;
                    ulong value_l = value & 0x00000000FFFFFFFF;
                    value_l = ((ulong)byte_index) | saltkey & value;
                    value_h = ((ulong)gigabyte_index) | saltkey2 & value_h;

                    value = value_l | value_h << 32;

                    output[byte_index] = (byte)(bf[byte_index] ^ DecodeFileHeader_ShiftBytesRight(value, c));

                    gigabyte_index = (uint)DecodeFileHeader_ShiftBytesRight((ulong)byte_index, 32);
                }
            }

            return output;

        }

        private ulong DecodeFileHeader_ShiftBytesRight(ulong d, byte s)
        {
            if (s > 0x40)
                return 64;
            return d >> s;
        }

        public MAS2Reader(string file)
        {
            this.mas2_file = file;
            Files = new List<MAS2File>();
            reader = new BinaryReader(System.IO.File.OpenRead(file));
            ReadHeader();

            int files = file_header.Length / 256;
            uint FilePosition = (uint)reader.BaseStream.Position;
            reader.Close();

            for (int f = 0; f < files; f++)
            {
                string filename = ASCIIEncoding.ASCII.GetString(file_header, f * 256 + 16, 128);
                filename = filename.Substring(0, filename.IndexOf('\0'));
                string filename_path = ASCIIEncoding.ASCII.GetString(file_header, f * 256 + 16 + filename.Length + 1, 128);
                filename_path = filename_path.Substring(0, filename_path.IndexOf('\0'));

                uint file_index = BitConverter.ToUInt32(file_header, f * 256);
                uint size_compressed = BitConverter.ToUInt32(file_header, 65*4 + f * 256);
                uint size_uncompressed = BitConverter.ToUInt32(file_header, 63 * 4 + f * 256);

                MAS2File masfile = new MAS2File{Master=this};

                masfile.CompressedSize = size_compressed;
                masfile.UncompressedSize = size_uncompressed;
                masfile.Index = file_index;
                masfile.FileOffset = FilePosition;
                masfile.Filename = filename;
                masfile.Filename_Path = filename_path;

                FilePosition += masfile.CompressedSize;
                this.Files.Add(masfile);
            }
        }

        #region Simple search methods.
        public bool ContainsFile(string file)
        {
            return (Files.FindAll(delegate(MAS2File f) { return f.Filename.Contains(file); }).Count >= 1);
        }

        public List<MAS2File> GetFile(string file)
        {
            return Files.FindAll(delegate(MAS2File f) { return f.Filename.Contains(file); });
        }
        #endregion
        #region Extract files in MAS2File
        public void ExtractFile(MAS2File f, string target)
        {
            BinaryReader reader = new BinaryReader(System.IO.File.OpenRead(this.mas2_file));
            reader.BaseStream.Seek(f.FileOffset, SeekOrigin.Begin);
            byte[] RawData = reader.ReadBytes((int)f.CompressedSize);

            if (f.IsCompressed)
            {
                byte[] OutputData = new byte[f.UncompressedSize];

                // MAS2 compression consists of a simple inflate/deflate process.
                ZlibCodec codec = new ZlibCodec(CompressionMode.Decompress);
                codec.InitializeInflate();
                codec.InputBuffer = RawData;
                codec.NextIn = 0;
                codec.AvailableBytesIn = RawData.Length;

                codec.OutputBuffer = OutputData;
                codec.NextOut = 0;
                codec.AvailableBytesOut = OutputData.Length;

                codec.Inflate(FlushType.None);
                codec.EndInflate();

                System.IO.File.WriteAllBytes(target, OutputData);
            }
            else
            {
                System.IO.File.WriteAllBytes(target, RawData);
            }

        }
        public byte[] ExtractBytes(MAS2File f)
        {
            BinaryReader reader = new BinaryReader(System.IO.File.OpenRead(this.mas2_file));
            reader.BaseStream.Seek(f.FileOffset, SeekOrigin.Begin);
            byte[] RawData = reader.ReadBytes((int)f.CompressedSize);
            reader.Close();

            if (f.IsCompressed)
            {
                byte[] OutputData = new byte[f.UncompressedSize];

                // MAS2 compression consists of a simple inflate/deflate process.
                ZlibCodec codec = new ZlibCodec(CompressionMode.Decompress);
                codec.InitializeInflate();
                codec.InputBuffer = RawData;
                codec.NextIn = 0;
                codec.AvailableBytesIn = RawData.Length;

                codec.OutputBuffer = OutputData;
                codec.NextOut = 0;
                codec.AvailableBytesOut = OutputData.Length;

                codec.Inflate(FlushType.None);
                codec.EndInflate();

                return OutputData;
            }
            else
            {
                return RawData;
            }

        }

        public string ExtractString(MAS2File f)
        {
            return ASCIIEncoding.ASCII.GetString(ExtractBytes(f));
        }
#endregion
        #region Extract functions on index
        public void ExtractFile(int index, string target)
        {
            ExtractFile(Files[index], target);
        }

        public string ExtractString(int index)
        {
            return ExtractString(Files[index]);
        }

        public byte[] ExtractBytes(int index)
        {
            return ExtractBytes(Files[index]);
        }
        #endregion
    }
}