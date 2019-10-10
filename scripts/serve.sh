#!/usr/bin/env bash
ROOT=$(git rev-parse --show-toplevel)
cd $ROOT/builds
python -m SimpleHTTPServer 7580
