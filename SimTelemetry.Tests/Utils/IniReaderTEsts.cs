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
            catch (Exception ex)
            {

            }
            Assert.IsTrue(ioException);

            var reader =
                new IniReader(@"C:\Program Files (x86)\rFactor\rFm\BMWM3.rfm");
            reader.AddHandler(x =>
                                  {
                                      if (x.NestedGroupName.Contains("SeasonScoringInfo"))
                                           Debug.WriteLine(x.NestedGroupName + "." + x.Key + "=" + x.ReadAsString());
                                  } );
            reader.Parse();
        }
    }
}
