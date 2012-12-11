/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
using System;
using System.Diagnostics;
using System.Threading;
using Triton.Memory;

namespace SimTelemetry.Objects.Utilities
{
    public class MemoryPolledReader : MemoryReader
    {
        public ISimulator sim;

        public int Base
        {
            get
            {
                if (m_ReadProcess == null) return 0;
                return (int)m_ReadProcess.MainModule.BaseAddress;
            }
        }

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
