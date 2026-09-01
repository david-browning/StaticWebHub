// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StaticWebHub.Generator.Models;

namespace StaticWebHub.Generator.Serializing;

internal interface IPageDeserializer
{
   Task<BasicPage> DeserializeAsync(
      Stream stream,
      CancellationToken cancellationToken = default);
}
