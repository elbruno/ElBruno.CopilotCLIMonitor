# Versioning strategy

ElBruno.CopilotCLIMonitor uses [Semantic Versioning 2.0.0](https://semver.org/).

## Version format

`MAJOR.MINOR.PATCH`

Examples:
- `1.0.0`
- `1.1.0`
- `1.1.1`
- `1.2.0-beta.1` (pre-release)

## Bump rules

- **MAJOR**: Breaking changes in public behavior, CLI contracts, or integration behavior.
- **MINOR**: Backward-compatible new functionality.
- **PATCH**: Backward-compatible fixes, reliability improvements, or documentation-only release fixes.

## Pre-release policy

Pre-releases are allowed using semantic suffixes:

- `-alpha.N`
- `-beta.N`
- `-rc.N`

Example: `2.0.0-rc.1`

## Release implementation in this repository

1. Update version via `scripts/version-bump.ps1 -Version X.Y.Z` (or `X.Y.Z-suffix` for pre-release).
   - Or auto-increment from current version via `scripts/version-auto-increment.ps1 -Increment patch|minor|major`.
   - Use `-DryRun` first to preview the computed next version.
2. Update `CHANGELOG.md` and draft release notes from `.github/release-notes-template.md`.
3. Run build/test validation.
4. Trigger release pipeline (`.github/workflows/release-v1.0.yml`) with the same version.
5. Publish NuGet package(s) for the new version.

## Validation

The version-bump script validates that version numbers follow semantic versioning format before writing project files.
