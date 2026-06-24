#!/usr/bin/env bash
set -euo pipefail
BASE="${SMOKE_BASE_URL:-http://localhost:8080}"
echo "Smoke testing $BASE"
[ "$(curl -s -o /dev/null -w '%{http_code}' "$BASE/health")" = "200" ] || { echo "health failed"; exit 1; }
code=$(curl -s -o /dev/null -w '%{http_code}' -H 'Content-Type: application/json' \
  -d '{"filename":"a.txt","sourceType":"transcript","text":"hi","target":"json"}' "$BASE/api/convert")
[ "$code" = "200" ] || { echo "convert smoke failed ($code)"; exit 1; }
echo "smoke OK"
