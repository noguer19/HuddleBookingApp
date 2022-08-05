using DeskBooker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace DeskBooker.DataAccess;

public class DeskBookerContext : DbContext
{
    public DeskBookerContext(DbContextOptions<DeskBookerContext> options) : base(options)
    {
    }

    public DbSet<DeskBooking> DeskBooking { get; set; }

    public DbSet<Desk> Desk { get; set; }
    public DbSet<MeetingRoom> MeetingRoom { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Desk>().HasKey("Id");
        modelBuilder.Entity<DeskBooking>().Property(p => p.BookingStartTime).IsRequired(false);
        modelBuilder.Entity<DeskBooking>().Property(p => p.BookingEndTime).IsRequired(false);
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Desk>().HasData(
          new Desk { Id = 1, Description = "Desk 1 - Close to left side windows" },
          new Desk { Id = 2, Description = "Desk 2 - Close to men bathroom" },
          new Desk { Id = 3, Description = "Desk 3 - Close to women bathroom" },
          new Desk { Id = 4, Description = "Desk 4 - Close to right side windows" },
          new Desk { Id = 5, Description = "Desk 5 - Close to front side windows" },
          new Desk { Id = 6, Description = "Desk 6 - Close to front side windows" },
          new Desk { Id = 7, Description = "Desk 7 - Close to front side windows" },
          new Desk { Id = 8, Description = "Desk 8 - Close to front side windows" },
          new Desk { Id = 9, Description = "Desk 9 - Close to right side windows" },
          new Desk { Id = 10, Description = "Desk 10 - Close to right side windows" }
        );

        modelBuilder.Entity<MeetingRoom>().HasData(
            new MeetingRoom
            {
                Id = 1,
                Active = true,
                MaxPeopleAllowed = 10,
                RoomName = "Momotombo",
                ImageUrl = "https://huddlenetwork.com/wp-content/uploads/2020/10/W-Membresias-Sala-de-Reuniones-1024x576.jpg",
                RoomDescription = "Meeting room with space for up to 10 people, perfect for meetings with your teamwork, partners or customers. This room is equiped with a big table and confortable chairs, as well as a 42\" TV and a private bathroom."
            },
            new MeetingRoom
            {
                Id = 2,
                Active = true,
                MaxPeopleAllowed = 6,
                RoomName = "Guardabarranco",
                ImageUrl = "https://huddlenetwork.com/wp-content/uploads/2020/10/DSCN8461-1024x576.jpg",
                RoomDescription = "Meeting room with space for up to 6 people. This room is equiped with a 42\" TV, a table for up to 6 people and confortable chairs."
            },
            new MeetingRoom
            {
                Id = 3,
                Active = true,
                MaxPeopleAllowed = 4,
                RoomName = "Sacuanjoche",
                ImageUrl = "https://huddlenetwork.com/wp-content/uploads/elementor/thumbs/Sala-de-reuniones-Espacio-de-trabajo-2-scaled-oy6g4gq3fidu7me7vbvabxtzw6yelbzl9cakp950qo.jpg",
                RoomDescription = "Meeting room with space for up to 4 peole, perfect for small teams meetings. This room is equiped with a 42\" TV, 4 confortable chairs and a rounded table."
            });


        modelBuilder.Entity<DeskBooking>().HasData(
            new DeskBooking
            {
                Id = 1,
                BookingTypeId = 1,
                FirstName = "Luis",
                LastName = "Noguera",
                Email = "luisn@nicasource.com",
                Active = true,
                Date = new DateTime(2022, 05, 05),
                DeskId = 1,
                Notes = "Test Desk Booking"
            },
            new DeskBooking
            {
                Id = 2,
                BookingTypeId = 2,
                FirstName = "Jeeson",
                LastName = "López",
                Email = "jlopez@nicasource.com",
                Active = true,
                Date = new DateTime(2022, 05, 05),
                Notes = "Test Meeting Room Booking",
                MeetingRoomId = 1,
                BookingStartTime = new DateTime(2022, 05, 05, 10, 0, 0),
                BookingEndTime = new DateTime(2022, 05, 05, 11, 0, 0)
            });
    }
}
