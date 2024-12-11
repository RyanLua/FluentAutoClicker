# Contributing Guidelines

We welcome contributions from the community. If you would like to contribute, please follow the guidelines below.

## Fork, Clone, Branch and Create your PR

Once you've discussed your proposed feature/fix/etc. with a team member, and an approach or a spec has been written and approved, it's time to start development:

1. Fork the repo on GitHub if you haven't already
1. Clone your fork locally
1. Create a feature branch
1. Work on your changes
1. Create a [Draft Pull Request (PR)](https://github.blog/2019-02-14-introducing-draft-pull-requests/)
1. When ready, mark your PR as "ready for review".

## Building the code

1. Clone the repository
2. Configure your system
   * Please use the [configuration file](.configurations/configuration.dsc.yaml). This can be applied by either:
     * Dev Home's machine configuration tool
     * WinGet configuration. If you have WinGet version [v1.6.2631 or later](https://github.com/microsoft/winget-cli/releases), run `winget configure .configurations/configuration.dsc.yaml` in an elevated shell from the project root so relative paths resolve correctly
   * Alternatively, if you already are running the minimum OS version, have Visual Studio installed, and have developer mode enabled, you may configure your Visual Studio directly via the .vsconfig file. To do this:
     * Open the Visual Studio Installer, select “More” on your product card and then "Import configuration"
     * Specify the .vsconfig file at the root of the repo and select “Review Details”

### Rules

- **Follow the pattern of what you already see in the code.**
- Please follow as Modern C# of a style as you can and reference the [.NET Engineering Guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines) as much as you possibly can.
- Try to package new functionality/components into libraries that have nicely defined interfaces.
- Package new functionality into classes or refactor existing functionality into a class as you extend the code.
- When adding new classes/methods/changing existing code, add new unit tests or update the existing tests.

## License

By contributing, you agree that your contributions will be licensed under the [GNU Affero General Public License v3.0](https://github.com/RyanLua/FluentAutoClicker?tab=AGPL-3.0-1-ov-file).
