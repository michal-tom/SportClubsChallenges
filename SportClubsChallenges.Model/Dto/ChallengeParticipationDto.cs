﻿namespace SportClubsChallenges.Model.Dto
{
    using SportClubsChallenges.Utils.Attributes;
    using SportClubsChallenges.Utils.Enums;
    using SportClubsChallenges.Utils.Helpers;
    using System;

    public class ChallengeParticipationDto
    {
        public long ChallengeId { get; set; }

        public string ChallengeName { get; set; }

        public DateTime ChallengeStartDate { get; set; }

        public DateTime ChallengeEndDate { get; set; }

        public bool IsChallengeActive { get; set; }

        public byte CompetitionType { get; set; }

        public ChallengeCompetitionTypeEnum ChallengeCompetitionType { get; set; }

        public int ChallengeParticipantsCount { get; set; }

        public string ClubName { get; set; }

        public string ClubIconUrl { get; set; }

        public int Rank { get; set; }

        public int Score { get; set; }

        public ChallengeStatusEnum ChallengeStatus =>
            this.ChallengeStartDate.Date > DateTime.Now.Date ? ChallengeStatusEnum.Upcoming :
            this.ChallengeEndDate.Date < DateTime.Now.Date ? ChallengeStatusEnum.Finished :
            !this.IsChallengeActive ? ChallengeStatusEnum.Inactive : ChallengeStatusEnum.Active;

        public ChallengeScoreUnit ScoreUnit => EnumsHelper.GetEnumAttribute<UnitAttribute>((ChallengeCompetitionTypeEnum) this.ChallengeCompetitionType).Unit;
    }
}