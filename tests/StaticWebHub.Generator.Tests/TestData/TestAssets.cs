// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System;
using System.Text;
using StaticWebHub.Generator.Content;

namespace StaticWebHub.Generator.Tests.TestData;

internal static class TestAssets
{
   public static StoredAsset CreateTextAsset(
      string assetKey,
      string text,
      string? contentType = null)
   {
      var length = Encoding.UTF8.GetByteCount(text);
      var lastModified =
         new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

      return new StoredAsset
      {
         AssetKey = assetKey,
         ContentType = contentType ?? AssetHelpers.GetContentType(assetKey),
         ContentLength = length,
         Text = text,
         LastModifiedUtc = lastModified,
         EntityTag = AssetHelpers.GetEntityTag(
            assetKey,
            lastModified,
            length),
      };
   }
}
