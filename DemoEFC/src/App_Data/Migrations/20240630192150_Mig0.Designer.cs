﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using src.App_Data.Context;

#nullable disable

namespace src.App_Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240630192150_Mig0")]
    partial class Mig0
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("src.App_Data.Entities.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Area")
                        .HasColumnType("numeric");

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Population")
                        .HasColumnType("numeric");

                    b.Property<int>("StateId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("StateId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("src.App_Data.Entities.Il", b =>
                {
                    b.Property<int>("IlId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IlId"));

                    b.Property<string>("IlAdi")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IlId");

                    b.ToTable("Iller");
                });

            modelBuilder.Entity("src.App_Data.Entities.Ilce", b =>
                {
                    b.Property<int>("IlceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IlceId"));

                    b.Property<int>("IlId")
                        .HasColumnType("integer");

                    b.Property<string>("IlceAdi")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IlceId");

                    b.HasIndex("IlId");

                    b.ToTable("Ilceler");
                });

            modelBuilder.Entity("src.App_Data.Entities.Mahalle", b =>
                {
                    b.Property<int>("MahalleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MahalleId"));

                    b.Property<string>("MahalleAdi")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PostaKodu")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SemtBucakBeldeId")
                        .HasColumnType("integer");

                    b.HasKey("MahalleId");

                    b.HasIndex("SemtBucakBeldeId");

                    b.ToTable("Mahalleler");
                });

            modelBuilder.Entity("src.App_Data.Entities.SemtBucakBelde", b =>
                {
                    b.Property<int>("SemtBucakBeldeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SemtBucakBeldeId"));

                    b.Property<int>("IlceId")
                        .HasColumnType("integer");

                    b.Property<string>("SemtBucakBeldeAdi")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SemtBucakBeldeId");

                    b.HasIndex("IlceId");

                    b.ToTable("SemtBucakBeldeler");
                });

            modelBuilder.Entity("src.App_Data.Entities.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("StateName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("src.App_Data.Entities.City", b =>
                {
                    b.HasOne("src.App_Data.Entities.State", null)
                        .WithMany("Cities")
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("src.App_Data.Entities.Ilce", b =>
                {
                    b.HasOne("src.App_Data.Entities.Il", "Il")
                        .WithMany("Ilceler")
                        .HasForeignKey("IlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Il");
                });

            modelBuilder.Entity("src.App_Data.Entities.Mahalle", b =>
                {
                    b.HasOne("src.App_Data.Entities.SemtBucakBelde", "SemtBucakBelde")
                        .WithMany("Mahalleler")
                        .HasForeignKey("SemtBucakBeldeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SemtBucakBelde");
                });

            modelBuilder.Entity("src.App_Data.Entities.SemtBucakBelde", b =>
                {
                    b.HasOne("src.App_Data.Entities.Ilce", "Ilce")
                        .WithMany("SemtBucakBeldeler")
                        .HasForeignKey("IlceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ilce");
                });

            modelBuilder.Entity("src.App_Data.Entities.Il", b =>
                {
                    b.Navigation("Ilceler");
                });

            modelBuilder.Entity("src.App_Data.Entities.Ilce", b =>
                {
                    b.Navigation("SemtBucakBeldeler");
                });

            modelBuilder.Entity("src.App_Data.Entities.SemtBucakBelde", b =>
                {
                    b.Navigation("Mahalleler");
                });

            modelBuilder.Entity("src.App_Data.Entities.State", b =>
                {
                    b.Navigation("Cities");
                });
#pragma warning restore 612, 618
        }
    }
}
