using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using SunmiV2MAUI.Platforms.Android;

namespace SunmiV2MAUI;
public partial class MainPage : ContentPage
{
    BluetoothPrinterService _bluetoothPrinterService = new BluetoothPrinterService();
    public MainPage()
	{
		InitializeComponent();
	}

    private void PrintQrButton_Clicked(object sender, EventArgs e)
    {
        byte[] qrBytes = System.Text.Encoding.ASCII.GetBytes("Hello MAUI");
        int dataLength = qrBytes.Length + 3;
        byte dataPL = (byte)(dataLength % 256);
        byte dataPH = (byte)(dataLength / 256);
        var bytes = new List<byte>();
        bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00 }); // Select model
        bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x43, 0x08 });  // Set module size (8)
        bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x45, 0x30 });  // Set error correction
        bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, dataPL, dataPH, 0x31, 0x50, 0x30 }); // Start store qr data.
        bytes.AddRange(qrBytes);
        bytes.AddRange(new byte[] { 0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x51, 0x30 }); // Print qr data from previous 80 code.
#if ANDROID
        _bluetoothPrinterService.PrintQR(bytes);
#endif
    }

    private void PrintTextButton_Clicked(object sender, EventArgs args)
    {
        var e = new EPSON();
        var buffer = ByteSplicer.Combine(
            e.CenterAlign(),
            e.SetStyles(PrintStyle.Bold),
            e.PrintLine("Hello MAUI"),
            e.SetStyles(PrintStyle.None),
            e.CenterAlign(),
            e.PrintLine("Native mobile and desktop apps with C# and XAML"),
            e.PrintLine("Run on Android, iOS, macOS, and Windows"),
            e.PrintLine(".................................")            
            );
#if ANDROID
        _bluetoothPrinterService.Print(buffer);
#endif

    }

    private void PrintBarCode_Clicked(object sender, EventArgs args)
    {
        var e = new EPSON();
        var buffer = ByteSplicer.Combine(
            e.CenterAlign(),
            e.SetBarcodeHeightInDots(48),
            e.PrintBarcode(BarcodeType.CODE39, "HELLO MAUI"),
            e.PrintLine("") );
#if ANDROID
        _bluetoothPrinterService.Print(buffer);
#endif       
    }

    private async void SelectPrinter_Clicked(object sender, EventArgs args)
    {
        await Navigation.PushAsync(new SelectPrinter());
    }

}

