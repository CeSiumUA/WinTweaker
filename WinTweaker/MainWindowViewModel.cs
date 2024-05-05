using Microsoft.Win32;
using System.ComponentModel;
using System.Management;
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
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice");
        int usbPorts = 0;
        foreach (ManagementObject mo in searcher.Get())
        {
            usbPorts++;
        }

        return usbPorts.ToString();
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
        using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_Tcpip_NetworkInterface");
        foreach (ManagementObject mo in searcher.Get())
        {
            var downloadSpeed = (ulong)mo["BytesReceivedPersec"];
            return $"{downloadSpeed / 1024 / 1024} MB/s";
        }

        return Resources.EntryRetreiveErrorString;
    }
}
