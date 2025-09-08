// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

string[] menuItems = { "Clients", "Extras", "Github", "Quit" };
int selected = 0;
bool running = true;
List<string> outputMessages = new()
{
    "[System] Initialized ADM",
    "[System] Welcome ",
    "[System] Source Available on Github"
};

void DrawPanel(int left, int top, int width, int height, string title, Action contentDrawer = null)
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

    // Title
    Console.SetCursorPosition(left + 2, top);
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write(title);
    Console.ResetColor();

    // Content
    contentDrawer?.Invoke();
}

void DrawMenuPanel()
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

void DrawCreditsPanel()
{
    int left = 25, top = 2, width = 32, height = 9;
    DrawPanel(left, top, width, height, " Credits ", () =>
    {
        Console.SetCursorPosition(left + 2, top + 2);
        Console.Write("csaba: Goat");
        Console.SetCursorPosition(left + 2, top + 3);
        Console.Write("Lyte: Rip");
    });
}

void DrawChangelogPanel()
{
    int left = 58, top = 2, width = 45, height = 9;
    DrawPanel(left, top, width, height, " Changelog ", () =>
    {
        Console.SetCursorPosition(left + 2, top + 2);
        Console.Write("Ported to C++ (From c#)");
        Console.SetCursorPosition(left + 2, top + 3);
    });
}

void DrawConsoleOutputPanel()
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

bool IsJavawRunning()
{
    try
    {
        return Process.GetProcessesByName("javaw").Any();
    }
    catch
    {
        return false;
    }
}
void OpenGithub()
{
    Process.Start("https://github.com/csabap2842");
}




void DrawClientsBox()
{
    string clientsRoot = Path.Combine(AppContext.BaseDirectory, "Clients");
    if (!Directory.Exists(clientsRoot))
    {
        outputMessages.Add("[Error] Clients folder not found.");
        DrawUI();
        return;
    }

    var clientFolders = Directory.GetDirectories(clientsRoot)
        .Select(Path.GetFileName)
        .ToArray();

    if (clientFolders.Length == 0)
    {
        outputMessages.Add("[Error] No clients found in Clients folder.");
        DrawUI();
        return;
    }

    int client = 0;
    int width = 60, height = Math.Max(8, clientFolders.Length + 6), left = 15, top = 5;
    bool inBox = true;

    void Draw()
    {
        Console.Clear();
        DrawPanel(left, top, width, height, " Clients ", () =>
        {
            for (int i = 0; i < clientFolders.Length; i++)
            {
                Console.SetCursorPosition(left + 4, top + 2 + i);
                if (i == client)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(clientFolders[i].PadRight(width - 8));
                Console.ResetColor();
            }
            Console.SetCursorPosition(left + 2, top + height - 2);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("↑/↓: Select  Enter: Run  Backspace/ESC: Back".PadRight(width - 4));
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
                if (client > 0) { client--; Draw(); }
                break;
            case ConsoleKey.DownArrow:
                if (client < clientFolders.Length - 1) { client++; Draw(); }
                break;
            case ConsoleKey.Enter:
                {
                    string clientName = clientFolders[client];
                    string clientFolder = Path.Combine(clientsRoot, clientName);

                    // Find a launchable file: .exe, start.cmd, or start.bat
                    string exePath = Directory.GetFiles(clientFolder, "*.exe").FirstOrDefault();
                    if (exePath == null)
                        exePath = Directory.GetFiles(clientFolder, "start.cmd").FirstOrDefault();
                    if (exePath == null)
                        exePath = Directory.GetFiles(clientFolder, "start.bat").FirstOrDefault();

                    if (!IsJavawRunning())
                    {
                        outputMessages.Add("[Error] javaw.exe is not running. Please start Minecraft first.");
                    }
                    else if (exePath != null && File.Exists(exePath))
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = exePath,
                                WorkingDirectory = clientFolder,
                                UseShellExecute = true
                            });
                            outputMessages.Add($"[System] Launched: {clientName}");
                        }
                        catch (Exception ex)
                        {
                            outputMessages.Add($"[Error] Failed to launch: {ex.Message}");
                        }
                    }
                    else
                    {
                        outputMessages.Add($"[Error] No launchable file found in: {clientFolder}");
                    }
                    inBox = false;
                }
                break;
            case ConsoleKey.Backspace:
            case ConsoleKey.Escape:
                inBox = false;
                break;
        }
    }
    DrawUI();
}

void DrawExtrasBox()
{
    string[] extras = { "ADM Dll injector" };
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
                outputMessages.Add($"[System] Selected extra: {extras[selectedExtra]}");
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

void DrawUI()
{
    Console.Clear();
    Console.CursorVisible = false;
    Console.Title = "ADM | MC Client Loader (Recode)";
    DrawMenuPanel();
    DrawCreditsPanel();
    DrawChangelogPanel();
    DrawConsoleOutputPanel();
}

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
            else
            {
                outputMessages.Add($"[System] {menuItems[selected]} selected.");
                DrawUI();
            }
            break;
        case ConsoleKey.Escape:
            running = false;
            break;
    }
}

Console.Clear();
Console.CursorVisible = true;
Console.WriteLine("Exiting");