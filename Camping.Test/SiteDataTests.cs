using camping.Core;
using camping.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Test
{
    [TestFixture]
    public class SiteDataTests
    {
        private SiteData siteData;

        [SetUp]
        public void Setup()
        {
            siteData = new SiteData();
        }

        [Test]
        public void GetSiteInfo_ReturnsListOfSites()
        {
            List<Site> sites = siteData.GetSiteInfo();

            Assert.IsNotNull(sites);
            Assert.IsInstanceOf<List<Site>>(sites);
        }
        [Test]
        public void GetStreetInfo_ReturnsListOfStreets()
        {
            List<Street> sites = siteData.GetStreetInfo();

            Assert.IsNotNull(sites);
            Assert.IsInstanceOf<List<Street>>(sites);
        }
        [Test]
        public void GetAreaInfo_ReturnsListOfAreas()
        {
            List<Area> areas = siteData.GetAreaInfo();

            Assert.IsNotNull(areas);
            Assert.IsInstanceOf<List<Site>>(areas);
        }

        [Test]
        public void GetCampSiteID_ReturnsListOfIntegers()
        {
            int reservationID = 1; 

            List<int> campSiteIDs = siteData.GetCampSiteID(reservationID);

            Assert.IsNotNull(campSiteIDs);
            Assert.IsInstanceOf<List<int>>(campSiteIDs);
        }

        [Test]
        public void GetAvailability_ReturnsListOfReservationDates()
        {
            int siteID = 1;

            List<ReservationDates> availability = siteData.GetAvailability(siteID);

            Assert.IsNotNull(availability);
            Assert.IsInstanceOf<List<ReservationDates>>(availability);
        }
    }
}
