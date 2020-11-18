#!/bin/sh -l
dotnet restore ElevatorChallenge.sln
dotnet build ElevatorChallenge.sln -c Release
dotnet test ElevatorChallenge.sln