# Gamers-Hub
A Minimal Viable Product (MVP) in ASP .NET C#

1. Utilization of Code-First Migrations for creating the database.
2. Implementation of WebAPI and LINQ for performing CRUD operations.

Entities:
1. Games
2. Genres
3. Buyers

Data Flow (Relationship) in Database Tables:
1. Games and Buyers have a Many-to-Many relationship.
2. Games and Genres have a Many-to-One relationship.
3. CRUD operations can be performed on each entity.

Features:
1. The games list displays all games in the database.
2. By clicking on the names of the games, detailed information about each game can be viewed.
3. Games can be linked with genres and multiple buyers.
4. The genre list showcases all genres available in the database.
5. By clicking on a genre name, a list of games linked to that specific genre can be viewed.
6. The buyer list presents all buyers in the database.
7. By clicking on a buyer's name, a list of games purchased by that buyer is displayed.
