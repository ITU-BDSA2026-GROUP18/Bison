#!/usr/bin/env bash
sqlite3 /tmp/bison.db < data/schema.sql
sqlite3 /tmp/bison.db < data/dump.sql
