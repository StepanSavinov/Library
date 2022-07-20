using AutoMapper;
using Epam.Library.Entities;
using Epam.Library.WebAPI.Models;

namespace Epam.Library.WebAPI.Profiles;

public class PolygraphyReadProfile : Profile
{
    public PolygraphyReadProfile()
    {
        CreateMap<Polygraphy, PolygraphyReadModel>();
        CreateMap<Book, BookReadModel>().ForMember(book => book.Authors, act => act.Ignore());
        CreateMap<NewspaperIssue, NewspaperIssueReadModel>().ForMember(issue => issue.ISSN, act => act.Ignore());
        CreateMap<Patent, PatentReadModel>().ForMember(patent => patent.Authors, act => act.Ignore());
    }
}