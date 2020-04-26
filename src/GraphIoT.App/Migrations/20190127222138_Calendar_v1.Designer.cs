﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhilipDaubmeier.GraphIoT.App.Database;

namespace PhilipDaubmeier.GraphIoT.App.Migrations
{
    [DbContext(typeof(PersistenceContext))]
    [Migration("20190127222138_Calendar_v1")]
    partial class Calendar_v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GraphIoT.App.Database.Model.AuthData", b =>
                {
                    b.Property<string>("AuthDataId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DataContent");

                    b.HasKey("AuthDataId");

                    b.ToTable("AuthDataSet");
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.Calendar", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Owner");

                    b.HasKey("Id");

                    b.ToTable("Calendars");
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.CalendarAppointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusyState");

                    b.Property<Guid>("CalendarId");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.HasKey("Id");

                    b.HasIndex("CalendarId");

                    b.ToTable("CalendarAppointments");
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.CalendarOccurence", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AppointmentId");

                    b.Property<Guid>("CalendarAppointmentId");

                    b.Property<DateTime>("EndTime");

                    b.Property<bool>("IsFullDay");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentId");

                    b.ToTable("CalendarOccurances");
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.CalendarAppointment", b =>
                {
                    b.HasOne("GraphIoT.App.Database.Model.Calendar", "Calendar")
                        .WithMany("Appointments")
                        .HasForeignKey("CalendarId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.CalendarOccurence", b =>
                {
                    b.HasOne("GraphIoT.App.Database.Model.CalendarAppointment", "CalendarAppointment")
                        .WithMany("Occurences")
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
