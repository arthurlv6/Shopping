using AutoMapper;

namespace Shared
{
    public class ModelProfile:Profile
    {
        public ModelProfile()
        {
            CreateMap<Stock, StockDto>().ReverseMap();
        }
    }
}
