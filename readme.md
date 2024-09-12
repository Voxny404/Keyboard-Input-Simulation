# Keyboard Input Simulation
This library allows you to simulate keyboard input on Windows systems using C# and provides a JavaScript interface for easy integration with Node.js applications.

## Features
- Simulate key presses and releases
- Support for various keyboard layouts, including English and German
- Interface for both C# and JavaScript

## C# Usage
Installation
1. Clone the Repository:
````
git clone https://github.com/Voxny404/keyboard-input-simulation.git

````
2. Build the Project:

 1. Navigate to the .NET Framework Directory:

    The csc.exe compiler is usually located in one of the following paths, depending on the version of the .NET Framework installed:
    ````
    C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
    ````
    or for 64-bit applications:
    ````
    C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
    ````
    - v4.0.30319 corresponds to .NET Framework 4.x versions.
    - Adjust the version number (vX.X.XXX) if you have a different version installed.
 2. Accessing CSC:
    To compile C# code using csc from the command line:

    - Open Command Prompt:
    - Run csc: Compile your C# file by specifying the file name. For example, if you have a file named keyboard.cs, you would use:
     ````
    C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe keyboard.cs
    ````

3. Test the keyboard.exe:
````
keyboard.exe <layout> <key>

````
- <layout>: Specify the keyboard layout (e.g., "english" or "german").
- <key>: The key to be simulated (e.g., "enter", "a", "1").

## JavaScript Usage
1. Require the Module:
````
const key = require("./keyboard/key");
````
2. Send a word: 
````
key.sendWord("Testing keys!");
````
3. Send a Single Key:
````
key.send("a");
````

## Example Code 
````
const key = require("./keyboard/key");

// Send a sequence of keys
key.sendWord("Hello World!");

// Send individual keys
key.send("enter");
key.send("a");
key.send("1");

````

## Notes
- This library currently only works on Windows systems.
- Ensure you have the necessary permissions to simulate keyboard input.
- For correct simulation, the appropriate keyboard layout must be active.

## Contributing
Feel free to contribute to this project by submitting issues, creating pull requests, or providing feedback.

## License
This project is licensed under the MIT License - see the LICENSE file for details.