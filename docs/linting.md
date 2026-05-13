# Linting rules

This repository uses:

- Root `.editorconfig` for formatting and code-style defaults
- `StyleCop.Analyzers` for additional C# linting diagnostics

## Run lint/build checks

```powershell
dotnet build -v minimal
```

The build surfaces style and analyzer diagnostics in CI and local development.

## Rule customization

- EditorConfig controls formatting and IDE analyzer severities.
- StyleCop rules can be tuned via `dotnet_diagnostic.<RuleId>.severity` entries.
