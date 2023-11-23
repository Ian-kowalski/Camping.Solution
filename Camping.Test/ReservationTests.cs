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
        [TestCase("Jelle", "Bouman", "het", "bertram", "Mepple", "7944NS", 26, 12345678, false)] // bestaat al, dus false
        [TestCase("Leroy", "Staaks", "de", "gertrude", "Hoogeveen", "1235JD", 51, 12345678, false)] // bestaat al, dus false
        public void Reservation_AddVisitor(
            string firstName, string lastName, string preposition,
            string adress, string city, string postalcode,
            int houseNumber, int phoneNumber,
            bool result)
        {
            VisitorRepository visiterRepo = new();

            

            if (visiterRepo.addVisitor(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber) == result)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }

        }

        [Test]
        [TestCase()]
        public void Reservation_AddReservationLine()
        {
            ReservationData reservationRepo = new();
            VisitorRepository visiterRepo = new();

            if (reservationRepo.addReservationLine(1, 1) == 1)
            {
                Assert.Pass();
            }
            else {
                Assert.Fail();
            }
           
            

        }


    }
}
