#!/usr/bin/env bash
set -euo pipefail

cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.."

dotnet test AITravelPlanner.sln --configuration Release -- RunConfiguration.TreatNoTestsAsError=true
