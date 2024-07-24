using AutoMapper;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Mappings
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<TransactionEntity, Transaction>();
            CreateMap<Transaction, TransactionEntity>();
        }
    }
}
