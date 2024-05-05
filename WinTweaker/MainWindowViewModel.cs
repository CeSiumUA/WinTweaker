using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using WinTweaker.Properties;

namespace WinTweaker;

public class MainWindowViewModel : INotifyPropertyChanged
{

    public string SystemInfoLabelText => Resources.SystemInformationString;
    public string OsVersionLabelText => Resources.OperatingSystemString;
    public string MotherBoardLabelText => Resources.MotherBoardString;
    public string CpuLabelText => Resources.ProcessorString;
    public string GpuLabelText => Resources.GraphicsCardString;
    public string MonitorLabelText => Resources.MonitorString;
    public string RamLabelText => Resources.RandomAccessMemoryString;
    public string StorageLabelText => Resources.StorageString;
    public string UsbPortsLabelText => Resources.UsbString;
    public string UptimeLabelText => Resources.UptimeString;
    public string IpAddressLabelText => Resources.IpAddressString;
    public string DownloadSpeedLabelText => Resources.DownloadSpeedString;
    public string PingLabelText => Resources.PingString;

    public string OsVersionInfo => GetOsVersion();
    public string MotherBoardInfo => GetMotherBoardInfo();
    public string CpuInfo => GetCpuInfo();
    public string GpuInfo => GetGpuInfo();
    public string MonitorInfo => GetMonitorInfo();
    public string RamInfo => GetRamInfo();
    public string StorageInfo => GetStorageInfo();
    public string UsbPortsInfo => GetUsbPortsInfo();
    public string UptimeInfo => GetUptimeInfo();
    public string IpAddressInfo => GetIpAddressInfo();
    public string DownloadSpeedInfo => GetDownloadSpeedInfo();
    public string PingInfo => GetPingInfo();

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string GetOsVersion()
    {
        var productName = (string?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", string.Empty);
        var currentVersion = (string?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", string.Empty);
        var currentBuild = (string?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", string.Empty);

        if(productName == null || currentVersion == null || currentBuild == null)
        {
            return Resources.EntryRetreiveErrorString;
        }

        return $"{productName} {currentVersion} (Build {currentBuild})";
    }

    private string GetMotherBoardInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
        foreach (ManagementObject mo in searcher.Get())
        {
            var mbManufacturer = (string?)mo["Manufacturer"];
            var mbProduct = (string?)mo["Product"];
            if(mbManufacturer == null || mbProduct == null)
            {
                return Resources.EntryRetreiveErrorString;
            }
            return $"{mbManufacturer} {mbProduct}";
        }

        return Resources.EntryRetreiveErrorString;
    }

    private string GetCpuInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        foreach (ManagementObject mo in searcher.Get())
        {
            var cpuName = (string?)mo["Name"];
            var cpuCores = (uint?)mo["NumberOfCores"];
            var cpuClockSpeed = (uint?)mo["MaxClockSpeed"];
            if(cpuName == null || cpuCores == null || cpuClockSpeed == null)
            {
                return Resources.EntryRetreiveErrorString;
            }
            return $"{cpuName} ({cpuCores} Cores @ {cpuClockSpeed} MHz)";
        }

        return Resources.EntryRetreiveErrorString;
    }

    private string GetGpuInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
        foreach (ManagementObject mo in searcher.Get())
        {
            var gpuName = (string?)mo["Name"];
            var gpuDriverVersion = (string?)mo["DriverVersion"];
            if(gpuName == null || gpuDriverVersion == null)
            {
                return Resources.EntryRetreiveErrorString;
            }
            return $"{gpuName} ({gpuDriverVersion})";
        }

        return Resources.EntryRetreiveErrorString;
    }

    private string GetMonitorInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
        foreach (ManagementObject mo in searcher.Get())
        {
            var monitorName = (string?)mo["Name"];
            var screenHeight = (uint?)mo["ScreenHeight"];
            var screenWidth = (uint?)mo["ScreenWidth"];
            if(monitorName == null || screenHeight == null || screenWidth == null)
            {
                return Resources.EntryRetreiveErrorString;
            }
            return $"{monitorName} ({screenWidth}x{screenHeight})";
        }

        return Resources.EntryRetreiveErrorString;
    }

    private string GetRamInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
        ulong totalRam = 0;
        foreach (ManagementObject mo in searcher.Get())
        {
            totalRam += (ulong)mo["Capacity"];
        }

        return $"{totalRam / 1024 / 1024 / 1024} GB";
    }

    private string GetStorageInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
        ulong totalStorage = 0;
        foreach (ManagementObject mo in searcher.Get())
        {
            totalStorage += (ulong)mo["Size"];
        }

        return $"{totalStorage / 1024 / 1024 / 1024} GB";
    }

    private string GetUsbPortsInfo()
    {
        using var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub");
        int usbPorts = searcher.Get().Count;

        var removableDrivesCount = DriveInfo.GetDrives().Count(d => d.DriveType == DriveType.Removable);

        return $"{Resources.ConnectedUsbDevicesString} - {usbPorts} ({Resources.RemovableUsbDevicesString} - {removableDrivesCount})";
    }

    private string GetUptimeInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        foreach (ManagementObject mo in searcher.Get())
        {
            var uptime = (string)mo["LastBootUpTime"];
            return $"{uptime} Days";
        }

        return Resources.EntryRetreiveErrorString;
    }

    private string GetIpAddressInfo()
    {
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
        foreach (ManagementObject mo in searcher.Get())
        {
            var ipAddresses = (string[])mo["IPAddress"];
            return ipAddresses[0];
        }

        return Resources.EntryRetreiveErrorString;
    }

    private string GetDownloadSpeedInfo()
    {
        try
        {
            using WebClient client = new WebClient();
            Stopwatch stopwatch = new Stopwatch();

            string url = Settings.Default.DownloadSpeedTestingHost; // URL of a file to download
            string filePath = ".512MB.zip"; // Local file path to save the file

            stopwatch.Start();
            client.DownloadFile(url, filePath);
            stopwatch.Stop();

            FileInfo fileInfo = new FileInfo(filePath);
            long fileSizeInBytes = fileInfo.Length;
            double fileSizeInKb = fileSizeInBytes / 1024.0;
            double fileSizeInMb = fileSizeInKb / 1024.0;

            double secondsPassed = stopwatch.Elapsed.TotalSeconds;
            double speedInKbps = fileSizeInKb / secondsPassed;
            double speedInMbps = fileSizeInMb / secondsPassed;

            File.Delete(filePath);

            if (speedInMbps < 1)
            {
                return $"{speedInKbps:0.00} Kbps";
            }
            else
            {
                return $"{speedInMbps:0.00} Mbps";
            }
        }
        catch (Exception)
        {
            return Resources.EntryRetreiveErrorString;
        }
    }

    private string GetPingInfo()
    {
        var pingSender = new Ping();
        var testingHost = Settings.Default.PingTestingHost;
        var reply = pingSender.Send(testingHost);

        if(reply.Status == IPStatus.Success)
        {
            return $"ping = {reply.RoundtripTime} ms ({testingHost})";
        }

        return Resources.EntryRetreiveErrorString;
    }
}
