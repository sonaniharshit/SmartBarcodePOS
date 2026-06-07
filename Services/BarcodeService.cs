using BarcodeStandard;
using SkiaSharp;

namespace SmartBarcodePOS_Pro.Services;

public class BarcodeService : IBarcodeService
{
    private readonly IWebHostEnvironment _environment;

    public BarcodeService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string GenerateCode128Barcode(string sku)
    {
        var folder = Path.Combine(_environment.WebRootPath, "barcodes");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = $"{sku}.png";
        var filePath = Path.Combine(folder, fileName);

        var barcode = new Barcode();

        using var image = barcode.Encode(
            BarcodeStandard.Type.Code128,
            sku,
            SKColors.Black,
            SKColors.White,
            360,
            120
        );

        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.Open(filePath, FileMode.Create);
        data.SaveTo(stream);

        return $"/barcodes/{fileName}";
    }
}
