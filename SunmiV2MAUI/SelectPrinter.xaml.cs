using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using SunmiV2MAUI.Platforms.Android;

namespace SunmiV2MAUI;

public partial class SelectPrinter : ContentPage
{
    BluetoothPrinterService _bluetoothPrinterService = new BluetoothPrinterService();
    public SelectPrinter()
	{
		InitializeComponent();
	}

    private async void SelectPrinterButton_Clicked(object sender, EventArgs e)
    {
        
        if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Model == "V2")
        {
            if (_bluetoothPrinterService.IsBluetoothEnabled())
            {
                var devices = _bluetoothPrinterService.GetAvailableDevices();
                if (devices != null && devices.Count > 0)
                {
                    var choices = devices.Select(d => d.Title).ToArray();
                    string action = await Application.Current.MainPage.DisplayActionSheet("Select printer device.", "Cancel", null, choices);
                    if (choices.Contains(action))
                    {
                        SelectDevice(action);
                    }
                }
                else
                {
                    await DisplayAlert("Select Printer", "No device.", "OK");
                }
            }
            else
            {
                await DisplayAlert("No bluetooth", "Please turn bluetooth on", "OK");
            }
        }
    }

    private void PrintQrButton_Clicked(object sender, EventArgs e)
    {
        var epson = new EPSON();
        var buffer = ByteSplicer.Combine(
            epson.CenterAlign(),
            epson.SetBarcodeHeightInDots(48),
            epson.PrintBarcode(BarcodeType.CODE39, printBox.Text),
            epson.PrintLine(""));
#if ANDROID
        _bluetoothPrinterService.Print(buffer);
#endif  
       
    }

    private void PrintTextButton_Clicked(object sender, EventArgs args)
    {
        var e = new EPSON();
        var buffer = ByteSplicer.Combine(
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine(printBox.Text) );
#if ANDROID
        _bluetoothPrinterService.Print(buffer);
#endif 
    }

    void SelectDevice(string printerName)
    {
        if (_bluetoothPrinterService.SetCurrentDevice(printerName))
        {
            var current = _bluetoothPrinterService.GetCurrentDevice();
            if (current != null)
            {
                printerNameEntry.Text = current.Title;
                printQrButton.IsEnabled = printTextButton.IsEnabled = true;
            }
        }
    }
}
