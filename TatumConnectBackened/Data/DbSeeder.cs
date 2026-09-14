using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TatumConnectBackened.Common.Constants;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            //Apply pending migrations
            await context.Database.MigrateAsync();
            await SeedSuperAdminAsync(context);
            await SeedBillersAndProductsAsync(context);
        }
        //Super Admin
        private static async Task SeedSuperAdminAsync(AppDbContext context)
        {
            const string email = "superadmin@tatumconnect.com";
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                if (!existingUser.IsRegistrationVerified || !existingUser.IsActive)
                {
                    existingUser.IsRegistrationVerified = true;
                    existingUser.IsActive = true;
                    await context.SaveChangesAsync();
                }
                return;
            }
            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                //Developement password.
                //change this immediately in a real environment

                PasswordHash = HashPassword("Admin@123456"),
                FirstName = "Tatum",
                LastName = "Super Admin",
                Phone = "+2348000000000",
                Department = "Administration",
                ProfileImageUrl = null,
                StaffId = await GenerateStaffIdAsync(context),
                Role = UserRoles.SuperAdmin,
                IsActive = true,
                IsRegistrationVerified = true,
                PasswordSetupToken = null,
                PasswordSetupTokenExpiresAt = null,
                CreatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(superAdmin);
            await context.SaveChangesAsync();
        }
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        private static async Task<string> GenerateStaffIdAsync(
            AppDbContext context)
        {
            var year = DateTime.UtcNow.Year;

            var lastStaffId =
                await context.Users
                    .Where(u =>
                        u.StaffId != null &&
                        u.StaffId.StartsWith(
                            $"STF-{year}-"))
                    .OrderByDescending(
                        u => u.StaffId)
                    .Select(u => u.StaffId)
                    .FirstOrDefaultAsync();

            var nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastStaffId))
            {
                var numberPart =
                    lastStaffId
                        .Split('-')
                        .Last();

                if (int.TryParse(
                    numberPart,
                    out var currentNumber))
                {
                    nextNumber =
                        currentNumber + 1;
                }
            }

            return $"STF-{year}-{nextNumber:D6}";
        }

        // =========================================================
        // BILLERS
        // =========================================================

        private static async Task SeedBillersAndProductsAsync(AppDbContext context)
        {
            var mtn =
                await GetOrCreateBillerAsync(
                    context,
                    "MTN Nigeria",
                    "MTN",
                    "MTN Nigeria telecommunications services.",
                    BillerCategory.TeleCommunication);

            var airtel =
                await GetOrCreateBillerAsync(
                    context,
                    "Airtel Nigeria",
                    "AIRTEL",
                    "Airtel Nigeria telecommunications services.", BillerCategory.TeleCommunication);

            var glo =
                await GetOrCreateBillerAsync(
                    context,
                    "Globacom Nigeria",
                    "GLO",
                    "Glo Nigeria telecommunications services.", BillerCategory.TeleCommunication);

            var nineMobile =
                await GetOrCreateBillerAsync(
                    context,
                    "9mobile Nigeria",
                    "9MOBILE",
                    "9mobile Nigeria telecommunications services.", BillerCategory.TeleCommunication);

            await context.SaveChangesAsync();

            // =====================================================
            // MTN
            // =====================================================

            var mtnAirtime =
                await GetOrCreateProductAsync(
                    context,
                    mtn.Id,
                    "MTN Airtime",
                    "MTN_AIRTIME",
                    ProductCategory.Airtime,
                    "Purchase MTN airtime.",
                    allowsCustomAmount: true,
                    requiresProductItem: false,
                    unitPrice: null,
                    requiredFields: AirtimeFields());

            var mtnData =
                await GetOrCreateProductAsync(
                    context,
                    mtn.Id,
                    "MTN Data",
                    "MTN_DATA",
                    ProductCategory.Data,
                    "Purchase MTN data bundles.",
                    allowsCustomAmount: false,
                    requiresProductItem: true,
                    unitPrice: null,
                    requiredFields: DataFields());

            // =====================================================
            // AIRTEL
            // =====================================================

            var airtelAirtime =
                await GetOrCreateProductAsync(
                    context,
                    airtel.Id,
                    "Airtel Airtime",
                    "AIRTEL_AIRTIME",
                    ProductCategory.Airtime,
                    "Purchase Airtel airtime.",
                    allowsCustomAmount: true,
                    requiresProductItem: false,
                    unitPrice: null,
                    requiredFields: AirtimeFields());

            var airtelData =
                await GetOrCreateProductAsync(
                    context,
                    airtel.Id,
                    "Airtel Data",
                    "AIRTEL_DATA",
                    ProductCategory.Data,
                    "Purchase Airtel data bundles.",
                    allowsCustomAmount: false,
                    requiresProductItem: true,
                    unitPrice: null,
                    requiredFields: DataFields());

            // =====================================================
            // GLO
            // =====================================================

            var gloAirtime =
                await GetOrCreateProductAsync(
                    context,
                    glo.Id,
                    "Glo Airtime",
                    "GLO_AIRTIME",
                    ProductCategory.Airtime,
                    "Purchase Glo airtime.",
                    allowsCustomAmount: true,
                    requiresProductItem: false,
                    unitPrice: null,
                    requiredFields: AirtimeFields());

            var gloData =
                await GetOrCreateProductAsync(
                    context,
                    glo.Id,
                    "Glo Data",
                    "GLO_DATA",
                    ProductCategory.Data,
                    "Purchase Glo data bundles.",
                    allowsCustomAmount: false,
                    requiresProductItem: true,
                    unitPrice: null,
                    requiredFields: DataFields());

            // =====================================================
            // 9MOBILE
            // =====================================================

            var nineMobileAirtime =
                await GetOrCreateProductAsync(
                    context,
                    nineMobile.Id,
                    "9mobile Airtime",
                    "9MOBILE_AIRTIME",
                    ProductCategory.Airtime,
                    "Purchase 9mobile airtime.",
                    allowsCustomAmount: true,
                    requiresProductItem: false,
                    unitPrice: null,
                    requiredFields: AirtimeFields());

            var nineMobileData =
                await GetOrCreateProductAsync(
                    context,
                    nineMobile.Id,
                    "9mobile Data",
                    "9MOBILE_DATA",
                    ProductCategory.Data,
                    "Purchase 9mobile data bundles.",
                    allowsCustomAmount: false,
                    requiresProductItem: true,
                    unitPrice: null,
                    requiredFields: DataFields());

            await context.SaveChangesAsync();

            // =====================================================
            // MTN DATA ITEMS
            // =====================================================

            await CreateProductItemAsync(
                context,
                mtnData.Id,
                "MTN 1GB",
                "MTN_DATA_1GB",
                1000m,
                "1 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                mtnData.Id,
                "MTN 2GB",
                "MTN_DATA_2GB",
                2000m,
                "2 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                mtnData.Id,
                "MTN 5GB",
                "MTN_DATA_5GB",
                5000m,
                "5 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                mtnData.Id,
                "MTN 10GB",
                "MTN_DATA_10GB",
                10000m,
                "10 GB",
                "30 days");

            // =====================================================
            // AIRTEL DATA ITEMS
            // =====================================================

            await CreateProductItemAsync(
                context,
                airtelData.Id,
                "Airtel 1GB",
                "AIRTEL_DATA_1GB",
                1000m,
                "1 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                airtelData.Id,
                "Airtel 2GB",
                "AIRTEL_DATA_2GB",
                2000m,
                "2 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                airtelData.Id,
                "Airtel 5GB",
                "AIRTEL_DATA_5GB",
                5000m,
                "5 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                airtelData.Id,
                "Airtel 10GB",
                "AIRTEL_DATA_10GB",
                10000m,
                "10 GB",
                "30 days");

            // =====================================================
            // GLO DATA ITEMS
            // =====================================================

            await CreateProductItemAsync(
                context,
                gloData.Id,
                "Glo 1GB",
                "GLO_DATA_1GB",
                1000m,
                "1 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                gloData.Id,
                "Glo 3GB",
                "GLO_DATA_3GB",
                3000m,
                "3 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                gloData.Id,
                "Glo 5GB",
                "GLO_DATA_5GB",
                5000m,
                "5 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                gloData.Id,
                "Glo 10GB",
                "GLO_DATA_10GB",
                10000m,
                "10 GB",
                "30 days");

            // =====================================================
            // 9MOBILE DATA ITEMS
            // =====================================================

            await CreateProductItemAsync(
                context,
                nineMobileData.Id,
                "9mobile 1GB",
                "9MOBILE_DATA_1GB",
                1000m,
                "1 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                nineMobileData.Id,
                "9mobile 2GB",
                "9MOBILE_DATA_2GB",
                2000m,
                "2 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                nineMobileData.Id,
                "9mobile 5GB",
                "9MOBILE_DATA_5GB",
                5000m,
                "5 GB",
                "30 days");

            await CreateProductItemAsync(
                context,
                nineMobileData.Id,
                "9mobile 10GB",
                "9MOBILE_DATA_10GB",
                10000m,
                "10 GB",
                "30 days");

            await context.SaveChangesAsync();
        }


        private static async Task<Biller>
            GetOrCreateBillerAsync(
                AppDbContext context,
                string name,
                string code,
                string description,
                BillerCategory category,
                string? logoUrl = null)
        {
            var biller =
                await context.Billers
                    .FirstOrDefaultAsync(
                        b => b.Code == code);

            if (biller != null)
            {
                biller.Name = name;

                biller.Description = description;

                biller.Category = category;

                if (!string.IsNullOrWhiteSpace(logoUrl))
                {
                    biller.LogoUrl = logoUrl;
                }

                biller.IsActive = true;

                biller.UpdatedAt = DateTime.UtcNow;

                return biller;
            }

            biller = new Biller
            {
                Id = Guid.NewGuid(),

                Name = name,

                Code = code,

                Category = category,

                LogoUrl = logoUrl,

                Description = description,

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

            await context.Billers.AddAsync(biller);

            return biller;
        }


        private static async Task<Product>
    GetOrCreateProductAsync(
        AppDbContext context,
        Guid billerId,
        string name,
        string code,
        ProductCategory category,
        string description,
        bool allowsCustomAmount,
        bool requiresProductItem,
        decimal? unitPrice,
        string requiredFields)
        {
            var product =
                await context.Products
                    .FirstOrDefaultAsync(
                        p => p.Code == code);

            if (product != null)
            {
                // ==========================================
                // UPDATE EXISTING PRODUCT
                // ==========================================

                product.BillerId = billerId;

                product.Name = name;

                product.Category = category;

                product.Description = description;

                product.UnitPrice = unitPrice;

                product.AllowsCustomAmount =
                    allowsCustomAmount;

                product.RequiresProductItem =
                    requiresProductItem;

                product.RequiredFields =
                    requiredFields;

                product.IsActive = true;

                product.UpdatedAt = DateTime.UtcNow;

                return product;
            }

            // ==========================================
            // CREATE NEW PRODUCT
            // ==========================================

            product = new Product
            {
                Id = Guid.NewGuid(),

                BillerId = billerId,

                Code = code,

                Name = name,

                Category = category,

                Description = description,

                UnitPrice = unitPrice,

                AllowsCustomAmount =
                    allowsCustomAmount,

                RequiresProductItem =
                    requiresProductItem,

                IsActive = true,

                RequiredFields =
                    requiredFields,

                CreatedAt = DateTime.UtcNow
            };

            await context.Products.AddAsync(product);

            return product;
        }

        // =========================================================
        // PRODUCT ITEM
        // =========================================================

        private static async Task CreateProductItemAsync(
            AppDbContext context,
            Guid productId,
            string name,
            string code,
            decimal unitPrice,
            string package,
            string validity)
        {
            var exists =
                await context.ProductItems
                    .AnyAsync(
                        i => i.Code == code);

            if (exists)
                return;

            var item = new ProductItem
            {
                Id = Guid.NewGuid(),

                ProductId = productId,

                Name = name,

                Code = code,

                UnitPrice = unitPrice,

                Description =
                    $"{package} data bundle valid for {validity}",

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

            await context.ProductItems.AddAsync(item);
        }

        // =========================================================
        // REQUIRED FIELDS
        // =========================================================

        private static string AirtimeFields()
        {
            return """
            {
                "fields": [
                    {
                        "name": "phoneNumber",
                        "label": "Phone Number",
                        "type": "phone",
                        "required": true,
                        "validation": {
                            "pattern": "^\\+?[0-9]{10,15}$"
                        }
                    }
                ]
            }
            """;
        }

        private static string DataFields()
        {
            return """
            {
                "fields": [
                    {
                        "name": "phoneNumber",
                        "label": "Phone Number",
                        "type": "phone",
                        "required": true,
                        "validation": {
                            "pattern": "^\\+?[0-9]{10,15}$"
                        }
                    }
                ]
            }
            """;
        }
    }
  
}
