namespace SportClubsChallenges.Model.Dto
{
    using System;
    using SportClubsChallenges.Utils.Enums;

    public class OverallStatsDto
    {
        public byte? PreferedActivityTypeId { get; set; }

        public ActivityTypeEnum? PreferedActivityType => (ActivityTypeEnum?) this.PreferedActivityTypeId;

        public PeriodStatsDto TotalStats { get; set; }

        public PeriodStatsDto YearStats { get; set; }

        public PeriodStatsDto MonthStats { get; set; }

        public PeriodStatsDto WeekStats { get; set; }
    }
}
