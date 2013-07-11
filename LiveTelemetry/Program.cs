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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveTelemetry
{
    internal static class Program
    {
        private static bool ReportingError = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#if DEBUG
#else
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LiveTelemetry());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogError(e.Exception, false);
        }

        private static void CurrentDomain_FirstChanceException(object sender,
                                                               System.Runtime.ExceptionServices.
                                                                   FirstChanceExceptionEventArgs e)
        {
            LogError(e.Exception, true);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError((Exception) e.ExceptionObject, false);
        }

        private static void LogError(Exception ex, bool firstchance)
        {
            if (ReportingError)
            {
                // Then this is an error during error reporting. Cancel this..
                // However it still sucks.
                return;
            }
            ReportingError = true;

            try
            {
                FileStream fs;
                if (File.Exists("debug.txt") == false)
                    fs = File.Create("debug.txt");
                else
                {
                    fs = File.Open("debug.txt", FileMode.Append);
                }
                StringBuilder error = new StringBuilder();
                error.AppendLine(
                    "*******************************************************************************************");
                error.Append("                           ");
                if (firstchance)
                {
                    error.AppendLine("Silent Exception");
                }
                else
                {
                    error.AppendLine("Fatal Exception");
                }
                error.AppendLine(
                    "*******************************************************************************************");

                error.AppendLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "." +
                                 DateTime.Now.Millisecond);
                error.AppendLine("-----------------------------------------------------------------");
                error.Append("Simulator: ");
                if (TelemetryApplication.SimulatorAvailable == false) error.AppendLine("No simulator running");
                else error.AppendLine(TelemetryApplication.Simulator.Name.ToLower());

                error.AppendLine("-----------------------------------------------------------------");
                error.Append("Session: ");
                if (TelemetryApplication.TelemetryAvailable == false) error.AppendLine("No session running");
                else error.AppendLine(TelemetryApplication.Telemetry.Session.Info.Type.ToString());

                if (TelemetryApplication.TelemetryAvailable)
                {
                    error.AppendLine("-----------------------------------------------------------------");
                    error.Append("Driver: ");
                    if (TelemetryApplication.SimulatorAvailable && TelemetryApplication.Telemetry.Drivers != null &&
                        TelemetryApplication.Telemetry.Player != null)
                    {
                        error.AppendLine(TelemetryApplication.Telemetry.Player.Name + " / " +
                                         TelemetryApplication.Telemetry.Player.CarClasses + " / " +
                                         TelemetryApplication.Telemetry.Player.CarModel);
                        error.AppendLine("Laps: " + TelemetryApplication.Telemetry.Player.Laps);
                    }
                    else if (TelemetryApplication.SimulatorAvailable == false)
                        error.AppendLine("Error in Sim object");
                    else if (TelemetryApplication.Telemetry.Drivers == null)
                        error.AppendLine("Error in Drivers object");
                    else if (TelemetryApplication.Telemetry.Player == null)
                        error.AppendLine("No player found");

                    error.AppendLine("Cars: " + TelemetryApplication.Telemetry.Session.Cars);
                    error.AppendLine("-----------------------------------------------------------------");
                    error.AppendLine("Track loaded: " + ((TelemetryApplication.TrackAvailable == false) ? "No" : "Yes"));
                    error.AppendLine("Track: " + TelemetryApplication.Telemetry.Session.Track);
                    if (TelemetryApplication.TrackAvailable)
                    {
                        error.AppendLine("Track: " + TelemetryApplication.Track.Name);
                        error.AppendLine("Route points: " + TelemetryApplication.Track.Route.Count());
                    }

                }


                error.AppendLine("-----------------------------------------------------------------");
                error.AppendLine("Simulator Event log:");
                error.AppendLine("TODO");

                error.AppendLine("-----------------------------------------------------------------");
                error.AppendLine("Exception info:");
                error.AppendLine(ex.Message);

                error.AppendLine("-----------------------------------------------------------------");
                error.AppendLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    error.AppendLine("-----------------------------------------------------------------");
                    error.AppendLine("INNER Exception info:");
                    error.AppendLine(ex.InnerException.Message);

                    error.AppendLine("-----------------------------------------------------------------");
                    error.AppendLine(ex.InnerException.StackTrace);
                }
                error.AppendLine("-----------------------------------------------------------------");

                byte[] sb = ASCIIEncoding.ASCII.GetBytes(error.ToString());
                fs.Write(sb, 0, sb.Length);
                fs.Close();
                if (!firstchance)
                {
                    Error err = new Error();
                    err.ShowDialog();
                }
            }
            catch (Exception w)
            {

            }

            ReportingError = false;
        }
    }
}