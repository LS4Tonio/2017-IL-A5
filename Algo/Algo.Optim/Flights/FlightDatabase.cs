using System;
using System.Collections.Generic;
using System.IO;

namespace Algo.Optim
{
    public class FlightDatabase
    {
        private readonly string _path;
        private readonly Dictionary<string, IList<SimpleFlight>> _cache;
        private KayakSession _kayak;

        public FlightDatabase(string path)
        {
            _path = path[path.Length - 1] != '\\' ? path + '\\' : path;
            _cache = new Dictionary<string, IList<SimpleFlight>>();
            Airport.Initialize(path + "airports.txt");
        }

        private KayakSession KayakSession => _kayak ?? (_kayak = new KayakSession());

        public IList<SimpleFlight> GetFlights(DateTime day, Airport from, Airport to)
        {
            IList<SimpleFlight> flights;
            var p = String.Format("{3}{0:yyyy}\\{0:MM}-{0:dd}\\{1}-{2}.txt", day.Date, from.Code, to.Code, _path);
            if (!_cache.TryGetValue(p, out flights))
            {
                if (File.Exists(p))
                {
                    flights = SimpleFlight.Load(p);
                }
                else
                {
                    flights = KayakSession.SimpleFlightSearch(from.Code, to.Code, day);
                    Directory.CreateDirectory(Path.GetDirectoryName(p));
                    SimpleFlight.Save(flights, p);
                }
                _cache.Add(p, flights);
            }
            return flights;
        }
    }
}