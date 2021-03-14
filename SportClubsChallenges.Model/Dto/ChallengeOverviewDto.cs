namespace SportClubsChallenges.Model.Dto
{
    using System;

    public class ChallengeOverviewDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string ClubName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte RivalryType { get; set; }

        public string RivalryTypeDescription { get; set; }

        public int ParticipantsCount { get; set; }

        public bool IsAthleteRegistred { get; set; }
    }
}