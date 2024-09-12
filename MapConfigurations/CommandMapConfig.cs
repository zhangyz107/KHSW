using AutoMapper;
using Khsw.Instrument.Demo.DataModels;
using Khsw.Instrument.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.MapConfigurations
{
    public class CommandMapConfig : Profile
    {
        public CommandMapConfig()
        {
            CreateMap<Command,CommandDataModel>();
            CreateMap<Command, CommandDataModel>().ReverseMap();
        }
    }
}
