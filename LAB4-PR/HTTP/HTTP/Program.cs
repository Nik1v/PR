using Newtonsoft.Json;
using System.Text;
using UtmShop.Dto;

var client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:44370/api/");

bool exit = false;
while (!exit)
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("     1. Enumerate the list of categories");
    Console.WriteLine("     2. Details about a category");
    Console.WriteLine("     3. Create a new category");
    Console.WriteLine("     4. Delete a category");
    Console.WriteLine("     5. Change the title of a category");
    Console.WriteLine("     6. Create a new product in a category");
    Console.WriteLine("     7. List of products in a category");
    Console.WriteLine("     0. Exit");

    Console.Write("\nEnter the desired option: ");
    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            await EnumerateCategories();
            break;

        case "2":
            await DisplayCategoryDetails();
            break;

        case "3":
            await CreateNewCategory();
            break;

        case "4":
            await DeleteCategory();
            break;

        case "5":
            await ModifyCategoryTitle();
            break;

        case "6":
            await CreateNewProduct();
            break;

        case "7":
            await ListProductsInCategory();
            break;

        case "0":
            exit = true;
            break;

        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }
}

async Task EnumerateCategories()
{
    var response = await client.GetAsync("Category/categories");
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        var categories = JsonConvert.DeserializeObject<CategoryShortDto[]>(content);

        foreach (var category in categories!)
        {
            Console.WriteLine($"  {category.Id}.{category.Name}: {category.ItemsCount}");
        }
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}

async Task DisplayCategoryDetails()
{
    Console.Write("\nEnter category id: ");
    var categoryId = Console.ReadLine();

    var response = await client.GetAsync($"Category/categories/{categoryId}/products");
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<List<ProductShortDto>>(content);

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id}.{product.Title} = {product.Price}");
        }
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine($"The category with ID {categoryId} was not found.");
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}

async Task CreateNewCategory()
{
    Console.Write("\nEnter title: ");
    var title = Console.ReadLine();

    var newCategory = new CreateCategoryDto { Title = title! };
    var serializedCategory = JsonConvert.SerializeObject(newCategory);
    var content = new StringContent(serializedCategory, Encoding.UTF8, "application/json");

    var response = await client.PostAsync("Category/categories", content);
    if (response.IsSuccessStatusCode)
    {
        var createdCategory = await response.Content.ReadAsStringAsync();
        var category = JsonConvert.DeserializeObject<CategoryShortDto>(createdCategory);

        Console.WriteLine($"The category with ID {category!.Id} and name {category.Name} was successfully created.");
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}

async Task DeleteCategory()
{
    Console.Write("\nEnter the ID of the category you want to delete: ");
    var categoryId = Console.ReadLine();

    var response = await client.DeleteAsync($"Category/categories/{categoryId}");

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"The category with ID {categoryId} was successfully deleted.");
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}

async Task ModifyCategoryTitle()
{
    Console.Write("\nEnter the ID of the category whose title you want to change: ");
    var categoryId = Console.ReadLine();

    var response = await client.GetAsync($"Category/categories/{categoryId}");
    if (!response.IsSuccessStatusCode)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"The category with ID {categoryId} was not found.");
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
        return;
    }

    Console.Write("\nEnter the new category title: ");
    var newTitle = Console.ReadLine();

    var categoryToUpdate = new CreateCategoryDto { Title = newTitle! };
    var serializedCategoryToUpdate = JsonConvert.SerializeObject(categoryToUpdate);
    var content = new StringContent(serializedCategoryToUpdate, Encoding.UTF8, "application/json");

    response = await client.PutAsync($"Category/{categoryId}", content);
    if (response.IsSuccessStatusCode)
    {
        var updatedCategory = await response.Content.ReadAsStringAsync();
        var category = JsonConvert.DeserializeObject<CategoryShortDto>(updatedCategory);

        Console.WriteLine($"The category with ID {category!.Id} has been successfully updated with the new title {category.Name}.");
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}

async Task CreateNewProduct()
{
    Console.Write("\nAdd a new product to a category. Enter Category ID: ");
    var categoryId = Console.ReadLine();

    if (!long.TryParse(categoryId, out long categoryIdParsed))
    {
        Console.WriteLine("The ID entered is not valid. Please enter a number.");
        return;
    }

    Console.Write("\nEnter the product name: ");
    var productName = Console.ReadLine();

    Console.Write("\nEnter the price of the product: ");
    var productPrice = Console.ReadLine();

    if (!decimal.TryParse(productPrice, out decimal productPriceParsed))
    {
        Console.WriteLine("The price entered is not valid. Please enter a number.");
        return;
    }

    var newProduct = new ProductShortDto
    {
        Title = productName!,
        Price = productPriceParsed
    };

    var serializedProduct = JsonConvert.SerializeObject(newProduct);
    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");

    var response = await client.PostAsync($"Category/categories/{categoryIdParsed}/products", content);

    if (response.IsSuccessStatusCode)
    {
        var createdProduct = await response.Content.ReadAsStringAsync();
        var product = JsonConvert.DeserializeObject<ProductShortDto>(createdProduct);

        Console.WriteLine($"The product with ID {product!.Id} and name {product.Title} was successfully created.");
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine($"The category with ID {categoryIdParsed} was not found.");
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}

async Task ListProductsInCategory()
{
    Console.Write("\nEnter the category ID to view the products: ");
    var categoryId = Console.ReadLine();

    var response = await client.GetAsync($"Category/categories/{categoryId}/products");
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<IList<ProductShortDto>>(content);

        foreach (var product in products!)
        {
            Console.WriteLine($"{product.Id}.{product.Title} = {product.Price}");
        }
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine($"The category with ID {categoryId} was not found.");
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}
