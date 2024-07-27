using Godot;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Clipboard
{
    public static void SetText(string text)
    {
        if (OS.GetName() == "Windows")
        {
            SetTextWindows(text);
        }
        else if (OS.GetName() == "macOS")
        {
            SetTextMacOS(text);
        }
        else if (OS.GetName() == "X11")
        {
            SetTextUnix(text);
        }
        else
        {
            GD.PrintErr("Clipboard functionality is not implemented for this platform.");
        }
    }

    // Windows
    private static void SetTextWindows(string text)
    {
        OpenClipboard(IntPtr.Zero);
        EmptyClipboard();
        IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)((text.Length + 1) * 2));
        IntPtr target = GlobalLock(hGlobal);
        Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
        Marshal.WriteInt16(target + text.Length * 2, 0);
        GlobalUnlock(hGlobal);
        SetClipboardData(CF_UNICODETEXT, hGlobal);
        CloseClipboard();
        GlobalFree(hGlobal);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetClipboardData(uint uFormat, IntPtr hMem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EmptyClipboard();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalFree(IntPtr hMem);

    private const uint CF_UNICODETEXT = 13;
    private const uint GMEM_MOVEABLE = 0x0002;

    // macOS
    private static void SetTextMacOS(string text)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"echo '{EscapeBashArgument(text)}' | pbcopy\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var process = new Process { StartInfo = psi };
        process.Start();
        process.WaitForExit();
    }

    // Unix (Linux)
    private static void SetTextUnix(string text)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "sh",
            Arguments = $"-c \"echo -n {EscapeBashArgument(text)} | xclip -selection clipboard\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var process = new Process { StartInfo = psi };
        process.Start();
        process.WaitForExit();
    }

    private static string EscapeBashArgument(string argument)
    {
        return argument.Replace("'", "'\\''");
    }
}
