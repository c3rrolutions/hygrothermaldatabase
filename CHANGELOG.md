# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Add GraphQL query `verificationCode` [5dd0a94bcfbd3de69a3b29f0a07be406617fbf83](https://github.com/building-envelope-data/database/commit/5dd0a94bcfbd3de69a3b29f0a07be406617fbf83)
- Add pages for individual data entries [6e1a5a484b2054e0eca168fe58b4860caef8d872](https://github.com/building-envelope-data/database/commit/6e1a5a484b2054e0eca168fe58b4860caef8d872)
- Enable automatic persisted queries [f1f7a73f3001bc89d429b103b360ccd36ea32e0d](https://github.com/building-envelope-data/database/commit/f1f7a73f3001bc89d429b103b360ccd36ea32e0d)
- Login through the OpenId Connect endpoints of the metabase [#44](https://github.com/building-envelope-data/database/pull/44)
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-

### Changed

- Upgrade packages to latest compatible versions in backend and frontend [#29](https://github.com/building-envelope-data/database/pull/29)
- Persist data protection keys in database [bbac9c7e84a21dcc67bc5a9446032b862af51b5b](https://github.com/building-envelope-data/database/commit/bbac9c7e84a21dcc67bc5a9446032b862af51b5b)
- Build Docker images with BuildKit [4ce5fa2c4dbe67de14f5e1c9be22896921349b50](https://github.com/building-envelope-data/database/commit/4ce5fa2c4dbe67de14f5e1c9be22896921349b50)
- Improve OpenId Connect usage and security [#46](https://github.com/building-envelope-data/database/pull/46)
- Do not manage JWT keys and GnuPG keys in files. JWT keys are created and rotated by the C# webserver daily. GnuPG keys are manually managed within the shell `make bootstrap` and stored in the volume `gnupg`.
- Use `forge.mk` to build and push images to the server noting the image tag, which is just the commit hash. On the server use `deploy.mk deploy TARGET=<the-commit-hash>` to deploy backend and frontend based on those images.
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-

### Removed

-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-

### Fixed

- Remove explicit content-type as it results in requests from banana cake pop responding with 404 in commit [90ee575397544158a603cd7946bd6fec5e4c1407](https://github.com/building-envelope-data/database/commit/90ee575397544158a603cd7946bd6fec5e4c1407)
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-
-

## [v1.0.0] - 2022-02-28

[Unreleased]: https://github.com/building-envelope-data/database/compare/v1.0.0...HEAD

[v1.0.0]: https://github.com/building-envelope-data/database/compare/352f44e492ca66d7835970cdf007b2897e686e30...v1.0.0
