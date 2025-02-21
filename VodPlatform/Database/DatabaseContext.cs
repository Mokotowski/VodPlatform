using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using VodPlatform.Database;

namespace VodPlatform.Database
{
    public class DatabaseContext : IdentityDbContext<UserModel>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<Watchlist> Watchlist { get; set; }
        public DbSet<Watched> Watched { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<PlaylistPermision> PlaylistPermision { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Watchlist>()
                .HasOne(g => g.User)
                .WithMany(u => u.Watchlist)
                .HasForeignKey(f => f.Id_User);

            modelBuilder.Entity<Watched>()
                .HasOne(g => g.User)
                .WithMany(u => u.Watched)
                .HasForeignKey(f => f.Id_User);

            modelBuilder.Entity<Movie>()
                .HasOne(g => g.Series)
                .WithMany(u => u.Movies)
                .HasForeignKey(f => f.Id_Series);

            modelBuilder.Entity<PlaylistPermision>()
                .HasOne(g => g.Series)
                .WithMany(u => u.PlaylistPermision)
                .HasForeignKey(f => f.Id_Series);


            base.OnModelCreating(modelBuilder);
        }
    }
}
