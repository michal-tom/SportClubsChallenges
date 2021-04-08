namespace SportClubsChallenges.Database.Data
{
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Utils.Enums;
    using System;
    using System.Linq;

    internal static class SportClubsChallengesDbInitializer
    {
        internal static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityType>().HasData(
                Enum.GetValues(typeof(ActivityTypeEnum))
                    .Cast<ActivityTypeEnum>()
                    .Select(p => new ActivityType { Id = (byte) p, Name = p.ToString() })
            );
        }
    }
}