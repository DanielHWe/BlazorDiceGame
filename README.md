# BlazorDiceGame

[![Travis Build Status](https://travis-ci.org/DanielHWe/BlazorDiceGame.svg?branch=master)](https://travis-ci.org/DanielHWe/BlazorDiceGame)

A simple dice game programmed as .Net Core Blazor WebAssembly.

## Idea

After seeing [Blatris](https://builtwithdot.net/project/183/blazor-experimental-tetris-bletris) as Blaor Tetris I had the idear to use same SVG functions to build a simple dice and move game.

In current state it is only possible to play on same screen, in the next step I want to bruing in a RESTService for beeing able to use it as a multiplayer game.

# How to get started

## Deploy

Get the Zip from releases and deploy the wwwroot folder to a webspace. In the moment it is expected that the url is on domain level.

## Modify and Compile

There are two simple pages:
* Index.razor just define players and player names
* game.razor game view include svg handling

And there is one Service:
Model/GameFiledModel, representing the model of the game and the game logic. 

# Next steps
* RESTService for PvP
* Show Message for winner

# Demo
[http://blazordicegame.daniel-wehrle.de/](http://blazordicegame.daniel-wehrle.de/)
