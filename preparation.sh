#!/usr/bin/env bash

whoami

mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/

LICENSE=UNITY_LICENSE_CONTENT

echo "${!LICENSE}" | tr -d '\r' > /root/.local/share/unity3d/Unity/Unity_lic.ulf