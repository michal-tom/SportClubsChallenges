﻿namespace SportClubsChallenges.Domain.Mappings
{
    using AutoMapper;
    using SportClubsChallenges.Database.Entities;
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

            this.CreateMap<Challenge, ChallengeDto>()
                .ForMember(dest => dest.ChallengeTypeDescription, opt => opt.MapFrom(src => EnumsHelper.GetEnumDescription((ChallengeTypeEnum) src.ChallengeType)))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FirstName + " " + src.Owner.LastName))
                .ForMember(dest => dest.ActivityTypes, opt => opt.MapFrom(src => string.Join(",", src.ChallengeActivityTypes.Select(p => p.ActivityType.Name))))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ChallengeParticipants.Count))
                .ReverseMap();

            this.CreateMap<AthleteStravaToken, StravaToken>();
        }
    }
}
