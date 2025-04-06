using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Utility
{
    public static class RazorPartialToString
    {
        public static async Task<string> RenderPartialViewToString<TModel>(
            PageModel pageModel,
            string partialViewName,
            TModel model)
        {
            var httpContext = pageModel.HttpContext;
            var serviceProvider = httpContext.RequestServices;

            var viewEngine = serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();
            var actionContext = new ActionContext(httpContext, pageModel.RouteData, pageModel.PageContext.ActionDescriptor);

            using var output = new StringWriter();

            var viewResult = viewEngine.FindView(actionContext, partialViewName, false);

            if (viewResult.View == null)
            {
                throw new InvalidOperationException($"View '{partialViewName}' not found.");
            }

            var viewData = new ViewDataDictionary<TModel>(
                metadataProvider: pageModel.MetadataProvider,
                modelState: pageModel.ModelState)
            {
                Model = model
            };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                new TempDataDictionary(httpContext, tempDataProvider),
                output,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return output.ToString();
        }
    }
}
