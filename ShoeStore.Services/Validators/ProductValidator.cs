using ShoeStoreData.Models;

namespace ShoeStore.Services.Validators;

public static class ProductValidator
{
    public static void Validate(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product), "Товар не может быть пустым");

        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Название товара обязательно");

        if (product.Price <= 0)
            throw new ArgumentException("Цена товара должна быть больше нуля");

        if (product.SubcategoryId <= 0)
            throw new ArgumentException("Необходимо указать подкатегорию");

        if (string.IsNullOrWhiteSpace(product.Manufacturer))
            throw new ArgumentException("Производитель обязателен");
    }
}