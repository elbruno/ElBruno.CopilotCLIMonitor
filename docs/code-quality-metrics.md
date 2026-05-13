# Code quality metrics

This repository integrates automated static analysis using:

- **CodeQL** (`.github/workflows/codeql.yml`)
- **SonarQube** (`.github/workflows/sonarqube.yml`)

## CodeQL

- Runs on pushes and pull requests for `main` and `develop`
- Uploads security/code-quality findings to GitHub Security tab
- Scheduled weekly scan

## SonarQube

- Runs when `SONAR_TOKEN` and `SONAR_HOST_URL` secrets are configured
- Executes build + test analysis and publishes metrics to SonarQube

## Required secrets for SonarQube workflow

- `SONAR_TOKEN`
- `SONAR_HOST_URL`
