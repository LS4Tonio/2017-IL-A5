﻿using System;
using NUnit.Framework;
using Algo.Optim;

namespace Algo.Tests
{
    [TestFixture]
    public class Optim
    {
        [Test]
        public void GetFlights()
        {
            var db = new FlightDatabase(@"D:\LS4Tonio\IN'TECH_INFO\Algorithmie\2017-IL-A5\Algo\ThirdParty\FlightData");

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
    }
}