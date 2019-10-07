#!/usr/bin/env bash
ROOT=$(git rev-parse --show-toplevel)
cd $ROOT/builds/1.0-webgl/zero-coast
python -m SimpleHTTPServer 7580
