#!/usr/bin/env bash

myvar=$(echo 'cat //test-run/@value' | xmllint --shell $TEST_PLATFORM-results.xml | awk -F'[="]' '!/>/{print $(NF-1)}')
echo "$myvar"

