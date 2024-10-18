using ProductService.Models;

namespace ProductService.Validators;

public static class Validator
{
    public static bool IsValidUUID(string id)
    {
        return Guid.TryParse(id, out _);
    }

    public static bool IsValidPrice(decimal price)
    {
        return price >= 0;
    }

    public static bool IsValidDescription(string description)
    {
        return !string.IsNullOrWhiteSpace(description);
    }

    public static bool IsValidStock(int stock)
    {
        return stock >= 0;
    }

    public static bool AreValidCategories(List<Category> categories)
    {
        if (categories == null || categories.Count == 0)
            return false;

        return categories.All(category => IsValidUUID(category.Id) && !string.IsNullOrWhiteSpace(category.Name));
    }
}