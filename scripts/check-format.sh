#!/usr/bin/env bash
set -euo pipefail

cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.."

dotnet format whitespace AITravelPlanner.sln --verify-no-changes
