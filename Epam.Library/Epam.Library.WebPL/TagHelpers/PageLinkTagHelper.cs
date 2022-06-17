using Epam.Library.WebPL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epam.Library.WebPL.TagHelpers;

public class PageLinkTagHelper : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }
    public PageViewModel PageModel { get; set; }
    public string PageAction { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
        output.TagName = "div";
        
        var tag = new TagBuilder("ul");
        tag.AddCssClass("pagination");
        
        var currentItem = CreateTag(PageModel.PageNumber, urlHelper);
        
        if (PageModel.PageNumber != PageModel.FirstPage)
        {
            var firstItem = CreateTag(PageModel.FirstPage, urlHelper);
            tag.InnerHtml.AppendHtml(firstItem);
        }
        
        if (PageModel.HasPreviousPage)
        {
            var prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
            tag.InnerHtml.AppendHtml(prevItem);
        }
        
        tag.InnerHtml.AppendHtml(currentItem);
        
        if (PageModel.HasNextPage)
        {
            var nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
            tag.InnerHtml.AppendHtml(nextItem);
        }

        if (PageModel.PageNumber != PageModel.TotalPages)
        {
            var lastItem = CreateTag(PageModel.TotalPages, urlHelper);
            tag.InnerHtml.AppendHtml(lastItem);
        }
        output.Content.AppendHtml(tag);
    }

    private TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
    {
        var item = new TagBuilder("li");
        var link = new TagBuilder("a");
        if (pageNumber == PageModel.PageNumber)
        {
            item.AddCssClass("active");
        }
        else
        {
            link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber });
        }
        item.AddCssClass("page-item");
        link.AddCssClass("page-link");
        link.InnerHtml.Append(pageNumber.ToString());
        item.InnerHtml.AppendHtml(link);
        return item;
    }
}