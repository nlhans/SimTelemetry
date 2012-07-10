using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public class DataCollector
    {
        public const int SleepTime = 10;
        private int ticker = -1;
        private FileStream strw;
        private Thread Logger;
        string _LogFile;
        private bool LoggerRunning = true;

        private Dictionary<string, int> _Mapping_DriverPlayer = new Dictionary<string, int>();
        private Dictionary<string, int> _Mapping_DriverGeneral = new Dictionary<string, int>();
        private Dictionary<string, int> _Mapping_Session = new Dictionary<string, int>();

        private PropertyDescriptorCollection DriverPlayer_Properties;
        private PropertyDescriptorCollection Driver_Properties;
        private PropertyDescriptorCollection Session_Properties;

        private Dictionary<string, double>  DriverPlayer_Frequency = new Dictionary<string, double>();
        private Dictionary<string, double> Driver_Frequency = new Dictionary<string, double>();
        private Dictionary<string, double> Session_Frequency = new Dictionary<string, double>();

        public DataCollector()
        {
        }

        private int LastLaps = -1;

        public void Run()
        {
            if (Logger ==null)
            {
                Logger = new Thread(LoggerThread);
                Logger.IsBackground = true;
                Logger.Start();
                LoggerRunning = true;
            }
        }
        public void Stop()
        {
            if (Logger != null)
            {
                LoggerRunning = false;
                Thread.Sleep(100);
                Logger.Abort();
                Logger = null;
            }

        }

        public void LoggerThread()
        {
            try
            {
                while (LoggerRunning)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(TickAsync));
                    System.Threading.Thread.Sleep(DataCollector.SleepTime);
                }
                this.Close();
            }
            catch (Exception)
            {
            }
        }

        private bool TickAsyncBusy = false;
        internal void TickAsync(object n)
        {
            if (TickAsyncBusy == false)
            {
                TickAsyncBusy = true;
                Tick();
                TickAsyncBusy = false;
            }
        }

        public void Tick()
        {
            ticker++;

            // Start new log session
            byte[] session = Log_Session();
            byte[] player = Log_DriverPlayer();
            byte[] general = Log_DriverGeneral();

            if (Telemetry.m.Sim.Drivers.AllDrivers.Count > 0 && Telemetry.m.Sim.Drivers.AllDrivers[0].Laps != LastLaps)
            {

                strw.WriteLine("[Lap]");
                strw.WriteLine("Number=" + Telemetry.m.Sim.Drivers.AllDrivers[0].Laps);
                strw.WriteLine("LastLap=" + Telemetry.m.Sim.Drivers.AllDrivers[0].LapTime_Last);

                LastLaps = Telemetry.m.Sim.Drivers.AllDrivers[0].Laps;

            }

            if (session.Length == 0 && player.Length == 0 && general.Length == 0) 
                return;
            // Add new line
            strw.WriteLine("[Data@"+ticker+"]");
            strw.WriteLine(session.Length+","+player.Length+","+general.Length);
            // Real data

            strw.Write(session,0,session.Length);
            strw.Write(player,0,player.Length);
            strw.Write(general, 0, general.Length);
            strw.WriteLine("");
            strw.Flush();
        }

        public void CreateLog(string name)
        {
            int indexer = 0;
            ticker = 0;
            _LogFile = name;
            // Write header
            strw = File.Open(name, FileMode.Create, FileAccess.Write, FileShare.None);

            DriverPlayer_Properties = TypeDescriptor.GetProperties(typeof(IDriverPlayer));
            Driver_Properties = TypeDescriptor.GetProperties(typeof(IDriverGeneral));
            Session_Properties = TypeDescriptor.GetProperties(typeof(ISession));

            DriverPlayer_Frequency = new Dictionary<string, double>();
            Driver_Frequency = new Dictionary<string, double>();
            Session_Frequency = new Dictionary<string, double>();

            _Mapping_Session = new Dictionary<string, int>();
            _Mapping_DriverPlayer = new Dictionary<string, int>();
            _Mapping_DriverGeneral = new Dictionary<string, int>();

            strw.WriteLine("[Information]");
            //rw.WriteLine("Revision=" + Telemetry.m.Sim.Revision);
            strw.WriteLine("");

            // Map ALL properties of DriverPlayer to numbers
            strw.WriteLine("[DriverPlayer]");

            indexer = 0;
            PropertyInfo[] pic = typeof(IDriverPlayer).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = DriverPlayer_Properties[pi.Name];
                if (fi.Attributes.Contains(new Unloggable()))
                    continue;
                
                object[] attrs = pi.GetCustomAttributes(typeof(Unloggable), false);
                if (attrs.Length == 1)
                    continue;
                indexer++;
                _Mapping_DriverPlayer.Add(fi.Name, indexer);

                strw.WriteLine(fi.Name + "=" + indexer + ","+ fi.PropertyType.Name);



                double Frequency = 1.0; // 1Hz normal

                // Get frequency
                attrs = pi.GetCustomAttributes(typeof(Loggable), false);
                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;

                DriverPlayer_Frequency.Add(fi.Name, Frequency);
            }
            strw.WriteLine("");

            // Map ALL properties of Driver to numbers
            strw.WriteLine("[DriverGeneral]");

            indexer = 0;
             pic = typeof(IDriverGeneral).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = Driver_Properties[pi.Name];
                if (fi.Attributes.Contains(new Unloggable()))
                    continue;

                object[] attrs = pi.GetCustomAttributes(typeof(Unloggable), false);
                if (attrs.Length == 1)
                    continue;

                indexer++;
                _Mapping_DriverGeneral.Add(fi.Name, indexer);

                strw.WriteLine(fi.Name + "=" + indexer + "," + fi.PropertyType.Name);


                double Frequency = 1.0; // 1Hz normal

                // Get frequency
                attrs = pi.GetCustomAttributes(typeof(Loggable), false);
                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;

                Driver_Frequency.Add(fi.Name, Frequency);
            }
            strw.WriteLine("");

            // Map ALL properties of Session to numbers
            strw.WriteLine("[Session]");

            indexer = 0;
           pic = typeof(ISession).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = Session_Properties[pi.Name];
                if (fi.Attributes.Contains(new Unloggable()))
                    continue;
                
                object[] attrs = pi.GetCustomAttributes(typeof(Unloggable), false);
                if (attrs.Length == 1)
                    continue;

                indexer++;
                _Mapping_Session.Add(fi.Name, indexer);

                strw.WriteLine(fi.Name + "=" + indexer + "," + fi.PropertyType.Name);

                double Frequency = 1.0; // 1Hz normal

                // Get frequency
                attrs =pi.GetCustomAttributes(typeof(Loggable), false);
                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;

                Session_Frequency.Add(fi.Name, Frequency);


            }
            strw.WriteLine("");
            strw.WriteLine("");
            strw.WriteLine("************* START OF DATA *************");
        }

        private byte[] Log_Session()
        {
            return __Dump(typeof(ISession), Telemetry.m.Sim.Session, _Mapping_Session, Session_Properties, Session_Frequency);

        }

        private byte[] Log_DriverGeneral()
        {
            lock(Telemetry.m.Sim.Drivers.AllDrivers){
                foreach (IDriverGeneral g in Telemetry.m.Sim.Drivers.AllDrivers)
                {
                    if (g.IsPlayer)
                        return __Dump(typeof(IDriverPlayer), g, _Mapping_DriverGeneral, Driver_Properties, Driver_Frequency);

                }
            }

            return new byte[0];
        }

        private byte[] Log_DriverPlayer()
        {
            return __Dump(typeof(IDriverPlayer), Telemetry.m.Sim.Player, _Mapping_DriverPlayer, DriverPlayer_Properties, DriverPlayer_Frequency);
        }

        private byte[] __Dump(Type ty, object o, Dictionary<string, int> mapping, PropertyDescriptorCollection myObjectFields, Dictionary<string, double> Frequencies)
        {
            // TODO: Do dumping.
            return new byte[0]; 
            List<byte> output = new List<byte>();

            foreach (PropertyDescriptor fi in myObjectFields)
            {
                if (mapping.ContainsKey(fi.Name) == false) continue;
                // Get ID
                int id = mapping[fi.Name];

                double Frequency = Frequencies[fi.Name]; // 1Hz normal

                // Get frequency
                /*attrs = fi.GetCustomAttributes(typeof(Loggable), false);
                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;
                */
                double val = (ticker* 1.0*SleepTime)/1000.0;
                double val2 = 1.0 / Frequency;
                if (val % val2 < 0.001)
                {
                    byte[] b = new byte[0];
                    object thing = fi.GetValue(o);
                    Type t = fi.PropertyType;
                    
                    if (t == typeof(double))
                        b = BitConverter.GetBytes((double)thing);

                    if (t == typeof(float))
                        b = BitConverter.GetBytes((float)thing);

                    if (t == typeof(Int32))
                        b = BitConverter.GetBytes((Int32)thing);

                    if (t == typeof(byte))
                        b = BitConverter.GetBytes((byte)thing);

                    if (t == typeof(bool))
                        b = BitConverter.GetBytes((bool)thing);

                    if (t == typeof(string))
                        b = ASCIIEncoding.ASCII.GetBytes((string) thing);
                    

                    output.AddRange(BitConverter.GetBytes((Int32)id));
                    output.AddRange(b);
                    if (t == typeof(string))
                        output.Add(0);

                    
                }
            }
            return output.ToArray();
        }

        public void Close()
        {
            strw.WriteLine("[END]");
            strw.WriteLine("");
            strw.Close();
        }

        public string GetFile()
        {
            return _LogFile;
        }
    }
}