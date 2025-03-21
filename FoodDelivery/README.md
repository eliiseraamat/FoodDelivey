dotnet tool install --global dotnet-ef

dotnet ef migrations add --project DAL --startup-project WebApi --context AppDbContext InitialCreate
dotnet ef migrations --project DAL --startup-project WebApo remove

dotnet ef database --project DAL --startup-project WebApi update
dotnet ef database --project DAL --startup-project WebApi drop

dotnet run --project WebApi
