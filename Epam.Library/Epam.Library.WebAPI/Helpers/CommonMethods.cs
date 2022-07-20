using AutoMapper;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using Epam.Library.WebAPI.Models;

namespace Epam.Library.WebAPI.Helpers;

public static class CommonMethods
{
    internal static List<PolygraphyReadModel> MapPolygraphyList(IMapper mapper, ILibraryLogic libraryLogic,
        IAuthorLogic authorLogic, List<Polygraphy> polygraphies)
    {
        var mappedPolygraphies = new List<PolygraphyReadModel>();
        foreach (var polygraphy in polygraphies)
        {
            switch (polygraphy)
            {
                case Book book:
                {
                    var mappedBook = mapper.Map<BookReadModel>(book);
                    mappedBook.Authors = authorLogic.GetAuthorsByIds(book.Authors);
                    mappedPolygraphies.Add(mappedBook);
                    break;
                }
                case NewspaperIssue newspaperIssue:
                {
                    var mappedIssue = mapper.Map<NewspaperIssueReadModel>(newspaperIssue);
                    mappedIssue.ISSN = libraryLogic.GetNewspaperById(newspaperIssue.NewspaperId).ISSN;
                    mappedPolygraphies.Add(mappedIssue);
                    break;
                }
                case Patent patent:
                {
                    var mappedPatent = mapper.Map<PatentReadModel>(patent);
                    mappedPatent.Authors = authorLogic.GetAuthorsByIds(patent.Authors);
                    mappedPolygraphies.Add(mappedPatent);
                    break;
                }
            }
        }

        return mappedPolygraphies;
    }
}