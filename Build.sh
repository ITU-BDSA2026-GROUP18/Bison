#!/bin/bash

release=0
debug=0

while getopts "rdc" opt; do
  case $opt in
    r) release=1 ;;
    d) debug=1 ;;
    ?) echo "usage: $0 [-r] [-d]"; exit 1 ;;
  esac
done

if [[ $release -eq 0 && $debug -eq 0 ]]; then
  release=1
  debug=1
fi

[[ $debug -eq 1 ]] && echo "building debug" && dotnet build Bison.CLI -c Debug
[[ $release -eq 1 ]] && echo "building release" && dotnet build Bison.CLI -c Release
