﻿using HotelBooking.BLL;
using HotelBooking.DAL;
using HotelBooking.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        //------------Createbooking------------

        [Test]
        [TestCase(1, 9)]
        [TestCase(21, 22)]
        public void CreateBooking_ValidInputInOccupied_BookingCreated(int stNo, int edNo)
        {
            DateTime startdate = DateTime.Today.AddDays(stNo);
            DateTime enddate = DateTime.Today.AddDays(edNo);
            Booking newBooking = new Booking
            {
                Id = 4,
                StartDate = startdate,
                EndDate = enddate,
                CustomerId = 1,
                RoomId = 1,
                IsActive = true
            };
            BookingManager manager = CreateBookingManager(newBooking);
            var createdBooking = manager.CreateBooking(newBooking);
            Assert.AreEqual(createdBooking, createdBooking);
        }

        [Test]
        [TestCase(9, 21)]
        [TestCase(9, 10)]
        [TestCase(9, 20)]
        [TestCase(10, 21)]
        [TestCase(10, 10)]
        [TestCase(10, 20)]
        [TestCase(20, 20)]
        [TestCase(20, 21)]
        public void CreateBooking_InValidInputInOccupied_NotCreated(int stNo, int edNo)
        {
            DateTime startdate = DateTime.Today.AddDays(stNo);
            DateTime enddate = DateTime.Today.AddDays(edNo);
            Booking newBooking = new Booking
            {
                Id = 4,
                StartDate = startdate,
                EndDate = enddate,
                CustomerId = 1,
                RoomId = 1,
                IsActive = true
            };
            BookingManager manager = CreateBookingManager(newBooking);
            var createdBooking = manager.CreateBooking(newBooking);
            Assert.AreEqual(null, createdBooking);
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(2, 1)]
        [TestCase(-1, 1)]
        public void CreateBooking_InvalidInput_ThrowsArgumentException(int stNo, int edNo)
        {
            Booking newBooking = new Booking
            {
                Id = 4,
                StartDate = DateTime.Today.AddDays(stNo),
                EndDate = DateTime.Today.AddDays(edNo),
                CustomerId = 1,
                RoomId = 1,
                IsActive = true
            };
            BookingManager manager = CreateBookingManager(newBooking);
            var createdBooking = Assert.Throws<ArgumentException>(()
               => manager.CreateBooking(newBooking));
            StringAssert.Contains("Start and end date cannot be set to before current date, and the start date later than the end date", createdBooking.Message);
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void CreateBooking_ValidInput_BookingIsCreated(int stNo, int edNo)
        {
            DateTime date = DateTime.Today.AddDays(stNo);
            DateTime date2 = DateTime.Today.AddDays(edNo);
            Booking newBooking = new Booking
            {
                Id = 4,
                StartDate = date,
                EndDate = date2,
                CustomerId = 1,
                RoomId = 1,
                IsActive = true
            };
            BookingManager manager = CreateBookingManager(newBooking);
            var createdBooking = manager.CreateBooking(newBooking);
            Assert.AreEqual(newBooking, createdBooking);
        }

        private BookingManager CreateBookingManager(Booking booking)
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);

            List<Booking> bookings = new List<Booking>
            {
                new Booking { Id=1, StartDate=start, EndDate=end, IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=2, StartDate=start, EndDate=end, IsActive=true, CustomerId=2, RoomId=2 }
            };

            List<Room> rooms = new List<Room>
            {
                new Room { Id = 1 },
                new Room { Id = 2 }
            };

            // Fake RoomRepository using NSubstitute
            IRepository<Room> roomRepository = Substitute.For<IRepository<Room>>();
            roomRepository.GetAll().Returns(rooms);

            // Fake BookingRepository using NSubstitute
            IRepository<Booking> bookingRepository = Substitute.For<IRepository<Booking>>();
            bookingRepository.Add(booking).Returns(booking);
            bookingRepository.GetAll().Returns(bookings);

            return new BookingManager(bookingRepository, roomRepository);
        }

        //------------FindAvailableRoom------------
        [Test]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            BookingManager manager = CreateBookingManager();
            DateTime date = DateTime.Today.AddDays(5);

            int roomId = manager.FindAvailableRoom(date, date);

            Assert.AreNotEqual(-1, roomId);
        }        

        [Test]
        [TestCase(1,-1)]
        [TestCase(-1, 1)]
        public void FindAvailableRoom_InvalidInput_ThrowsExceptionError(int stNo, int edNo)
        {
            BookingManager manager = CreateBookingManager();
            DateTime startdate = DateTime.Today.AddDays(stNo);
            DateTime enddate = DateTime.Today.AddDays(edNo);
            var ex = Assert.Throws<ArgumentException>(()
                => manager.FindAvailableRoom(startdate, enddate));
            StringAssert.Contains("Start and end date cannot be set to before current date, and the start date later than the end date", ex.Message);
        }
        //------------GetFullyOccupiedDates------------
        [Test]
        public void GetFullyOccupiedDates_NoOccupiedDates_ReturnsEmptyListOfDates()
        {
            BookingManager manager = CreateBookingManager();
            var date = DateTime.Today;
            DateTime dateTest = date.AddDays(35);
            var myList = new List<DateTime>();
            var datesList = manager.GetFullyOccupiedDates(dateTest, dateTest.AddDays(1));
            Assert.AreEqual(myList, datesList);
        }

        [Test]
        public void GetFullyOccupiedDates_StartDateIsLaterThanEndDate_ThrowsExceptionStartDateIsGreater()
        {
            BookingManager manager = CreateBookingManager();
            DateTime today = DateTime.Today;
            var ex = Assert.Throws<ArgumentException>(()
                => manager.GetFullyOccupiedDates(today.AddDays(1), today));
            Assert.That(ex.Message,
                Is.EqualTo("The start date cannot be later than the end date."));
        }
        //------------------YearToDisplay---------------------

        [Test]
        public void YearToDisplay_ThisYear_Returns2017()
        {
            BookingManager manager = CreateBookingManager();
            var result = manager.YearToDisplay(2017);
            Assert.AreEqual(2017, result);
        }

        [Test]
        public void YearToDisplay_LastYear_Returns2016()
        {
            BookingManager manager = CreateBookingManager();
            var result = manager.YearToDisplay(2016);
            Assert.AreEqual(2016, result);
        }

        [Test]
        public void YearToDisplay_YearIsPastMinYear_Returns2016()
        {
            BookingManager manager = CreateBookingManager();
            var result = manager.YearToDisplay(2015);
            Assert.AreEqual(2016, result);
        }

        [Test]
        public void YearToDisplay_YearIsOverMaxYear_Returns2017()
        {
            BookingManager manager = CreateBookingManager();
            var result = manager.YearToDisplay(2018);
            Assert.AreEqual(2017, result);
        }

        //------------------MinBookingDate() And MaxBookingDate()--------------
        [Test]
        public void MinBookingDate_LastYearBookingAdded_ReturnLastYearDate()
        {
            BookingManager manager = CreateBookingManager();
            DateTime LastYearStart = new DateTime(2016, 02, 02);
            var result = manager.MinBookingDate();
            Assert.AreEqual(LastYearStart, result);
        }

        [Test]
        public void MinBookingDate_CurrentDateAddedplus20Days_ReturnCurrentDatePlus20Days()
        {
            BookingManager manager = CreateBookingManager();
            DateTime lastBookingDate = DateTime.Today.AddDays(20);
            var result = manager.MaxBookingDate();
            Assert.AreEqual(lastBookingDate, result);
        }
        //---------------------------------------------------------------------------
        private BookingManager CreateBookingManager()
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            DateTime LastYearStart = new DateTime(2016, 02, 02);
            DateTime LastYearEnd = new DateTime(2016, 02, 22);

            List<Booking> bookings = new List<Booking>
            {
                new Booking { Id=1, StartDate=start, EndDate=end, IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=2, StartDate=start, EndDate=end, IsActive=true, CustomerId=2, RoomId=2 },
                new Booking {Id=3, StartDate = LastYearStart, EndDate = LastYearEnd, IsActive=true, CustomerId = 3, RoomId = 3 },
            };

            List<Room> rooms = new List<Room>
            {
                new Room { Id = 1 },
                new Room { Id = 2 }
            };

            // Fake BookingRepository using NSubstitute
            IRepository<Booking> bookingRepository = Substitute.For<IRepository<Booking>>();
            bookingRepository.GetAll().Returns(bookings);

            // Fake RoomRepository using NSubstitute
            IRepository<Room> roomRepository = Substitute.For<IRepository<Room>>();
            roomRepository.GetAll().Returns(rooms);

            return new BookingManager(bookingRepository, roomRepository);
        }
    }


}

