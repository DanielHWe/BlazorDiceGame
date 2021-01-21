# BlazorDiceGame

[![Travis Build Status](https://travis-ci.org/DanielHWe/BlazorDiceGame.svg?branch=master)](https://travis-ci.org/DanielHWe/BlazorDiceGame)

A simple dice game programmed as .Net Core Blazor WebAssembly. It can be played standalone in one browser or as network game by use of signalR.

## Idea

After seeing [Blatris](https://builtwithdot.net/project/183/blazor-experimental-tetris-bletris) as Blaor Tetris I had the idear to use same SVG functions to build a simple dice and move game.

Current version is first with network game support by use of signalR

# How to get started

## Deploy

For the one browser support, get the Zip from releases and deploy the wwwroot folder to a webspace. In the moment it is expected that the url is on domain level.

For Deployment of the signalR Server part you must deploy the DiceGameSignalR project. I will create a deployment script later.

Server Url can be changed in wwwroot/appSettings.json.

## Modify and Compile

There are folowing C# projects
* DiceGame.Interfaces: Some common interfaces and helper
* DiceGame: Blazor WebAssembly project (for understanding the project please start here)
* DiceGameSignalRServer: SignalR server part for network game support
* DiceGameFunction: Serverless Project (currently not working) 

In the DiceGame project, there are two simple pages:
* Index.razor just define players and player names
* game.razor game view include svg handling

And there is one Service:
Services/GameSevice, representing the model of the game and the game logic. 


# Next steps
* Deployment Scripts
* Code Cleanup
* Static Code Analysis
* 4 Player Mode
* Winning animation
* Unit Tests
* Ingame Chat
* Finish Serverless Mode

# Demo
[http://blazordicegame.daniel-wehrle.de/](http://blazordicegame.daniel-wehrle.de/)

## Multilanguage

Thanks to [aksoftware98](https://github.com/aksoftware98/multilanguages) for the multilaguage Blazor package.