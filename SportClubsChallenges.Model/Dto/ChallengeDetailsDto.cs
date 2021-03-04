﻿namespace SportClubsChallenges.Model.Dto
{
    using SportClubsChallenges.Model.Attributes;
    using SportClubsChallenges.Model.Enums;
    using SportClubsChallenges.Utils.Helpers;
    using System;
    using System.Collections.Generic;

    public class ChallengeDetailsDto
    {
        public ChallengeDetailsDto()
        {
            this.IsActive = true;
            this.ChallengeType = (byte) ChallengeTypeEnum.Distance;
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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte ChallengeType { get; set; }

        public string ChallengeTypeDescription { get; set; }

        public bool PreventManualActivities { get; set; }

        public bool IncludeOnlyGpsActivities { get; set; }

        public long AuthorId { get; set; }

        public string AuthorName { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EditionDate { get; set; }

        public string ActivityTypes { get; set; }

        public IEnumerable<byte> ActivityTypesIds { get; set; }

        public int ParticipantsCount { get; set; }

        public string ScoreUnit => EnumsHelper.GetEnumAttribute<UnitAttribute>((ChallengeTypeEnum) this.ChallengeType)?.Unit ?? string.Empty;
    }
}
