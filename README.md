# StaticWebHub

StaticWebHub is a small reference implementation and starter project for generating a static website from JSON page definitions and Razor templates.

The project demonstrates how to:

- define site pages as structured JSON;
- render several reusable page types with Razor;
- organize pages by locale;
- generate a root language-selection page from site configuration;
- inject HTML, plain-text, and Markdown assets into page content;
- generate forms that POST JSON to server-side endpoints;
- add optional Azure Functions for dynamic behavior;
- test the static site and Functions together locally with the Azure Static Web Apps CLI; and
- build and deploy the generated site with GitHub Actions and Azure Static Web Apps.

The output of the generator is an ordinary directory of HTML, CSS, JavaScript, and other static files. Azure is used by the sample project as a hosting and serverless API platform, but the static generator itself produces normal static files.

## Project structure

```text
StaticWebHub/
├── .github/
│   └── workflows/
│       └── azure-static-web-apps-*.yml
│
├── api/
│   ├── StaticActivityHub.Api/
│   │   ├── Functions/
│   │   ├── Program.cs
│   │   └── StaticActivityHub.Api.csproj
│   └── Test-Api.ps1
│
├── assets/
│   ├── content/
│   ├── site.css
│   └── site.js
│
├── content/
│   └── en-us/
│       ├── activities.page.json
│       ├── index.page.json
│       ├── information.page.json
│       ├── markdown.page.json
│       ├── old-information.page.json
│       ├── request-builder.page.json
│       └── unpublished-test.page.json
│
├── src/
│   └── StaticActivityHub.Generator/
│       ├── Content/
│       ├── Generation/
│       ├── Models/
│       ├── Views/
│       └── Program.cs
│
├── site.json
├── package.json
├── StaticWebHub.slnx
└── LICENSE
```

`dist/` is generated output and is excluded from source control.

## Prerequisites

To build the current sample project you need:

- the .NET 10 SDK for the static-site generator;
- the .NET 9 SDK/runtime for the sample Azure Functions project;
- Node.js and npm for the Azure Static Web Apps CLI; and
- Azure Functions Core Tools v4 when running the Functions project locally.

Visual Studio is optional, but the solution can be opened with `StaticWebHub.slnx`.

Python is optional. The included `serve.cmd` uses Python's simple HTTP server for static-only testing.

## Getting started

Clone the repository and restore the dependencies:

```powershell
git clone https://github.com/david-browning/StaticWebHub.git
cd StaticWebHub

dotnet restore
npm install
```

Generate the site:

```powershell
dotnet run `
    --project src/StaticActivityHub.Generator `
    -- `
    --root . `
    --clean
```

The generated website is written to:

```text
dist/
```

A typical build contains:

```text
dist/
├── index.html
├── assets/
│   ├── site.css
│   ├── site.js
│   └── ...
│
└── en-us/
    ├── index/
    │   └── index.html
    ├── activities/
    │   └── index.html
    ├── information/
    │   └── index.html
    └── ...
```

The root `index.html` is generated from `site.json` and provides links to the configured locale home pages.

## Generator command line

The generator currently supports the following options:

| Option | Short | Default | Description |
| --- | --- | --- | --- |
| `--root` | `-r` | Required | Project root directory |
| `--content` | `-c` | `content` | Content directory relative to the root |
| `--assets` | `-a` | `assets` | Asset directory relative to the root |
| `--output` | `-o` | `dist` | Generated output directory |
| `--page-pattern` | `-p` | `*.page.json` | Search pattern used to discover page definitions |
| `--clean` | | `false` | Deletes the output directory before generation |
| `--verbose` | `-v` | `false` | Writes additional generation information |

For example:

```powershell
dotnet run `
    --project src/StaticActivityHub.Generator `
    -- `
    --root . `
    --output dist `
    --clean `
    --verbose
```

## Site configuration

Site-wide configuration is stored in `site.json`.

The current sample has this structure:

```json
{
  "title": "Example Learning Hub",
  "defaultLocale": "en-us",
  "locales": [
    {
      "code": "en-us",
      "displayName": "English",
      "homePage": "index"
    },
    {
      "code": "es-419",
      "displayName": "Español",
      "homePage": "index"
    },
    {
      "code": "fr-002",
      "displayName": "Français",
      "homePage": "index"
    }
  ]
}
```

The generator uses this file to create the root `dist/index.html` language-selection page.

A locale home page is linked as:

```text
/{locale-code}/{home-page}/
```

For example:

```text
/en-us/index/
```

