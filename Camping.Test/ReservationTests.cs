using camping.Core;
using camping.Database;
using System.Linq;


namespace camping.Test
{
    [TestFixture]
    public class ReservationTests
    {

        private SshConnection sshConnection;
        private RetrieveData retrieveData;
        private Reservation TestReservation;
        private Reservation TestAddReservation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            sshConnection = new SshConnection();

            SiteData siteData = new();
            ReservationRepository reservationData = new();
            retrieveData = new(siteData, reservationData);
        }

        [SetUp]
        public void Setup()
        {
            retrieveData.addReservation(1,DateTime.Today.ToString("MM-dd-yyyy"),DateTime.Today.ToString("MM-dd-yyyy"),"Test","Unit","Setup","database","camping","1234XX","1a",06123456);
            TestReservation = retrieveData.Reservations[retrieveData.Reservations.Count - 1];
        }


        [Test]
        public void DeleteReservation()
        {
            Assert.IsTrue(retrieveData.DeleteReservation(TestReservation.ReservationID) );
        }

        [Test]
        public void addReservation()
        {
            Assert.IsTrue(retrieveData.addReservation(2, DateTime.Today.ToString("MM-dd-yyyy"), DateTime.Today.ToString("MM-dd-yyyy"), "addReservationLine", "Unit", "Setup", "database", "camping", "1234XX", "1a", 06123456));
            TestAddReservation = retrieveData.Reservations[retrieveData.Reservations.Count - 1];
        }

        [Test]
        public void GetOtherAvailableReservation()
        {
            Assert.IsTrue(retrieveData.GetOtherAvailableReservations(TestReservation.SiteID, TestReservation.StartDate.ToShortDateString(), TestReservation.EndDate.ToShortDateString(), TestReservation.ReservationID));
        }

        [Test]
        public void UpdateReservation()
        {
            Visitor visitor = new Visitor(TestReservation.Visitor.VisitorID, TestReservation.Visitor.FirstName, TestReservation.Visitor.LastName, "Update", TestReservation.Visitor.Adress, TestReservation.Visitor.City, TestReservation.Visitor.PostalCode, TestReservation.Visitor.HouseNumber, TestReservation.Visitor.PhoneNumber);
            Assert.IsTrue(retrieveData.UpdateReservation(TestReservation.ReservationID, TestReservation.StartDate, visitor, TestReservation.EndDate, TestReservation.SiteID));
        }

        [Test]
        public void HasUpcomingReservations()
        {
            Assert.IsTrue(retrieveData.HasUpcomingReservations(TestReservation.SiteID));
        }

        [TearDown]
        public void TearDown()
        {
            if(retrieveData.Reservations.Contains(TestReservation) )
            {
                retrieveData.DeleteReservation(TestReservation.ReservationID);
            }

            if (retrieveData.Reservations.Contains(TestAddReservation))
            {
                retrieveData.DeleteReservation(TestAddReservation.ReservationID);
            }

        }


        [OneTimeTearDown]
        public void OneTimeTearDown() { sshConnection.BreakConnection(); }

    }
}
