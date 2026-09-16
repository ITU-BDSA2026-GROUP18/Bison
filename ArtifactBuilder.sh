#!/bin/bash

cd ./src/Bison.CLI

printf "\n===========================\nBuilding for\nOS: Windows\nArchitecture: X86_64\n===========================\n\n" && dotnet publish -c Release --runtime win-x64 --self-contained

printf "\n===========================\nBuilding for\nOS: Windows\nArchitecture: Aarch64\n===========================\n\n" && dotnet publish -c Release --runtime win-arm64 --self-contained

printf "\n===========================\nBuilding for\nOS: Linux\nArchitecture: X86_64\n===========================\n\n" && dotnet publish -c Release --runtime linux-x64 --self-contained

printf "\n===========================\nBuilding for\nOS: Linux\nArchitecture: Aarch64\n===========================\n\n" && dotnet publish -c Release --runtime linux-arm64 --self-contained

printf "\n===========================\nBuilding for\nOS: MacOS\nArchitecture: X86_64\n===========================\n\n" && dotnet publish -c Release --runtime osx-x64 --self-contained

printf "\n===========================\nBuilding for\nOS: MacOS\nArchitecture: Aarch64\n===========================\n\n" && dotnet publish -c Release --runtime osx-arm64 --self-contained

