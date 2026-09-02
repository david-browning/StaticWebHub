// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Tests.TestData;

internal static class TestPageJson
{
   public static string Serialize(BasicPage page)
   {
      return JsonSerializer.Serialize<BasicPage>(
         page,
         CreateOptions());
   }

   public static MemoryStream OpenStream(BasicPage page)
   {
      return OpenStream(Serialize(page));
   }

   public static MemoryStream OpenStream(string json)
   {
      return new MemoryStream(
         Encoding.UTF8.GetBytes(json),
         writable: false);
   }

   private static JsonSerializerOptions CreateOptions()
   {
      var options = new JsonSerializerOptions
      {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         WriteIndented = true,
      };

      options.Converters.Add(
         new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

      return options;
   }
}
