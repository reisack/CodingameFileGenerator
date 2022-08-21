# Codingame File Generator

A CLI application for generating a unique C# file containing all C# code on a specified folder.  
So you can use it for generate your app code in one file and submit it to a Codingame puzzle.  

## Command line arguments
| Long | Short | Description | Example |
| ------------- | ------------- | ------------- | ------------- |
| `--output`  | `-o`  | Output file folder | `--output C:\\Dev\\my-project` (Default value : Same as executable) |
| `--root-folder`  | `-r`  | Root folder for recursively finding C# source files | `--root-folder C:\\Dev\\my-project\\src` (Default value : Same as executable) |
| `--first-file`  | `-f`  | Content of the provided file name will be at the top of output file | `--first-file botconts` (for BotConsts.cs file) |

## A complete example
This command line : `C:\Dev\codingame-bots\CodingameFileGenerator\CodingameFileGenerator.exe -r C:\Dev\codingame-bots\ocean-of-code -o C:\Dev\codingame-bots -f program`  

Will generate a `_codingame_output.cs` file on `C:\Dev\codingame-bots` containing all C# code in `C:\Dev\codingame-bots\ocean-of-code` folder. Starting with `program.cs` file content.

## Generate a unique file for your project
The best way is to add a **Post-build event command line** *(On project properties, Build Events)*.  

![image](https://user-images.githubusercontent.com/27150821/185811891-2efcc8af-73ed-4f14-8298-07a2387ca4a7.png)  

So your `_codingame_output.cs` file will be generated after a succeeded build.
