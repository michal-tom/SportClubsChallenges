namespace SportClubsChallenges.Model.Dto
{
    using SportClubsChallenges.Model.Attributes;
    using SportClubsChallenges.Model.Enums;
    using SportClubsChallenges.Utils.Helpers;
    using System;

    public class ChallengeParticipationDto
    {
        public long ChallengeId { get; set; }

        public string ChallengeName { get; set; }

        public DateTime ChallengeStartDate { get; set; }

        public DateTime ChallengeEndDate { get; set; }

        public bool IsChallengeActive { get; set; }

        public ChallengeTypeEnum ChallengeType { get; set; }

        public int ChallengeParticipantsCount { get; set; }

        public string ClubName { get; set; }

        public int Rank { get; set; }

        public int Score { get; set; }

        public string ScoreUnit => EnumsHelper.GetEnumAttribute<UnitAttribute>((ChallengeTypeEnum)this.ChallengeType)?.Unit ?? string.Empty;
    }
}
