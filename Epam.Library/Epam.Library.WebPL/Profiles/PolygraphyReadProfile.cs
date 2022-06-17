using AutoMapper;
using Epam.Library.Entities;
using Epam.Library.WebPL.Models;

namespace Epam.Library.WebPL.Profiles;

public class PolygraphyReadProfile : Profile
{
    public PolygraphyReadProfile()
    {
        CreateMap<Polygraphy, PolygraphyReadViewModel>();
        CreateMap<Book, BookReadViewModel>().ForMember(book => book.Authors, act => act.Ignore());
        CreateMap<NewspaperIssue, NewspaperIssueReadViewModel>().ForMember(issue => issue.ISSN, act => act.Ignore());
        CreateMap<Patent, PatentReadViewModel>().ForMember(patent => patent.Authors, act => act.Ignore());
    }
}