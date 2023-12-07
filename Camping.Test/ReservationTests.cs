using camping.Database;

namespace Camping.Test
{
    public class ReservationTests
    {

        SshConnection sshConnection;
        [OneTimeSetUp]
        public void Setup()
        {
            sshConnection = new SshConnection();
        }


        [Test]
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", "26", 12345678, 2)]
        [TestCase("Jelle", "Bouman", "de", "bertram", "Mepple", "7944NS", "26", 12345678, -1)]
        public void Reservation_GetVisitor_ID(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            string houseNumber, int phoneNumber,
            int result)
        {
            VisitorRepository visiterRepo = new();

            int ID = visiterRepo.getVisitorID(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);

            if (ID == result)
            {
                Assert.Pass($"Expected: {result} Got: {ID}");
            }
            else
            {
                Assert.Fail($"Expected: {result} Got: {ID}");
            }
            

        }

        [Test]
        [TestCase("11-30-2023", "12-04-2023", 1, 1)]
        [TestCase("12-15-2023", "11-26-2023", 2, 2)]
        [TestCase("11-30-2024", "12-04-2024", 1, -1)]
        public void Reservation_GetReservation_ID(
            string startDate, string endDate, int visitorID,
            int result)
        {
            ReservationRepository reservationRepo = new();

            int ID = reservationRepo.getReservationID(visitorID, startDate, endDate);

            if (ID == result)
            {
                Assert.Pass($"Expected: {result} Got: {ID}");
            }
            else
            {
                Assert.Fail($"Expected: {result} Got: {ID}");
            }
            

        }

        [Test]
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", "26", 12345678, "12-15-2023", "11-26-2023", 2)]
        [TestCase("Jelle", "Bouman", "de", "bertram", "Mepple", "7944NS", "26", 12345678, "12-15-2024", "11-26-2024", -1)]
        public void Reservation_Visitor_GetReservation_ID(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            string houseNumber, int phoneNumber,
            string startDate, string endDate,
            int result)
        {
            ReservationRepository reservationRepo = new();
            VisitorRepository visiterRepo = new();

            int visitorID = visiterRepo.getVisitorID(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);

            int ID = reservationRepo.getReservationID(visitorID, startDate, endDate);

            if (ID == result)
            {
                Assert.Pass($"Expected: {result} Got: {ID}");
            }
            else
            {
                Assert.Fail($"Expected: {result} Got: {ID}");
            }

        }

        [Test]
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", "26", 12345678)] // bestaat al, dus false
        [TestCase("Leroy", "Staaks", "de", "gertrude", "Hoogeveen", "1235JD","51", 12345678)] // bestaat al, dus false
        public void Reservation_AddVisitor_AlreadyExists(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            string houseNumber, int phoneNumber)
        {
            VisitorRepository visiterRepo = new();

            if (!visiterRepo.addVisitor(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("Visitor added even though not unique");
            }

        }
        [Test]
        [TestCase("T", "e", "s", "t", "Unique Visitor", "1234AB")] // unieke visitor
        public void Reservation_AddVisitor_DoesNotExist(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode)
        {
            VisitorRepository visiterRepo = new();

            Random random = new Random();


            if (visiterRepo.addVisitor(firstName, lastName, preposition, adress, city, postalcode, random.Next(1, 99).ToString(), random.Next(10000000, 99999999)))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("Returned false Expected true");
            }

        }

        [Test]
        [TestCase(1, "Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", "26", 12345678, "12-02-2023", "12-06-2023")]
        public void Reservation_AddReservation_Unavailable(
            int campSite,
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            string houseNumber, int phoneNumber,
            string startDate, string endDate)
        {
            ReservationRepository reservationRepo = new();

            if (!reservationRepo.addReservation(campSite, startDate, endDate, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("Reservation added even though unavailable");
            }

        }

        [Test]
        [TestCase(1, "12-03-2023", "12-06-2023", false)]
        [TestCase(1, "12-05-2023", "12-06-2023", true)]
        public void Reservation_GetAvailableReservation(int campSite, string startDate, string endDate, bool available)
        {
            ReservationRepository reservationRepo = new();

            if (reservationRepo.GetAvailableReservation(campSite, startDate, endDate) == available)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        [TestCase(4, "12-15-2023", "11-27-2023", 3)]
        public void Reservation_UpdateReservation_Update(int resID, DateTime startDate, DateTime endDate, int campSiteID)
        {
            ReservationRepository reservationRepo = new();

            Assert.IsTrue(reservationRepo.UpdateReservation(resID, startDate, endDate, campSiteID));

        }

        [Test]
        [TestCase(2, "Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", "26", 12345678)]
        public void Reservation_UpdateVisitor_Update(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
        {
            ReservationRepository reservationRepo = new();

            Assert.IsTrue(reservationRepo.UpdateVisitor(visitorID, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber));
            
        }


        [Test]
        [TestCase(2, "12-02-2025", "12-05-2025", "test", "delete", "res", "this Street", "here", "2332XX", "22", 54717700)]
        public void Reservation_DeleteReservation_delete(int campsId, string sDate, string eDate, string fName, string prop, string lName, string adres, string city, string postcode, string huisnummer, int phoneNumber)
        {
            ReservationRepository   reservationRepo = new();
            VisitorRepository visitorRepository = new();
            reservationRepo.addReservation(campsId, sDate, eDate, fName, prop, lName, adres, city, postcode, huisnummer, phoneNumber);
            int visitorID = visitorRepository.getVisitorID(fName, lName, prop, adres, city, postcode, huisnummer, phoneNumber);
            int resID = reservationRepo.getReservationID(visitorID, sDate, eDate);

            if (reservationRepo.DeleteReservation(resID))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
            
        }

        [OneTimeTearDown]
        public void TearDown() { sshConnection.BreakConnection(); }

    }
}
