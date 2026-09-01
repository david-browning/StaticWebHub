// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Validation;

internal class ContentPageValidator : PageValidator<ContentPage>
{
   protected override async Task<ValidationResult> ValidateTypedPageAsync(
      ContentPage page,
      CancellationToken cancellationToken = default)
   {
      var builder = new ValidationResultBuilder();
      if (string.IsNullOrWhiteSpace(page.RenderedContent))
      {
         builder.AddError(
            ValidationCodes.RequiredFieldMissing,
            $"Page does not contain any rendered content.",
            page.Id);
         // No more validation
         return builder.Build();
      }

      // Get all the nodes in the RenderedContent.
      var nodes = ParseFragment(page.RenderedContent);

      // Flatten the nodes so we can inspect the elements in sequence.
      var elements = GetAllElements(nodes).ToArray();

      var forbidden = elements.Where(e => IsForbiddenElement(e));
      foreach(var element in forbidden)
      {
         builder.AddError(
            ValidationCodes.InvalidFormat,
            $"The HTML content contains the forbidden element \"{element}\".",
            page.Id);
      }

      if(elements.Any(e =>  e.LocalName.Equals("script", StringComparison.OrdinalIgnoreCase)))
      {
         builder.AddWarning(
            ValidationCodes.DiscouragedValue,
            "The rendered HTML contains a script element which is discouraged. " +
            $"Use the \"{nameof(BasicPage.Scripts)}\" property instead.",
            page.Id);
      }

      if (elements.Any(e => e.LocalName.Equals("style", StringComparison.OrdinalIgnoreCase)))
      {
         builder.AddWarning(
            ValidationCodes.DiscouragedValue,
            "The rendered HTML contains a style element which is discouraged. " +
            $"Use the \"{nameof(BasicPage.StyleSheets)}\" property instead.",
            page.Id);
      }

      ValidateImageTags(elements.Where(e => IsImageElement(e)), page, builder);
      ValidateAnchorTags(elements.Where(e => IsAnchorElement(e)), page, builder);

      return builder.Build();
   }

   private IReadOnlyList<INode> ParseFragment(string html)
   {
      var parser = new HtmlParser();
      var document = parser.ParseDocument(
         "<article id=\"content-root\"></article>");
      var root = document.QuerySelector("#content-root") ??
         throw new InvalidOperationException("Could not create content root.");
      return parser.ParseFragment(html, root).ToList();
   }

   private IEnumerable<IElement> GetElementsRecursive(INode node)
   {
      if (node is IElement element)
      {
         yield return element;
      }

      foreach (var child in node.ChildNodes)
      {
         foreach (var descendant in GetElementsRecursive(child))
         {
            yield return descendant;
         }
      }
   }

   private IEnumerable<IElement> GetAllElements(IEnumerable<INode> nodes)
   {
      foreach (var node in nodes)
      {
         foreach (var element in GetElementsRecursive(node))
         {
            yield return element;
         }
      }
   }

   private bool IsForbiddenElement(IElement element)
   {
      return _forbiddenElements.Contains(element.LocalName);
   }

   private bool IsImageElement(IElement element)
   {
      return element.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase);
   }

   private bool IsAnchorElement(IElement element)
   {
      return element.LocalName.Equals("a", StringComparison.OrdinalIgnoreCase);
   }

   /// <summary>
   /// Assume the list of elements only contains image tags.
   /// </summary>
   /// <param name="nodes"></param>
   /// <param name="builder"></param>
   private void ValidateImageTags(
      IEnumerable<IElement> nodes,
      BasicPage page,
      ValidationResultBuilder builder)
   {
      foreach(var node in nodes)
      {
         if(!node.HasAttribute("alt"))
         {
            builder.AddWarning(
               ValidationCodes.DiscouragedValue,
               "Page contains an <img> tag without an alt value.",
               page.Id);
         }
      }
   }

   /// <summary>
   /// Assume the list of elements only contains a tags.
   /// </summary>
   /// <param name="nodes"></param>
   /// <param name="builder"></param>
   private void ValidateAnchorTags(
      IEnumerable<IElement> nodes,
      BasicPage page,
      ValidationResultBuilder builder)
   {
      foreach(var  node in nodes)
      {
        var href = node.GetAttribute("href");
         if(string.IsNullOrWhiteSpace(href))
         {
            builder.AddWarning(
               ValidationCodes.InvalidUrl,
               "Page contains an <a> tag without an href value. This may be intentional.",
               page.Id);
         }
      }
   }

   private static readonly HashSet<string> _forbiddenElements =
      new(StringComparer.OrdinalIgnoreCase)
   {
      // Document-level structure. A content fragment must not define
      // or replace the surrounding HTML document.
      "html",
      "head",
      "body",
      "title",
      "base",
      "meta",
   };
}
