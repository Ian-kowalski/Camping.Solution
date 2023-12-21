using camping.Database;
using Newtonsoft.Json.Bson;

namespace Camping.Test
{
    public class ReservationTests
    {

        SshConnection sshConnection;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            sshConnection = new SshConnection();
        }

        [SetUp]
        public void Setup()
        {

        }

        public void DeleteReservation(int reservationID)
        {

        }


        public void addReservationLine(int campSiteID, int reservationID)
        {

        }


        public void addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
        {

        }

        public void getReservationID(int visitorID, string startDate, string endDate)
        {

        }

        public void GetOtherAvailableReservation(int campSite, string startDate, string endDate, int reservationID)
        {

        }

        public void UpdateReservationLines(int campSiteID, int reservationID)
        {

        }

        public void UpdateReservation(int reservationID, DateTime startDate, DateTime endDate, int campSiteID)
        {

        }

        public void HasUpcomingReservations(int campSiteID, DateTime date)
        {

        }

        [TearDown]
        public void TearDown() 
        {

        }


        [OneTimeTearDown]
        public void OneTimeTearDown() { sshConnection.BreakConnection(); }

    }
}
