using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Database.POCO;

namespace WebApiApplication.Database
{
    public class FoodDeliveryDbContext : DbContext
    {
        public FoodDeliveryDbContext(DbContextOptions options) : base(options) { }
        public DbSet<ClientAddress> ClientAddresses { get; set; }
        public DbSet<RestaurantAddress> RestaurantAddresses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<MenuPosition> MenuPositions { get; set; }
        public DbSet<MenuSection> MenuSections { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<RestaurantEmployee> RestaurantEmployees { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuSection>()
                .HasOne(ms => ms.Restaurant)
                .WithMany(r => r.MenuSections)
                .IsRequired();

            modelBuilder.Entity<MenuPosition>()
                .HasOne(mp => mp.MenuSection)
                .WithMany(ms => ms.MenuPositions)
                .IsRequired();

            modelBuilder.Entity<RestaurantEmployee>()
                .HasOne(re => re.Restaurant)
                .WithMany(r => r.RestaurantEmployees);
                //.IsRequired();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DiscountCode)
                .WithMany(dc => dc.Orders);

            modelBuilder.Entity<Complaint>()
              .HasOne(c => c.AttendingEmployee)
              .WithMany(re => re.AttendedComplaints);

            modelBuilder.Entity<DiscountCode>()
              .HasOne(dc => dc.AppliedToRestaurant)
              .WithMany(r => r.DiscountCodes);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .IsRequired().OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Order_MenuPosition>()
               .HasOne(omp => omp.MenuPosition)
               .WithMany(mp => mp.OrdersMenuPositions)
               .IsRequired();

            modelBuilder.Entity<Order_MenuPosition>()
              .HasOne(omp => omp.Order)
              .WithMany(mp => mp.OrdersMenuPositions)
              .IsRequired();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Restaurant)
                .WithMany(r => r.Reviews)
                .IsRequired();

            modelBuilder.Entity<RestaurantAddress>()
                .HasOne(ra => ra.Restaurant)
                .WithOne(r => r.Address)
                .HasForeignKey<Restaurant>(r => r.AddressForeignKey)
                .IsRequired();

            // ONE TO ONE RELATION
            //modelBuilder.Entity< Obiekt ktory ma byc kluczem obcym [typ] >()
            //    .HasOne(ra => ra. Obiekt ktory ma miec klucz obcy )
            //    .WithOne(r => r. Obiekt ktory ma byc kluczem obcym )
            //    .HasForeignKey< Obiekt który ma miec klucz obcy [typ] >(r => r. properta w obiekcie (który ma klucz obcy) bedaca kluczem obcym)
            //    .IsRequired(); <----- oznacza ze klucz obcy w obiekcie ktory ma klucz obcy nie moze byc null

            modelBuilder.Entity<Order>()
               .HasOne(o => o.Complaint)
               .WithOne(c => c.Order)
               .HasForeignKey<Complaint>(c => c.OrderForeignKey)
               .IsRequired();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany(a => a.DeliveredOrders)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ResponsibleEmployee)
                .WithMany(re => re.ResponsibleForOrders)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Reviews)
                .IsRequired();

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Complaints)
                .IsRequired().OnDelete(DeleteBehavior.ClientNoAction); //TODO: Test case: Remove client. Then should happen: Remove order, remove complaint. 

            modelBuilder.Entity<Client>()
                .HasOne(c => c.Address)
                .WithMany(ca => ca.Clients);

            modelBuilder.Entity<Client>()
               .HasMany(c => c.FavouriteRestaurants)
               .WithMany(r => r.FavouriteForClients)
               .UsingEntity(j => j.ToTable("Clients_FavouriteRestaurants"));

            // CONVERSIONS
            modelBuilder.Entity<Order>()
               .Property(m => m.OrderState)
               .HasConversion(
                   v => v.ToString(),
                   v => (OrderState)Enum.Parse(typeof(OrderState), v));

            modelBuilder.Entity<Order>()
              .Property(m => m.PaymentMethod)
              .HasConversion(
                  v => v.ToString(),
                  v => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), v));

            modelBuilder.Entity<Restaurant>()
             .Property(m => m.State)
             .HasConversion(
                 v => v.ToString(),
                 v => (RestaurantState)Enum.Parse(typeof(RestaurantState), v));

            // UNIQUE CONSTRAINTS (INDEXES)
            modelBuilder.Entity<Client>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<RestaurantEmployee>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Admin>()
              .HasIndex(u => u.Email)
              .IsUnique();
        }

    }
}
