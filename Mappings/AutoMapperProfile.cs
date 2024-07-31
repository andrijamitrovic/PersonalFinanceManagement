using AutoMapper;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.CategoryModels;
using PersonalFinanceManagement.Models.TransactionModels;
using System.Globalization;

namespace PersonalFinanceManagement.Mappings
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<TransactionEntity, Transaction>()
                .ForMember(x => x.Date, y => y.MapFrom(z => z.Date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<Transaction, TransactionEntity>()
                .ForMember(x => x.Date, y => y.MapFrom(z => DateOnly.Parse(z.Date)));
            CreateMap<PagedSortedFilteredList<TransactionEntity>, PagedSortedFilteredList<Transaction>>();
            CreateMap<CategoryEntity, Category>();
            CreateMap<Category, CategoryEntity>();
            CreateMap<TransactionSplit, TransactionSplitEntity>();
            CreateMap<TransactionSplitEntity, TransactionSplit>();
        }
    }
}
