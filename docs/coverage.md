# Code coverage reports

Generate local coverage metrics and collect reports in CI.

## Local coverage run

```powershell
.\scripts\run-coverage.ps1
```

Output folder:

- `artifacts\coverage\results\**\coverage.cobertura.xml`

## CI coverage

The `Build and Test` workflow runs tests with `XPlat Code Coverage` and uploads coverage reports as workflow artifacts:

- `coverage-reports`

## Notes

- Coverage format: **Cobertura XML**
- Test target: `tests/ElBruno.CopilotCLIMonitor.Tests`
