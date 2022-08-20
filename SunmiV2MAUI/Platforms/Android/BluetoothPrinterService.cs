using Android.Bluetooth;
using Android.Content;
using SunmiV2MAUI.Model;
using AndroidApp = Android.App.Application;

namespace SunmiV2MAUI.Platforms.Android
{
    public class BluetoothPrinterService
    {       
        private BluetoothDevice _connectedDevice;
        
        public List<BluetoothDeviceInfo> GetAvailableDevices()
        {
            BluetoothManager bluetoothManager = (BluetoothManager)AndroidApp.Context.GetSystemService(Context.BluetoothService);
            if (bluetoothManager.Adapter != null && bluetoothManager.Adapter.IsEnabled)
            {
                List<BluetoothDeviceInfo> result = new List<BluetoothDeviceInfo>();
                foreach (var pairedDevice in bluetoothManager.Adapter.BondedDevices)
                {
                    result.Add(new BluetoothDeviceInfo
                    {
                        Title = pairedDevice.Name,
                        MacAddress = pairedDevice.Address
                    });
                }
                return result;
            }
            return null;
        }

        public BluetoothDeviceInfo GetCurrentDevice()
        {
            if (_connectedDevice != null)
            {
                return new BluetoothDeviceInfo
                {
                    Title = _connectedDevice.Name,
                    MacAddress = _connectedDevice.Address
                };
            }
            return null;
        }

        public bool SetCurrentDevice(string printerName)
        {
            BluetoothManager bluetoothManager = (BluetoothManager)AndroidApp.Context.GetSystemService(Context.BluetoothService);

            if (bluetoothManager.Adapter != null && bluetoothManager.Adapter.IsEnabled)
            {
                foreach (var pairedDevice in bluetoothManager.Adapter.BondedDevices)
                {
                    if (pairedDevice.Name == printerName)
                    {
                        _connectedDevice = pairedDevice;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsBluetoothEnabled()
        {
            try
            {
                BluetoothManager bluetoothManager = (BluetoothManager)AndroidApp.Context.GetSystemService(Context.BluetoothService);
                if (bluetoothManager.Adapter != null && bluetoothManager.Adapter.IsEnabled)
                {
                    return true;
                }
                else
                {
                    bluetoothManager.Adapter.Enable();
                    while (!bluetoothManager.Adapter.IsEnabled)
                    {
                        System.Threading.Thread.Sleep(30);
                    }
                    return true;
                }
            }
            catch 
            {
                return false;
            }
        }
        public void Print(byte[] data)
        {
            var btAdapter = BluetoothUtil.GetBTAdapter();
            var status = IsBluetoothEnabled();
            if (status)
            {
                var device = BluetoothUtil.GetDevice(btAdapter);
                var socket = BluetoothUtil.GetSocket(device);
                BluetoothUtil.PrintData(data, socket);
            }

        }
        public async void PrintQR(List<byte> bytes)
        {
            var btAdapter = BluetoothUtil.GetBTAdapter();
            var status = IsBluetoothEnabled();
            if (status)
            {
                var device = BluetoothUtil.GetDevice(btAdapter);
                var socket = BluetoothUtil.GetSocket(device);
                await BluetoothUtil.PrintQR(bytes, socket);
            }

        }
    }
}
