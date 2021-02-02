namespace SportClubsChallenges.Database.Data
{
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Enums;
    using System;
    using System.Linq;

    public class SportClubsChallengesDbContext : DbContext
    {
        public SportClubsChallengesDbContext(DbContextOptions<SportClubsChallengesDbContext> options) : base(options)
        {
        }

        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<AthleteStravaToken> AthleteStravaTokens { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<ClubMember> ClubMembers { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeParticipant> ChallengeParticipants { get; set; }
        public DbSet<ChallengeActivityType> ChallengeActivityTypes { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Challenge>()
                .HasOne(p => p.Owner)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChallengeActivityType>()
                .HasKey(c => new { c.ChallengeId, c.ActivityTypeId });

            modelBuilder.Entity<ChallengeParticipant>()
                .HasKey(c => new { c.ChallengeId, c.AthleteId });

            modelBuilder.Entity<ChallengeParticipant>()
               .HasOne(pt => pt.Challenge)
               .WithMany(p => p.ChallengeParticipants)
               .HasForeignKey(pt => pt.ChallengeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChallengeParticipant>()
               .HasOne(pt => pt.Athlete)
               .WithMany(p => p.ChallengeParticipants)
               .HasForeignKey(pt => pt.AthleteId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClubMember>()
                .HasKey(c => new { c.ClubId, c.AthleteId });

            modelBuilder.Entity<ClubMember>()
               .HasOne(pt => pt.Club)
               .WithMany(p => p.ClubMembers)
               .HasForeignKey(pt => pt.ClubId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClubMember>()
               .HasOne(pt => pt.Athlete)
               .WithMany(p => p.ClubMembers)
               .HasForeignKey(pt => pt.AthleteId)
               .OnDelete(DeleteBehavior.Restrict);

            this.Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityType>().HasData(
                Enum.GetValues(typeof(ActivityTypeEnum))
                    .Cast<ActivityTypeEnum>()
                    .Select(p => new ActivityType { Id = (byte)p, Name = ((ActivityTypeEnum)p).ToString() })
            );

            modelBuilder.Entity<AthleteStravaToken>().HasData(
                new AthleteStravaToken { Id = 1, AccessToken = new Guid().ToString(), RefreshToken = new Guid().ToString(), ExpirationDate = DateTime.Now.AddDays(1) }
            );

            modelBuilder.Entity<Athlete>().HasData(
                new Athlete { Id = 1, FirstName = "John", LastName = "Smith", AthleteStravaTokenId = 1 }
            );

            modelBuilder.Entity<Club>().HasData(
                new Club { Id = 1, Name = "Bike Club", SportType = "Bike"  },
                new Club { Id = 2, Name = "Club for runners", SportType = "Run" }
            );
        }
    }
}
