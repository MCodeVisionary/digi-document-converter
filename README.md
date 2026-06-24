# digi-document-converter

Converts an inputted document to **PDF**, **Word (.docx)**, or **JSON** for the
**Digitalization** project. Part of the **DocPortal** AppTrust application.

## Stack
| Concern | Technology |
|---------|------------|
| Library | `Digi.DocumentConverter.Core` — published as a **NuGet** package |
| API | ASP.NET Core 8 (`POST /api/convert`) |
| PDF | QuestPDF · Word | OpenXML · JSON | System.Text.Json |
| Container | **Docker** · Deploy | **Helm** |

## API
`POST /api/convert`
```json
{ "filename": "notes.txt", "sourceType": "transcript", "text": "...", "target": "pdf" }
```
`target` ∈ `pdf | word | json`. Returns the converted file.

## Tests
| Type | How |
|------|-----|
| Unit | `dotnet test tests/DigiDocumentConverter.UnitTests` (validates PDF/docx/JSON output) |
| Functional | `dotnet test tests/DigiDocumentConverter.FunctionalTests` |
| Smoke | `SMOKE_BASE_URL=... tests/smoke/smoke-test.sh` |
| Performance | `k6 run tests/performance/load-test.js` |

## CI/CD & evidence
Builds & tests, **publishes the NuGet package** to `digi-nuget-dev-local`, pushes
the image to `digi-docker-dev-local` and chart to `digi-helm-dev-local`, runs
SonarQube, and creates **signed in-toto/DSSE evidence** on both the NuGet package
and the Docker image (test results + mocked smoke/performance, SonarQube, SLSA
provenance, build signature). Registers a **DocPortal** AppTrust version.
