initial commands to run in Api/ directory(where you have .csproj):

Start postgresql service if using docker
1. docker-compose up --build 

2. dotnet build
if no error, goto 2

3. dotnet ef migrations add InitialMigration

4. dotnet ef database update

run project, or whatever you choose e.g F5 on visual studio
5. dotnet run

If you made it here, omoo welldone & goodluck youll need alot of it :) !!!!