using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDTO>();
            CreateMap<AccountForCreationDTO, Account>();
            CreateMap<Event, EventDTO>();
            CreateMap<EventForCreationDTO, Event>();
            CreateMap<EventForUpdateDTO, Event>().ReverseMap();
            CreateMap<AccountForUpdateDTO, Account>().ReverseMap();
        }
    }
}
