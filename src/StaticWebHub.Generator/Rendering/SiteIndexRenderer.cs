using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Razor.Templating.Core;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Rendering;

internal sealed class SiteIndexRenderer
{
   public Task<string> GenerateAsync(
     SiteConfiguration configuration,
     CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(configuration);
      cancellationToken.ThrowIfCancellationRequested();
      var model = new SiteIndexViewModel
      {
         Title = configuration.Title,

         Locales = configuration.Locales.Select(locale => new SiteIndexLocale
         {
            DisplayName = locale.DisplayName,
            Url = GetLocaleHomeUrl(locale),
            IsDefault = string.Equals(
               locale.Code, configuration.DefaultLocale,
               StringComparison.OrdinalIgnoreCase)
         }).ToArray()
      };

      return RazorTemplateEngine.RenderAsync("/Views/SiteIndex.cshtml", model);
   }

   private static string GetLocaleHomeUrl(SiteLocale locale)
   {
      return $"/{locale.Code}/{locale.HomePage}/";
   }
}
