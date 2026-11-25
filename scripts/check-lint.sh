#!/usr/bin/env bash
set -euo pipefail

cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.."

dotnet format analyzers AITravelPlanner.sln --verify-no-changes --severity info
npm --prefix src/WebFrontend run lint
