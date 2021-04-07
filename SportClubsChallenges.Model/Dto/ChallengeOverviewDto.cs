namespace SportClubsChallenges.Model.Dto
{
    using System;
    using SportClubsChallenges.Utils.Enums;

    public class ChallengeOverviewDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string ClubName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte CompetitionType { get; set; }

        public string CompetitionTypeDescription { get; set; }

        public int ParticipantsCount { get; set; }

        public bool IsAthleteRegistred { get; set; }

        public ChallengeStatusEnum Status => 
            this.StartDate.Date > DateTime.Now.Date ? ChallengeStatusEnum.Upcoming :
            this.EndDate.Date < DateTime.Now.Date ? ChallengeStatusEnum.Finished :
            !this.IsActive ? ChallengeStatusEnum.Inactive : ChallengeStatusEnum.Active;
    }
}