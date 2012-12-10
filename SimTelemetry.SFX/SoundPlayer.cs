﻿/*************************************************************************
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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;

namespace SimTelemetry
{
    public class SoundPlayer : IDisposable
    {
        private Device soundDevice;
        private SecondaryBuffer soundBuffer;
        private int samplesPerUpdate;
        private AutoResetEvent[] fillEvent = new AutoResetEvent[2];
        private Thread thread;
        private PullAudio pullAudio;
        private PullDouble pullFrequency;
        private PullDouble pullVolume;
        private short channels;
        private bool halted;
        private bool running;

        private static int i = 0;
        public static void PullAudio(short[] buf, int length)
        {
            try
            {
                int start_i = i;
                double freq = 5;
                for (; i < start_i + length; i++)
                {
                    double sampleValue = (Math.Sin(Convert.ToDouble(i * freq / 2 * 2 * Math.PI) / Convert.ToDouble(length))) * 30000 +
                                         2000;

                    if (double.IsNaN(sampleValue))
                        sampleValue = 0;
                    short val = short.Parse(Math.Round(sampleValue).ToString());
                    buf[i - start_i] = val;

                }
            }
            catch (Exception ex)
            {

            }
        }

        public PullDouble PullAFrequency { set { this.pullFrequency = value; } }
        public PullDouble PullVolume { set { this.pullVolume = value; } }
        private string samplefile = "";
        private Control _owner;

        public SoundPlayer(Control owner, PullAudio pullAudio, string sample, short channels)
        {
            if (sample == null || File.Exists(sample) == false)
                return;
            this.channels = channels;
            this.pullAudio = pullAudio;
            this.samplefile = sample;
            this._owner = owner;


            this.soundDevice = new Device();
            this.soundDevice.SetCooperativeLevel(_owner, CooperativeLevel.Priority);

            // Set up our wave format to 44,100Hz, with 16 bit resolution
            WaveFormat wf = new WaveFormat();
            wf.FormatTag = WaveFormatTag.Pcm;
            wf.SamplesPerSecond = 44100;
            wf.BitsPerSample = 16;
            wf.Channels = channels;
            wf.BlockAlign = (short)(wf.Channels * wf.BitsPerSample / 8);
            wf.AverageBytesPerSecond = wf.SamplesPerSecond * wf.BlockAlign;

            this.samplesPerUpdate = 512;

            // Create a buffer with 2 seconds of sample data
            BufferDescription bufferDesc = new BufferDescription();
            bufferDesc.BufferBytes = this.samplesPerUpdate * wf.BlockAlign * 2;
            bufferDesc.ControlPositionNotify = true;
            bufferDesc.GlobalFocus = true;
            bufferDesc.ControlFrequency = true;
            bufferDesc.ControlEffects = true;
            bufferDesc.ControlVolume = true;

            this.soundBuffer = new SecondaryBuffer(samplefile, bufferDesc, this.soundDevice);
            this.soundBuffer.Volume = 0;

            Notify notify = new Notify(this.soundBuffer);
            fillEvent[0] = new AutoResetEvent(false);
            fillEvent[1] = new AutoResetEvent(false);

            // Set up two notification events, one at halfway, and one at the end of the buffer
            BufferPositionNotify[] posNotify = new BufferPositionNotify[2];
            posNotify[0] = new BufferPositionNotify();
            posNotify[0].Offset = bufferDesc.BufferBytes / 2 - 1;
            posNotify[0].EventNotifyHandle = fillEvent[0].Handle;
            posNotify[1] = new BufferPositionNotify();
            posNotify[1].Offset = bufferDesc.BufferBytes - 1;
            posNotify[1].EventNotifyHandle = fillEvent[1].Handle;

            notify.SetNotificationPositions(posNotify);

            this.thread = new Thread(new ThreadStart(SoundPlayback));
            this.thread.Priority = ThreadPriority.Lowest;
            this.thread.IsBackground = true;

            this.Pause();
            this.running = true;

            this.thread.Start();
        }

        public void Keepup()
        {
            this.soundBuffer.Play(0, BufferPlayFlags.Looping);
        }

        public void Pause()
        {
            if (this.thread == null) return;
            if (this.halted) return;

            this.halted = true;

            Monitor.Enter(this.thread);
        }

        public void Resume()
        {
            if (this.thread == null) return;
            if (!this.halted) return;

            this.halted = false;

            Monitor.Pulse(this.thread);
            Monitor.Exit(this.thread);
        }

        public void Close()
        {
            this.running = false;
            Thread.Sleep(10);
            this.thread.Abort();
            this.thread = null;
        }

        private void SoundPlayback()
        {
            lock (this.thread)
            {
                if (!this.running) return;

                // Set up the initial sound buffer to be the full length
                int bufferLength = this.samplesPerUpdate * 2 * this.channels;
                short[] soundData = new short[bufferLength];

                // Prime it with the first x seconds of data
                this.pullAudio(soundData, soundData.Length);

                // Start it playing
                this.soundBuffer.Play(0, BufferPlayFlags.Looping);

                while (this.running)
                {
                    if (this.halted)
                    {
                        Monitor.Pulse(this.thread);
                        Monitor.Wait(this.thread);
                    }

                    // Wait on one of the notification events
                    WaitHandle.WaitAny(this.fillEvent, 3, true);

                    // Save the position we were at
                    double freq = 1, vol = 0;
                    if (this.pullFrequency == null)
                        freq = 1;
                    else
                    {
                        freq = this.pullFrequency();
                        if(freq == -1)
                        {
                            this.soundBuffer.SetCurrentPosition(0);
                            freq = this.pullFrequency();
                        }
                        
                    }
                    if (pullVolume == null)
                        vol = 0;
                    else
                        vol = this.pullVolume();

                    if (vol > 1) vol = 1;
                    if (vol < 0) vol = 0;
                    if (freq < 0) freq = 0;

                    if (double.IsInfinity(freq)) freq = 1;
                    if (double.IsNaN(freq)) freq = 1;
                    if (double.IsNaN(vol)) vol = 1;
                    if (double.IsInfinity(vol)) vol = 1;
                    int _freq = Convert.ToInt32(Math.Round(44100 * freq));
                    this.soundBuffer.Frequency = Math.Min(192000, Math.Max(1000, _freq));

                    double thing = (int)Volume.Min;
                    if (vol != 0)

                        thing = Math.Log10(1.0 / vol) * 1000;
                    if (thing >= 10000 || thing < 0)
                        thing = 9999;

                    this.soundBuffer.Volume = Convert.ToInt32(Math.Round(0 - thing));
                    Thread.Sleep(1);

                }
            }
        }

        public void Dispose()
        {
            this.running = false;
            this.Resume();

            if (this.soundBuffer != null)
            {
                this.soundBuffer.Dispose();
            }
            if (this.soundDevice != null)
            {
                this.soundDevice.Dispose();
            }
        }
    }
}