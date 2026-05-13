# Performance testing

Performance validation focuses on responsiveness of event ingestion and filtering.

## Covered scenarios

- High-volume event ingestion in `EventStore`
- Concurrent event ingestion under parallel load
- Dashboard filtering performance on large event lists

## Automated tests

Performance-oriented tests are located in:

- `tests/ElBruno.CopilotCLIMonitor.Tests/Performance/PerformanceBenchmarksTests.cs`

They enforce upper time budgets for core operations to detect regressions.

## Run performance tests only

```powershell
dotnet test --filter "FullyQualifiedName~PerformanceBenchmarksTests"
```

## Notes

- Budgets are tuned to be stable in CI while still detecting major regressions.
- These are guardrail tests, not full micro-benchmark suites.
