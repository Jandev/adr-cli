# ADR CLI tooling

A command-line tool for working with Architecture Decision Records (ADRs).

# Installation

* Download the latest version of the adr-cli tool from the [Releases page](https://github.com/Jandev/adr-cli/releases)  
It's available for both Windows & Linux.

* Copy the executable to a location mentioned in the PATH of your system.

# Usage

You can use this tool to create a directory for your ADR documents to be stored in and create new ADR files.  
An index file will also be created on initial setup.

## Init
`adr-cli init` 

This will create the necessary folder (`/docs/adr`) from where you are running the command, including the initial document (`0001-record-architecture-decisions.md`) and the index file (`0000-index.md`).  
Once created, it will try to open the initial file in your Markdown editor.

## List
`adr-cli init` 

Placeholder, does nothing yet.  
This command is inherited from the previous implementation and can be deleted.

## New
`adr-cli new "New decision"` 

Will create a new ADR document with the mentioned title and a follow-up number.  
The document will have the `Proposed` status.

The `-s` flag (supercedes) is not implemented at this time.

## Link

Placeholder, does nothing yet.  
This command is inherited from the previous implementation and can be deleted.

## Generate

Placeholder, does nothing yet.  
This command is inherited from the previous implementation and can be deleted.


# Publishing

If you don't want to use the [Releases published in this repository](https://github.com/Jandev/adr-cli/releases), you can also publish them yourselves.

The `FolderProfile` in this solution is set up to do this.

If you're a fan of the dotnet CLI, publish using the following command:

```powershell
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
```

# Credits

This tool was originally started by [GingerTommy](https://github.com/GingerTommy/adr-cli). After having waited for a couple of months for my PR's to get approved, I've decided to create a new repository for the tool and use the original as a base.
