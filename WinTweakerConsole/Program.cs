using Microsoft.Win32;
using System.Management;

var osString = System.Environment.OSVersion.ToString();

Console.WriteLine($"OS Version: {osString}");

var productName = (string?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", string.Empty);
var currentVersion = (string?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", string.Empty);
var currentBuild = (string?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", string.Empty);

Console.WriteLine("OS Name: " + productName);
Console.WriteLine("OS Version: " + currentVersion);
Console.WriteLine("OS Build: " + currentBuild);

ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
foreach (ManagementObject mo in searcher.Get())
{
    Console.WriteLine("Manufacturer: {0}", (string)mo["Manufacturer"]);
    Console.WriteLine("Product: {0}", (string)mo["Product"]);
}

searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
foreach (ManagementObject mo in searcher.Get())
{
    Console.WriteLine("Name: {0}", (string)mo["Name"]);
    Console.WriteLine("NumberOfCores: {0}", (uint)mo["NumberOfCores"]);
    Console.WriteLine("ClockSpeed: {0}", (uint)mo["MaxClockSpeed"]);
}

searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
foreach (ManagementObject mo in searcher.Get())
{
    Console.WriteLine("Name: {0}", (string)mo["Name"]);
    Console.WriteLine("DriverVersion: {0}", (string)mo["DriverVersion"]);
}

searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
foreach (ManagementObject mo in searcher.Get())
{
    Console.WriteLine("Name: {0}", (string)mo["Name"]);
    Console.WriteLine("ScreenHeight: {0}", (uint?)mo["ScreenHeight"] ?? 0);
    Console.WriteLine("ScreenWidth: {0}", (uint?)mo["ScreenWidth"] ?? 0);
}