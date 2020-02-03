#!/usr/bin/env bash

echo "Building a standalone player to use for the APlib side"
echo "$(pwd)"
build=$1
platform=$2
extenstionName=$3

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $(pwd)/Unity/AIGym \
  -batchmode \
  -nographics \
  -logFile /dev/stdout \
  -username $UNITY_USERNAME \
  -password $UNITY_PASSWORD \
  -$build $(pwd)/gym/$platform/bin/AIGym$extenstionName \
  -quit

  UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

exit $UNITY_EXIT_CODE