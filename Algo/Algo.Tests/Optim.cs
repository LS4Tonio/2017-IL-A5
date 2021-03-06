﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Algo.Optim;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.IO;

namespace Algo.Tests
{
    [TestFixture]
    public class Optim
    {
        private static string GetFlightDataPath([CallerFilePath]string thisFilePath = null)
        {
            string algoPath = Path.GetDirectoryName(Path.GetDirectoryName(thisFilePath));
            return Path.Combine(algoPath, "ThirdParty", "FlightData");
        }

        [Test]
        public void opening_database_from_ThirdParty_FlightData()
        {
            FlightDatabase db = new FlightDatabase(GetFlightDataPath());

            {
                var f0 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("BER"), Airport.FindByCode("LHR"));
                var f1 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("CDG"), Airport.FindByCode("LHR"));
                var f2 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("MRS"), Airport.FindByCode("LHR"));
                var f3 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("LYS"), Airport.FindByCode("LHR"));
                var f4 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("MAN"), Airport.FindByCode("LHR"));
                var f5 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("BIO"), Airport.FindByCode("LHR"));
                var f6 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("JFK"), Airport.FindByCode("LHR"));
                var f7 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("TUN"), Airport.FindByCode("LHR"));
                var f8 = db.GetFlights(new DateTime(2010, 7, 26), Airport.FindByCode("MXP"), Airport.FindByCode("LHR"));
            }
            {
                var f0 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("BER"), Airport.FindByCode("LHR"));
                var f1 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("CDG"), Airport.FindByCode("LHR"));
                var f2 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("MRS"), Airport.FindByCode("LHR"));
                var f3 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("LYS"), Airport.FindByCode("LHR"));
                var f4 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("MAN"), Airport.FindByCode("LHR"));
                var f5 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("BIO"), Airport.FindByCode("LHR"));
                var f6 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("JFK"), Airport.FindByCode("LHR"));
                var f7 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("TUN"), Airport.FindByCode("LHR"));
                var f8 = db.GetFlights(new DateTime(2010, 7, 27), Airport.FindByCode("MXP"), Airport.FindByCode("LHR"));
            }

            {
                var f0 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("BER"));
                var f1 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("CDG"));
                var f2 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("MRS"));
                var f3 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("LYS"));
                var f4 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("MAN"));
                var f5 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("BIO"));
                var f6 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("JFK"));
                var f7 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("TUN"));
                var f8 = db.GetFlights(new DateTime(2010, 8, 3), Airport.FindByCode("LHR"), Airport.FindByCode("MXP"));
            }
            {
                var f0 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("BER"));
                var f1 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("CDG"));
                var f2 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("MRS"));
                var f3 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("LYS"));
                var f4 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("MAN"));
                var f5 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("BIO"));
                var f6 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("JFK"));
                var f7 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("TUN"));
                var f8 = db.GetFlights(new DateTime(2010, 8, 4), Airport.FindByCode("LHR"), Airport.FindByCode("MXP"));
            }
        }

        [Test]
        public void creating_a_Meeting()
        {
            Meeting m = new Meeting(GetFlightDataPath(), 676);
            Assert.That(m.Guests.Count, Is.EqualTo(9));
            foreach (var g in m.Guests)
            {
                Assert.That(g.ArrivalFlights.Count, Is.GreaterThan(3));
                Assert.That(g.DepartureFlights.Count, Is.GreaterThan(3));
                Console.WriteLine($"{g.Name} ({g.Location.Code}): {g.ArrivalFlights.Count} {g.DepartureFlights.Count}");
            }
            Console.WriteLine($"Cardinality = {m.SolutionCardinality}");
        }

        [TestCase(100, false, 1)]
        [TestCase(100, true, 1)]
        [TestCase(400, false, 2)]
        [TestCase(400, true, 2)]
        [TestCase(800, true, 3)]
        public void random_algorithm(int loop, bool useMonteCarlo, int seed)
        {
            Meeting m = new Meeting(GetFlightDataPath(), seed);
            for (int i = 0; i < 10; ++i)
            {
                m.TryRandom(loop, useMonteCarlo);
                Console.WriteLine($"After {(i + 1) * loop} random => Best = {m.BestSolution.Cost} €, Worst = {m.WorstSolution.Cost} €.");
            }
        }

        [TestCase(10, 10, 124)]
        [TestCase(10, 100, 124)]
        [TestCase(10, 200, 124)]
        [TestCase(10, 300, 124)]
        [TestCase(10, 300, 125)]
        [TestCase(10, 400, 125)]
        public void using_Simulated_Annealing(int loop, int countPerTempDecrease, int seed)
        {
            int totalAWin = 0;
            int totalAMWin = 0;
            int totalMAWin = 0;
            Meeting m = new Meeting(GetFlightDataPath(), seed);
            for (int i = 0; i < 10; ++i)
            {
                Tuple<int, int, int> win = m.TrySimulatedAnnealing(loop, countPerTempDecrease);
                totalAWin += win.Item1;
                totalAMWin += win.Item2;
                totalMAWin += win.Item3;
                Console.WriteLine($"After {(i + 1) * loop} random => Best = {m.BestSolution.Cost} €, Worst = {m.WorstSolution.Cost} €.");
                Console.WriteLine($"SA wins MC {win.Item1} and SA+MC wins SA {win.Item2} and MC+SA wins SA {win.Item3} times out of {loop}.");
            }
            Console.WriteLine($"SA wins MC {totalAWin} and SA+MC wins SA {totalAMWin} and MC+SA wins SA {totalMAWin} times out of {10 * loop}.");
        }

        [TestCase(124)]
        [TestCase(9)]
        [TestCase(1336)]
        [TestCase(42)]
        [TestCase(12345)]
        [TestCase(98765)]
        [TestCase(1000)]
        [TestCase(1)]
        public void simulated_annealing_cost(int seed)
        {
            Meeting m = new Meeting(GetFlightDataPath(), seed);
            var beeeeeeest = m.RecuitSimule();
            Console.WriteLine($"Best cost after simulated annealing {beeeeeeest.Cost}");
        }
    }
}