# CRM Школа танцев "Ритм"

Веб-приложение для управления танцевальной школой. Система позволяет вести учёт клиентов, тренеров, групп, занятий и финансов с разграничением доступа по ролям.

## Для кого

- **Администратор** — управляет базой клиентов, тренеров и групп, создаёт пользователей
- **Директор** — просматривает аналитику: заполненность групп, популярность направлений, нагрузку тренеров
- **Бухгалтер** — отслеживает выручку, долги и состояние абонементов
- **Тренер** — видит своё расписание и списки учеников в группах, меняет статус занятий
- **Клиент** — просматривает своё расписание, группы и оплаты

## Функциональность

- Авторизация по email и паролю с JWT токеном
- Разграничение доступа по ролям
- Управление клиентами — добавление, редактирование, удаление
- Расписание занятий с фильтрацией по тренеру и клиенту
- Учёт абонементов и задолженностей
- Аналитика заполненности групп и нагрузки тренеров
- Создание новых пользователей с привязкой к клиенту или тренеру

## Технологии

**Бэкенд:**
- ASP.NET Core 9
- Entity Framework Core
- SQL Server
- JWT авторизация
- BCrypt для хэширования паролей

**Фронтенд:**
- HTML / CSS / JavaScript
- nginx alpine (в Docker)

**Инфраструктура:**
- Docker / Docker Compose

## Быстрый запуск через Docker

### Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Запуск

```bash
git clone https://github.com/le0pardprint/dance.git
cd dance
docker-compose up --build
```

База данных создаётся и заполняется тестовыми данными автоматически.

После запуска:
- Фронтенд: [http://localhost](http://localhost)
- API: [http://localhost:5052](http://localhost:5052)
- Swagger: [http://localhost:5052/swagger](http://localhost:5052/swagger)

### Остановка

```bash
docker-compose down
```

## Запуск без Docker (локально)

### Требования

- [.NET SDK 7+](https://dotnet.microsoft.com/download) (версия 7, 8 или 9)
- [SQL Server LocalDB](https://learn.microsoft.com/ru-ru/sql/database-engine/configure-windows/sql-server-express-localdb) (устанавливается вместе с Visual Studio)
- [Visual Studio Code](https://code.visualstudio.com/) с расширением [Live Server](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer)

### Шаги

**1. Клонировать репозиторий**
```bash
git clone https://github.com/le0pardprint/dance.git
cd dance
```

**2. Запустить бэкенд**
```bash
cd dance.API
dotnet restore
dotnet run
```
Бэкенд запустится на `http://localhost:5052`. База данных и тестовые данные создадутся автоматически.

**3. Запустить фронтенд**

- Открой папку `dance-frontend` в VS Code
- Правой кнопкой на `login.html` → **Open with Live Server**
- Браузер откроется на `http://127.0.0.1:5500/login.html`