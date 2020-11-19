# Deploy to Azure by Azure cli

param (
    [parameter(Mandatory=$true)][String]$subscriptionId,
    [parameter(Mandatory=$true)][String]$resourceGroupName,
    [parameter(Mandatory=$true)][String]$deploymentName,
    [parameter(Mandatory=$true)][String]$location
)

##################################
# variables
##################################
$appName = "${deploymentName}App";
$appPlanName = "${deploymentName}Plan";
$storageAccountName = "log${deploymentName}";
$storageContainerName = '$web';
$projWebApp = "..\DiceGameSignalRServer\DiceGameSignalRServer.csproj";
$projPage = ".\DiceGame.csproj";
$configuration = "Release";
$buildOutputWebApp = "$pwd\outputWebApp"
$buildOutputPage = "$pwd\outputpage"

##################################
# Global Stuff
##################################
$ErrorActionPreference = "Stop"

Function CheckExitCode($errorMessage) {
    if ($LASTEXITCODE -ne 0 ) {
        Write-Error $errorMessage
        Exit -1
    }
}

az account set --subscription $subscriptionId

CheckExitCode "Please ansure Azure Cli is logged in"

##################################
# Resource Group
##################################

$rgexists = az group exists -n $resourceGroupName --subscription $subscriptionId

if ($rgexists -ieq "false") {
   "Resourcegroup not found => start create" | Write-Host -ForegroundColor green 
   az group create --name $resourceGroupName --location $location --subscription $subscriptionId

   CheckExitCode "Create Resource Group Failed"
} else {
   Write-Host "Resourcegroup exists" -ForegroundColor Green
}


##################################
# Create App
##################################

$global:ErrorActionPreference = "SilentlyContinue"

az webapp show --name $appName --resource-group $resourceGroupName

if ($LASTEXITCODE -ne 0 ) {
    "`r`nCreate Plan" | Write-Host -foregroundcolor green

    "`r`naz appservice plan create -g $resourceGroupName -n `"${appPlanName}`" --sku FREE --location $location" | Write-Host
    az appservice plan create -g $resourceGroupName -n "${appPlanName}" --sku FREE --location $location

    CheckExitCode "create appservice plan failed"

    "`r`n`r`nCreate webapp`r`n" | Write-Host -foregroundcolor green

    "`r`naz webapp create --name $appName --resource-group $resourceGroupName --plan `"${appPlanName}`"" | Write-Host
    az webapp create --name $appName --resource-group $resourceGroupName --plan "${appPlanName}"

   CheckExitCode  "create function failed"
}

##################################
# Settings
##################################

# TODO

##################################
# Build and Deploy Web App
##################################

# Build
Write-Host "Build $projWebApp"
if (-not (Test-Path $buildOutputWebApp)) {
  mkdir $buildOutputWebApp
}
dotnet publish $projWebApp --configuration $configuration --output $buildOutputWebApp
CheckExitCode "Build Failed"

# ZIP output
Write-Host "Zip "
$zipInput = [System.IO.Path]::Combine($buildOutputWebApp, "*")
if ([string]::IsNullOrEmpty($zipFile)) {    
    $zipFile = ".\\$deploymentName.zip"
}
$zipOutputDir = [System.IO.Path]::GetDirectoryName($zipFile)
New-Item -ItemType Directory -Force -Path $zipOutputDir
Compress-Archive -Path $zipInput -DestinationPath $zipFile -Force
CheckExitCode "Zip failed"

# Deploy
Write-Host "*** Deploy WebApp into resource '$appName'"
az webapp deployment source config-zip -g $resourceGroupName -n $appName --src $zipFile
CheckExitCode "WebApp deployment failed"

##################################
# Build and Deploy Page
##################################

# Build
Write-Host "Build $projPage"
if (-not (Test-Path $buildOutputPage)) {
  mkdir $buildOutputPage
}
dotnet publish $projPage --configuration $configuration --output $buildOutputPage
CheckExitCode "Build Failed"

# Deploy
Write-Host "Deploy Resources"
$storageAccountKey = az storage account keys list -g $resourceGroupName -n $storageAccountName
$storageAccountKeyString = ($storageAccountKey | ConvertFrom-Json )[0].value
# az storage blob upload-batch leads to an error in Powershell-ISE as the information about uploaded files is written to stderr. According to https://github.com/Azure/azure-cli/issues/5714 this is by design
# in powershell everything is fine
az storage blob upload-batch -d $storageContainerName -s "$buildOutputPage\wwwroot" --account-name $storageAccountName --account-key $storageAccountKeyString --pattern "*" 
CheckExitCode "Deploy Resources failed - az storage blob upload-batch failed"

Write-Host "Deploy Resources complete"