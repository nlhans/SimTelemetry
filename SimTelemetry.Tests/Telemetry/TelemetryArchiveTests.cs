using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    public class TelemetryArchiveTests
    {
        [Test]
        public void StoreLogFile()
        {
            var dummyDataSource = new MockDataSource();

            var archive =  new TelemetryArchive();
            var loggerFromZero = new TelemetryLogger("test", new TelemetryLoggerConfiguration(true, true, true, true));
            loggerFromZero.SetDatasource(dummyDataSource);
            loggerFromZero.SetAnnotater(archive);
            var logWriter = new LogFileWriter("test.zip","TelemetryArchiveTestsStoreLogFile");

            GlobalEvents.Fire(new SessionStarted(), true);

            loggerFromZero.Update(0);
            loggerFromZero.Update(10000);
            loggerFromZero.Update(20000);

            GlobalEvents.Fire(new SessionStopped(), true);

            Thread.Sleep(5000);

        }

        [Test]
        public void QualifyTest()
        {
            var dummyDataSource = new MockDataSource();

            var archive = new TelemetryArchive();
            var loggerFromZero = new TelemetryLogger("test", new TelemetryLoggerConfiguration(true, true, true, true));
            loggerFromZero.SetDatasource(dummyDataSource);
            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.False(archive.QualifiesForStorage(loggerFromZero));

            loggerFromZero.Update(0);

            Assert.False(archive.QualifiesForStorage(loggerFromZero));

            loggerFromZero.Update(1000);

            Assert.False(archive.QualifiesForStorage(loggerFromZero));

            loggerFromZero.Update(5000);

            Assert.False(archive.QualifiesForStorage(loggerFromZero));

            loggerFromZero.Update(10000);

            Assert.True(archive.QualifiesForStorage(loggerFromZero));

            loggerFromZero.Close();

            var loggerFromElse = new TelemetryLogger("test", new TelemetryLoggerConfiguration(true, true, true, true));
            loggerFromElse.SetDatasource(dummyDataSource);
            GlobalEvents.Fire(new SessionStarted(), true);

            Assert.False(archive.QualifiesForStorage(loggerFromElse));

            loggerFromElse.Update(10000);

            Assert.False(archive.QualifiesForStorage(loggerFromElse));

            loggerFromElse.Update(13000);

            Assert.False(archive.QualifiesForStorage(loggerFromElse));

            loggerFromElse.Update(0);

            Assert.False(archive.QualifiesForStorage(loggerFromElse));

            loggerFromElse.Update(20000);

            Assert.True(archive.QualifiesForStorage(loggerFromElse));

            loggerFromElse.Update(24*3600*1000);

            Assert.True(archive.QualifiesForStorage(loggerFromElse));
        }
    }
}
