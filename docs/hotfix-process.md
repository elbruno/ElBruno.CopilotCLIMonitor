# Hotfix process

Use this flow for emergency production fixes that should ship as a patch release.

## Trigger criteria

A hotfix is appropriate when all of the following are true:

- The issue impacts current users in production.
- The impact is high enough that waiting for the next planned release is not acceptable.
- The fix can be safely delivered as a backward-compatible patch.

## Branching and scope

1. Branch from `main` using `hotfix/<issue-or-short-description>`.
2. Keep the change set minimal and focused on the production defect.
3. Avoid unrelated refactors, feature work, or cleanup changes in the hotfix branch.

## Validation checklist

1. Reproduce the bug (or capture failing test coverage).
2. Implement the fix and add/adjust automated tests.
3. Run repository build and tests from repo root:
   - `dotnet build -v minimal`
   - `dotnet test -v minimal`
4. Confirm no regression in affected CLI/systray paths.

## Release and publishing

1. Bump **PATCH** version using `scripts/version-bump.ps1 -Version X.Y.Z`.
2. Update `CHANGELOG.md` with hotfix details.
3. Trigger release workflow (`.github/workflows/release-v1.0.yml`) with the same version.
4. Publish all NuGet packages/libraries for the new version.

## Post-release follow-up

1. Verify package availability on NuGet.
2. Document root cause and mitigation in issue/PR notes.
3. If needed, schedule a follow-up hardening task for broader cleanup.
