namespace SportClubsChallenges.Mappings
{
    using AutoMapper;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Model.Enums;
    using SportClubsChallenges.Strava.Model;
    using SportClubsChallenges.Utils.Helpers;
    using System.Linq;

    public class DtoModelMappingsProfile : Profile
    {
        public DtoModelMappingsProfile()
        {
            this.CreateMap<Club, ClubDto>();

            this.CreateMap<Challenge, ChallengeDetailsDto>()
                .ForMember(dest => dest.ChallengeTypeDescription, opt => opt.MapFrom(src => EnumsHelper.GetEnumDescription((ChallengeTypeEnum) src.ChallengeType)))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FirstName + " " + src.Owner.LastName))
                .ForMember(dest => dest.ActivityTypes, opt => opt.MapFrom(src => string.Join(",", src.ChallengeActivityTypes.Select(p => p.ActivityType.Name))))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ChallengeParticipants.Count))
                .ReverseMap();

            this.CreateMap<Challenge, ChallengeOverviewDto>()
                .ForMember(dest => dest.ChallengeTypeDescription, opt => opt.MapFrom(src => EnumsHelper.GetEnumDescription((ChallengeTypeEnum) src.ChallengeType)))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ChallengeParticipants.Count));

            this.CreateMap<ChallengeParticipant, ChallengeParticipationDto>()
                .ForMember(dest => dest.ChallengeId, opt => opt.MapFrom(src => src.ChallengeId))
                .ForMember(dest => dest.ChallengeName, opt => opt.MapFrom(src => src.Challenge.Name))
                .ForMember(dest => dest.ChallengeStartDate, opt => opt.MapFrom(src => src.Challenge.StartDate))
                .ForMember(dest => dest.ChallengeEndDate, opt => opt.MapFrom(src => src.Challenge.EndDate))
                .ForMember(dest => dest.ChallengeType, opt => opt.MapFrom(src => (ChallengeTypeEnum) src.Challenge.ChallengeType))
                .ForMember(dest => dest.IsChallengeActive, opt => opt.MapFrom(src => src.Challenge.IsActive))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Challenge.Club.Name))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));

            this.CreateMap<ChallengeParticipant, ChallengeRankPositionDto>()
                .ForMember(dest => dest.AthleteId, opt => opt.MapFrom(src => src.AthleteId))
                .ForMember(dest => dest.AthleteName, opt => opt.MapFrom(src => src.Athlete.FirstName + " " + src.Athlete.LastName))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));

            this.CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => (ActivityTypeEnum) src.ActivityTypeId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.LocalDateTime))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.LocalDateTime))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance))
                .ForMember(dest => dest.Elevation, opt => opt.MapFrom(src => src.Elevation))
                .ForMember(dest => dest.Pace, opt => opt.MapFrom(src => src.Pace))
                .ForMember(dest => dest.IsManual, opt => opt.MapFrom(src => src.IsManual))
                .ForMember(dest => dest.IsGps, opt => opt.MapFrom(src => src.IsGps));

            this.CreateMap<AthleteStravaToken, StravaToken>()
                .ForMember(dest => dest.DatabaseId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
