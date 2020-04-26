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
    [Migration("20190305211151_Heating_v1")]
    partial class Heating_v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
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

                    b.Property<string>("LocationLong")
                        .IsRequired()
                        .HasMaxLength(80);

                    b.Property<string>("LocationShort")
                        .IsRequired()
                        .HasMaxLength(32);

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

                    b.Property<Guid?>("AppointmentId");

                    b.Property<Guid>("CalendarAppointmentId");

                    b.Property<DateTime>("EndTime");

                    b.Property<int?>("ExBusyState");

                    b.Property<string>("ExLocationLong")
                        .HasMaxLength(80);

                    b.Property<string>("ExLocationShort")
                        .HasMaxLength(32);

                    b.Property<string>("ExSummary")
                        .HasMaxLength(120);

                    b.Property<bool>("IsFullDay");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentId");

                    b.ToTable("CalendarOccurances");
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.ViessmannHeatingData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BoilerTempCurve")
                        .HasMaxLength(800);

                    b.Property<string>("BoilerTempMainCurve")
                        .HasMaxLength(800);

                    b.Property<string>("BurnerActiveCurve")
                        .HasMaxLength(100);

                    b.Property<double>("BurnerHoursTotal");

                    b.Property<string>("BurnerMinutesCurve")
                        .HasMaxLength(800);

                    b.Property<string>("BurnerModulationCurve")
                        .HasMaxLength(800);

                    b.Property<string>("BurnerStartsCurve")
                        .HasMaxLength(800);

                    b.Property<int>("BurnerStartsTotal");

                    b.Property<string>("Circuit0PumpCurve")
                        .HasMaxLength(100);

                    b.Property<string>("Circuit0TempCurve")
                        .HasMaxLength(800);

                    b.Property<string>("Circuit1PumpCurve")
                        .HasMaxLength(100);

                    b.Property<string>("Circuit1TempCurve")
                        .HasMaxLength(800);

                    b.Property<DateTime>("Day");

                    b.Property<string>("DhwCirculationPumpCurve")
                        .HasMaxLength(100);

                    b.Property<string>("DhwPrimaryPumpCurve")
                        .HasMaxLength(100);

                    b.Property<string>("DhwTempCurve")
                        .HasMaxLength(800);

                    b.Property<string>("OutsideTempCurve")
                        .HasMaxLength(800);

                    b.HasKey("Id");

                    b.ToTable("ViessmannHeatingTimeseries");
                });

            modelBuilder.Entity("GraphIoT.App.Database.Model.ViessmannSolarData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Day");

                    b.Property<string>("SolarCollectorTempCurve")
                        .HasMaxLength(800);

                    b.Property<string>("SolarHotwaterTempCurve")
                        .HasMaxLength(800);

                    b.Property<string>("SolarPumpStateCurve")
                        .HasMaxLength(100);

                    b.Property<string>("SolarSuppressionCurve")
                        .HasMaxLength(100);

                    b.Property<string>("SolarWhCurve")
                        .HasMaxLength(800);

                    b.Property<int?>("SolarWhTotal");

                    b.HasKey("Id");

                    b.HasIndex("Day")
                        .IsUnique();

                    b.ToTable("ViessmannSolarTimeseries");
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
                        .HasForeignKey("AppointmentId");
                });
#pragma warning restore 612, 618
        }
    }
}
