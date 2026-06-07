namespace SmartBarcodePOS_Pro.Services;

public interface IBarcodeService
{
    string GenerateCode128Barcode(string sku);
}
