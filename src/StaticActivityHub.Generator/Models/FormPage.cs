// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StaticActivityHub.Generator.Models;

internal sealed class FormPage : BasicPage
{
   public override PageType PageType => PageType.Form;

   public required string SubmitUrl { get; init; }

   public required string SubmitText { get; init; }

   public required IReadOnlyList<FormField> Fields { get; init; }
}

internal sealed class FormField
{
   public required string Id { get; init; }

   public required FormFieldType Type { get; init; }

   public required string Label { get; init; }

   public string? HelpText { get; init; }

   public bool Required { get; init; }

   public string? Placeholder { get; init; }

   public double? Minimum { get; init; }

   public double? Maximum { get; init; }

   public IReadOnlyList<FormOption> Options { get; init; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter<FormFieldType>))]
public enum FormFieldType
{
   Text,
   Textarea,
   Number,
   Select,
   Checkbox,
   Radio
}

internal sealed class FormOption
{
   public required string Value { get; init; }

   public required string Label { get; init; }
}
