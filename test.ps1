$startedDir = $pwd

# Build
dotnet build

# SPublish Function
cd ..\DiceGameFunction
dotnet publish DiceGame\DiceGameFunction.csproj

#Start function
cd $startedDir
$arg = "-noexit -command ${pwd}\startfunction_local.ps1"
Start-Process -FilePath "powershell" -ArgumentList $arg -WorkingDirectory "..\DiceGameFunction\bin\Debug\netcoreapp3.0\" 
