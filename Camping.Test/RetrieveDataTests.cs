﻿using camping.Core;
using camping.Database;
using Camping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Test
{
    [TestFixture]
    public class RetrieveDataTests
    {
        [Test]
        public void GetCampSiteID_ReturnsListOfIntegers()
        {
            SiteData siteData = new();
            ReservationData reservationData = new();
            RetrieveData retrieveData = new(siteData, reservationData);

            List<int> campSiteIDs = retrieveData.GetCampSiteID();

            Assert.IsNotNull(campSiteIDs);
            Assert.IsInstanceOf<List<int>>(campSiteIDs);
        }


        [Test]
        public void GetSurfaceArea_ReturnsListOfIntegers()
        {
            SiteData siteData = new();
            ReservationData reservationData = new();

            RetrieveData retrieveData = new(siteData, reservationData);

            List<int> result = retrieveData.GetSurfaceArea();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<int>>(result);
        }

        [TestCase(1, true)]
        [TestCase(5, false)]
        public void GetDate_ReturnsTrueWhenNoOverlappingReservations(int siteID, bool excpected)
        {
            var siteData = new SiteData(); // Assuming SiteData has actual data or interacts with a database
            var reservationData = new ReservationData(); // Assuming ReservationData has actual data or interacts with a database
            var retrieveData = new RetrieveData(siteData, reservationData);

            Assert.IsTrue(retrieveData.GetDate(siteID) == excpected);
        }
    }
}