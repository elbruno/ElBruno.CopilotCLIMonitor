# Decision: generated NuGet icon stays on the packed path

- **Date:** 2026-05-13
- **Agent:** Bender
- Keep the package icon path as `images\nuget-icon.png` in the CLI project and replace the old fallback artwork with a GPT-Image-2-generated base asset.
- Finalize the icon with a deterministic monitor/notification overlay so it stays readable at NuGet package-icon sizes.

## Rationale

- Preserving the existing packed path avoids touching package metadata or publish workflow wiring.
- A generated base asset satisfies the branding request now that `t2i` is configured, while the lightweight overlay keeps the systray + CLI + notification concept crisp at 256px and below.
