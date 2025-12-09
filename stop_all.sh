#!/bin/bash

echo "Stopping Angular frontend..."
pkill -f "ng serve" &>/dev/null

echo "Stopping .NET backend..."
pkill -f "dotnet run" &>/dev/null

echo "Both frontend and backend servers have been stopped."

