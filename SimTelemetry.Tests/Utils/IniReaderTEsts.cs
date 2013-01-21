using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests.Utils
{
    [TestFixture]
    public class IniReaderTests
    {
        [Test]
        public void IniInit()
        {
            bool ioException = false;
            try
            {
                var read = new IniReader("test.txt");
            }
            catch (IOException ex)
            {
                ioException = true;
            }
            catch
            {
                Assert.Fail();
            }
            Assert.IsTrue(ioException);

            var reader = new IniReader(@"C:\Program Files (x86)\rFactor\rFm\BMWM3.rfm");
            reader.AddHandler(x => Debug.WriteLine(x.NestedGroupName + "." + x.Key + "=" + string.Join(",",x.ReadAsStringArray())));
            reader.Parse();
        }
    }
}
