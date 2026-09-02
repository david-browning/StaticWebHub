# StaticWebHub.Generator.Tests skeleton

Copy the contents of this directory over the existing empty MSTest project named
`StaticWebHub.Generator.Tests`.

## One-time setup

Keep the `.csproj` created by Visual Studio/MSTest. Make sure it targets `net10.0`
and references the generator project:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\StaticWebHub.Generator\StaticWebHub.Generator.csproj" />
</ItemGroup>
```

Because the generator intentionally keeps most implementation types `internal`,
grant only the test assembly friend access. Add this to
`src/StaticWebHub.Generator/StaticWebHub.Generator.csproj`:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="StaticWebHub.Generator.Tests" />
</ItemGroup>
```

Delete the template `UnitTest1.cs` after copying this skeleton.

## Layout

- `Validation/` - validators and diagnostics.
- `Serializing/` - JSON/page deserialization.
- `Rendering/` - page and asset rendering.
- `Content/` - content stores, asset helpers, and asset resolution.
- `IO/` - filesystem page source/writer behavior.
- `Generation/` - `SiteGenerator` orchestration.
- `Cli/` - a small set of true command-line/exit-code tests.
- `TestData/` - fresh valid page-model factories and JSON helpers.
- `Infrastructure/` - reusable test-only stores, temporary directories, assertions,
  and temporary site projects.

## TestPages convention

Every `TestPages` factory returns a fresh valid model. Do not expose shared static
model instances, because a test should never be able to contaminate another test.

Example:

```csharp
var page = TestPages.CreateFormPage(
   fields:
   [
      TestPages.CreateTextField(id: "duplicate"),
      TestPages.CreateTextField(id: "duplicate", label: "Other")
   ]);
```

## Suggested testing boundary

1. Unit-test validators and helpers directly.
2. Use `InMemoryContentStore` unless filesystem behavior is what you are testing.
3. Use `TemporaryDirectory` / `TestSiteProject` for filesystem integration tests.
4. Test `SiteGenerator` in-process.
5. Keep CLI tests limited to argument parsing, exit codes, console behavior,
   and a few final end-to-end cases.

Two validator classes contain starter tests. The rest are intentionally skeletons
with suggested first cases in comments.
