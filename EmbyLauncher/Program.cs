using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace EmbyLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //show the splash screen while we wait
            Thread t = new Thread(new ThreadStart(ShowSplash));
            t.Start();

            //let's check the network connection
            bool netDetected = false;
            bool stopTrying = false;
            DateTime time = DateTime.Now;
            TimeSpan timeout = new TimeSpan(0, Properties.Settings.Default.WaitForNetwork, 0);
            while (!stopTrying)
            {
                if (IsNetworkAvailable())
                {
                    netDetected = true;
                    break;
                }
                else
                {
                    Thread.Sleep(Properties.Settings.Default.SleepDuration);
                    if (DateTime.Now.Subtract(time).TotalMilliseconds > timeout.TotalMilliseconds)
                    {
                        stopTrying = true;
                    }
                }
            }

            //get rid of the splash screen
            t.Abort();

            //do we have a network connection?
            if (!netDetected)
            {
                //no network detected. Let's see if the user wants to adjust
                Application.Run(new IncreaseWait());
            }
            else if (!LaunchEmby())
            {
                //Can't find emby.theater. Try to fix the emby path
                Application.Run(new FixEmbyPath());
            }
        }

        /// <summary>
        /// Displays the Splash Screen while the application is detecting a network connection.
        /// </summary>
        private static void ShowSplash()
        {
            Application.Run(new SplashScreen());
        }

        /// <summary>
        /// Finds and launches the Emby Theater application.
        /// Application path can be overriden via EmbyLocationOverride config setting.
        /// </summary>
        private static bool LaunchEmby()
        {
            string embyLoc=Properties.Settings.Default.EmbyLocationOverride;
            if (string.IsNullOrWhiteSpace(embyLoc))
                embyLoc=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.EmbyLocation);

            if (File.Exists(embyLoc))
            {
                Process.Start(embyLoc);
                return true;
            }
            else
            {
                //what have we done
                return false;
            }
        }

        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(0);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }
    }
}