The repository currently includes sample page definitions only for `en-us`. Add corresponding content for other configured locales, or remove unused locales from `site.json`.

## Page definitions

Pages are JSON files matching:

```text
*.page.json
```

The default filesystem source searches the content directory recursively.

Every page derives from a common page model with properties including:

```json
{
  "viewType": "content",
  "id": "example",
  "locale": "en-us",
  "title": "Example Page",
  "subtitle": "Optional subtitle",
  "description": "Optional description",
  "slug": "optional-custom-slug",
  "order": 10,
  "isPublished": true
}
```

`viewType` is the polymorphic JSON discriminator that determines which page model and Razor view are used.

The current supported values are:

| `viewType` | Purpose |
| --- | --- |
| `hub` | A collection of navigation cards |
| `launcher` | A collection of activities and prompts |
| `form` | A generated form that can POST JSON |
| `content` | General rendered content |
| `redirect` | A client-side redirect page |

Pages with:

```json
"isPublished": false
```

are skipped during generation.

If `slug` is not supplied, the page `id` is used for its output path.

For example:

```json
{
  "id": "information",
  "locale": "en-us"
}
```

is written to:

```text
dist/en-us/information/index.html
```

`Order` currently exists on the page model but is not used by the generation pipeline to sort pages.

## Hub pages

A Hub page contains a collection of navigation items.

Each item can specify:

- a title;
- description;
- destination;
- optional icon; and
- optional tags.

See:

```text
content/en-us/index.page.json
```

for a complete example.

## Launcher pages

Launcher pages provide reusable activity cards containing prompts.

An activity can contain:

- an ID;
- title;
- description;
- tags;
- an optional tip;
- a prompt; and
- an optional provider label.

The included JavaScript provides copy-to-clipboard behavior for generated prompts.

See:

```text
content/en-us/activities.page.json
```

for an example.

## Form pages

Form pages generate HTML forms from JSON definitions.

The current field types are:

- `text`
- `textarea`
- `number`
- `select`
- `checkbox`
- `radio`

A Form page also declares its POST endpoint:

```json
{
  "submitUrl": "/api/jsonrequest",
  "submitText": "Submit Sample Request"
}
```

The generated page uses `assets/site.js` to:

1. collect the form values;
2. convert number inputs to JavaScript numbers;
3. convert checkboxes to booleans;
4. serialize the data as JSON;
5. POST the JSON to the configured endpoint; and
6. display the returned response.

See:

```text
content/en-us/request-builder.page.json
```

for a sample containing every currently supported field type.

## Content pages and asset injection

Content pages expose a `renderedContent` property.

Content can be written directly into the page JSON, or it can reference a file from the configured assets directory.

For example:

```json
{
  "renderedContent": "{{content/markdown-page.md}}"
}
```

resolves:

```text
assets/content/markdown-page.md
```

during site generation.

Asset references use:

```text
{{asset-key}}
```

The asset resolver walks the parsed JSON tree recursively, so asset references can occur in string values rather than being limited specifically to `renderedContent`.

The current asset renderer supports:

| Type | Behavior |
| --- | --- |
| `.html` | Injected as HTML |
| `.txt` | HTML-encoded and converted into paragraphs |
| `.md`, `.markdown` | Converted to HTML with Markdig |

Markdown rendering uses Markdig's advanced extensions with embedded raw HTML disabled.

The source assets directory is also copied into:

```text
dist/assets/
```

during generation.

### Trusted HTML

HTML assets are currently treated as trusted content and are not sanitized before being injected. Do not use untrusted HTML as source content without adding appropriate sanitization.

Image rendering through the `{{...}}` asset-injection pipeline is not currently implemented. Image and other static files can still be copied into `dist/assets` and referenced normally by the generated site.

## Redirect pages

Redirect pages specify a destination:

```json
{
  "viewType": "redirect",
  "destination": "/en-us/information/",
  "permanent": true
}
```

The current Razor implementation produces an HTML page using a zero-delay meta refresh and a normal fallback link.

The `permanent` value is displayed by the sample view, but it does not currently produce an HTTP `301` or `308` response because the generated page is static HTML.

## Local development

### Static site and Functions together

Generate `dist` first, then run:

```powershell
npm run serve
```

The npm script runs the Azure Static Web Apps CLI against:

```text
./dist
```

and the sample Function project:

```text
./api/StaticActivityHub.Api
```

This gives the static frontend and `/api/...` Functions a common local origin similar to an Azure Static Web Apps deployment.

