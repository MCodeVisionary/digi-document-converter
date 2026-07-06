# digi-document-converter

Converts documents to **PDF**, **Word (.docx)**, or **JSON** for the
**DocPortal** project. Part of the **DocPortal** AppTrust application.

## Stack

| Concern   | Technology |
|-----------|------------|
| Library   | `Digi.DocumentConverter.Core` — NuGet package |
| API       | ASP.NET Core 8 (`POST /api/convert`) |
| PDF       | QuestPDF |
| Word      | OpenXML |
| Container | Docker |
| Deploy    | Helm |

## API

`POST /api/convert`
```json
{ "filename": "notes.txt", "sourceType": "transcript", "text": "...", "target": "pdf" }
```
`target` ∈ `pdf | word | json`. Returns the converted file as a download.

## Layout

```
src/DigiDocumentConverter.Core/    NuGet library (conversion logic)
src/DigiDocumentConverter.Api/     ASP.NET Core host
tests/DigiDocumentConverter.UnitTests/
tests/DigiDocumentConverter.FunctionalTests/
helm/digi-document-converter/      Helm chart
Dockerfile
.github/workflows/ci.yml           CI/CD pipeline
```

## Local dev

```bash
dotnet run --project src/DigiDocumentConverter.Api
# POST http://localhost:5000/api/convert
```

## Tests

| Type        | Command |
|-------------|---------|
| Unit        | `dotnet test tests/DigiDocumentConverter.UnitTests` |
| Functional  | `dotnet test tests/DigiDocumentConverter.FunctionalTests` |
| Performance | `k6 run tests/performance/load-test.js` |

---

## CI/CD Pipeline & Evidence

On every push to `main`, the pipeline builds all three artifacts, attaches
**SLSA v1.0 provenance** and **test-result evidence** to each, promotes through
four lifecycle stages, and registers a **DocPortal** AppTrust version.

### Artifacts published to JFrog

| Artifact | Repository |
|----------|-----------|
| NuGet package (`Digi.DocumentConverter.Core`) | `docportal-doc-converter-nuget-dev-local` |
| Docker image (`digi-document-converter`) | `docportal-doc-converter-docker-dev-local` |
| Helm chart (`digi-document-converter`) | `docportal-doc-converter-helm-dev-local` |

### Evidence flow

#### 1. SLSA provenance (all artifacts, at publish time)

SLSA v1.0 provenance is attached to every artifact immediately after publish.
Each record captures the GitHub Actions run ID, commit SHA, and ref.

```bash
# NuGet package
jf evd create \
  --package-name "Digi.DocumentConverter.Core" --package-version "$VERSION" \
  --package-repo-name "docportal-doc-converter-nuget-dev-local" \
  --predicate provenance-nuget.json \
  --predicate-type "https://slsa.dev/provenance/v1" \
  --provider-id github \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"

# Docker image
jf evd create \
  --package-name "digi-document-converter" --package-version "$VERSION" \
  --package-repo-name "docportal-doc-converter-docker-dev-local" \
  --predicate provenance-docker.json \
  --predicate-type "https://slsa.dev/provenance/v1" \
  --provider-id github \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"

# Helm chart
jf evd create \
  --package-name "digi-document-converter" --package-version "$VERSION" \
  --package-repo-name "docportal-doc-converter-helm-dev-local" \
  --predicate provenance-helm.json \
  --predicate-type "https://slsa.dev/provenance/v1" \
  --provider-id github \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"
```

The provenance JSON uses the SLSA v1.0 schema (`buildDefinition` +
`runDetails`), which renders as the GitHub logo in JFrog's evidence panel.

#### 2. Build-level evidence (SLSA provenance + build signature)

Attached to the JFrog build-info record after `jf rt build-publish`:

```bash
# SLSA provenance on the build
jf evd create \
  --build-name "digi-document-converter" --build-number "$GITHUB_RUN_NUMBER" \
  --project "docportal" \
  --predicate provenance.json \
  --predicate-type "https://slsa.dev/provenance/v1" \
  --provider-id github \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"

# Build signature
jf evd create \
  --build-name "digi-document-converter" --build-number "$GITHUB_RUN_NUMBER" \
  --project "docportal" \
  --predicate build-sig.json \
  --predicate-type "https://jfrog.com/evidence/build-signature/v1" \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"
```

#### 3. DEV stage — unit tests (xUnit)

```bash
# Docker, NuGet, and Helm each get unit-test evidence
jf evd create \
  --package-name "<name>" --package-version "$VERSION" \
  --package-repo-name "<repo>" \
  --predicate reports/unit-predicate.json \
  --predicate-type "https://jfrog.com/evidence/testing-results/v1" \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"
```

Predicate fields: `name`, `stage` (`DEV`), `result` (`PASSED`/`FAILED`),
`build`, `suites` (tool: `xunit`, passed, failed, total), `timestamp`.

Release bundle is created and promoted to **DEV**.

#### 4. QA stage — functional tests (xUnit + WebApplicationFactory)

```bash
# Docker + NuGet get functional-test evidence
jf evd create \
  --package-name "<name>" --package-version "$VERSION" \
  --package-repo-name "<repo>" \
  --predicate reports/functional-predicate.json \
  --predicate-type "https://jfrog.com/evidence/integration-results/v1" \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"
```

Release bundle promoted to **QA**.

#### 5. STAGING stage — performance tests (k6)

```bash
# Docker gets performance evidence (with raw k6 JSON attached)
jf evd create \
  --package-name "digi-document-converter" --package-version "$VERSION" \
  --package-repo-name "docportal-doc-converter-docker-dev-local" \
  --predicate reports/perf-predicate.json \
  --predicate-type "https://jfrog.com/evidence/performance-results/v1" \
  --attach-local reports/performance.json \
  --attach-artifactory-temp-path "docportal-doc-converter-generic-dev-local/digi-document-converter/$GITHUB_RUN_NUMBER/evidence-temp" \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"
```

Predicate fields: `tool` (k6), `total_requests`, `p95_ms`, `failure_rate`,
`passed` (pass criteria: `failure_rate < 0.01` and `p95_ms < 500`).

Release bundle promoted to **STAGING**.

#### 6. PROD stage

Release bundle promoted to **PROD**.

#### 7. Release bundle evidence

```bash
jf evd create \
  --release-bundle "digi-document-converter" --release-bundle-version "$VERSION" \
  --project "docportal" \
  --predicate reports/unit-predicate.json \
  --predicate-type "https://jfrog.com/evidence/testing-results/v1" \
  --key "$EVIDENCE_PRIVATE_KEY" --key-alias "docportal-evidence-key"
```

### Evidence summary per artifact

| Artifact | Evidence records |
|----------|-----------------|
| NuGet package | SLSA provenance (GitHub), unit tests, functional tests |
| Docker image | SLSA provenance (GitHub), build SLSA, build signature, unit tests, functional tests, performance |
| Helm chart | SLSA provenance (GitHub), unit tests |
| Release bundle | unit test results |

### AppTrust

After PROD promotion, a **DocPortal** AppTrust application version is created
combining all three services. The manifest at
`docportal-client-portal-generic-dev-local/docportal/manifest.json` tracks
each service version. When all three are present, `jf apptrust version-create
docportal <N> --skip-unassigned` creates the version, `version-promote` moves
it through QA and STAGING, and `version-release` releases to PROD.

### Required secrets / variables

| Name | Purpose |
|------|---------|
| `vars.JF_URL` | JFrog platform URL |
| `secrets.EVIDENCE_PRIVATE_KEY` | PEM private key for signing evidence |
| OIDC provider `github-docportal` | Keyless auth to JFrog |
