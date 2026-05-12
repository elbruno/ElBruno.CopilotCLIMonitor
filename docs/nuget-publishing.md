# NuGet publishing

This document explains how ElBruno.copilotclimon is published to NuGet and configured as a Trusted Publisher.

## Overview

ElBruno.copilotclimon is published as a .NET Tool to NuGet.org. The publishing process uses GitHub Actions with OpenID Connect (OIDC) for secure, token-free authentication.

## Package information

**Package ID:** `ElBruno.copilotclimon`

**Package type:** .NET Tool

**Installation:**

```powershell
dotnet tool install -g ElBruno.copilotclimon
```

**Repository:** https://github.com/elbruno/ElBruno.copilotclimon

## Trusted Publisher setup

ElBruno.copilotclimon is configured as a NuGet Trusted Publisher. This allows secure publishing without storing API keys.

### Configuration

**Publisher name:** ElBruno

**Repository:** elbruno/ElBruno.copilotclimon

**Authentication method:** OpenID Connect (OIDC)

### Benefits

- ✅ No API keys stored in CI/CD secrets
- ✅ Automatic token generation per publish
- ✅ Secure, short-lived credentials
- ✅ Audit trail through GitHub
- ✅ Reduced security surface

## Release process

### 1. Update version

Edit `.csproj` files to update version:

```xml
<Version>1.0.0</Version>
```

### 2. Update CHANGELOG

Document changes in `CHANGELOG.md`:

```markdown
## [1.0.0] - 2026-05-12

### Added
- Feature 1
- Feature 2

### Fixed
- Bug 1
```

### 3. Commit changes

```bash
git commit -m "chore: release v1.0.0"
```

### 4. Create Git tag

```bash
git tag v1.0.0
```

### 5. Push to GitHub

```bash
git push origin main
git push origin v1.0.0
```

### 6. Create GitHub Release

1. Go to https://github.com/elbruno/ElBruno.copilotclimon/releases
2. Click "Create a new release"
3. Select tag: `v1.0.0`
4. Title: `Version 1.0.0`
5. Description: Copy from `CHANGELOG.md`
6. Click "Publish release"

### 7. Automated publishing

When a GitHub Release is published, the `release.yml` workflow automatically:

1. Extracts version from tag
2. Builds the project
3. Runs tests
4. Creates NuGet package
5. Publishes to NuGet.org
6. Uploads artifacts to release

## Publishing workflows

### Build and Test Workflow

**File:** `.github/workflows/dotnet.yml`

Triggers on:
- Push to `main` or `develop`
- Pull requests to `main` or `develop`

Steps:
1. Setup .NET 10
2. Restore dependencies
3. Build (Release configuration)
4. Run tests
5. Create NuGet package
6. Upload artifacts

### Release Workflow

**File:** `.github/workflows/release.yml`

Triggers on:
- GitHub Release published

Steps:
1. Extract version from tag
2. Setup .NET 10
3. Restore and build
4. Run tests
5. Create NuGet package
6. Publish to NuGet.org using Trusted Publisher
7. Upload package to GitHub Release

## Project file configuration

### NuGet metadata in .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    
    <!-- NuGet Package Metadata -->
    <PackageId>ElBruno.copilotclimon</PackageId>
    <Version>1.0.0</Version>
    <Title>GitHub Copilot CLI Monitor</Title>
    <Authors>ElBruno</Authors>
    <Description>Windows Systray application for monitoring long-running GitHub Copilot CLI tasks with native notifications.</Description>
    <PackageProjectUrl>https://github.com/elbruno/ElBruno.copilotclimon</PackageProjectUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <RepositoryUrl>https://github.com/elbruno/ElBruno.copilotclimon</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <PackageTags>copilot;cli;notifications;systray;wpc;dotnet-tool</PackageTags>
    <PackageIcon>icon.png</PackageIcon>
    
    <!-- Semantic Versioning -->
    <VersionPrefix>1.0.0</VersionPrefix>
    <VersionSuffix></VersionSuffix>
    
    <!-- .NET Tool -->
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>copilotclimon</ToolCommandName>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>
  
  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\"/>
    <None Include="images/icon.png" Pack="true" PackagePath="\"/>
  </ItemGroup>

</Project>
```

## Local publishing (testing)

To test the package locally before publishing to NuGet:

```bash
# Build the project
dotnet build --configuration Release

# Pack the package
dotnet pack --configuration Release --output ./test-packages

# Uninstall current version (if installed)
dotnet tool uninstall -g ElBruno.copilotclimon

# Install from local package
dotnet tool install -g ElBruno.copilotclimon --add-source ./test-packages
```

## Verification

After publishing to NuGet, verify the package:

```bash
# Uninstall local version
dotnet tool uninstall -g ElBruno.copilotclimon

# Install from NuGet
dotnet tool install -g ElBruno.copilotclimon

# Verify installation
copilotclimon --version
```

## NuGet.org management

### Access

NuGet package is owned and managed by the ElBruno account.

Owners can be managed at: https://www.nuget.org/packages/ElBruno.copilotclimon/Ownership

### Package page

https://www.nuget.org/packages/ElBruno.copilotclimon

### Statistics

Download statistics and version history available on the NuGet.org package page.

## Troubleshooting

### Publishing fails with authentication error

**Cause:** OIDC token expired or misconfigured

**Solution:**
1. Verify GitHub Actions has permissions to publish
2. Check that repository is linked to Trusted Publisher
3. Re-run the release workflow

### Package not visible on NuGet

**Cause:** Package is still being indexed

**Solution:**
1. Wait 15-30 minutes for indexing
2. Clear NuGet cache: `nuget locals all -clear`
3. Search NuGet.org directly

### Version conflict

**Cause:** Version already published

**Solution:**
1. Use different version number
2. Or delete the version from NuGet.org (within 24 hours of publish)

### Tool not found after install

**Cause:** NuGet cache issue

**Solution:**
```bash
# Clear NuGet cache
nuget locals all -clear

# Reinstall
dotnet tool install -g ElBruno.copilotclimon
```

## Related

- [Setup guide](docs/setup.md)
- [GitHub repository](https://github.com/elbruno/ElBruno.copilotclimon)
- [NuGet package](https://www.nuget.org/packages/ElBruno.copilotclimon)
