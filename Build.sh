#!/bin/bash

release=0
debug=0
flags=()

while getopts "rdf:" opt; do
  case $opt in
    r) release=1 ;;
    d) debug=1 ;;
    f) flags+=("$OPTARG") ;;
    ?) echo "usage: $0 [-r] [-d] [-f flag_name]"; exit 1 ;;
  esac
done

if [[ $release -eq 0 && $debug -eq 0 ]]; then
  release=1
  debug=1
fi

IFS=';' dc="${flags[*]}"
unset IFS

flag_args=()
[[ -n "$dc" ]] && flag_args=(-p:DefineConstants="$dc")

[[ $debug -eq 1 ]] && echo "building debug" && dotnet build Bison.CLI -c Debug "${flag_args[@]}"
[[ $release -eq 1 ]] && echo "building release" && dotnet build Bison.CLI -c Release "${flag_args[@]}"
