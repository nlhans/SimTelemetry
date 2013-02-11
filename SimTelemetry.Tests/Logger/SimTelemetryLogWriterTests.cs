using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Tests.Logger
{
    [TestFixture]
    public class SimTelemetryLogWriterTests
    {
        private MemoryProvider _virtualMemory;

        private float _time;

        public void CreatePool()
        {
            Process p = Process.GetProcessesByName("devenv")[0];
            MemoryReader reader = new MemoryReader();
            reader.Open(p, false);
            _virtualMemory = new MemoryProvider(reader);

            var sessionPool = new MemoryPool("Session", MemoryAddress.Static, 0, 0, 0);
            sessionPool.Add(new MemoryFieldFunc<float>("Time", (pool) => _time));

            var templateDriver = new MemoryPool("TemplateDriver", MemoryAddress.Static, 0, 0, 0);

            templateDriver.Add(new MemoryFieldConstant<float>("Speed", 7));
            templateDriver.Add(new MemoryFieldConstant<float>("Meter", 1337.0f));
            templateDriver.Add(new MemoryFieldConstant<float>("RPM", 5000));
            templateDriver.Add(new MemoryFieldConstant<int>("Gear", 6));
            templateDriver.Add(new MemoryFieldConstant<int>("Laps", 5));
            
            templateDriver.SetTemplate(true);


            _virtualMemory.Add(sessionPool);
            _virtualMemory.Add(templateDriver);
        }

        private void CreateDriver( string name, bool AI)
        {
            var myTemplateDriver = _virtualMemory.Get("TemplateDriver");
            var myDriver = myTemplateDriver.Clone("Driver " + name, 0);

            bool firstDriver = (_virtualMemory.Pools.Count(x => x.Name.StartsWith("Driver")) == 0) ? true : false;

            myDriver.Add(new MemoryFieldConstant<int>("Index", _virtualMemory.Pools.Count(x => x.Name.StartsWith("Driver"))));
            myDriver.Add(new MemoryFieldConstant<bool>("IsAI", AI));
            myDriver.Add(new MemoryFieldConstant<bool>("IsPlayer", firstDriver));
            if (firstDriver)
                myDriver.Add(new MemoryFieldConstant<float>("EngineLifetime", 123456));
            _virtualMemory.Add(myDriver);

            GlobalEvents.Fire(new DriversAdded(null, new List<TelemetryDriver> {new TelemetryDriver(null, myDriver)}), true);
        }

        private void RemoveDriver(string driver)
        {
            var myDriver = _virtualMemory.Get("Driver " + driver);
            _virtualMemory.Remove(myDriver);
            GlobalEvents.Fire(new DriversRemoved(null, new List<TelemetryDriver> { new TelemetryDriver(null, myDriver) }), true);
        }

        [Test]
        public void TestStructure_LogAll()
        {
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(new TelemetryLogConfiguration(true, true, true, true));

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false); // Player 'Henk' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // AI  Driver 'frits' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Frits"));
            Assert.AreEqual(3 + 5, writer.Log.FindGroup("Driver Frits").Fields.Count()); // 3 added in CreateDriver(), 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Frits").Groups.Count());

            CreateDriver("Piet", true); // AI Driver 'piet' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Piet"));
            Assert.AreEqual(3 + 5, writer.Log.FindGroup("Driver Piet").Fields.Count()); // 3 added in CreateDriver(), 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Piet").Groups.Count());

            RemoveDriver("Frits"); // driver left game.

            // Logs structure must remain:
            Assert.True(writer.Log.ContainsGroup("Driver Frits"));
            Assert.AreEqual(3 + 5, writer.Log.FindGroup("Driver Frits").Fields.Count()); // 3 added in CreateDriver(), 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Frits").Groups.Count());

            CreateDriver("John", false); // Player 'John' enters game

            Assert.True(writer.Log.ContainsGroup("Driver John"));
            Assert.AreEqual(3 + 5, writer.Log.FindGroup("Driver John").Fields.Count()); // 3 added in CreateDriver(), 5 in templateDriver. No player bonus, as it's not "THE" player.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver John").Groups.Count());
        }

        [Test]
        public void TestStructure_LogPlayer_TimepathAll()
        {
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(new TelemetryLogConfiguration(true, false, true, true));

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Frits"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver Frits").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Frits").Groups.Count());

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Piet"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver Piet").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Piet").Groups.Count());

            RemoveDriver("Frits"); // driver left game.

            // Logs structure must remain:
            Assert.True(writer.Log.ContainsGroup("Driver Frits"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver Frits").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Frits").Groups.Count());

            CreateDriver("John", false); // Player 'John' enters game

            Assert.True(writer.Log.ContainsGroup("Driver John"));
            Assert.AreEqual(3 + 5, writer.Log.FindGroup("Driver John").Fields.Count()); // 3 added in CreateDriver(), 5 in templateDriver. Full Telemetry.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver John").Groups.Count());
        }

        [Test]
        public void TestStructure_LogPlayer_TimepathPlayer()
        {
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(new TelemetryLogConfiguration(true, false, true, false));

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Piet")); // Driver is AI, no telemetry&timepath logging

            RemoveDriver("Frits"); // driver left game.

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("John", false); // Player 'John' enters game

            Assert.True(writer.Log.ContainsGroup("Driver John"));
            Assert.AreEqual(3 + 5, writer.Log.FindGroup("Driver John").Fields.Count()); // 3 added in CreateDriver(), 5 in templateDriver. Full Telemetry.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver John").Groups.Count());
        }

        [Test]
        public void TestStructure_LogMeOnly_TimepathPlayer()
        {
            var cfg = new TelemetryLogConfiguration(false, false, true, false);
            cfg.Add(0);
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(cfg);

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Piet")); // Driver is AI, no telemetry&timepath logging

            RemoveDriver("Frits"); // driver left game.

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("John", false); // Player 'John' enters game

            Assert.True(writer.Log.ContainsGroup("Driver John"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver John").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver John").Groups.Count());
        }

        [Test]
        public void TestStructure_LogMeOnly_TimepathAll()
        {
            var cfg = new TelemetryLogConfiguration(false, false, true, true);
            cfg.Add(0);
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(cfg);

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Frits"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver Frits").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Frits").Groups.Count());

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.True(writer.Log.ContainsGroup("Driver Piet"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver Piet").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Piet").Groups.Count());

            RemoveDriver("Frits"); // driver left game.

            // Logs structure must remain:
            Assert.True(writer.Log.ContainsGroup("Driver Frits"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver Frits").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Frits").Groups.Count());

            CreateDriver("John", false); // Player 'John' enters game

            Assert.True(writer.Log.ContainsGroup("Driver John"));
            Assert.AreEqual(3 + 3, writer.Log.FindGroup("Driver John").Fields.Count()); // 3 added in CreateDriver(), 3 in templateDriver. Timepath only!
            Assert.AreEqual(0, writer.Log.FindGroup("Driver John").Groups.Count());
        }

        [Test]
        public void TestStructure_LogMeOnly_TimepathNone()
        {
            var cfg = new TelemetryLogConfiguration(false, false, false, false);
            cfg.Add(0);
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(cfg);

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Piet")); // Driver is AI, no telemetry&timepath logging

            RemoveDriver("Frits"); // driver left game.

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("John", false); // Player 'John' enters game

            Assert.False(writer.Log.ContainsGroup("Driver John")); // Player is not on selective list, no timepaths, no telemetry&timepath logging
        }

        [Test]
        public void TestStructure_LogMeOnly_TimepathAI()
        {
            var cfg = new TelemetryLogConfiguration(false, false, false, true);
            cfg.Add(0);
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(cfg);

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Piet")); // Driver is AI, no telemetry&timepath logging

            RemoveDriver("Frits"); // driver left game.

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("John", false); // Player 'John' enters game

            Assert.False(writer.Log.ContainsGroup("Driver John")); // Player is not on selective list, no timepaths, no telemetry&timepath logging

            // NOTES: TimePath 'All' is turned off. TimePath AI is optional out, not optional in.
        }

        [Test]
        public void TestStructure_LogAI_TimepathAI()
        {
            var cfg = new TelemetryLogConfiguration(false, true, false, true);
            cfg.Add(0);
            //
            CreatePool();
            SimTelemetryLogWriter writer = new SimTelemetryLogWriter();
            writer.Initialize(null, _virtualMemory);
            writer.UpdateConfiguration(cfg);

            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.AreEqual(1, writer.Log.Groups.Count());
            Assert.True(writer.Log.ContainsGroup("Session"));
            Assert.AreEqual(1, writer.Log.FindGroup("Session").Fields.Count());
            Assert.AreEqual(0, writer.Log.FindGroup("Session").Groups.Count());

            Assert.False(writer.Log.ContainsGroup("TemplateDriver"));

            CreateDriver("Henk", false);

            Assert.True(writer.Log.ContainsGroup("Driver Henk"));
            Assert.AreEqual(3 + 5 + 1, writer.Log.FindGroup("Driver Henk").Fields.Count()); // 3 added in CreateDriver(), 1 added in CreateDriver() for player only, 5 in templateDriver.
            Assert.AreEqual(0, writer.Log.FindGroup("Driver Henk").Groups.Count());

            CreateDriver("Frits", true); // Driver 'frits' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("Piet", true); // Driver 'piet' enters game

            Assert.False(writer.Log.ContainsGroup("Driver Piet")); // Driver is AI, no telemetry&timepath logging

            RemoveDriver("Frits"); // driver left game.

            Assert.False(writer.Log.ContainsGroup("Driver Frits")); // Driver is AI, no telemetry&timepath logging

            CreateDriver("John", false); // Player 'John' enters game

            Assert.False(writer.Log.ContainsGroup("Driver John")); // Player is not on selective list, no timepaths, no telemetry&timepath logging

            // NOTES: Telemetry + TimePath 'All' is turned off. Telemetry + TimePath AI is optional out, not optional in.
        }
    }
}

