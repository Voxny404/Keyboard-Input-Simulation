using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

class Program
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public uint type;
        public MOUSEKEYBOARDHWHEEL mkhw;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct MOUSEKEYBOARDHWHEEL
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;
        [FieldOffset(0)]
        public KEYBDINPUT ki;
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    const uint INPUT_KEYBOARD = 1;
    const uint KEYEVENTF_KEYUP = 0x0002;
    const ushort VK_SHIFT = 0x10;
    const ushort VK_MENU = 0x12; // AltGr is equivalent to Alt (Right Alt)

    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            string layout = args[0].ToLower();
            string character = args[1].ToLower();
            Map(character, layout);
        }
    }

    private static void Map(string character, string layout)
    {
        Dictionary<string, byte> keyMappings = GetSharedLayout();
        if (layout == "english")
        {
            keyMappings = AddEnglishSpecific(keyMappings);
        }
        else if (layout == "german")
        {
            keyMappings = AddGermanSpecific(keyMappings);
        }

        // Check if the character exists in the mapping
        if (!keyMappings.ContainsKey(character))
        {
            Console.WriteLine("Character not mapped : " + character);
            return;
        }

        // Determine if shift or AltGr is needed
        bool needsShift = IsShiftRequired(character, layout);
        bool needsAltGr = IsAltGrRequired(character, layout);

        // Send key press with modifiers
        SendKeyPress(keyMappings[character], needsShift, needsAltGr);
    }

    private static bool IsShiftRequired(string character, string layout)
    {
        // Shift required for certain characters in both English and German
        string shiftChars = "!\"§$%&/()=?*+~<>|:;'";

        return shiftChars.Contains(character);
    }

    private static bool IsAltGrRequired(string character, string layout)
    {
        // AltGr is required for certain characters on German keyboard layouts
        string altGrChars = "@€{}[]\\|";

        return altGrChars.Contains(character);
    }

    private static void SendKeyPress(byte vkCode, bool needsShift, bool needsAltGr)
    {
        List<INPUT> inputs = new List<INPUT>();

        // Add Shift or AltGr press if needed
        if (needsShift)
        {
            inputs.Add(CreateKeyInput(VK_SHIFT, false)); // Shift down
        }
        if (needsAltGr)
        {
            inputs.Add(CreateKeyInput(VK_MENU, false)); // AltGr (Right Alt) down
        }

        // Key down
        inputs.Add(CreateKeyInput(vkCode, false));

        // Key up
        inputs.Add(CreateKeyInput(vkCode, true));

        // Release Shift or AltGr if pressed
        if (needsShift)
        {
            inputs.Add(CreateKeyInput(VK_SHIFT, true)); // Shift up
        }
        if (needsAltGr)
        {
            inputs.Add(CreateKeyInput(VK_MENU, true)); // AltGr (Right Alt) up
        }

        SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
    }

    private static INPUT CreateKeyInput(ushort vkCode, bool keyUp)
    {
        INPUT input = new INPUT
        {
            type = INPUT_KEYBOARD,
            mkhw = new MOUSEKEYBOARDHWHEEL
            {
                ki = new KEYBDINPUT
                {
                    wVk = vkCode,
                    dwFlags = keyUp ? KEYEVENTF_KEYUP : 0
                }
            }
        };
        return input;
    }

    // Shared Layout for both English and German
    private static Dictionary<string, byte> GetSharedLayout()
    {
        return new Dictionary<string, byte>
        {
            // Letters (A-Z)
            { "a", 0x41 }, { "b", 0x42 }, { "c", 0x43 }, { "d", 0x44 }, { "e", 0x45 },
            { "f", 0x46 }, { "g", 0x47 }, { "h", 0x48 }, { "i", 0x49 }, { "j", 0x4A },
            { "k", 0x4B }, { "l", 0x4C }, { "m", 0x4D }, { "n", 0x4E }, { "o", 0x4F },
            { "p", 0x50 }, { "q", 0x51 }, { "r", 0x52 }, { "s", 0x53 }, { "t", 0x54 },
            { "u", 0x55 }, { "v", 0x56 }, { "w", 0x57 }, { "x", 0x58 }, { "y", 0x59 },
            { "z", 0x5A },

            // Numbers (0-9)
            { "0", 0x30 }, { "1", 0x31 }, { "2", 0x32 }, { "3", 0x33 }, { "4", 0x34 },
            { "5", 0x35 }, { "6", 0x36 }, { "7", 0x37 }, { "8", 0x38 }, { "9", 0x39 },

            // Control keys
            { "space", 0x20 }, { "enter", 0x0D }, { "backspace", 0x08 }, { "tab", 0x09 },
            { "esc", 0x1B }, { "left", 0x25 }, { "up", 0x26 }, { "right", 0x27 }, { "down", 0x28 },

            // Function keys (F1 - F12)
            { "f1", 0x70 }, { "f2", 0x71 }, { "f3", 0x72 }, { "f4", 0x73 }, { "f5", 0x74 },
            { "f6", 0x75 }, { "f7", 0x76 }, { "f8", 0x77 }, { "f9", 0x78 }, { "f10", 0x79 },
            { "f11", 0x7A }, { "f12", 0x7B },

            // Numpad (0-9)
            { "numpad0", 0x60 }, { "numpad1", 0x61 }, { "numpad2", 0x62 }, { "numpad3", 0x63 },
            { "numpad4", 0x64 }, { "numpad5", 0x65 }, { "numpad6", 0x66 }, { "numpad7", 0x67 },
            { "numpad8", 0x68 }, { "numpad9", 0x69 }, { "numlock", 0x90 },

            // Common Symbols
            { ",", 0xBC }, { ".", 0xBE }, { "-", 0xBD }, { "/", 0xBF }, { ";", 0xBA },
            { "\\", 0xDC }, { "=", 0xBB }
        };
    }

    // English-specific characters
    private static Dictionary<string, byte> AddEnglishSpecific(Dictionary<string, byte> list)
    {
        AddIfNotExists(list, "!", 0x31); // Shift + 1
        AddIfNotExists(list, "@", 0x32); // Shift + 2
        AddIfNotExists(list, "#", 0x33); // Shift + 3
        AddIfNotExists(list, "$", 0x34); // Shift + 4
        AddIfNotExists(list, "%", 0x35); // Shift + 5
        AddIfNotExists(list, "^", 0x36); // Shift + 6
        AddIfNotExists(list, "&", 0x37); // Shift + 7
        AddIfNotExists(list, "*", 0x38); // Shift + 8
        AddIfNotExists(list, "(", 0x39); // Shift + 9
        AddIfNotExists(list, ")", 0x30); // Shift + 0
        AddIfNotExists(list, "_", 0xBD); // Shift + -
        AddIfNotExists(list, "+", 0xBB); // Shift + =
        AddIfNotExists(list, "{", 0xDB); // Shift + [
        AddIfNotExists(list, "}", 0xDD); // Shift + ]
        AddIfNotExists(list, "|", 0xDC); // Shift + \
        AddIfNotExists(list, "\"", 0xDE); // Shift + '
        AddIfNotExists(list, "<", 0xBC); // Shift + ,
        AddIfNotExists(list, ">", 0xBE); // Shift + .
        AddIfNotExists(list, "?", 0xBF); // Shift + /
        AddIfNotExists(list, "'", 0xDE); // Shift + '
        AddIfNotExists(list, "[", 0xDB);
        AddIfNotExists(list, "]", 0xDD);
        return list;
    }

    // German-specific characters
    private static Dictionary<string, byte> AddGermanSpecific(Dictionary<string, byte> list)
    {
        AddIfNotExists(list, "ü", 0xBA);  // German umlauts
        AddIfNotExists(list, "ö", 0xC0);
        AddIfNotExists(list, "ä", 0xDE);
        AddIfNotExists(list, "ß", 0xDB);
        AddIfNotExists(list, "´", 0xDD);
         AddIfNotExists(list, "'", 0xBF); // Shift + /
        AddIfNotExists(list, "+", 0xBB);  // German "+"
        AddIfNotExists(list, "#", 0xBF);  // German "#"
        AddIfNotExists(list, "-", 0xBD);  // German "-"
        AddIfNotExists(list, ",", 0xBC);  // German ","
        AddIfNotExists(list, ".", 0xBE);  // German "."
        AddIfNotExists(list, ":", 0xBE);  // Shift + "."
        AddIfNotExists(list, ";", 0xBC);  // shift + ","
        AddIfNotExists(list, "€", 0x05);  // AltGr + E for "€"

        // Symbols that require Shift key on German keyboard
        AddIfNotExists(list, "!", 0x31); // Shift + 1
        AddIfNotExists(list, "\"", 0x32); // Shift + 2 (German keyboard uses " instead of @)
        AddIfNotExists(list, "§", 0x33); // Shift + 3 (German keyboard uses § instead of #)
        AddIfNotExists(list, "$", 0x34); // Shift + 4
        AddIfNotExists(list, "%", 0x35); // Shift + 5
        AddIfNotExists(list, "&", 0x36); // Shift + 6
        AddIfNotExists(list, "/", 0x37); // Shift + 7 (German keyboard uses / instead of *)
        AddIfNotExists(list, "(", 0x38); // Shift + 8
        AddIfNotExists(list, ")", 0x39); // Shift + 9
        AddIfNotExists(list, "=", 0x30); // Shift + - (German keyboard uses = instead of _)
        AddIfNotExists(list, "?", 0xDB); // Shift + / (German keyboard uses ? instead of /)
        AddIfNotExists(list, "{", 0x37); // Shift + {
        AddIfNotExists(list, "}", 0x30); // Shift + }
        AddIfNotExists(list, "[", 0x39); // AltGr + [
        AddIfNotExists(list, "]", 0x38); // AltGr + ]
        AddIfNotExists(list, "|", 0xDC); // Shift + \
        AddIfNotExists(list, "~", 0xDE); // Shift + ' (German keyboard uses ~ instead of ")
        AddIfNotExists(list, "<", 0xBC); // Shift + ,
        AddIfNotExists(list, ">", 0xBE); // Shift + .
        AddIfNotExists(list, "?", 0xBF); // Shift + / (German keyboard uses ? instead of /)
        // Add more as needed
        return list;
    }

    private static void AddIfNotExists(Dictionary<string, byte> dict, string key, byte value)
    {
        if (!dict.ContainsKey(key))
        {
            dict[key] = value;
        }
    }
}
