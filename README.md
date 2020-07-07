# ADR CLI tooling

A command-line tool for working with Architecture Decision Records (ADRs).

# Publishing

You can use the `FolderProfile` in this solution to publish the project.

If you're a fan of the dotnet CLI, publish using the following command:

```powershell
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
```