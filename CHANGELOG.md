# Changelog

## [Unreleased]

## [1.1.1] - 2020-09-14

### Fixed

- Very likely to fail to popup after being imported

## [1.1.0] - 2020-08-19

### Changed

- Replace Json.NET with SimpleJSON which is more light-weighted
- Integrate UserSettings into RegistrySettings
- Change the format of RegistrySettings to ScriptableObject from json for more convenient editing in editor
- Prevent auto checking while transitioning to play mode

### Removed

- Redundant internal codes for license accepting procedure

### Fixed

- `NullReferenceException` during importing the VIVE Registry Tool

## [1.0.0] - 2020-08-10

### Added

- Window to add or remove VIVE registry
- Auto check if VIVE registry is already added in the project on reload
- Functionality of managing scope registries in `Packages/manifest.json`