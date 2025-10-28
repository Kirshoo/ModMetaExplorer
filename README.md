# Mod Meta Explorer
This program is designed to be used by moderators of the PEAK speedrunning community in combination with ModVerifier
to easily update the whitelist of allowed mods.

## Building
To build the program yourself, first you will need to copy this repository using the following command:
```
git clone https://github.com/Kirshoo/ModMetaExplorer.git
```

Once you downloaded the repository and opened the directory, you can run the following command to build 
the program for your `$SYSTEM`. Replace `$SYSTEM` with one of the following values, depending on your system:
- win-x64 for Windows
- linux-x64 for Linux
- osx-x64 for macOs

```
dotnet publish -c Release -r $SYSTEM --self-contained true /p:PublishSingleFile=true
```

Once built, executable could be found in `ModMetaExplorer/bin/Release/net8.0/$SYSTEM/publish` with the name `ModMetaExplorer.exe`.

## Usage
You can use ModMetaExplorer on a specific file
```
ModMetaExplorer.exe "path/to/mod.dll"
```
or on whole directories
```
ModMetaExplorer.exe "path/to/folder/containing/mods"
```

ModMetaExplorer also accepts a few flags:
`-j` to convert output from plaintext into .json format
`--output <filename>` to write to file with `<filename>` instead of console
