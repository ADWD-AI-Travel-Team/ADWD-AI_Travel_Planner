#!/usr/bin/env bash
set -euo pipefail

cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.."

if [[ ! -f src/WebFrontend/node_modules/typescript/bin/tsc ]]; then
  echo "Frontend dependencies are missing. Run npm ci in src/WebFrontend first." >&2
  exit 1
fi

dotnet build AITravelPlanner.sln --configuration Release -warnaserror
node src/WebFrontend/node_modules/typescript/bin/tsc --build src/WebFrontend/tsconfig.json
