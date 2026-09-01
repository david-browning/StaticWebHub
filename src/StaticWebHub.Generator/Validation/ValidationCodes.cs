// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;

namespace StaticWebHub.Generator.Validation;

/// <summary>
/// Defines stable diagnostic codes and their general descriptions.
/// Validation code meanings should not be changed after they are published.
/// </summary>
internal static class ValidationCodes
{
   // A required property, value, or field was not supplied.
   public const string RequiredFieldMissing = "SWH1001";

   // A supplied string is empty or whitespace when a value is required.
   public const string RequiredValueEmpty = "SWH1002";

   // A supplied value is not valid for the property or operation.
   public const string InvalidValue = "SWH1003";

   // A value does not match the expected format.
   public const string InvalidFormat = "SWH1004";

   // A numeric or comparable value falls outside its permitted range.
   public const string ValueOutOfRange = "SWH1005";

   // A collection contains a value that must be unique more than once.
   public const string DuplicateValue = "SWH1006";

   // A collection that must contain one or more items is empty.
   public const string RequiredCollectionEmpty = "SWH1007";

   // Two or more supplied values conflict with one another.
   public const string ConflictingValues = "SWH1008";

   // A referenced asset does not exist in the configured content store.
   public const string AssetNotFound = "SWH1101";

   // A referenced asset exists but is not the expected content type.
   public const string InvalidAssetContentType = "SWH1102";

   // An asset key or local resource path is not valid.
   public const string InvalidAssetKey = "SWH1103";

   // An asset exists but could not be successfully read.
   public const string AssetReadFailed = "SWH1104";

   // The same asset was referenced more than once where duplicates are unnecessary.
   public const string DuplicateAssetReference = "SWH1105";

   // A reference points to another page, item, or resource that cannot be resolved.
   public const string ReferenceNotFound = "SWH1201";

   // A URL or route value is malformed or otherwise invalid.
   public const string InvalidUrl = "SWH1202";

   // A page slug, route, or other generated path conflicts with another output.
   public const string DuplicateOutputPath = "SWH1203";

   // A supplied identifier is invalid for use as an application identifier.
   public const string InvalidIdentifier = "SWH1204";

   // A value is valid but discouraged and may produce undesirable output.
   public const string DiscouragedValue = "SWH2001";

   // A property or configuration value is accepted but has no effect.
   public const string UnusedValue = "SWH2002";

   // A configuration works but relies on behavior that may be ambiguous or surprising.
   public const string AmbiguousConfiguration = "SWH2003";

   // The file will be skipped.
   public const string NotPublished = "SWH9001";

   public static IReadOnlyDictionary<string, string> Messages { get; } =
      new Dictionary<string, string>(StringComparer.Ordinal)
      {
         [RequiredFieldMissing] =
            "A required field is missing.",

         [RequiredValueEmpty] =
            "A required value is empty.",

         [InvalidValue] =
            "A supplied value is invalid.",

         [InvalidFormat] =
            "A supplied value has an invalid format.",

         [ValueOutOfRange] =
            "A supplied value is outside the permitted range.",

         [DuplicateValue] =
            "A value that must be unique appears more than once.",

         [RequiredCollectionEmpty] =
            "A required collection does not contain any items.",

         [ConflictingValues] =
            "Two or more supplied values conflict with one another.",

         [AssetNotFound] =
            "A referenced asset could not be found.",

         [InvalidAssetContentType] =
            "A referenced asset has an unexpected content type.",

         [InvalidAssetKey] =
            "An asset key is invalid.",

         [AssetReadFailed] =
            "A referenced asset could not be read.",

         [DuplicateAssetReference] =
            "An asset is referenced more than once.",

         [ReferenceNotFound] =
            "A referenced item could not be found.",

         [InvalidUrl] =
            "A URL or route is invalid.",

         [DuplicateOutputPath] =
            "More than one item resolves to the same output path.",

         [InvalidIdentifier] =
            "An identifier is invalid.",

         [DiscouragedValue] =
            "A supplied value is valid but discouraged.",

         [UnusedValue] =
            "A supplied value has no effect.",

         [AmbiguousConfiguration] =
            "The configuration is valid but ambiguous.",

         [NotPublished] = 
            "The page will not be published.",
      };

   public static string GetMessage(string code)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(code);

      if (!Messages.TryGetValue(code, out var message))
      {
         throw new ArgumentException(
            $"Unknown validation code '{code}'.", nameof(code));
      }

      return message;
   }
}