#!/bin/bash

if [ -z "$1" ]; then
  echo "Usage: ./create_task.sh <task_number>"
  exit 1
fi

TASK_NUM=$(printf "%02d" $1)

LIB_NAME="task$TASK_NUM"
TEST_NAME="task${TASK_NUM}tests"

echo "Creating $LIB_NAME and $TEST_NAME..."

if [ ! -f *.sln ]; then
  dotnet new sln -n practice-2026
fi

dotnet new classlib -n $LIB_NAME
dotnet new xunit -n $TEST_NAME

dotnet sln add $LIB_NAME/$LIB_NAME.csproj
dotnet sln add $TEST_NAME/$TEST_NAME.csproj

dotnet add $TEST_NAME/$TEST_NAME.csproj reference $LIB_NAME/$LIB_NAME.csproj
