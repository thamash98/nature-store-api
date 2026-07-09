using CeyloneNature.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CeyloneNature.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, RoleManager<IdentityRole<int>> roleManager, UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        foreach (var role in new[] { "admin", "customer" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        var adminEmail = config["Seed:AdminEmail"] ?? "admin@ceylonnature.com";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin@12345";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, Name = "Store Admin", EmailConfirmed = true };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "admin");
        }

        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Herbal Teas", Slug = "herbal-teas", Image = "https://images.unsplash.com/photo-1563911892437-1feda0179e1b?w=600&q=80", Featured = true },
                new Category { Name = "Natural Powders", Slug = "organic-powders", Image = "https://images.unsplash.com/photo-1615485500704-8e990f9900f7?w=600&q=80", Featured = true },
                new Category { Name = "Spices", Slug = "spices", Image = "https://images.unsplash.com/photo-1599599810769-bcde5a160d32?w=600&q=80", Featured = false },
                new Category { Name = "Wellness & Handmade", Slug = "wellness", Image = "https://images.unsplash.com/photo-1571072641041-a1596aa74e29?w=600&q=80", Featured = false }
            );
        }

        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product
                {
                    Name = "Organic Ceylon Cinnamon", Slug = "organic-ceylon-cinnamon",
                    Category = "Spices", CategorySlug = "spices", Price = 18.00m, Rating = 4.5, ReviewCount = 245,
                    Image = "https://images.unsplash.com/photo-1599599810769-bcde5a160d32?w=400&q=80",
                    Images = new() {
                        "https://images.unsplash.com/photo-1599599810769-bcde5a160d32?w=800&q=80",
                        "https://images.unsplash.com/photo-1585421514738-01798e348b17?w=800&q=80",
                        "https://images.unsplash.com/photo-1583209814683-c023dd293cc6?w=800&q=80",
                    },
                    Description = "Our Ceylon Cinnamon is harvested from the mineral-rich soils of Sri Lanka. Unlike the common Cassia variety, true Ceylon cinnamon has a delicate, sweet flavor and is ground in small batches to preserve all its natural oils and beneficial compounds.",
                    ShortDescription = "Premium true cinnamon from Sri Lanka, harvested from certified organic farms.",
                    Ingredients = "100% Pure Cinnamomum verum (True Ceylon Cinnamon)",
                    Benefits = new() { "Regulates blood sugar levels", "Anti-inflammatory properties", "Rich in antioxidants", "Supports heart health" },
                    Usage = "Add 1/2 teaspoon to teas, smoothies, baked goods, or savory dishes daily.",
                    Shipping = "Ships within 2-3 business days. Free shipping over $50.",
                    InStock = true, StockCount = 45, IsBestSeller = true,
                    Tags = new() { "organic", "spice", "cinnamon", "ceylon" }
                },
                new Product
                {
                    Name = "Blue Butterfly Pea Tea", Slug = "blue-butterfly-pea-tea",
                    Category = "Herbal Teas", CategorySlug = "herbal-teas", Price = 24.00m, Rating = 4.8, ReviewCount = 189,
                    Image = "https://images.unsplash.com/photo-1563911892437-1feda0179e1b?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1563911892437-1feda0179e1b?w=800&q=80" },
                    Description = "Vibrant blue herbal tea made from Clitoria ternatea flowers, naturally caffeine-free with a stunning color that shifts to purple with lemon juice.",
                    ShortDescription = "Stunning blue herbal tea rich in antioxidants.",
                    Benefits = new() { "Rich in anthocyanins", "Caffeine-free", "Supports cognitive health", "Natural color-changing" },
                    Usage = "Steep 1 teaspoon in hot water for 5 minutes. Add lemon for a purple color change.",
                    InStock = true, StockCount = 30, IsNew = true,
                    Tags = new() { "tea", "herbal", "butterfly-pea", "organic" }
                },
                new Product
                {
                    Name = "Wild Turmeric Root Powder", Slug = "wild-turmeric-root-powder",
                    Category = "Organic Powders", CategorySlug = "organic-powders", Price = 15.00m, Rating = 4.6, ReviewCount = 312,
                    Image = "https://images.unsplash.com/photo-1615485500704-8e990f9900f7?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1615485500704-8e990f9900f7?w=800&q=80" },
                    Description = "Stone-ground wild turmeric with high curcumin content, harvested from the forests of Sri Lanka. Rich in antioxidants and anti-inflammatory compounds.",
                    ShortDescription = "High-curcumin wild turmeric from Sri Lankan forests.",
                    Benefits = new() { "Powerful anti-inflammatory", "Supports joint health", "Boosts immunity", "Aids digestion" },
                    Usage = "Mix 1 teaspoon in warm milk, smoothies, or cooking.",
                    InStock = true, StockCount = 60, IsBestSeller = true,
                    Tags = new() { "turmeric", "powder", "organic", "anti-inflammatory" }
                },
                new Product
                {
                    Name = "Ruby Hibiscus Infusion", Slug = "ruby-hibiscus-infusion",
                    Category = "Herbal Teas", CategorySlug = "herbal-teas", Price = 22.00m, Rating = 4.3, ReviewCount = 97,
                    Image = "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=800&q=80" },
                    Description = "Vibrant ruby-red herbal infusion from dried Hibiscus sabdariffa petals. Naturally tart, refreshing, and loaded with vitamin C.",
                    ShortDescription = "Refreshing ruby red hibiscus tea packed with vitamin C.",
                    Benefits = new() { "High in vitamin C", "Supports blood pressure", "Antioxidant rich", "Caffeine-free" },
                    Usage = "Steep 2 teaspoons in hot or cold water for 5-10 minutes.",
                    InStock = true, StockCount = 25,
                    Tags = new() { "hibiscus", "tea", "herbal" }
                },
                new Product
                {
                    Name = "Pure Moringa Leaf Powder", Slug = "pure-moringa-leaf-powder",
                    Category = "Organic Powders", CategorySlug = "organic-powders", Price = 19.00m, Rating = 4.7, ReviewCount = 221,
                    Image = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&q=80" },
                    Description = "Cold-dried moringa leaves ground into a fine powder, preserving all nutrients. One of the most nutrient-dense plants on earth.",
                    ShortDescription = "The superfood powder packed with vitamins and minerals.",
                    Benefits = new() { "Contains 90+ nutrients", "Boosts energy naturally", "Anti-aging properties", "Supports brain health" },
                    Usage = "Add 1-2 teaspoons to smoothies, juices, or salad dressings.",
                    InStock = true, StockCount = 40, IsBestSeller = true,
                    Tags = new() { "moringa", "superfood", "powder", "organic" }
                },
                new Product
                {
                    Name = "Whole Black Peppercorns", Slug = "whole-black-peppercorns",
                    Category = "Spices", CategorySlug = "spices", Price = 12.00m, Rating = 4.4, ReviewCount = 156,
                    Image = "https://images.unsplash.com/photo-1596040033229-a9821ebd058d?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1596040033229-a9821ebd058d?w=800&q=80" },
                    Description = "Sun-dried whole black peppercorns from Kerala, known for their intense aroma and bold flavor. Rich in piperine to enhance nutrient absorption.",
                    ShortDescription = "Bold, aromatic black pepper from heritage pepper vines.",
                    Benefits = new() { "Enhances nutrient absorption", "Digestive aid", "Antioxidant properties", "Anti-inflammatory" },
                    Usage = "Grind fresh over any dish for maximum flavor and health benefits.",
                    InStock = true, StockCount = 80,
                    Tags = new() { "pepper", "spice", "organic" }
                },
                new Product
                {
                    Name = "Organic Ashwagandha Root Powder", Slug = "organic-ashwagandha-root-powder",
                    Category = "Organic Powders", CategorySlug = "organic-powders", Price = 24.99m, OriginalPrice = 32.00m, Discount = 22, Rating = 4.7, ReviewCount = 128,
                    Image = "https://images.unsplash.com/photo-1609167830420-571e4c2c2d2e?w=400&q=80",
                    Images = new() {
                        "https://images.unsplash.com/photo-1609167830420-571e4c2c2d2e?w=800&q=80",
                        "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&q=80",
                    },
                    Description = "Sourced directly from organic farms in Sri Lanka, our premium Ashwagandha powder is processed at low temperatures to preserve its powerful adaptogenic properties. Known for reducing stress and promoting mental clarity.",
                    ShortDescription = "Adaptogenic ashwagandha for stress relief and mental clarity.",
                    Ingredients = "100% Organic Withania somnifera root powder",
                    Benefits = new() { "Reduces stress and anxiety", "Improves sleep quality", "Boosts energy and stamina", "Supports thyroid function" },
                    Usage = "Mix 1/2-1 teaspoon in warm milk or water before bed.",
                    Shipping = "Ships within 2-3 business days. Free shipping over $50.",
                    InStock = true, StockCount = 35, IsBestSeller = true,
                    Tags = new() { "ashwagandha", "adaptogen", "powder", "ayurveda" }
                },
                new Product
                {
                    Name = "Premium Ceylon Green Tea", Slug = "premium-ceylon-green-tea",
                    Category = "Herbal Teas", CategorySlug = "herbal-teas", Price = 24.00m, Rating = 4.9, ReviewCount = 303,
                    Image = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=800&q=80" },
                    Description = "High-elevation Ceylon green tea with a delicate, vegetal flavor. Hand-picked from the Nuwara Eliya highlands, the coolest growing region of Sri Lanka.",
                    ShortDescription = "Delicate high-elevation green tea from Nuwara Eliya.",
                    Benefits = new() { "High in EGCG antioxidants", "Boosts metabolism", "Supports focus", "Heart healthy" },
                    Usage = "Brew at 80°C for 2-3 minutes. Do not over-steep to avoid bitterness.",
                    InStock = true, StockCount = 50,
                    Tags = new() { "tea", "green-tea", "ceylon", "organic" }
                },
                new Product
                {
                    Name = "Cold-Pressed Coconut Oil", Slug = "cold-pressed-coconut-oil",
                    Category = "Organic Powders", CategorySlug = "organic-powders", Price = 18.50m, Rating = 4.5, ReviewCount = 189,
                    Image = "https://images.unsplash.com/photo-1571072641041-a1596aa74e29?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1571072641041-a1596aa74e29?w=800&q=80" },
                    Description = "Cold-pressed virgin coconut oil from mature coconuts harvested in Sri Lanka. Unrefined, unbleached, and free from any additives.",
                    ShortDescription = "Pure virgin coconut oil, cold-pressed and unrefined.",
                    Benefits = new() { "Supports heart health", "Boosts metabolism", "Moisturizes skin and hair", "Antimicrobial properties" },
                    Usage = "Use for cooking, baking, or as a skin and hair moisturizer.",
                    InStock = true, StockCount = 28,
                    Tags = new() { "coconut", "oil", "organic", "cold-pressed" }
                },
                new Product
                {
                    Name = "Golden Turmeric Latte Mix", Slug = "golden-turmeric-latte-mix",
                    Category = "Organic Powders", CategorySlug = "organic-powders", Price = 21.00m, Rating = 4.6, ReviewCount = 142,
                    Image = "https://images.unsplash.com/photo-1615485291234-9d694218aeb3?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1615485291234-9d694218aeb3?w=800&q=80" },
                    Description = "A warming blend of turmeric, ginger, cinnamon, black pepper, and coconut sugar. Just add warm milk for a golden latte.",
                    ShortDescription = "Ready-to-mix golden latte blend with turmeric and warming spices.",
                    Benefits = new() { "Anti-inflammatory blend", "Warming and comforting", "Supports digestion", "Caffeine-free" },
                    Usage = "Add 1 heaping teaspoon to warm milk of choice. Stir or froth.",
                    InStock = true, StockCount = 20, IsNew = true,
                    Tags = new() { "turmeric", "latte", "golden-milk", "blend" }
                },
                new Product
                {
                    Name = "Ceylon Matcha Grade A", Slug = "ceylon-matcha-grade-a",
                    Category = "Herbal Teas", CategorySlug = "herbal-teas", Price = 32.00m, Rating = 4.8, ReviewCount = 87,
                    Image = "https://images.unsplash.com/photo-1515823662972-da6a2e4d3002?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1515823662972-da6a2e4d3002?w=800&q=80" },
                    Description = "Ceremonial-grade matcha from the highland tea gardens of Sri Lanka. Stone-ground to an ultra-fine powder with a vibrant green color.",
                    ShortDescription = "Ceremonial grade matcha with a vibrant green color.",
                    Benefits = new() { "High in L-theanine", "Calm focus without jitters", "Rich in chlorophyll", "Powerful antioxidants" },
                    Usage = "Sift 1-2 grams into a bowl. Add 70°C water and whisk in a W motion.",
                    InStock = true, StockCount = 3, IsBestSeller = true,
                    Tags = new() { "matcha", "tea", "ceremonial", "organic" }
                },
                new Product
                {
                    Name = "Wild Forest Honey", Slug = "wild-forest-honey",
                    Category = "Organic Powders", CategorySlug = "organic-powders", Price = 18.50m, Rating = 4.9, ReviewCount = 267,
                    Image = "https://images.unsplash.com/photo-1558642452-9d2a7deb7f62?w=400&q=80",
                    Images = new() { "https://images.unsplash.com/photo-1558642452-9d2a7deb7f62?w=800&q=80" },
                    Description = "Raw, unfiltered honey harvested by traditional beekeepers from the rainforests of Sri Lanka. Naturally crystallizing, with complex floral notes.",
                    ShortDescription = "Raw unfiltered honey from Sri Lankan rainforests.",
                    Benefits = new() { "Rich in enzymes", "Natural antibacterial", "Soothes sore throats", "Energy booster" },
                    Usage = "Enjoy 1 tablespoon daily. Do not heat above 40°C to preserve enzymes.",
                    InStock = true, StockCount = 12,
                    Tags = new() { "honey", "raw", "organic", "wild" }
                }
            );
        }

        await db.SaveChangesAsync();
    }
}
