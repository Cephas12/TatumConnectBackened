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

        private static async Task SeedBillersAndProductsAsync(AppDbContext context)
        {
            if (await context.Billers.AnyAsync()) return;

            var billers = new[]
            {
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "MTN",
                    Name = "MTN Nigeria",
                    Category = BillerCategory.TeleCommunication,
                    Description = "MTN Airtime & Data Services",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "AIRTEL",
                    Name = "Airtel Nigeria",
                    Category = BillerCategory.TeleCommunication,
                    Description = "Airtel Telecom Services",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "DSTV",
                    Name = "DSTV Cable TV",
                    Category = BillerCategory.CableTv,
                    Description = "DSTV Subscription Services",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Biller
                {
                    Id = Guid.NewGuid(),
                    Code = "EKEDC",
                    Name = "Eko Electricity",
                    Category = BillerCategory.Electricity,
                    Description = "Eko Electricity Distribution Company",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Billers.AddRangeAsync(billers);
            await context.SaveChangesAsync();
        }
    }
  
}
