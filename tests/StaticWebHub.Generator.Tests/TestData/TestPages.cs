// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Tests.TestData;

internal static class TestPages
{
   public static HubPage CreateHubPage(
      string id = "test-hub",
      IReadOnlyList<HubItem>? items = null,
      IReadOnlyList<ScriptLinkReference>? scripts = null,
      IReadOnlyList<StylesheetLinkReference>? styleSheets = null)
   {
      return new HubPage
      {
         Id = id,
         Locale = "en-us",
         Title = "Test Hub",
         Subtitle = "Test hub subtitle",
         Description = "Test hub description",
         Slug = id,
         Order = 0,
         IsPublished = true,
         Scripts = scripts ?? [],
         StyleSheets = styleSheets ?? [],
         Items = items ?? [CreateHubItem()],
      };
   }

   public static HubItem CreateHubItem(
      string title = "Test Item",
      string description = "Test item description",
      string target = "/en-us/target/")
   {
      return new HubItem
      {
         Title = title,
         Description = description,
         Target = target,
         Tags = ["test"],
      };
   }

   public static LauncherPage CreateLauncherPage(
      string id = "test-launcher",
      IReadOnlyList<LauncherActivity>? activities = null,
      IReadOnlyList<string>? instructions = null,
      IReadOnlyList<ScriptLinkReference>? scripts = null,
      IReadOnlyList<StylesheetLinkReference>? styleSheets = null)
   {
      return new LauncherPage
      {
         Id = id,
         Locale = "en-us",
         Title = "Test Launcher",
         Subtitle = "Test launcher subtitle",
         Description = "Test launcher description",
         Slug = id,
         Order = 0,
         IsPublished = true,
         Scripts = scripts ?? [],
         StyleSheets = styleSheets ?? [],
         Instructions = instructions ?? ["Choose an activity."],
         Activities = activities ?? [CreateLauncherActivity()],
      };
   }

   public static LauncherActivity CreateLauncherActivity(
      string id = "activity-1",
      string title = "Test Activity",
      string description = "Test activity description",
      string prompt = "Perform the test activity.",
      string? provider = "Test Provider")
   {
      return new LauncherActivity
      {
         Id = id,
         Title = title,
         Description = description,
         Prompt = prompt,
         Provider = provider,
         Tags = ["test"],
         Tip = "This is test data.",
      };
   }

   public static FormPage CreateFormPage(
      string id = "test-form",
      string submitUrl = "/api/test",
      string submitText = "Submit",
      IReadOnlyList<FormField>? fields = null,
      IReadOnlyList<ScriptLinkReference>? scripts = null,
      IReadOnlyList<StylesheetLinkReference>? styleSheets = null)
   {
      return new FormPage
      {
         Id = id,
         Locale = "en-us",
         Title = "Test Form",
         Subtitle = "Test form subtitle",
         Description = "Test form description",
         Slug = id,
         Order = 0,
         IsPublished = true,
         Scripts = scripts ?? [],
         StyleSheets = styleSheets ?? [],
         SubmitUrl = submitUrl,
         SubmitText = submitText,
         Fields = fields ?? [CreateTextField()],
      };
   }

   public static FormField CreateTextField(
      string id = "name",
      string label = "Name",
      bool required = true,
      string? placeholder = "Enter a name")
   {
      return new FormField
      {
         Id = id,
         Type = FormFieldType.Text,
         Label = label,
         Required = required,
         Placeholder = placeholder,
         HelpText = "Test text field.",
      };
   }

   public static FormField CreateTextareaField(
      string id = "notes",
      string label = "Notes",
      bool required = false)
   {
      return new FormField
      {
         Id = id,
         Type = FormFieldType.Textarea,
         Label = label,
         Required = required,
         Placeholder = "Enter notes",
      };
   }

   public static FormField CreateNumberField(
      string id = "quantity",
      string label = "Quantity",
      double? minimum = 0,
      double? maximum = 100)
   {
      return new FormField
      {
         Id = id,
         Type = FormFieldType.Number,
         Label = label,
         Minimum = minimum,
         Maximum = maximum,
         Placeholder = "0",
      };
   }

   public static FormField CreateSelectField(
      string id = "choice",
      string label = "Choice",
      IReadOnlyList<FormOption>? options = null)
   {
      return new FormField
      {
         Id = id,
         Type = FormFieldType.Select,
         Label = label,
         Options = options ??
         [
            CreateOption("one", "One"),
            CreateOption("two", "Two"),
         ],
      };
   }

   public static FormField CreateRadioField(
      string id = "radio-choice",
      string label = "Radio Choice",
      IReadOnlyList<FormOption>? options = null)
   {
      return new FormField
      {
         Id = id,
         Type = FormFieldType.Radio,
         Label = label,
         Options = options ??
         [
            CreateOption("one", "One"),
            CreateOption("two", "Two"),
         ],
      };
   }

   public static FormField CreateCheckboxField(
      string id = "accept",
      string label = "Accept",
      bool required = false)
   {
      return new FormField
      {
         Id = id,
         Type = FormFieldType.Checkbox,
         Label = label,
         Required = required,
      };
   }

   public static FormOption CreateOption(
      string value = "value",
      string label = "Label")
   {
      return new FormOption
      {
         Value = value,
         Label = label,
      };
   }

   public static ContentPage CreateContentPage(
      string id = "test-content",
      string renderedContent = "<p>Test content.</p>",
      IReadOnlyList<ScriptLinkReference>? scripts = null,
      IReadOnlyList<StylesheetLinkReference>? styleSheets = null)
   {
      return new ContentPage
      {
         Id = id,
         Locale = "en-us",
         Title = "Test Content",
         Subtitle = "Test content subtitle",
         Description = "Test content description",
         Slug = id,
         Order = 0,
         IsPublished = true,
         Scripts = scripts ?? [],
         StyleSheets = styleSheets ?? [],
         RenderedContent = renderedContent,
      };
   }

   public static RedirectPage CreateRedirectPage(
      string id = "test-redirect",
      string destination = "/en-us/target/",
      bool permanent = true)
   {
      return new RedirectPage
      {
         Id = id,
         Locale = "en-us",
         Title = "Test Redirect",
         Subtitle = "Test redirect subtitle",
         Description = "Test redirect description",
         Slug = id,
         Order = 0,
         IsPublished = true,
         Destination = destination,
         Permanent = permanent,
      };
   }

   public static ScriptLinkReference CreateScript(
      string assetKey = "scripts/test.js",
      bool defer = true)
   {
      return new ScriptLinkReference
      {
         AssetKey = assetKey,
         Defer = defer,
      };
   }

   public static StylesheetLinkReference CreateStylesheet(
      string assetKey = "styles/test.css")
   {
      return new StylesheetLinkReference
      {
         AssetKey = assetKey,
      };
   }
}
