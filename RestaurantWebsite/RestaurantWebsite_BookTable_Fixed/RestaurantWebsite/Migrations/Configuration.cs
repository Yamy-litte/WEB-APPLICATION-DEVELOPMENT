namespace RestaurantWebsite.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RestaurantWebsite.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RestaurantWebsite.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RestaurantWebsite.Models.AppDbContext context)
        {
            // =====================================================
            // 1. USERS
            // =====================================================

            var users = new List<User>
            {
                new User
                {
                    FullName = "Nguyen Van Customer",
                    Email = "customer@gmail.com",
                    PasswordHash = HashPassword("123456"),
                    Phone = "0901000001",
                    Address = "Ho Chi Minh City",
                    Role = "Customer",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },

                new User
                {
                    FullName = "Nguyen Van Staff",
                    Email = "staff@gmail.com",
                    PasswordHash = HashPassword("123456"),
                    Phone = "0901000002",
                    Address = "Ho Chi Minh City",
                    Role = "Staff",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },

                new User
                {
                    FullName = "Administrator",
                    Email = "admin@gmail.com",
                    PasswordHash = HashPassword("123456"),
                    Phone = "0901000003",
                    Address = "Ho Chi Minh City",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            };

            foreach (var user in users)
            {
                context.Users.AddOrUpdate(
                    u => u.Email,
                    user
                );
            }

            context.SaveChanges();


            // =====================================================
            // 2. CATEGORIES - 6 CATEGORIES
            // =====================================================

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Appetizers",
                    Description = "Món khai vị",
                    ImageUrl = "/Content/images/categories/appetizers.jpg",
                    IsActive = true
                },

                new Category
                {
                    Name = "Main Dishes",
                    Description = "Món chính",
                    ImageUrl = "/Content/images/categories/main-dishes.jpg",
                    IsActive = true
                },

                new Category
                {
                    Name = "Rice & Noodles",
                    Description = "Cơm và mì",
                    ImageUrl = "/Content/images/categories/rice-noodles.jpg",
                    IsActive = true
                },

                new Category
                {
                    Name = "Seafood",
                    Description = "Hải sản",
                    ImageUrl = "/Content/images/categories/seafood.jpg",
                    IsActive = true
                },

                new Category
                {
                    Name = "Drinks",
                    Description = "Đồ uống",
                    ImageUrl = "/Content/images/categories/drinks.jpg",
                    IsActive = true
                },

                new Category
                {
                    Name = "Desserts",
                    Description = "Tráng miệng",
                    ImageUrl = "/Content/images/categories/desserts.jpg",
                    IsActive = true
                }
            };

            foreach (var category in categories)
            {
                context.Categories.AddOrUpdate(
                    c => c.Name,
                    category
                );
            }

            context.SaveChanges();


            // =====================================================
            // 3. DISHES - 30 DISHES
            // =====================================================

            var appetizers = context.Categories
                .First(c => c.Name == "Appetizers");

            var mainDishes = context.Categories
                .First(c => c.Name == "Main Dishes");

            var riceNoodles = context.Categories
                .First(c => c.Name == "Rice & Noodles");

            var seafood = context.Categories
                .First(c => c.Name == "Seafood");

            var drinks = context.Categories
                .First(c => c.Name == "Drinks");

            var desserts = context.Categories
                .First(c => c.Name == "Desserts");


            var dishes = new List<Dish>
            {
                // APPETIZERS
                new Dish
                {
                    Name = "Spring Rolls",
                    Description = "Fresh Vietnamese spring rolls with vegetables and shrimp.",
                    Price = 45000,
                    PreparationTime = 10,
                    Status = "Available",
                    CategoryId = appetizers.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Fried Chicken Wings",
                    Description = "Crispy fried chicken wings served with sauce.",
                    Price = 65000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = appetizers.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "French Fries",
                    Description = "Crispy golden French fries.",
                    Price = 35000,
                    PreparationTime = 10,
                    Status = "Available",
                    CategoryId = appetizers.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Fried Dumplings",
                    Description = "Crispy dumplings with savory filling.",
                    Price = 50000,
                    PreparationTime = 12,
                    Status = "Available",
                    CategoryId = appetizers.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Garlic Bread",
                    Description = "Toasted bread with garlic butter.",
                    Price = 40000,
                    PreparationTime = 8,
                    Status = "Available",
                    CategoryId = appetizers.Id,
                    CreatedAt = DateTime.Now
                },


                // MAIN DISHES
                new Dish
                {
                    Name = "Grilled Beef Steak",
                    Description = "Tender beef steak grilled with special sauce.",
                    Price = 189000,
                    PreparationTime = 25,
                    Status = "Available",
                    CategoryId = mainDishes.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Grilled Chicken",
                    Description = "Grilled chicken served with vegetables.",
                    Price = 99000,
                    PreparationTime = 20,
                    Status = "Available",
                    CategoryId = mainDishes.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Pork Chop",
                    Description = "Juicy grilled pork chop with house sauce.",
                    Price = 119000,
                    PreparationTime = 22,
                    Status = "Available",
                    CategoryId = mainDishes.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Beef Burger",
                    Description = "Beef burger with cheese and fresh vegetables.",
                    Price = 89000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = mainDishes.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Chicken Burger",
                    Description = "Crispy chicken burger with fresh vegetables.",
                    Price = 79000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = mainDishes.Id,
                    CreatedAt = DateTime.Now
                },


                // RICE & NOODLES
                new Dish
                {
                    Name = "Fried Rice",
                    Description = "Traditional fried rice with vegetables and egg.",
                    Price = 65000,
                    PreparationTime = 12,
                    Status = "Available",
                    CategoryId = riceNoodles.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Beef Fried Rice",
                    Description = "Fried rice with tender beef and vegetables.",
                    Price = 85000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = riceNoodles.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Chicken Fried Rice",
                    Description = "Fried rice with chicken and vegetables.",
                    Price = 75000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = riceNoodles.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Beef Noodles",
                    Description = "Vietnamese noodles with beef and herbs.",
                    Price = 79000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = riceNoodles.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Chicken Noodles",
                    Description = "Noodles with chicken and fresh vegetables.",
                    Price = 69000,
                    PreparationTime = 15,
                    Status = "Available",
                    CategoryId = riceNoodles.Id,
                    CreatedAt = DateTime.Now
                },


                // SEAFOOD
                new Dish
                {
                    Name = "Grilled Shrimp",
                    Description = "Fresh shrimp grilled with garlic butter.",
                    Price = 149000,
                    PreparationTime = 20,
                    Status = "Available",
                    CategoryId = seafood.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Fried Calamari",
                    Description = "Crispy fried calamari served with sauce.",
                    Price = 129000,
                    PreparationTime = 18,
                    Status = "Available",
                    CategoryId = seafood.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Grilled Salmon",
                    Description = "Grilled salmon fillet with lemon butter sauce.",
                    Price = 199000,
                    PreparationTime = 25,
                    Status = "Available",
                    CategoryId = seafood.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Seafood Hotpot",
                    Description = "Hotpot with shrimp, squid and fresh vegetables.",
                    Price = 249000,
                    PreparationTime = 30,
                    Status = "Available",
                    CategoryId = seafood.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Butter Garlic Prawns",
                    Description = "Prawns cooked with butter and garlic.",
                    Price = 169000,
                    PreparationTime = 20,
                    Status = "Available",
                    CategoryId = seafood.Id,
                    CreatedAt = DateTime.Now
                },


                // DRINKS
                new Dish
                {
                    Name = "Coca Cola",
                    Description = "Chilled Coca Cola.",
                    Price = 20000,
                    PreparationTime = 2,
                    Status = "Available",
                    CategoryId = drinks.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Orange Juice",
                    Description = "Fresh orange juice.",
                    Price = 35000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = drinks.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Lemon Tea",
                    Description = "Refreshing lemon tea.",
                    Price = 30000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = drinks.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Milk Coffee",
                    Description = "Vietnamese coffee with condensed milk.",
                    Price = 35000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = drinks.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Mineral Water",
                    Description = "Bottled mineral water.",
                    Price = 15000,
                    PreparationTime = 1,
                    Status = "Available",
                    CategoryId = drinks.Id,
                    CreatedAt = DateTime.Now
                },


                // DESSERTS
                new Dish
                {
                    Name = "Cheesecake",
                    Description = "Creamy cheesecake with berry sauce.",
                    Price = 55000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = desserts.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Chocolate Cake",
                    Description = "Rich chocolate cake.",
                    Price = 50000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = desserts.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Ice Cream",
                    Description = "Creamy vanilla ice cream.",
                    Price = 40000,
                    PreparationTime = 3,
                    Status = "Available",
                    CategoryId = desserts.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Fruit Salad",
                    Description = "Fresh seasonal fruit salad.",
                    Price = 45000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = desserts.Id,
                    CreatedAt = DateTime.Now
                },

                new Dish
                {
                    Name = "Tiramisu",
                    Description = "Classic Italian tiramisu.",
                    Price = 65000,
                    PreparationTime = 5,
                    Status = "Available",
                    CategoryId = desserts.Id,
                    CreatedAt = DateTime.Now
                }
            };

            foreach (var dish in dishes)
            {
                context.Dishes.AddOrUpdate(
                    d => d.Name,
                    dish
                );
            }

            context.SaveChanges();


            // =====================================================
            // 4. DISH IMAGES
            // =====================================================

            var allDishes = context.Dishes.ToList();

            foreach (var dish in allDishes)
            {
                if (!context.DishImages.Any(i => i.DishId == dish.Id))
                {
                    context.DishImages.Add(new DishImage
                    {
                        DishId = dish.Id,
                        ImageUrl = "/Resources/User/images/dish/dish" +
                                   dish.Id.ToString("00") +
                                   ".jpg",
                        IsPrimary = true
                    });
                }
            }

            context.SaveChanges();


            // =====================================================
            // 5. RESTAURANT TABLES - 10 TABLES
            // =====================================================

            var tables = new List<RestaurantTable>
            {
                new RestaurantTable
                {
                    TableNumber = "T01",
                    Capacity = 2,
                    Zone = "Indoor",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T02",
                    Capacity = 2,
                    Zone = "Indoor",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T03",
                    Capacity = 4,
                    Zone = "Indoor",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T04",
                    Capacity = 4,
                    Zone = "Indoor",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T05",
                    Capacity = 4,
                    Zone = "Window",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T06",
                    Capacity = 6,
                    Zone = "Window",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T07",
                    Capacity = 6,
                    Zone = "Outdoor",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T08",
                    Capacity = 8,
                    Zone = "Outdoor",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T09",
                    Capacity = 8,
                    Zone = "VIP",
                    Status = "Available",
                    IsActive = true
                },

                new RestaurantTable
                {
                    TableNumber = "T10",
                    Capacity = 10,
                    Zone = "VIP",
                    Status = "Available",
                    IsActive = true
                }
            };

            foreach (var table in tables)
            {
                context.RestaurantTables.AddOrUpdate(
                    t => t.TableNumber,
                    table
                );
            }

            context.SaveChanges();


            // =====================================================
            // 6. COMBOS
            // =====================================================

            var springRolls = context.Dishes
                .First(d => d.Name == "Spring Rolls");

            var friedChicken = context.Dishes
                .First(d => d.Name == "Fried Chicken Wings");

            var frenchFries = context.Dishes
                .First(d => d.Name == "French Fries");

            var grilledChicken = context.Dishes
                .First(d => d.Name == "Grilled Chicken");

            var friedRice = context.Dishes
                .First(d => d.Name == "Fried Rice");

            var cocaCola = context.Dishes
                .First(d => d.Name == "Coca Cola");

            var orangeJuice = context.Dishes
                .First(d => d.Name == "Orange Juice");

            var cheesecake = context.Dishes
                .First(d => d.Name == "Cheesecake");

            var chocolateCake = context.Dishes
                .First(d => d.Name == "Chocolate Cake");


            var combos = new List<Combo>
            {
                new Combo
                {
                    Name = "Family Combo",
                    Description = "Combo for family meals.",
                    Price = 299000,
                    ImageUrl = "/Resources/User/images/combo/combo01.jpg",
                    IsActive = true,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6)
                },

                new Combo
                {
                    Name = "Chicken Meal Combo",
                    Description = "Chicken meal with fries and drink.",
                    Price = 139000,
                    ImageUrl = "/Resources/User/images/combo/combo03.jpg",
                    IsActive = true,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6)
                },

                new Combo
                {
                    Name = "Dessert Combo",
                    Description = "Sweet dessert combo.",
                    Price = 99000,
                    ImageUrl = "/Resources/User/images/o2.jpg",
                    IsActive = true,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6)
                }
            };

            foreach (var combo in combos)
            {
                context.Combos.AddOrUpdate(
                    c => c.Name,
                    combo
                );
            }

            context.SaveChanges();


            // =====================================================
            // 7. COMBO ITEMS
            // =====================================================

            var familyCombo = context.Combos
                .First(c => c.Name == "Family Combo");

            var chickenCombo = context.Combos
                .First(c => c.Name == "Chicken Meal Combo");

            var dessertCombo = context.Combos
                .First(c => c.Name == "Dessert Combo");


            AddComboItem(context, familyCombo.Id, springRolls.Id, 1);
            AddComboItem(context, familyCombo.Id, grilledChicken.Id, 2);
            AddComboItem(context, familyCombo.Id, friedRice.Id, 2);
            AddComboItem(context, familyCombo.Id, cocaCola.Id, 2);

            AddComboItem(context, chickenCombo.Id, grilledChicken.Id, 1);
            AddComboItem(context, chickenCombo.Id, frenchFries.Id, 1);
            AddComboItem(context, chickenCombo.Id, cocaCola.Id, 1);

            AddComboItem(context, dessertCombo.Id, cheesecake.Id, 1);
            AddComboItem(context, dessertCombo.Id, chocolateCake.Id, 1);
            AddComboItem(context, dessertCombo.Id, orangeJuice.Id, 1);

            context.SaveChanges();


            // =====================================================
            // 8. PROMOTIONS
            // =====================================================

            var promotions = new List<Promotion>
            {
                new Promotion
                {
                    Code = "WELCOME10",
                    Name = "Welcome 10%",
                    Description = "10% discount for new customers.",
                    DiscountType = "Percentage",
                    DiscountValue = 10,
                    MinimumOrderAmount = 100000,
                    MaximumDiscount = 50000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(3),
                    UsageLimit = 100,
                    UsedCount = 0,
                    IsActive = true
                },

                new Promotion
                {
                    Code = "SAVE50K",
                    Name = "Save 50K",
                    Description = "Save 50,000 VND on qualifying orders.",
                    DiscountType = "Fixed",
                    DiscountValue = 50000,
                    MinimumOrderAmount = 300000,
                    MaximumDiscount = 50000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(3),
                    UsageLimit = 50,
                    UsedCount = 0,
                    IsActive = true
                },

                new Promotion
                {
                    Code = "WEEKEND15",
                    Name = "Weekend 15%",
                    Description = "15% weekend discount.",
                    DiscountType = "Percentage",
                    DiscountValue = 15,
                    MinimumOrderAmount = 200000,
                    MaximumDiscount = 70000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(2),
                    UsageLimit = 100,
                    UsedCount = 0,
                    IsActive = true
                }
            };

            foreach (var promotion in promotions)
            {
                context.Promotions.AddOrUpdate(
                    p => p.Code,
                    promotion
                );
            }

            context.SaveChanges();
        }


        // =====================================================
        // HELPER: ADD COMBO ITEM
        // =====================================================

        private void AddComboItem(
            RestaurantWebsite.Models.AppDbContext context,
            int comboId,
            int dishId,
            int quantity)
        {
            if (!context.ComboItems.Any(
                x => x.ComboId == comboId && x.DishId == dishId))
            {
                context.ComboItems.Add(new ComboItem
                {
                    ComboId = comboId,
                    DishId = dishId,
                    Quantity = quantity
                });
            }
        }


        // =====================================================
        // HELPER: SHA256 PASSWORD
        // =====================================================

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);

                return BitConverter.ToString(hash)
                    .Replace("-", "")
                    .ToLower();
            }
        }
    }
}