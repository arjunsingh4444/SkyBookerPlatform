#!/bin/bash
# You must have dotnet-sonarscanner installed (dotnet tool install --global dotnet-sonarscanner)
export PATH="$PATH:/Users/arjunsingh/.dotnet/tools"

echo "=== SkyBooker Backend SonarQube Scan ==="

PROJECT_KEY="SkyBooker_Backend"
SONAR_URL="http://localhost:9000"

# Replace this with the token you generate in SonarQube UI!
LOGIN_TOKEN="YOUR_SONAR_TOKEN_HERE"

if [ "$LOGIN_TOKEN" = "YOUR_SONAR_TOKEN_HERE" ]; then
    echo "ERROR: Please replace 'YOUR_SONAR_TOKEN_HERE' in run-sonar-backend.sh with a token generated from SonarQube."
    echo "1. Go to http://localhost:9000"
    echo "2. Log in with admin / admin"
    echo "3. Go to My Account -> Security -> Generate Token"
    exit 1
fi

echo "Cleaning previous builds..."
dotnet clean SkyBooker-Backend.sln

echo "Starting SonarScanner..."
dotnet sonarscanner begin /k:"$PROJECT_KEY" /d:sonar.host.url="$SONAR_URL" /d:sonar.login="$LOGIN_TOKEN"

echo "Building Solution..."
dotnet build SkyBooker-Backend.sln

echo "Ending SonarScanner and pushing results..."
dotnet sonarscanner end /d:sonar.login="$LOGIN_TOKEN"

echo "Done! Check http://localhost:9000 for your backend report."
