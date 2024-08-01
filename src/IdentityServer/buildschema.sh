#!/bin/bash

rm -rf "../Shared/Databases/Migrations/Identity"

dotnet ef migrations add Users -p ../Shared/ -s . -c IdentityAppDbContext -o ../Shared/Databases/Migrations/Identity
