## Overview
[PlayFab](https://playfab.com/) is a backend service by Microsoft for game developers, offering tools for live game management, all powered by Azure's cloud infrastructure.

By incorporating the [Openfort SDK](https://github.com/openfort-xyz/openfort-node) into Azure Functions, we create a seamless bridge to PlayFab. This connection allows Unity clients, through the PlayFab Unity SDK, to tap into these functions, making the entire range of Openfort functionalities readily available within the game environment.

## Prerequisites
+ [Create a PlayFab account and title](https://learn.microsoft.com/en-us/gaming/playfab/gamemanager/quickstart)
+ Prepare for Azure development:
    + [Configure environment](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-node?pivots=nodejs-model-v4#configure-your-environment)
    + [Sign in to Azure](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-node?pivots=nodejs-model-v4#sign-in-to-azure)
    + [Create a function app](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-node?pivots=nodejs-model-v4#create-the-function-app-in-azure)
+ [Sign in to dashboard.openfort.xyz](http://dashboard.openfort.xyz) and create a new project
+ Download or clone [sample project](https://github.com/dpradell-dev/openfort-playfab-unity-sample): //TODO change to openfort repo
    + Open [unity-client](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/unity-client) with Unity //TODO change to openfort repo 
    + Open [azure-backend](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/azure-backend) with VS Code //TODO change to openfort repo

## Set up Openfort

1. ### Add a Contract
This sample requires a Contract to run. We're using [0x38090d1636069c0ff1Af6bc1737Fb996B7f63AC0](https://mumbai.polygonscan.com/address/0x38090d1636069c0ff1Af6bc1737Fb996B7f63AC0) (NFT contract deployed in 80001 Mumbai). To follow this guide you can use it too.

### Auyeah

## Deploy Azure Functions



Add PlayFab title to Unity
Add PLAYFAB_API_KEY and PLAYFAB_SECRET_KEY to azure
Register functions to Playfab title
Add Openfort API Key to Azure config
Add Openfort ContractAddress to Azure config
Add Contract to Openfort
Add Policy to Openfort
Setup Policy and Contract (0x38090d1636069c0ff1Af6bc1737Fb996B7f63AC0) in Openfort?
keystore


Google play
Add google play sdk version 14


