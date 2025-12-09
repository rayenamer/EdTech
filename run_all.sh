#!/bin/bash

echo "Starting Angular frontend..."
(
  cd ~/EdTech/SellFlowFront || exit
  ng serve --host 0.0.0.0 --ssl true --ssl-cert localhost.pem --ssl-key localhost-key.pem
) &

echo "Starting .NET backend..."
(
  cd ~/EdTech/SellFlowApi/API || exit
  dotnet run --launch-profile https
) &

echo "*********ClienT:https://localhost:7030*********"
echo "************API:https://localhost:7030*********"
