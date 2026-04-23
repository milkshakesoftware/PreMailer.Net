# AGENTS.md

## Layout gotcha

The .sln and all projects live in the nested `PreMailer.Net/` folder, not the repo root. **Run all `dotnet` commands from `./PreMailer.Net/`** (matches `working-directory` in `.github/workflows/dotnetcore.yml`).

```
./PreMailer.Net/                     <- cwd for dotnet
  PreMailer.Net.sln
  PreMailer.Net/       (library, netstandard2.0 + net461, signed)
  PreMailer.Net.Tests/ (xunit, net9.0)
  Benchmarks/          (BenchmarkDotNet, net9.0, Exe)
```

## Commands

From `./PreMailer.Net/`:

- Restore/build: `dotnet restore` then `dotnet build -c Release --no-restore`
- All tests: `dotnet test --no-restore`
- Single test: `dotnet test --filter "FullyQualifiedName~PreMailerTests.MethodName"`
- Watch tests: `dotnet watch test --project PreMailer.Net.Tests/PreMailer.Net.Tests.csproj`
- Benchmarks: `dotnet run -c Release --project Benchmarks/Benchmarks.csproj`
- Pack (manual): `PreMailer.Net/nuget.bat` — calls `dotnet pack ... --include-symbols`

## Framework targets

- Library multi-targets **`netstandard2.0;net461`** — keep code compatible with both. No C# nullable reference types in the library (only Benchmarks has `<Nullable>enable</Nullable>`).
- `net461` gets an extra `System.Memory` package reference; don't remove it.
- Assembly is signed with `PreMailer.Net.snk` (`SignAssembly=true`). Any InternalsVisibleTo must include the public key.
- `LangVersion=latest` is set, so modern C# syntax is fine as long as it compiles for both TFMs.

## Dependencies

- Library depends only on **AngleSharp 1.4.0**. The DOM is an `IHtmlDocument`; expose it via `PreMailer.Document` (see README "Custom DOM Processing").
- Tests use xunit + Moq + coverlet. No fixtures/services required — tests are pure.

## Release flow

Publishing to NuGet is driven by GitHub **Releases**, not branch pushes (see `dotnetcore.yml`):

1. Create a GitHub release; tag like `v2.3.4`.
2. The `v` prefix is stripped and passed as `-p:Version=`; the release body becomes `PackageReleaseNotes`.
3. CI runs on `windows-latest` with .NET SDK `10.0.x` and pushes `*.nupkg` using `secrets.NUGET_APIKEY`.

Do not bump versions in the `.csproj` — version is supplied at pack time from the tag.

## Conventions

- README is packed into the nupkg (`PackageReadmeFile`). Keep README changes shippable.
- `-premailer-*` CSS properties are proxied to HTML attributes (see `AttributeToCss.cs`, tests in `PreMailerTests`). Preserve that contract.
- Unsupported pseudo-classes/elements must be logged to `InlineResult.Warnings`, not thrown.

## Local CI

README documents running workflows with `act`:
`act push --container-architecture=linux/arm64 --platform ubuntu-slim=node:lts-bullseye`
Note: the real CI uses `windows-latest`, so `act` won't fully reproduce it.
