using dance.API.Models;
using Microsoft.EntityFrameworkCore;

namespace dance.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Directions
            if (!await db.Directions.AnyAsync())
            {
                db.Directions.AddRange(
                    new Direction { Name = "Хип-хоп", Description = "Современный уличный танец", AgeGroup = "Дети, Подростки, Взрослые", Level = "Начальный, Средний, Продвинутый" },
                    new Direction { Name = "Брейк-данс", Description = "Акробатический уличный танец", AgeGroup = "Дети, Подростки", Level = "Начальный, Средний" },
                    new Direction { Name = "Современные танцы", Description = "Contemporary dance", AgeGroup = "Подростки, Взрослые", Level = "Средний, Продвинутый" },
                    new Direction { Name = "Dancehall", Description = "Ямайский уличный танец", AgeGroup = "Подростки, Взрослые", Level = "Средний" },
                    new Direction { Name = "Латина", Description = "Латиноамериканские танцы", AgeGroup = "Взрослые", Level = "Начальный, Средний, Продвинутый" }
                );
                await db.SaveChangesAsync();
            }

            // Trainers
            if (!await db.Trainers.AnyAsync())
            {
                var directions = await db.Directions.ToListAsync();
                db.Trainers.AddRange(
                    new Trainer { FirstName = "Сергей", LastName = "Иванов", Phone = "+7-999-111-22-33", Email = "sergey.ivanov@dance.ru", Direction_id = directions[0].Direction_id },
                    new Trainer { FirstName = "Алексей", LastName = "Петров", Phone = "+7-999-222-33-44", Email = "alexey.petrov@dance.ru", Direction_id = directions[1].Direction_id },
                    new Trainer { FirstName = "Анна", LastName = "Сидорова", Phone = "+7-999-333-44-55", Email = "anna.sidorova@dance.ru", Direction_id = directions[2].Direction_id },
                    new Trainer { FirstName = "Елена", LastName = "Мартинес", Phone = "+7-999-777-33-44", Email = "elena.martinez@dance.ru", Direction_id = directions[4].Direction_id },
                    new Trainer { FirstName = "Майкл", LastName = "Браун", Phone = "+7-999-888-11-22", Email = "michael.brown@dance.ru", Direction_id = directions[3].Direction_id }
                );
                await db.SaveChangesAsync();
            }

            // Clients
            if (!await db.Clients.AnyAsync())
            {
                db.Clients.AddRange(
                    new Client { FirstName = "Иван", LastName = "Иванов", Age = 12, Phone = "+7-999-123-45-67", Email = "ivan@gmail.com" },
                    new Client { FirstName = "Алексей", LastName = "Петров", Age = 14, Phone = "+79006667788", Email = "alexey@dance.ru" },
                    new Client { FirstName = "Светлана", LastName = "Сидорова", Age = 25, Phone = "+79007778899", Email = "sveta@dance.ru" },
                    new Client { FirstName = "Никита", LastName = "Фёдоров", Age = 17, Phone = "+79008889900", Email = "nikita@dance.ru" },
                    new Client { FirstName = "Ольга", LastName = "Новикова", Age = 22, Phone = "+79009990011", Email = "olga@dance.ru" },
                    new Client { FirstName = "Артём", LastName = "Морозов", Age = 15, Phone = "+79001231234", Email = "artem@dance.ru" }
                );
                await db.SaveChangesAsync();
            }

            // Groups
            if (!await db.Groups.AnyAsync())
            {
                var directions = await db.Directions.ToListAsync();
                var trainers = await db.Trainers.ToListAsync();
                db.Groups.AddRange(
                    new Group { Name = "Хип-хоп Дети Начальный", Direction_id = directions[0].Direction_id, Trainer_id = trainers[0].Trainer_id, Status = "Активна" },
                    new Group { Name = "Хип-хоп Подростки Средний", Direction_id = directions[0].Direction_id, Trainer_id = trainers[0].Trainer_id, Status = "Активна" },
                    new Group { Name = "Хип-хоп Взрослые Продвинутый", Direction_id = directions[0].Direction_id, Trainer_id = trainers[1].Trainer_id, Status = "Активна" },
                    new Group { Name = "Брейк-данс Дети Начальный", Direction_id = directions[1].Direction_id, Trainer_id = trainers[1].Trainer_id, Status = "Активна" },
                    new Group { Name = "Брейк-данс Подростки Средний", Direction_id = directions[1].Direction_id, Trainer_id = trainers[1].Trainer_id, Status = "Активна" },
                    new Group { Name = "Современные танцы Средний", Direction_id = directions[2].Direction_id, Trainer_id = trainers[2].Trainer_id, Status = "Активна" },
                    new Group { Name = "Латина Сальса Бачата", Direction_id = directions[4].Direction_id, Trainer_id = trainers[3].Trainer_id, Status = "Активна" },
                    new Group { Name = "Dancehall Основной", Direction_id = directions[3].Direction_id, Trainer_id = trainers[4].Trainer_id, Status = "Активна" }
                );
                await db.SaveChangesAsync();
            }

            // Classes
            if (!await db.Classes.AnyAsync())
            {
                var groups = await db.Groups.ToListAsync();
                var trainers = await db.Trainers.ToListAsync();
                db.Classes.AddRange(
                    new Class { Group_id = groups[0].Group_id, Trainer_id = trainers[0].Trainer_id, Date = new DateTime(2026, 5, 5), Time = new TimeSpan(10, 0, 0), Status = "Проведено" },
                    new Class { Group_id = groups[1].Group_id, Trainer_id = trainers[0].Trainer_id, Date = new DateTime(2026, 5, 6), Time = new TimeSpan(12, 0, 0), Status = "Проведено" },
                    new Class { Group_id = groups[0].Group_id, Trainer_id = trainers[0].Trainer_id, Date = new DateTime(2026, 5, 7), Time = new TimeSpan(10, 0, 0), Status = "Проведено" },
                    new Class { Group_id = groups[3].Group_id, Trainer_id = trainers[1].Trainer_id, Date = new DateTime(2026, 5, 8), Time = new TimeSpan(11, 0, 0), Status = "Проведено" },
                    new Class { Group_id = groups[5].Group_id, Trainer_id = trainers[2].Trainer_id, Date = new DateTime(2026, 5, 9), Time = new TimeSpan(14, 0, 0), Status = "Проведено" },
                    new Class { Group_id = groups[6].Group_id, Trainer_id = trainers[3].Trainer_id, Date = new DateTime(2026, 5, 10), Time = new TimeSpan(18, 0, 0), Status = "Проведено" },
                    new Class { Group_id = groups[0].Group_id, Trainer_id = trainers[0].Trainer_id, Date = new DateTime(2026, 5, 19), Time = new TimeSpan(10, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[1].Group_id, Trainer_id = trainers[0].Trainer_id, Date = new DateTime(2026, 5, 20), Time = new TimeSpan(12, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[0].Group_id, Trainer_id = trainers[0].Trainer_id, Date = new DateTime(2026, 5, 21), Time = new TimeSpan(10, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[2].Group_id, Trainer_id = trainers[1].Trainer_id, Date = new DateTime(2026, 5, 19), Time = new TimeSpan(14, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[3].Group_id, Trainer_id = trainers[1].Trainer_id, Date = new DateTime(2026, 5, 20), Time = new TimeSpan(11, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[4].Group_id, Trainer_id = trainers[1].Trainer_id, Date = new DateTime(2026, 5, 22), Time = new TimeSpan(13, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[5].Group_id, Trainer_id = trainers[2].Trainer_id, Date = new DateTime(2026, 5, 21), Time = new TimeSpan(14, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[6].Group_id, Trainer_id = trainers[3].Trainer_id, Date = new DateTime(2026, 5, 22), Time = new TimeSpan(18, 0, 0), Status = "Запланировано" },
                    new Class { Group_id = groups[7].Group_id, Trainer_id = trainers[4].Trainer_id, Date = new DateTime(2026, 5, 23), Time = new TimeSpan(16, 0, 0), Status = "Запланировано" }
                );
                await db.SaveChangesAsync();
            }

            // Registrations
            if (!await db.Registrations.AnyAsync())
            {
                var clients = await db.Clients.ToListAsync();
                var groups = await db.Groups.ToListAsync();
                db.Registrations.AddRange(
                    new Registration { Client_id = clients[0].Client_id, Group_id = groups[0].Group_id, Registration_date = new DateTime(2026, 4, 1) },
                    new Registration { Client_id = clients[0].Client_id, Group_id = groups[5].Group_id, Registration_date = new DateTime(2026, 4, 1) },
                    new Registration { Client_id = clients[1].Client_id, Group_id = groups[0].Group_id, Registration_date = new DateTime(2026, 4, 2) },
                    new Registration { Client_id = clients[1].Client_id, Group_id = groups[4].Group_id, Registration_date = new DateTime(2026, 4, 2) },
                    new Registration { Client_id = clients[2].Client_id, Group_id = groups[5].Group_id, Registration_date = new DateTime(2026, 4, 3) },
                    new Registration { Client_id = clients[2].Client_id, Group_id = groups[6].Group_id, Registration_date = new DateTime(2026, 4, 3) },
                    new Registration { Client_id = clients[3].Client_id, Group_id = groups[3].Group_id, Registration_date = new DateTime(2026, 4, 4) },
                    new Registration { Client_id = clients[3].Client_id, Group_id = groups[1].Group_id, Registration_date = new DateTime(2026, 4, 4) },
                    new Registration { Client_id = clients[4].Client_id, Group_id = groups[6].Group_id, Registration_date = new DateTime(2026, 4, 5) },
                    new Registration { Client_id = clients[4].Client_id, Group_id = groups[7].Group_id, Registration_date = new DateTime(2026, 4, 5) },
                    new Registration { Client_id = clients[5].Client_id, Group_id = groups[4].Group_id, Registration_date = new DateTime(2026, 4, 6) },
                    new Registration { Client_id = clients[5].Client_id, Group_id = groups[0].Group_id, Registration_date = new DateTime(2026, 4, 6) }
                );
                await db.SaveChangesAsync();
            }

            // Subscriptions
            if (!await db.Subscriptions.AnyAsync())
            {
                var clients = await db.Clients.ToListAsync();
                var groups = await db.Groups.ToListAsync();
                db.Subscriptions.AddRange(
                    new Subscription { Client_id = clients[0].Client_id, Group_id = groups[0].Group_id, Amount = 3000, Status = "Оплачен" },
                    new Subscription { Client_id = clients[0].Client_id, Group_id = groups[5].Group_id, Amount = 3500, Status = "Активен" },
                    new Subscription { Client_id = clients[1].Client_id, Group_id = groups[0].Group_id, Amount = 3000, Status = "Оплачен" },
                    new Subscription { Client_id = clients[1].Client_id, Group_id = groups[4].Group_id, Amount = 3200, Status = "Активен" },
                    new Subscription { Client_id = clients[2].Client_id, Group_id = groups[5].Group_id, Amount = 3500, Status = "Оплачен" },
                    new Subscription { Client_id = clients[2].Client_id, Group_id = groups[6].Group_id, Amount = 4000, Status = "Оплачен" },
                    new Subscription { Client_id = clients[3].Client_id, Group_id = groups[3].Group_id, Amount = 3000, Status = "Активен" },
                    new Subscription { Client_id = clients[3].Client_id, Group_id = groups[1].Group_id, Amount = 3500, Status = "Оплачен" },
                    new Subscription { Client_id = clients[4].Client_id, Group_id = groups[6].Group_id, Amount = 4000, Status = "Активен" },
                    new Subscription { Client_id = clients[4].Client_id, Group_id = groups[7].Group_id, Amount = 3800, Status = "Оплачен" },
                    new Subscription { Client_id = clients[5].Client_id, Group_id = groups[4].Group_id, Amount = 3200, Status = "Активен" },
                    new Subscription { Client_id = clients[5].Client_id, Group_id = groups[0].Group_id, Amount = 3000, Status = "Оплачен" }
                );
                await db.SaveChangesAsync();
            }

            // Users
            if (!await db.Users.AnyAsync())
            {
                var clients = await db.Clients.ToListAsync();
                var trainers = await db.Trainers.ToListAsync();
                db.Users.AddRange(
                    new User { Email = "admin@dance.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new User { Email = "director@dance.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Director", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new User { Email = "accountant@dance.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Accountant", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new User { Email = "trainer@dance.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Trainer", IsActive = true, CreatedAt = DateTime.UtcNow, Trainer_id = trainers[0].Trainer_id },
                    new User { Email = "client@dance.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Client", IsActive = true, CreatedAt = DateTime.UtcNow, Client_id = clients[0].Client_id }
                );
                await db.SaveChangesAsync();

                // Привязываем User_id к клиенту и тренеру
                var clientUser = await db.Users.FirstAsync(u => u.Email == "client@dance.ru");
                var trainerUser = await db.Users.FirstAsync(u => u.Email == "trainer@dance.ru");
                clients[0].User_id = clientUser.User_id;
                trainers[0].User_id = trainerUser.User_id;
                await db.SaveChangesAsync();
            }
        }
    }
}