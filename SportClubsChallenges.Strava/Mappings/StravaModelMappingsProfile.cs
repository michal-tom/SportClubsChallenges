namespace SportClubsChallenges.Strava.Mappings
{
    using System;
    using AutoMapper;
    using global::Strava.NET.Model;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Enums;
    using SportClubsChallenges.Utils.Helpers;

    public class StravaModelMappingsProfile : Profile
    {
        public StravaModelMappingsProfile()
        {
            this.CreateMap<SummaryActivity, Activity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ActivityTypeId, opt => opt.MapFrom(src => EnumsHelper.GetEnumIdByName<ActivityTypeEnum>(src.Type)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.StartDate.Value, DateTimeKind.Utc)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.StartDate.Value, DateTimeKind.Utc).AddSeconds(src.ElapsedTime.Value)))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.MovingTime ?? 0))
                .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => (int)Math.Round(src.Distance ?? 0)))
                .ForMember(dest => dest.Elevation, opt => opt.MapFrom(src => (int)Math.Round(src.TotalElevationGain ?? 0)))
                .ForMember(dest => dest.Pace, opt => opt.MapFrom(src => src.AverageSpeed ?? 0))
                .ForMember(dest => dest.IsManual, opt => opt.MapFrom(src => src.Manual))
                .ForMember(dest => dest.IsGps, opt => opt.MapFrom(src => src.Map != null && !string.IsNullOrEmpty(src.Map.SummaryPolyline)))
                .ForMember(dest => dest.Athlete, opt => opt.Ignore());
        }
    }
}
