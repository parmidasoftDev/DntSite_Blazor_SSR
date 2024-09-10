using DntSite.Web.Features.AppConfigs.Services.Contracts;
using DntSite.Web.Features.Common.RoutingConstants;

namespace DntSite.Web.Features.AppConfigs.Services;

public class AppAntiXssService(
    IAntiXssService antiXssService,
    IAppFoldersService appFoldersService,
    IHttpContextAccessor httpContextAccessor) : IAppAntiXssService
{
    public string GetSanitizedHtml(string? html)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return antiXssService.GetSanitizedHtml(html);
        }

        var baseUrl = httpContext.GetBaseUrl();

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return antiXssService.GetSanitizedHtml(html);
        }

        return antiXssService.GetSanitizedHtml(html, options: new FixRemoteImagesOptions
        {
            OutputImageFolder = appFoldersService.ArticleImagesFolderPath,
            HostUri = httpContext.GetBaseUri(),
            ImageUrlBuilder = savedFileName
                => baseUrl.CombineUrl(
                    $"{ApiUrlsRoutingConstants.File.HttpAny.Image}?name={Uri.EscapeDataString(savedFileName)}")
        });
    }
}