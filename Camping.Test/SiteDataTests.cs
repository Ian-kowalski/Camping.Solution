using camping.Core;
using camping.Database;

namespace camping.Test
{
    [TestFixture]
    public class SiteDataTests
    {
        SshConnection sshConnection;
        private RetrieveData retrieveData;
        private Reservation TestReservation;
        private SiteData siteData;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            sshConnection = new SshConnection();

            siteData = new();
            ReservationRepository reservationData = new();
            retrieveData = new(siteData, reservationData);
        }

        [SetUp]
        public void Setup()
        {
            retrieveData.addReservation(1, DateTime.Today.ToString("MM-dd-yyyy"), DateTime.Today.ToString("MM-dd-yyyy"), "Test", "Unit", "Setup", "database", "camping", "1234XX", "1a", 06123456);
            TestReservation = retrieveData.Reservations[retrieveData.Reservations.Count - 1];
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
            Assert.IsInstanceOf<List<Area>>(areas);
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
            List<ReservationDates> availability = siteData.GetAvailability(TestReservation.ReservationID);

            Assert.IsNotNull(availability);
            Assert.IsInstanceOf<List<ReservationDates>>(availability);
        }

        [TearDown]
        public void TearDown()
        {
            if (retrieveData.Reservations.Contains(TestReservation))
            {
                retrieveData.DeleteReservation(TestReservation.ReservationID);
            }

        }

        [OneTimeTearDown]
        public void OneTimeTearDown() { sshConnection.BreakConnection(); }
    }
}
