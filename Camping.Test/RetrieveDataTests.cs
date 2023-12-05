using camping.Core;
using camping.Database;
using Camping.Core;
using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace camping.Test
{
    [TestFixture]
    public class RetrieveDataTests
    {
        SshConnection sshConnection;
        [SetUp]
        public void Setup()
        {
            sshConnection = new SshConnection();
        }
        [Test]
        public void GetCampSiteID_ReturnsListOfIntegers()
        {
            SiteData siteData = new();
            ReservationRepository reservationData = new();
            RetrieveData retrieveData = new(siteData, reservationData);

            List<int> campSiteIDs = retrieveData.GetCampSiteID();

            Assert.IsNotNull(campSiteIDs);
            Assert.IsInstanceOf<List<int>>(campSiteIDs);
        }

        [Test]
        public void GetSurfaceArea_ReturnsListOfIntegers()
        {
            SiteData siteData = new();
            ReservationRepository reservationData = new();

            RetrieveData retrieveData = new(siteData, reservationData);

            List<int> result = retrieveData.GetSurfaceArea();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<int>>(result);
        }

        [Test]
        [TestCase(1, true)] // could fail due to changes in database
        [TestCase(5, false)]
        public void GetDate_ReturnsTrueWhenNoOverlappingReservations(int siteID, bool excpected)
        {
            var siteData = new SiteData(); 
            var reservationData = new ReservationRepository ();
            var retrieveData = new RetrieveData(siteData, reservationData);

            Assert.IsTrue(retrieveData.GetDate(siteID) == excpected);
        }


/*        [Test]
        [TestCase(4, "12-15-2024", "12-26-2024")]
        public void UpdateReservation_UpdatesReservationInfoInDatabase(int reservationID, DateTime startDate, DateTime endDate)
        {
            SiteData siteData = new();
            ReservationData reservationData = new();

            RetrieveData retrieveData = new(siteData, reservationData);

            Assert.IsTrue(retrieveData.UpdateReservation(reservationID, startDate, new Visitor(6, "Jelle", "Bouman", string.Empty, "Bertram", "Mepple", "8269HM", 28, 28), endDate));
        }*/
        [TestCleanup]
        public void Cleanup() { sshConnection.BreakConnection(); }
    }
}