### Debug the Functions project in Visual Studio

Start `StaticActivityHub.Api` under the debugger so that the local Functions host is available at:

```text
http://localhost:7071
```

Then run:

```powershell
npm run serve:debug
```

This starts the Static Web Apps CLI and proxies API requests to the already-running Functions host.

The generated pages can continue to use URLs such as:

```text
/api/jsonrequest
```

without embedding a development-only hostname.

### Static-only preview

For a very simple static preview on Windows:

```powershell
.\serve.cmd
```

This requires Python and serves `dist` at:

```text
http://localhost:8765/
```

This mode serves only static files. It does not proxy requests to the Azure Functions project.

## Sample Azure Functions

The included Functions project is:

```text
api/StaticActivityHub.Api
```

It currently contains two example HTTP-triggered Functions.

### `JsonRequest`

```text
POST /api/jsonrequest
```

This endpoint accepts arbitrary JSON and responds with an object containing:

- `accepted`;
- a sample message; and
- the received JSON payload.

The sample Form page posts to this endpoint.

### `TestFunction`

```text
GET or POST /api/test
```

This is a small test Function that returns a basic success message.

The repository also contains:

```text
api/Test-Api.ps1
```

for exercising the sample endpoints against a local Function host.

By default:

```powershell
.\api\Test-Api.ps1
```

uses:

```text
http://localhost:7071/api
```

A different base URL can be supplied with `-BaseUrl`.

## Generation pipeline

At a high level, normal pages pass through:

```text
*.page.json
     │
     ▼
FileSystemPageSource
     │
     ▼
BasicPageDeserializer
     │
     ▼
BasicPageAssetResolver
     │
     ▼
BasicPage
     │
     ▼
BasicPageRenderer
     │
     ▼
Razor view
     │
     ▼
HTML string
     │
     ▼
FileSystemPageWriter
     │
     ▼
dist/{locale}/{slug}/index.html
```

Site-level generation separately creates:

```text
site.json
    │
    ▼
SiteIndexRenderer
    │
    ▼
Views/SiteIndex.cshtml
    │
    ▼
dist/index.html
```

Static assets are copied into `dist/assets`.

## Extending the generator

The project separates several parts of the generation pipeline behind interfaces, including:

- `IPageSource`
- `IPageDeserializer`
- `IPageAssetResolver`
- `IAssetRenderer`
- `IPageRenderer`
- `IPageWriter`
- `IContentStore`

The current application wires these to filesystem-based and Razor-based implementations in `Program.cs`.

This allows the sample to be extended with alternative content sources, writers, renderers, or asset stores without changing the page-generation pipeline itself.

New page types require corresponding model, polymorphic JSON registration, and rendering support.

## Azure deployment

The repository includes a GitHub Actions workflow for Azure Static Web Apps.

For pushes to `main`, the workflow:

1. checks out the repository;
2. installs the .NET 10 SDK;
3. restores NuGet packages;
4. runs the static-site generator with `--clean`;
5. deploys the already-generated `dist` directory; and
6. builds and deploys the Azure Functions project.

The frontend deployment sets:

```yaml
app_location: "dist"
output_location: ""
skip_app_build: true
```

because StaticWebHub itself has already generated the complete static frontend.

The API source is supplied from:

```yaml
api_location: "api/StaticActivityHub.Api"
```

Pull requests also trigger deployments. Azure Static Web Apps can use those workflow runs as temporary preview environments, which are closed when the pull request closes.

### Using the repository as a template

The checked-in workflow is currently connected to the sample repository's Azure Static Web App through a GitHub Actions secret whose name is specific to that Azure resource.

If you fork or copy this repository for another site:

1. create your own Azure Static Web App;
2. obtain the deployment token for that resource;
3. add it as a GitHub Actions repository secret; and
4. update the workflow's `azure_static_web_apps_api_token` secret reference to use your secret.

Do not reuse another site's Azure deployment token.

`dist` does not need to be committed. GitHub Actions rebuilds it from the checked-in source before deployment.

## Current sample status

This repository is intended as a starter and reference implementation rather than a finished website.

Notable current limitations include:

- only the `en-us` locale currently contains sample page definitions;
- image rendering through the asset-injection pipeline is not implemented;
- HTML asset injection assumes trusted source HTML;
- the `Order` page property is not currently used for sorting; and
- static Redirect pages do not produce HTTP redirect status codes.

These are intended extension points for sites that need additional behavior.

## License

Copyright © 2026 4F Software LLC.

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.