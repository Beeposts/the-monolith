rmdir /S /Q "Data/Migrations"

dotnet ef migrations add Users -p ../Shared/ -s . -c IdentityAppDbContext -o ../Shared/Databases/Migrations/Identity
