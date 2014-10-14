using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using NUnit.Framework;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests.Utils
{
    [TestFixture]
    public class IniReaderTests
    {
        [ExpectedException(typeof(IOException))]
        [Test]
        public void TestIOException()
        {
            var read = new IniReader("test.txt");
        }

        [ExpectedException(typeof(Exception), ExpectedMessage = "This is not a tuple value")]
        [Test]
        public void TestTupleReadout()
        {
            var reader = new IniReader(TestConstants.TestFolder + "MP420_engine.ini");
            reader.AddHandler(x =>
                                  {
                                      if (x.Key == "IdleRPMLogic")
                                      {
                                          Assert.AreEqual(3900.0f, x.ReadAsFloat(0));
                                          Assert.AreEqual(4100.0f, x.ReadAsFloat(1));
                                      }
                                      if (x.Key == "LaunchEfficiency")
                                      {
                                          Assert.AreEqual(0.92f, x.ReadAsFloat());
                                          Assert.AreEqual(0.92f, x.ReadAsFloat(0));
                                           x.ReadAsFloat(1);
                                          // shouldn't reach this
                                          Assert.Fail("Exception failed");
                                      }
                                  });
            reader.Parse();
        }

        [Test]
        public void TestEngine()
        {
            var rpmTorques = 0;

            var reader = new IniReader(TestConstants.TestFolder + "MP420_engine.ini");
            reader.AddHandler(x =>
                                  {
                                      Debug.WriteLine(x.NestedGroupName + "." + x.Key + "=" + x.RawValue);


                                      if (x.Key == "RPMTorque")
                                      {
                                          rpmTorques++;

                                          switch (rpmTorques)
                                          {
                                              case 1:
                                                  Assert.AreEqual(0.0f, x.ReadAsFloat(0));
                                                  Assert.AreEqual(-37.0f, x.ReadAsFloat(1));
                                                  Assert.AreEqual(-37.0f, x.ReadAsFloat(2));
                                                  break;

                                              case 33:
                                                  Assert.AreEqual(16000.0f, x.ReadAsFloat(0));
                                                  Assert.AreEqual(-163.7f, x.ReadAsFloat(1));
                                                  Assert.AreEqual(353.5f, x.ReadAsFloat(2));
                                                  break;

                                              case 34:
                                                  Assert.AreEqual(16500.0, x.ReadAsDouble(0));
                                                  Assert.AreEqual(-169.9, x.ReadAsDouble(1));
                                                  Assert.AreEqual(350.8, x.ReadAsDouble(2));
                                                  break;

                                              case 35:
                                                  Assert.AreEqual(17000, x.ReadAsDouble(0));
                                                  Assert.AreEqual(17000, x.ReadAsInteger(0));
                                                  Assert.AreEqual(-176.3, x.ReadAsDouble(1));
                                                  Assert.AreEqual(348.4, x.ReadAsDouble(2));
                                                  break;
                                              case 47:
                                                  Assert.AreEqual(23000.0f, x.ReadAsFloat(0));
                                                  Assert.AreEqual(-401.9f, x.ReadAsFloat(1));
                                                  Assert.AreEqual(-294.3f, x.ReadAsFloat(2));
                                                  break;

                                              default:
                                                  // Don't care about the others...
                                                  break;
                                          }
                                      }

                                      if (x.Key == "FuelConsumption")
                                      {
                                          float v = 2.755f / 100000.0f;
                                          Assert.AreEqual(v, x.ReadAsFloat());
                                      }
                                      if (x.Key == "SpeedLimiter") Assert.AreEqual(1, x.ReadAsInteger());
                                      if (x.Key == "OnboardStarter") Assert.AreEqual(0, x.ReadAsInteger());
                                  });
            reader.Parse();

            Assert.AreEqual(47, rpmTorques);
        }

        [Test]
         public void TestLister()
        {
            var readCalls = 0;

            var reader = new IniReader(TestConstants.TestFolder + "Lister_Dunlop.veh");
            reader.AddHandler(x =>
                              {
                                  string group = x.NestedGroupName + "." + x.Key;

                                  if (x.Key == "Number")
                                  {
                                      // In this file it's "Lister_Dunlop"
                                      Assert.AreEqual("Main.Number", group);
                                      Assert.AreEqual("Lister_Dunlop", x.ReadAsString());
                                      Assert.AreEqual(-1, x.ReadAsInteger());
                                      Assert.AreEqual(-1, x.ReadAsDouble());
                                      Assert.AreEqual(-1, x.ReadAsFloat());
                                  }

                                  if (x.Key == "Team")
                                  {
                                      Assert.AreEqual("Main.Team",group);
                                      Assert.AreEqual("Lister", x.ReadAsString());
                                      Assert.AreEqual(-1, x.ReadAsInteger());
                                      Assert.AreEqual(-1, x.ReadAsDouble());
                                      Assert.AreEqual(-1, x.ReadAsFloat());
                                  }

                                  if (x.Key == "Classes")
                                  {
                                      Assert.AreEqual("Main.Classes", group);
                                      Assert.AreEqual("GT1", x.ReadAsString());
                                      Assert.AreEqual("GT1", x.ReadAsString(0));
                                      Assert.AreEqual("Lister", x.ReadAsString(1));
                                      Assert.AreEqual(-1, x.ReadAsInteger());
                                      Assert.AreEqual(-1, x.ReadAsInteger(0));
                                      Assert.AreEqual(-1, x.ReadAsInteger(1));
                                      Assert.AreEqual(-1, x.ReadAsDouble());
                                      Assert.AreEqual(-1, x.ReadAsDouble(0));
                                      Assert.AreEqual(-1, x.ReadAsDouble(1));
                                      Assert.AreEqual(-1, x.ReadAsFloat());
                                      Assert.AreEqual(-1, x.ReadAsFloat(0));
                                      Assert.AreEqual(-1, x.ReadAsFloat(1));
                                  }

                                  readCalls++;
                              });
            reader.Parse();

            Assert.AreEqual(21, readCalls);
        }

        [Test]
        public void TestRFM()
        {
            var readCalls = 0;
            var data = new Dictionary<string, List<string>>();

            var reader = new IniReader(TestConstants.TestFolder + "F1CTDP05.rfm");
            reader.AddHandler(x =>
                                  {
                                      string group = x.NestedGroupName + "." + x.Key;

                                      // Do some ReadAs() tests.
                                      if (x.Key == "VehiclesDir")
                                          Assert.AreEqual(@"GAMEDATA\VEHICLES\CTDP\", x.ReadAsString());
                                      if (x.Key == "PitGroup" && data.ContainsKey(group) == false) // do only on first entriy
                                          Assert.AreEqual(new string[2] {"2", "Group1"}, x.ReadAsStringArray());

                                      if (data.ContainsKey(group))
                                          data[group].Add(x.RawValue);
                                      else
                                          data.Add(group, new List<string>(new[] {x.RawValue}));
                                      readCalls++;
                                      Debug.WriteLine(group + "=" + x.RawValue);
                                  });
            reader.Parse();

            Assert.AreEqual(144, readCalls);
            Assert.True(data.ContainsKey("Main.SceneOrder."));
            Assert.AreEqual(12, data["Main.SceneOrder."].Count);
            Assert.True(data.ContainsKey("Main.Season = CTDP F1 2005.SceneOrder."));
            Assert.AreEqual(14, data["Main.Season = CTDP F1 2005.SceneOrder."].Count);
            Assert.True(data.ContainsKey("Main.PitGroupOrder.PitGroup"));
            Assert.AreEqual(17, data["Main.PitGroupOrder.PitGroup"].Count);

            Assert.AreEqual("CTDP F1 2005", data["Main.Mod Name"][0]);
            Assert.AreEqual("*", data["Main.Track Filter"][0]);
            Assert.AreEqual("39001", data["Main.Matchmaker TCP Port"][0]);
        }
    }
}
