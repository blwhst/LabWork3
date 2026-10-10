using ShoeStoreData.Models;
using ShoeStoreException;

namespace ShoeStore.Services.Validators;

public static class ProductValidator
{
    public static void Validate(Product product)
    {
        if (product == null)
            throw Exceptions.ProductIsNull();

        if (string.IsNullOrWhiteSpace(product.Name))
            throw Exceptions.ProductNameRequired();

        if (product.Price <= 0)
            throw Exceptions.InvalidPrice(product.Price);

        if (product.SubcategoryId <= 0)
            throw Exceptions.SubcategoryRequired();

        if (string.IsNullOrWhiteSpace(product.Manufacturer))
            throw Exceptions.ManufacturerRequired();
    }
}