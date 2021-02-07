namespace SportClubsChallenges.Domain.Mappings
{
    using AutoMapper;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Attributes;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Model.Enums;
    using SportClubsChallenges.Strava.Model;
    using SportClubsChallenges.Utils.Helpers;
    using System.Linq;

    public class ModelMappingsProfile : Profile
    {
        public ModelMappingsProfile()
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

            this.CreateMap<AthleteStravaToken, StravaToken>();
        }
    }
}
