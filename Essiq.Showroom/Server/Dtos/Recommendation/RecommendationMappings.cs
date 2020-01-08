﻿using Essiq.Showroom.Server.Models;

namespace Essiq.Showroom.Server.Dtos
{
    public class RecommendationMappings : AutoMapper.Profile
    {
        public RecommendationMappings()
        {
            CreateMap<RecommendConsultantCommand, ConsultantRecommendation>();
            CreateMap<ShowInterestCommand, ClientConsultantInterest>();
            CreateMap<UpdateInterestCommand, ClientConsultantInterest>();

            CreateMap<ConsultantRecommendation, ClientConsultantRecommendationDto>();
            CreateMap<ClientConsultantInterest, ClientConsultantInterestDto>();
        }
    }
}
