#!/usr/bin/env bash
sudo apt update && sudo apt upgrade
apt install curl -y

curl -F file=@$(pwd)/Unity/AIGym/$TEST_PLATFORM-results.xml -F channels=jenkins-notifications -H "Authorization: Bearer $SLACK_TOKEN" https://slack.com/api/files.upload