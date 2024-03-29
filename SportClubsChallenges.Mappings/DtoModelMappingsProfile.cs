﻿namespace SportClubsChallenges.Mappings
{
    using System;
    using System.Linq;
    using AutoMapper;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Model.Strava;
    using SportClubsChallenges.Utils.Enums;
    using SportClubsChallenges.Utils.Helpers;

    public class DtoModelMappingsProfile : Profile
    {
        private static TimeZoneInfo cestTimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        public DtoModelMappingsProfile()
        {
            this.CreateMap<Club, ClubDto>()
                .ForMember(dest => dest.ActiveChallengesCount, opt => opt.MapFrom(src => src.Challenges.Count(p => p.IsActive)))
                .ForMember(dest => dest.InactiveChallengesCount, opt => opt.MapFrom(src => src.Challenges.Count(p => !p.IsActive)))
                .ReverseMap()
                .ForMember(dest => dest.IconUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.SportType, opt => opt.Ignore());

            this.CreateMap<Athlete, AthleteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.IconUrlMedium, opt => opt.MapFrom(src => src.IconUrlMedium))
                .ForMember(dest => dest.IconUrlLarge, opt => opt.MapFrom(src => src.IconUrlLarge))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
                .ForMember(dest => dest.FirstLoginDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.FirstLoginDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.LastLoginDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.LastLoginDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.LastSyncDate, opt => opt.MapFrom(src => src.LastSyncDate.HasValue ? TimeZoneInfo.ConvertTime(src.LastSyncDate.Value, cestTimezone).LocalDateTime : (DateTime?) null));

            this.CreateMap<Challenge, ChallengeDetailsDto>()
                .ForMember(dest => dest.CompetitionTypeDescription, opt => opt.MapFrom(src => EnumsHelper.GetEnumDescription((ChallengeCompetitionTypeEnum)src.CompetitionType)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.StartDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.EndDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.CreationDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.EditionDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.EditionDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate.HasValue ? TimeZoneInfo.ConvertTime(src.UpdateDate.Value, cestTimezone).LocalDateTime : (DateTime?)null))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.ClubIconUrl, opt => opt.MapFrom(src => src.Club.IconUrl))
                .ForMember(dest => dest.Club, opt => opt.MapFrom(src => new ClubDto { Id = src.ClubId, Name = src.Club.Name }))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.FirstName + " " + src.Author.LastName))
                .ForMember(dest => dest.ActivityTypes, opt => opt.MapFrom(src => src.ChallengeActivityTypes.Select(p => p.ActivityType.Name)))
                .ForMember(dest => dest.ActivityTypesIds, opt => opt.MapFrom(src => src.ChallengeActivityTypes.Select(p => p.ActivityTypeId)))
                .ForMember(dest => dest.ActivityTypesProfile, opt => opt.MapFrom(src => src.ChallengeActivityTypes.Any() ? ActivityTypeProfileEnum.Custom.ToString() : ActivityTypeProfileEnum.All.ToString()))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ChallengeParticipants.Count));

            this.CreateMap<ChallengeDetailsDto, Challenge>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.StartDate, DateTimeKind.Local)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.EndDate, DateTimeKind.Local)))
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.EditionDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.Club, opt => opt.Ignore());

            this.CreateMap<Challenge, ChallengeOverviewDto>()
                .ForMember(dest => dest.CompetitionTypeDescription, opt => opt.MapFrom(src => EnumsHelper.GetEnumDescription((ChallengeCompetitionTypeEnum) src.CompetitionType)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.StartDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.EndDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ChallengeParticipants.Count));

            this.CreateMap<ChallengeParticipant, ChallengeParticipationDto>()
                .ForMember(dest => dest.ChallengeId, opt => opt.MapFrom(src => src.ChallengeId))
                .ForMember(dest => dest.ChallengeName, opt => opt.MapFrom(src => src.Challenge.Name))
                .ForMember(dest => dest.ChallengeStartDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.Challenge.StartDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.ChallengeEndDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.Challenge.EndDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.CompetitionType, opt => opt.MapFrom(src => src.Challenge.CompetitionType))
                .ForMember(dest => dest.ChallengeCompetitionType, opt => opt.MapFrom(src => (ChallengeCompetitionTypeEnum) src.Challenge.CompetitionType))
                .ForMember(dest => dest.IsChallengeActive, opt => opt.MapFrom(src => src.Challenge.IsActive))
                .ForMember(dest => dest.ChallengeParticipantsCount, opt => opt.MapFrom(src => src.Challenge.ChallengeParticipants.Count))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Challenge.Club.Name))
                .ForMember(dest => dest.ClubIconUrl, opt => opt.MapFrom(src => src.Challenge.Club.IconUrl))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));

            this.CreateMap<ChallengeParticipant, ChallengeRankPositionDto>()
                .ForMember(dest => dest.AthleteId, opt => opt.MapFrom(src => src.AthleteId))
                .ForMember(dest => dest.AthleteName, opt => opt.MapFrom(src => src.Athlete.FirstName + " " + src.Athlete.LastName))
                .ForMember(dest => dest.AthleteIconUrlMedium, opt => opt.MapFrom(src => src.Athlete.IconUrlMedium))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));

            this.CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AthleteName, opt => opt.MapFrom(src => src.Athlete.FirstName + " " + src.Athlete.LastName))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => (ActivityTypeEnum) src.ActivityTypeId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.StartDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.EndDate, cestTimezone).LocalDateTime))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance))
                .ForMember(dest => dest.Elevation, opt => opt.MapFrom(src => src.Elevation))
                .ForMember(dest => dest.Pace, opt => opt.MapFrom(src => src.Pace))
                .ForMember(dest => dest.IsManual, opt => opt.MapFrom(src => src.IsManual))
                .ForMember(dest => dest.IsGps, opt => opt.MapFrom(src => src.IsGps))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));

            this.CreateMap<AthleteStravaToken, StravaToken>()
                .ForMember(dest => dest.DatabaseId, opt => opt.MapFrom(src => src.Id));

            this.CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.CreationDate, cestTimezone).LocalDateTime));
        }
    }
}