using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactorCarRepository : ILazyRepositoryDataSource<Car, string>
    {
        // Search for all vehicles
        string path = @"C:\Program Files (x86)\rFactor\GameData\Vehicles\";

        public IList<string> GetIds()
        {
            string[] files = Directory.GetFiles(path, "*.veh", SearchOption.AllDirectories);
            var fileList = new List<string>(files);
            return fileList.Select(x => x.Substring(path.Length).ToLower()).ToList();
        }

        public bool Add(Car obj)
        {
            return false;
        }

        public bool Store(Car obj)
        {
            return false;
        }

        public Car Get(string id)
        {
            System.Threading.Thread.Sleep(10);
            Debug.WriteLine("Car::Get(\"" + id + "\")");
            //
            var scan = new IniScanner {IniFile = path + id};
            scan.Read();


            var team = scan.TryGetString("Team");
            var driver = scan.TryGetString("Driver");
            var description = scan.TryGetString("Description");
            var number = scan.TryGetInt32("Number");

            if (team.Length > 3)
                team = team.Substring(1, team.Length - 2);
            if (driver.Length > 3)
                driver = driver.Substring(1, driver.Length - 2);
            if (description.Length > 3)
                description = description.Substring(1, description.Length - 2);

            var classes = new List<string>();
            string sClasses = scan.TryGetString("Classes");
            if (sClasses.StartsWith("\"") && sClasses.Length > 3)
                sClasses = sClasses.Substring(1, sClasses.Length - 2);

            if (sClasses.StartsWith("\"") && sClasses.Length > 3)
                sClasses = sClasses.Substring(1, sClasses.Length - 2);
            if (sClasses.Contains(" "))
            {
                classes = new List<string>(sClasses.Split(" ".ToCharArray()));
            }
            else
            {
                classes = new List<string>(sClasses.Split(",".ToCharArray()));
            }

            var c = new Car(id, team, driver, description, number);
            c.Assign(classes);
            return c;
        }

        public bool Remove(string Id)
        {
            return false;
        }

        public bool Clear()
        {
            return false;
        }
    }
}