# ADR CLI tooling

A command-line tool for working with Architecture Decision Records (ADRs).

# Publishing

You can use the `FolderProfile` in this solution to publish the project.

If you're a fan of the dotnet CLI, publish using the following command:

```powershell
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
```

# Credits

This tool was originally started by [GingerTommy](https://github.com/GingerTommy/adr-cli). After having waited for a couple of months for my PR's to get approved, I've decided to create a new repository for the tool and use the original as a base.
