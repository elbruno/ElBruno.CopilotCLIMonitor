# Release branch strategy

This repository follows a three-branch release model:

- `main`: production-ready code
- `develop`: integration branch for upcoming release work
- `release/x.y.z`: stabilization branch for a specific release candidate

## Branch responsibilities

## `main`

- Always releasable
- Tagged for official releases
- Receives hotfix merges and completed release branch merges

## `develop`

- Default branch for ongoing feature integration
- May contain in-progress work not yet ready for release
- Source for creating `release/x.y.z` branches

## `release/x.y.z`

- Created from `develop` when scope for version `x.y.z` is frozen
- Accepts only stabilization changes (bug fixes, docs, version metadata)
- Merged into `main` for release and back-merged into `develop`

## Flow

1. Feature work merges into `develop`.
2. Create `release/x.y.z` from `develop`.
3. Stabilize and validate in release branch.
4. Merge `release/x.y.z` into `main`.
5. Tag and publish release artifacts/packages.
6. Merge `release/x.y.z` back into `develop`.

## Hotfixes

Hotfix branches start from `main`, then merge back into:

- `main` (for immediate patch release)
- `develop` (to keep long-lived branches aligned)
