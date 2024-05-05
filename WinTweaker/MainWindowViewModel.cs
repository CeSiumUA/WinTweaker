﻿using Microsoft.Win32;
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
    private string _osVersionInfo = Resources.CalculatingValueString;
    private string _motherBoardInfo = Resources.CalculatingValueString;
    private string _cpuInfo = Resources.CalculatingValueString;
    private string _gpuInfo = Resources.CalculatingValueString;
    private string _monitorInfo = Resources.CalculatingValueString;
    private string _ramInfo = Resources.CalculatingValueString;
    private string _storageInfo = Resources.CalculatingValueString;
    private string _usbPortsInfo = Resources.CalculatingValueString;
    private string _uptimeInfo = Resources.CalculatingValueString;
    private string _ipAddressInfo = Resources.CalculatingValueString;
    private string _downloadSpeedInfo = Resources.CalculatingValueString;
    private string _pingInfo = Resources.CalculatingValueString;

    private List<Task> _tasks = new();

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

    public string OsVersionInfo { get => _osVersionInfo; set { _osVersionInfo = value; OnPropertyChanged(nameof(OsVersionInfo)); } }
    public string MotherBoardInfo { get => _motherBoardInfo; set { _motherBoardInfo = value; OnPropertyChanged(nameof(MotherBoardInfo)); } }
    public string CpuInfo { get => _cpuInfo; set { _cpuInfo = value; OnPropertyChanged(nameof(CpuInfo)); } }
    public string GpuInfo { get => _gpuInfo; set { _gpuInfo = value; OnPropertyChanged(nameof(GpuInfo)); } }
    public string MonitorInfo { get => _monitorInfo; set { _monitorInfo = value; OnPropertyChanged(nameof(MonitorInfo)); } }
    public string RamInfo { get => _ramInfo; set { _ramInfo = value; OnPropertyChanged(nameof(RamInfo)); } }
    public string StorageInfo { get => _storageInfo; set { _storageInfo = value; OnPropertyChanged(nameof(StorageInfo)); } }
    public string UsbPortsInfo { get => _usbPortsInfo; set { _usbPortsInfo = value; OnPropertyChanged(nameof(UsbPortsInfo)); } }
    public string UptimeInfo { get => _uptimeInfo; set { _uptimeInfo = value; OnPropertyChanged(nameof(UptimeInfo)); } }
    public string IpAddressInfo { get => _ipAddressInfo; set { _ipAddressInfo = value; OnPropertyChanged(nameof(IpAddressInfo)); } }
    public string DownloadSpeedInfo { get => _downloadSpeedInfo; set { _downloadSpeedInfo = value; OnPropertyChanged(nameof(DownloadSpeedInfo)); } }
    public string PingInfo { get => _pingInfo; set { _pingInfo = value; OnPropertyChanged(nameof(PingInfo)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void CalculateValues()
    {
        var osVersionTask = new Task(() => OsVersionInfo = GetOsVersion());
        _tasks.Add(osVersionTask);

        var motherBoardTask = new Task(() => MotherBoardInfo = GetMotherBoardInfo());
        _tasks.Add(motherBoardTask);

        var cpuTask = new Task(() => CpuInfo = GetCpuInfo());
        _tasks.Add(cpuTask);

        var gpuTask = new Task(() => GpuInfo = GetGpuInfo());
        _tasks.Add(gpuTask);

        var monitorTask = new Task(() => MonitorInfo = GetMonitorInfo());
        _tasks.Add(monitorTask);

        var ramTask = new Task(() => RamInfo = GetRamInfo());
        _tasks.Add(ramTask);

        var storageTask = new Task(() => StorageInfo = GetStorageInfo());
        _tasks.Add(storageTask);

        var usbPortsTask = new Task(() => UsbPortsInfo = GetUsbPortsInfo());
        _tasks.Add(usbPortsTask);

        var uptimeTask = new Task(() => UptimeInfo = GetUptimeInfo());
        _tasks.Add(uptimeTask);

        var ipAddressTask = new Task(() => IpAddressInfo = GetIpAddressInfo());
        _tasks.Add(ipAddressTask);

        var downloadSpeedTask = new Task(() => DownloadSpeedInfo = GetDownloadSpeedInfo());
        _tasks.Add(downloadSpeedTask);

        var pingTask = new Task(() => PingInfo = GetPingInfo());
        _tasks.Add(pingTask);

        _tasks.ForEach(t => t.Start());
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
        var monitorEntries = searcher.Get();
        string monitorName = string.Empty;
        foreach (ManagementObject mo in monitorEntries)
        {
            monitorName = (string?)mo["Name"] ?? string.Empty;
        }

        var screen = System.Windows.Forms.Screen.PrimaryScreen;

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
        const string filePath = ".512MB.zip";
        try
        {
            File.Delete(filePath);

            using WebClient client = new WebClient();
            Stopwatch stopwatch = new Stopwatch();

            string url = Settings.Default.DownloadSpeedTestingHost;

            stopwatch.Restart();
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
