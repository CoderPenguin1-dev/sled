# Sharp Line-based EDitor

sled is a CLI line-based editor, similar to UNIX's ed.

The main reason I made this was for fun, but I'm currently using it for Minty Launcher's publish script.

There is basic script support for sled.

# General Usage
You can see command documentation by typing in `?`.
Read CONFIG.md learn how to configure sled.
## Modes
s - Load and execute commands from a file.

x - Execute commands given as parameters.

l - Load file into the buffer

## Example commands file:
```
a
Hello, World!
Hello, World!
Lorem Ipsum
.
r 3 Hello, World!
wq test.txt
```
Saved as `commands.txt`.
```
sled s commands.txt
```

## Example of passing commands as parameters.
```
sled x a "Hello, World!" "Hello, World!" "Lorem Ipsum" . "r 3 Hello, World!" "wq test.txt"
```

# Building
## Prerequisites
* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Building Steps
Clone the repo:
```
git clone https://github.com/CoderPenguin1-dev/sled.git
```

Move into build directory:
```
cd "sled/sled"
```

Build & Run Debug binary:
```
dotnet run
```
It should open up afterwards. The build will be in `sled\bin\Debug\net9.0`.
