using camping.Core;
using camping.Database;
using Camping.Core;

namespace camping.Test
{
    [TestFixture]
    public class RetrieveDataTests
    {
        SshConnection sshConnection;
        private RetrieveData retrieveData;
        private Reservation TestReservation;
        private SiteData siteData;
        ReservationRepository reservationData;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            sshConnection = new SshConnection();

            siteData = new();
            reservationData = new();
            retrieveData = new(siteData, reservationData);
        }

        [SetUp]
        public void Setup()
        {
            retrieveData.addReservation(1, DateTime.Today.ToString("MM-dd-yyyy"), DateTime.Today.ToString("MM-dd-yyyy"), "Test", "Unit", "Setup", "database", "camping", "1234XX", "1a", 06123456);
            TestReservation = retrieveData.Reservations[retrieveData.Reservations.Count - 1];
        }

        [Test]
        public void GetCampSiteID_ReturnsListOfIntegers()
        {
            List<int> campSiteIDs = retrieveData.GetCampSiteID();

            Assert.IsNotNull(campSiteIDs);
            Assert.IsInstanceOf<List<int>>(campSiteIDs);
        }

        [Test]
        public void GetSurfaceArea_ReturnsListOfIntegers()
        {
            List<int> result = retrieveData.GetSurfaceArea();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<int>>(result);
        }

        [Test]
        [TestCase("12-15-2024", "12-26-2024")]
        public void UpdateReservation_UpdatesReservationInfoInDatabase(DateTime startDate, DateTime endDate)
        {
            Assert.IsTrue(retrieveData.UpdateReservation(TestReservation.ReservationID, startDate, new Visitor(6, "Jelle", "Bouman", string.Empty, "Bertram", "Mepple", "8269HM", "28", 28), endDate,TestReservation.SiteID));
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
