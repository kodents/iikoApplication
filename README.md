# Clients API

ASP.NET Core 8 Web API для управления клиентами с использованием PostgreSQL.

## Возможности

- ✅ RESTful API для управления клиентами
- ✅ Интеграция с PostgreSQL
- ✅ Внедрение зависимостей (Dependency Injection)
- ✅ Документация Swagger
- ✅ Пакетные операции
- ✅ Поддержка async/await
- ✅ Валидация входных данных

## API Эндпоинты

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| GET | `/api/clients` | Получить всех клиентов |
| GET | `/api/clients/{id}` | Получить клиента по ID |
| POST | `/api/clients` | Создать нового клиента |
| PUT | `/api/clients/{id}` | Обновить клиента |
| DELETE | `/api/clients/{id}` | Удалить клиента |
| POST | `/api/clients/batch` | Создать нескольких клиентов (мин. 10) |

## Модель клиента

```json
{
  "clientId": 123456,
  "username": "john_doe",
  "systemId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}


## DTO для создания/обновления

```json
{
  "clientId": 123456,
  "username": "john_doe"
}
```

## Требования

- .NET 8.0 SDK
- PostgreSQL

## Установка и запуск

1. Клонируйте репозиторий
```bash
git clone https://github.com/kodents/iikoApplication
cd iikoApplication
```

2. Настройте подключение к базе в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=ClientAppDb;Username=postgres;Password=ваш_пароль;"
  }
}
```

3. Примените миграции базы данных:
```bash
dotnet ef database update
```

4. Запустите приложение:
```bash
dotnet run
```

5. Откройте Swagger UI: `https://localhost:7000`

## Примеры использования

### Создание клиента
```bash
curl -X POST "https://localhost:7000/api/clients" \
  -H "Content-Type: application/json" \
  -d '{"clientId": 1, "username": "john_doe"}'
```

### Пакетное создание клиентов
```bash
curl -X POST "https://localhost:7000/api/clients/batch" \
  -H "Content-Type: application/json" \
  -d '[{"clientId": 1, "username": "user1"}, {"clientId": 2, "username": "user2"}, ... (10+ клиентов)]'
```

### Получение клиента
```bash
curl -X GET "https://localhost:7000/api/clients/1"
```