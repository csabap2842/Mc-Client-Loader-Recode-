using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program
{
    static string[] menuItems = { "Clients", "Extras", "Github", "Quit" };
    static int selected = 0;
    static bool running = true;

    // console output
    static List<string> outputMessages = new()
    {
        "[System] Welcome",
        "[System] Creds to luvsy ",
    };

    // paths
    static readonly string baseDir = AppContext.BaseDirectory;
    static readonly string clientsDir = Path.Combine(baseDir, "Clients");

    static Dictionary<string, Dictionary<string, string>> clientCategories = new()
    {
        { "1.7+", new Dictionary<string, string>
            {
                { "VapeV4.16", Path.Combine(clientsDir, "16.exe") },
                { "VapeV4.17", Path.Combine(clientsDir, "17.exe") },
                { "VapeV4.18", Path.Combine(clientsDir, "start.cmd") }
            }
        },
        { "1.9+", new Dictionary<string, string>
            {
                
            }
        }
    };

    static void DrawPanel(int left, int top, int width, int height, string title, Action contentDrawer = null)
    {
        Console.SetCursorPosition(left, top);
        Console.Write("┌" + new string('─', width - 2) + "┐");
        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(left, top + i);
            Console.Write("│" + new string(' ', width - 2) + "│");
        }
        Console.SetCursorPosition(left, top + height - 1);
        Console.Write("└" + new string('─', width - 2) + "┘");

        Console.SetCursorPosition(left + 2, top);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(title);
        Console.ResetColor();
        contentDrawer?.Invoke();
    }

    static void DrawMenu()
    {
        int left = 2, top = 2, width = 22, height = 8;
        DrawPanel(left, top, width, height, " Commands ", () =>
        {
            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.SetCursorPosition(left + 2, top + 2 + i);
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(menuItems[i].PadRight(width - 4));
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(menuItems[i].PadRight(width - 4));
                }
            }
        });
    }

    static void DrawCredits()
    {
        int left = 25, top = 2, width = 32, height = 9;
        DrawPanel(left, top, width, height, " Credits ", () =>
        {
            Console.SetCursorPosition(left + 2, top + 2);
            Console.Write("csaba: Main Programmer");
            Console.SetCursorPosition(left + 2, top + 3);
            Console.Write("Zero: Made the original");
        });
    }

    static void DrawChangelog()
    {
        int left = 58, top = 2, width = 45, height = 9;
        DrawPanel(left, top, width, height, " Changelog ", () =>
        {
            Console.SetCursorPosition(left + 2, top + 2);
            Console.Write("V1.0 - Added Vape versions (4.16 - 4.18)");
            Console.SetCursorPosition(left + 2, top + 3);
            Console.Write("V1.01 Reduced File size + Improved code");
        });
    }

    static void DrawConsole()
    {
        int left = 2, top = 12, width = 101, height = 8;
        DrawPanel(left, top, width, height, " Console Output ", () =>
        {
            int maxLines = height - 4;
            int start = Math.Max(0, outputMessages.Count - maxLines);
            for (int i = 0; i < Math.Min(outputMessages.Count, maxLines); i++)
            {
                Console.SetCursorPosition(left + 2, top + 2 + i);
                Console.Write(outputMessages[start + i].PadRight(width - 4));
            }
            Console.SetCursorPosition(left + 2, top + height - 2);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Use ↑/↓ to navigate, Enter to select, ESC to exit.".PadRight(width - 4));
            Console.ResetColor();
        });
    }

    static void DrawClientsBox()
    {
        string[] categories = new string[clientCategories.Keys.Count];
        clientCategories.Keys.CopyTo(categories, 0);

        int selectedCategory = 0;
        int selectedClient = 0;
        bool inBox = true;

        void Draw()
        {
            Console.Clear();
            int width = 70, height = 20, left = 15, top = 3;

            DrawPanel(left, top, width, height, " Clients ", () =>
            {
                int catLeft = left + 2;
                foreach (var category in categories)
                {
                    Console.SetCursorPosition(catLeft, top + 2);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(category);
                    Console.ResetColor();

                    int i = 0;
                    foreach (var client in clientCategories[category])
                    {
                        Console.SetCursorPosition(catLeft, top + 4 + i);
                        if (categories[selectedCategory] == category && i == selectedClient)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.Write(client.Key.PadRight(20));
                        Console.ResetColor();
                        i++;
                    }

                    catLeft += 30;
                }

                Console.SetCursorPosition(left + 2, top + height - 2);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("↑/↓: Select client  →/←: Switch category  Enter: Run  Backspace/ESC: Back".PadRight(width - 4));
                Console.ResetColor();
            });
        }

        Draw();
        while (inBox)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedClient > 0) selectedClient--;
                    Draw();
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedClient < clientCategories[categories[selectedCategory]].Count - 1) selectedClient++;
                    Draw();
                    break;
                case ConsoleKey.LeftArrow:
                    if (selectedCategory > 0) { selectedCategory--; selectedClient = 0; }
                    Draw();
                    break;
                case ConsoleKey.RightArrow:
                    if (selectedCategory < categories.Length - 1) { selectedCategory++; selectedClient = 0; }
                    Draw();
                    break;
                case ConsoleKey.Enter:
                    var chosenClient = clientCategories[categories[selectedCategory]];
                    int idx = 0;
                    string exePath = "";
                    string clientName = "";
                    foreach (var kv in chosenClient)
                    {
                        if (idx == selectedClient)
                        {
                            clientName = kv.Key;
                            exePath = kv.Value;
                        }
                        idx++;
                    }

                    if (!File.Exists(exePath))
                    {
                        outputMessages.Add($"[Error] Missing file: {exePath}. Please check your Clients folder.");
                    }
                    else
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = exePath,
                                WorkingDirectory = Path.GetDirectoryName(exePath),
                                UseShellExecute = true
                            });
                            outputMessages.Add($"[System] Launched: {clientName}");
                        }
                        catch (Exception ex)
                        {
                            outputMessages.Add($"[Error] Failed to launch {clientName}: {ex.Message}");
                        }
                    }
                    inBox = false;
                    break;
                case ConsoleKey.Backspace:
                case ConsoleKey.Escape:
                    inBox = false;
                    break;
            }
        }
        DrawUI();
    }

    static void DrawExtrasBox()
    {
        string[] extras = { "ADM Dll injector (Soon maybe never)" };
        int selectedExtra = 0;
        int width = 40, height = 7, left = 25, top = 7;
        bool inBox = true;

        void Draw()
        {
            Console.Clear();
            DrawPanel(left, top, width, height, " Extras ", () =>
            {
                for (int i = 0; i < extras.Length; i++)
                {
                    Console.SetCursorPosition(left + 4, top + 2 + i);
                    if (i == selectedExtra)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(extras[i].PadRight(width - 8));
                    Console.ResetColor();
                }
                Console.SetCursorPosition(left + 2, top + height - 2);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Enter: Select  Backspace/ESC: Back".PadRight(width - 4));
                Console.ResetColor();
            });
        }

        Draw();
        while (inBox)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedExtra > 0) { selectedExtra--; Draw(); }
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedExtra < extras.Length - 1) { selectedExtra++; Draw(); }
                    break;
                case ConsoleKey.Enter:
                    break;
                case ConsoleKey.Backspace:
                case ConsoleKey.Escape:
                    inBox = false;
                    break;
            }
        }
        DrawUI();
    }

    static void DrawUI()
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.Title = "ADM | MC Client Loader (Recode)";
        DrawMenu();
        DrawCredits();
        DrawChangelog();
        DrawConsole();
    }

    static void Main()
    {
        DrawUI();

        while (running)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selected = (selected == 0) ? menuItems.Length - 1 : selected - 1;
                    DrawUI();
                    break;
                case ConsoleKey.DownArrow:
                    selected = (selected == menuItems.Length - 1) ? 0 : selected + 1;
                    DrawUI();
                    break;
                case ConsoleKey.Enter:
                    if (menuItems[selected] == "Quit")
                    {
                        running = false;
                    }
                    else if (menuItems[selected] == "Clients")
                    {
                        DrawClientsBox();
                    }
                    else if (menuItems[selected] == "Extras")
                    {
                        DrawExtrasBox();
                    }
                    else if (menuItems[selected] == "Github")
                    {
                        string githubUrl = "https://github.com/csabap2842/Mc-Client-Loader-Recode-/";
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = githubUrl,
                                UseShellExecute = true
                            });
                            outputMessages.Add($"[System] Opening GitHub:");
                        }
                        catch (Exception ex)
                        {
                            outputMessages.Add($"[Error] Could not open GitHub: {ex.Message}");
                        }
                        DrawUI();
                    }
                    break;
                case ConsoleKey.Escape:
                    running = false;
                    break;
            }
        }

        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine("Exiting");
    }
}
