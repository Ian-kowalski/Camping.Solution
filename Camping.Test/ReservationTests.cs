using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using camping.Database;
using Microsoft.VisualBasic;

namespace Camping.Test
{
    public class ReservationTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", 26, 12345678, 2)]
        [TestCase("Jelle", "Bouman", "de", "bertram", "Mepple", "7944NS", 26, 12345678, -1)]
        public void Reservation_GetVisitor_ID(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode, 
            int houseNumber, int phoneNumber,
            int result)
        {
            VisitorRepository visiterRepo = new();

            int ID = visiterRepo.getVisitorID(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);

            if (ID == result)
            {
                Assert.Pass($"Expected: {result} Got: {ID}");
            }
            else { 
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
            ReservationData reservationRepo = new();

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
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", 26, 12345678, "12-15-2023", "11-26-2023", 2)]
        [TestCase("Jelle", "Bouman", "de", "bertram", "Mepple", "7944NS", 26, 12345678, "12-15-2024", "11-26-2024", - 1)]
        public void Reservation_Visitor_GetReservation_ID(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            int houseNumber, int phoneNumber,
            string startDate, string endDate,
            int result)
        {
            ReservationData reservationRepo = new();
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
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", 26, 12345678)] // bestaat al, dus false
        [TestCase("Leroy", "Staaks", "de", "gertrude", "Hoogeveen", "1235JD", 51, 12345678)] // bestaat al, dus false
        public void Reservation_AddVisitor_AlreadyExists(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            int houseNumber, int phoneNumber)
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


            if (visiterRepo.addVisitor(firstName, lastName, preposition, adress, city, postalcode, random.Next(1,99), random.Next(10000000, 99999999)))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("Returned false Expected true");
            }

        }

        [Test]
        [TestCase(1, "Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", 26, 12345678, "12-02-2023", "12-06-2023")]
        public void Reservation_AddReservation_Unavailable(
            int campSite,
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            int houseNumber, int phoneNumber,
            string startDate, string endDate)
        {
            ReservationData reservationRepo = new();

            if (!reservationRepo.addReservation(campSite, startDate, endDate, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber))
            {
                Assert.Pass();
            }
            else {
                Assert.Fail("Reservation added even though unavailable");
            }

        }

        [Test]
        [TestCase(1, "12-03-2023", "12-06-2023", false)]
        [TestCase(1, "12-05-2023", "12-06-2023", true)]
        public void Reservation_GetAvailableReservation(int campSite, string startDate, string endDate, bool available)
        {
            ReservationData reservationRepo = new();

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
        [TestCase(4, "12-15-2023", 6, "11-27-2023")]
        public void Reservation_UpdateReservation_Update(int resID, DateTime startDate, int visitorID, DateTime endDate)
        {
            ReservationData reservationRepo = new();

            Assert.IsTrue(reservationRepo.UpdateReservation(resID, startDate, visitorID, endDate));

        }

        [Test]
        [TestCase(2, "Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", 26, 12345678)]
        public void Reservation_UpdateVisitor_Update(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalcode, int houseNumber, int phoneNumber)
        {
            ReservationData reservationRepo = new();

            Assert.IsTrue(reservationRepo.UpdateVisitor(visitorID, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber));
            
        }


/*        [Test]
        public void Reservation_DeleteReservation_delete()
        {
            ReservationData reservationRepo = new();
            reservationRepo.addReservation(2, "12-02-2025", "12-05-2025", "test","delete","res","this Street","here","2332XX",22, 54717700);
            //reservationRepo.getReservationID()

            if (reservationRepo.DeleteReservation(2))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }*/

    }
}
