﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiApplication.Database;

namespace WebApiApplication.Migrations
{
    [DbContext(typeof(FoodDeliveryDbContext))]
    [Migration("20210411143117_Discount code relation change")]
    partial class Discountcoderelationchange
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApiApplication.Database.MenuSection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RestaurantId");

                    b.ToTable("MenuSections");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.ClientAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ClientAddresses");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Complaint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Answer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOpened")
                        .HasColumnType("bit");

                    b.Property<int>("OrderForeignKey")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("OrderForeignKey")
                        .IsUnique();

                    b.ToTable("Complaints");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.DiscountCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AppliedToRestaurantId")
                        .HasColumnType("int");

                    b.Property<bool>("AppliesToAllRestaurants")
                        .HasColumnType("bit");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTo")
                        .HasColumnType("datetime2");

                    b.Property<double>("Percent")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AppliedToRestaurantId");

                    b.ToTable("DiscountCodes");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.MenuPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MenuSectionId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("MenuSectionId");

                    b.ToTable("MenuPositions");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DiscountCodeId")
                        .HasColumnType("int");

                    b.Property<double>("FinalPrice")
                        .HasColumnType("float");

                    b.Property<string>("OrderState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("OriginalPrice")
                        .HasColumnType("float");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("ClientId");

                    b.HasIndex("DiscountCodeId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Restaurant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AddressForeignKey")
                        .HasColumnType("int");

                    b.Property<string>("Contact")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfJoining")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Rate")
                        .HasColumnType("float");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ToPay")
                        .HasColumnType("float");

                    b.Property<double>("TotalPayment")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AddressForeignKey")
                        .IsUnique();

                    b.ToTable("Restaurants");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.RestaurantAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RestaurantAddresses");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.RestaurantEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRestaurateur")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RestaurantId");

                    b.ToTable("RestaurantEmployees");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Rate")
                        .HasColumnType("float");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("WebApiApplication.Database.MenuSection", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.Restaurant", "Restaurant")
                        .WithMany("MenuSections")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Client", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.ClientAddress", "Address")
                        .WithMany("Clients")
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Complaint", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.Client", "Client")
                        .WithMany("Complaints")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("WebApiApplication.Database.POCO.Order", "Order")
                        .WithOne("Complaint")
                        .HasForeignKey("WebApiApplication.Database.POCO.Complaint", "OrderForeignKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.DiscountCode", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.Restaurant", "AppliedToRestaurant")
                        .WithMany("DiscountCodes")
                        .HasForeignKey("AppliedToRestaurantId");

                    b.Navigation("AppliedToRestaurant");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.MenuPosition", b =>
                {
                    b.HasOne("WebApiApplication.Database.MenuSection", "MenuSection")
                        .WithMany("MenuPositions")
                        .HasForeignKey("MenuSectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuSection");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Order", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.ClientAddress", "Address")
                        .WithMany("DeliveredOrders")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiApplication.Database.POCO.Client", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiApplication.Database.POCO.DiscountCode", "DiscountCode")
                        .WithMany("Orders")
                        .HasForeignKey("DiscountCodeId");

                    b.HasOne("WebApiApplication.Database.POCO.Restaurant", "Restaurant")
                        .WithMany("Orders")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Client");

                    b.Navigation("DiscountCode");

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Restaurant", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.RestaurantAddress", "Address")
                        .WithOne("Restaurant")
                        .HasForeignKey("WebApiApplication.Database.POCO.Restaurant", "AddressForeignKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.RestaurantEmployee", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.Restaurant", "Restaurant")
                        .WithMany("RestaurantEmployees")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Review", b =>
                {
                    b.HasOne("WebApiApplication.Database.POCO.Client", "Client")
                        .WithMany("Reviews")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiApplication.Database.POCO.Restaurant", "Restaurant")
                        .WithMany("Reviews")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("WebApiApplication.Database.MenuSection", b =>
                {
                    b.Navigation("MenuPositions");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Client", b =>
                {
                    b.Navigation("Complaints");

                    b.Navigation("Orders");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.ClientAddress", b =>
                {
                    b.Navigation("Clients");

                    b.Navigation("DeliveredOrders");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.DiscountCode", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Order", b =>
                {
                    b.Navigation("Complaint");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.Restaurant", b =>
                {
                    b.Navigation("DiscountCodes");

                    b.Navigation("MenuSections");

                    b.Navigation("Orders");

                    b.Navigation("RestaurantEmployees");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("WebApiApplication.Database.POCO.RestaurantAddress", b =>
                {
                    b.Navigation("Restaurant");
                });
#pragma warning restore 612, 618
        }
    }
}
