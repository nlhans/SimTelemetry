using System;
using System.Diagnostics;
using System.Threading;
using Triton.Memory;

namespace SimTelemetry.Objects.Utilities
{
    public class MemoryPolledReader : MemoryReader
    {
        public ISimulator sim;

        public bool Attached { get; internal set; }
        public string ProcessName { get; set; }

        private Thread Poller { get; set; }
        public bool Active { get; set; }

        public MemoryPolledReader(ISimulator sim) : base()
        {
            this.sim = sim;
            Active = true;
            
            // Start up thread
            ProcessName = sim.ProcessName;

            Poller = new Thread(Polling);
            Activate();

            Triton.TritonBase.PreExit += new Triton.AnonymousSignal(TritonBase_PreExit);
        }

        void TritonBase_PreExit()
        {
            Active = false;
        }

        public void Activate()
        {
            Poller.Start();
        }

        public int HandleAddress
        {
            get { return ReadProcess.MainModule.BaseAddress.ToInt32(); }
        }

        private void Polling()
        {
            while(Active)
            {
                Thread.Sleep(1000);
                if (Attached)
                {
                    if (ReadProcess.HasExited)
                    {
                        this.CloseHandle();
                        ReadProcess = null;
                        Attached = false;
                        //sim.Host.Report_SimStop(sim);
                    }
                }
                else
                {
                    Process[] processes = Process.GetProcessesByName(ProcessName);
                    if (processes.Length == 1)
                    {
                        ReadProcess = processes[0];
                        OpenProcess();
                        Attached = true;
                        //sim.Host.Report_SimStart(sim);
                    }
                }
            }

        }
    }
}
