namespace SportClubsChallenges.Model.Dto
{
    using System;
    using System.Collections.Generic;
    using SportClubsChallenges.Utils.Attributes;
    using SportClubsChallenges.Utils.Enums;
    using SportClubsChallenges.Utils.Helpers;

    public class ChallengeDetailsDto
    {
        public ChallengeDetailsDto()
        {
            this.IsActive = true;
            this.CompetitionType = (byte) ChallengeCompetitionTypeEnum.Distance;
            this.CompetitionTypeDescription = ChallengeCompetitionTypeEnum.Distance.ToString();
            this.ParticipantsCount = 0;
            this.StartDate = DateTime.Now.Date;
            this.EndDate = DateTime.Now.Date;
            this.ActivityTypesIds = new List<byte>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public long ClubId { get; set; }

        public string ClubName { get; set; }

        public string ClubIconUrl { get; set; }

        public ClubDto Club{ get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte CompetitionType { get; set; }

        public string CompetitionTypeDescription { get; set; }

        public bool PreventManualActivities { get; set; }

        public bool IncludeOnlyGpsActivities { get; set; }

        public long AuthorId { get; set; }

        public string AuthorName { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EditionDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public IList<string> ActivityTypes { get; set; }

        public IList<byte> ActivityTypesIds { get; set; }

        public int ParticipantsCount { get; set; }

        public ChallengeScoreUnit ScoreUnit => EnumsHelper.GetEnumAttribute<UnitAttribute>((ChallengeCompetitionTypeEnum) this.CompetitionType).Unit;
    }
}
