﻿using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Algo.Optim
{
    public class Airport
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }

        private Airport(string[] five)
            : this(five[0], five[1], five[2], five[3], five[4])
        {
        }

        private Airport(string c, string n, string city, string state, string country)
        {
            Code = string.Intern(c);
            Name = n;
            City = city;
            State = state;
            Country = country;
        }

        public static ReadOnlyCollection<Airport> All;

        private static Dictionary<string, Airport> _byCode;
        private static Dictionary<string, Airport> _byCity;

        public static Airport FindByCode(string code)
        {
            Airport r;
            _byCode.TryGetValue(code, out r);
            return r;
        }

        public static Airport FindByCity(string city)
        {
            Airport r;
            _byCity.TryGetValue(city, out r);
            return r;
        }

        public static void Initialize(string path)
        {
            List<Airport> all = new List<Airport>();
            using (TextReader r = File.OpenText(path))
            {
                string line;
                while ((line = r.ReadLine()) != null) all.Add(new Airport(line.Split('|')));
            }
            All = new ReadOnlyCollection<Airport>(all.ToArray());
            _byCode = new Dictionary<string, Airport>();
            _byCity = new Dictionary<string, Airport>();
            foreach (Airport a in All)
            {
                _byCode.Add(a.Code, a);
                if (!_byCity.ContainsKey(a.City)) _byCity.Add(a.City, a);
            }
        }
    }
}