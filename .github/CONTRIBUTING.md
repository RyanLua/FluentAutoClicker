# Contributing Guidelines

We welcome contributions from the community. If you would like to contribute, please follow the guidelines below.

If this is your first time contributing, [learn how to contribute to a project through forking](https://docs.github.com/en/get-started/exploring-projects-on-github/contributing-to-a-project).

## Configure your environment

Learn how to configure your environment to develop this project. This assumes you meet the [system requirements for Windows app development](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/system-requirements).

### Using Dev Home or WinGet

You can use the [`configuration.dsc.yaml` configuration file](.configurations/configuration.dsc.yaml) to configure your environment. This can be applied by either:

* Dev Home's machine configuration tool
* WinGet configuration. If you have WinGet version [v1.6.2631 or later](https://github.com/microsoft/winget-cli/releases), run `winget configure .configurations/configuration.dsc.yaml` in an elevated shell from the project root so relative paths resolve correctly

### Using Visual Studio

If you already have Visual Studio installed and have developer mode enabled, you may import the [`.vsconfig` installation configuration file](.vsconfig).

1. Open the Visual Studio Installer and close Visual Studio.
1. On either the **Installed** tab or the **Available** tab, select **More** > **Import configuration** on the Visual Studio product card.
1. Locate the `.vsconfig` file from the project root, and then choose **Review details**.
1. Verify that your selections are accurate, and then choose **Modify**.

For more ways to install, learn how to [import or export installation configurations](https://learn.microsoft.com/en-us/visualstudio/install/import-export-installation-configurations?view=vs-2022).

## Rules

* **Follow the pattern of what you already see in the code.**
* Please follow as Modern C# of a style as you can and reference the [.NET Engineering Guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines) as much as you possibly can.
* Try to package new functionality/components into libraries that have nicely defined interfaces.
* Package new functionality into classes or refactor existing functionality into a class as you extend the code.
* When adding new classes/methods/changing existing code, add new unit tests or update the existing tests.

## License

By contributing, you agree that your contributions will be licensed under the [GNU Affero General Public License v3.0](https://github.com/RyanLua/FluentAutoClicker?tab=AGPL-3.0-1-ov-file).
