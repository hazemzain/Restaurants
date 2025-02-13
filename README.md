🍽️ Restaurants API

📌 Project Overview

Restaurants API is a .NET 8 Web API designed to manage restaurants and their dishes. It follows the Clean Architecture pattern, utilizing CQRS (Command Query Responsibility Segregation) with MediatR, FluentValidation, and Entity Framework Core.

The API supports CRUD operations for restaurants and dishes while enforcing authentication and authorization policies.

🚀 Features

✅ Restaurant Management: Create, retrieve, update, and delete restaurants.

✅ Dish Management: Add, retrieve, and delete dishes for a specific restaurant.

✅ Authentication & Authorization: Secure endpoints with role-based access.

✅ Fluent Validation: Ensures valid user input.

✅ CQRS Pattern with MediatR: Separates read and write operations for scalability.

✅ Unit & Integration Testing: Ensures API reliability using xUnit, FluentAssertions, and Moq.


🏗️ Tech Stack

🔹 ASP.NET Core 8 – Web API framework

🔹 Entity Framework Core – ORM for database management

🔹 MediatR – Implements CQRS for request handling

🔹 FluentValidation – Validates request DTOs

🔹 Moq & xUnit – Unit testing framework

🔹 FluentAssertions – Expressive assertions in tests

## Testing

The tests are located in the `tests/*` directory. 

Unit Testing Documentation for Restaurants API

📝 Overview

This repository contains unit and integration tests to ensure the reliability of the Restaurants API. The tests validate controllers, handlers, validators, and services, covering various edge cases and error handling scenarios.

🏗️ Technologies Used

xUnit – Unit testing framework

FluentAssertions – For expressive assertions

Moq – Mocking dependencies

MediatR – Handling CQRS pattern (Commands/Queries)

ASP.NET Core TestServer – Simulating HTTP requests

WebApplicationFactory – API testing without a real server

FluentValidation.TestHelper – Validating request DTOs

🔬 Test Cases Covered


🏠 RestaurantsController (api/restaurants)

✅ GET /api/restaurants?pageNumber=1&pageSize=10

200 OK → Returns paginated list of restaurants.

400 Bad Request → Invalid query parameters.

✅ GET /api/restaurants/{id}

200 OK → Returns restaurant details if found.

404 Not Found → If the restaurant does not exist.

✅ POST /api/restaurants

201 Created → Successfully creates a new restaurant.

400 Bad Request → Input validation failure.

403 Forbidden → Unauthorized user tries to create a restaurant.

✅ PATCH /api/restaurants/{id}

204 No Content → Successfully updates restaurant.

404 Not Found → Restaurant not found.

403 Forbidden → Unauthorized access.

✅ DELETE /api/restaurants/{id}

204 No Content → Successfully deletes restaurant.

404 Not Found → Restaurant does not exist.

🍽️ DishesController (api/restaurants/{restaurantId}/dishes)

✅ POST /api/restaurants/{restaurantId}/dishes

201 Created → Successfully creates a dish.

400 Bad Request → Invalid input.

✅ GET /api/restaurants/{restaurantId}/dishes

200 OK → Returns list of dishes.

403 Forbidden → Unauthorized access.

✅ GET /api/restaurants/{restaurantId}/dishes/{dishId}

200 OK → Returns dish details.

404 Not Found → Dish does not exist.

✅ DELETE /api/restaurants/{restaurantId}/dishes

204 No Content → Deletes all dishes.

404 Not Found → Restaurant not found.

⚙️ Unit Tests for Handlers

🎯 CreateRestaurantCommandHandler

✅ Creates restaurant successfully.

❌ Throws validation error for invalid input.

🎯 GetRestaurantByIdQueryHandler

✅ Returns restaurant when found.

❌ Throws NotFoundException for invalid ID.

🎯 UpdateRestaurantCommandHandler

✅ Updates restaurant successfully.

❌ Throws error if restaurant not found.

🎯 DeleteRestaurantCommandHandler

✅ Deletes restaurant successfully.

❌ Throws error if restaurant does not exist.

🎯 CreateDishCommandHandler

✅ Creates dish successfully.

❌ Throws error if restaurant does not exist.

🎯 GetDishesForRestaurantQueryHandler

✅ Returns dishes list.

❌ Handles scenario where no dishes exist.

🎯 DeleteDishesForRestaurantCommandHandler

✅ Deletes dishes successfully.

❌ Handles case where restaurant does not exist.

✅ Unit Tests for Validators

📌 CreateDishCommandValidator

✅ Valid dish → IsValid = true

❌ Name is empty → IsValid = false

❌ Price is negative → IsValid = false

❌ KiloCalories is negative → IsValid = false

📌 CreateRestaurantCommandValidator

✅ Valid restaurant → IsValid = true

❌ Name is too short → IsValid = false

❌ Address is missing → IsValid = false
![image](https://github.com/user-attachments/assets/7de9c603-55af-4c5a-a38c-33f521a36cff)


 
