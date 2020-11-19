$startedDir = $pwd

# Build
dotnet build

# SPublish Function
cd ..\DiceGameSignalRServer
dotnet publish DiceGameSignalRServer.csproj

#Start function
cd $startedDir
$arg = "-noexit -command ${pwd}\script\start_server_local.ps1"
Start-Process -FilePath "powershell" -ArgumentList $arg -WorkingDirectory "..\DiceGameSignalRServer\bin\Debug\netcoreapp3.1\" 