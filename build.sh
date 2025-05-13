#!/bin/bash
dotnet restore
dotnet build --no-restore 
dotnet publish Sefer.Backend.SharedConfig.Api.csproj --output ./build