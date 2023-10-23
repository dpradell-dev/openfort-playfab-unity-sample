# Openfort-PlayFab Integration in Unity

## Overview
[PlayFab](https://playfab.com/) is a backend service provided by Microsoft for game developers, offering tools for live game management, all powered by Azure's cloud infrastructure.

In this integration, we use the [Google Play Games plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity) to handle authentication with PlayFab, ensuring a seamless and secure login experience for users on Android devices.

By integrating the [Openfort SDK](https://github.com/openfort-xyz/openfort-node) into Azure Functions, we establish a seamless connection to PlayFab. Unity clients using the PlayFab Unity SDK can tap into these functions, accessing the full range of Openfort features within the game environment.

## Application Workflow

//TODO graphic

## Prerequisites
+ [Create a PlayFab account and title](https://learn.microsoft.com/en-us/gaming/playfab/gamemanager/quickstart)
+ Set up your Azure development environment:
    + [Configure your environment](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-node?pivots=nodejs-model-v4#configure-your-environment)
    + [Sign in to Azure](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-node?pivots=nodejs-model-v4#sign-in-to-azure)
    + [Create a function app](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-node?pivots=nodejs-model-v4#create-the-function-app-in-azure)
+ [Sign in to dashboard.openfort.xyz](http://dashboard.openfort.xyz) and create a new project
+ Download or clone the [sample project](https://github.com/dpradell-dev/openfort-playfab-unity-sample): 
    + Open [unity-client](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/unity-client) with Unity
    + Open [azure-backend](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/azure-backend) with VS Code
+ Watch the [video tutorial](https://www.youtube.com/watch?v=dbLpA2YB6vU) on configuring *Google Play Games for Unity* and apply it to [unity-client](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/unity-client). Note that:
    + Google Play Games SDK is already imported to [unity-client](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/unity-client)
    + Skip the instructions related to ***Unity Gaming Services***
    + We provide a [Keystore](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/unity-client/Keystore//TODOlink) (password: `android`). Use this instead of [creating a new one](https://youtu.be/dbLpA2YB6vU?t=273)
    + Skip the step to [create this script](https://youtu.be/dbLpA2YB6vU?t=1089)

## Set up Openfort

1. #### [Add a Contract](https://dashboard.openfort.xyz/assets/new)
   This sample requires a contract to run. We use [0x38090d1636069c0ff1Af6bc1737Fb996B7f63AC0](https://mumbai.polygonscan.com/address/0x38090d1636069c0ff1Af6bc1737Fb996B7f63AC0) (NFT contract deployed in 80001 Mumbai). You can use this for the guide:

   ![Policy rule image](docs-img/image-1.png)

2. #### [Add a Policy](https://dashboard.openfort.xyz/policies/new)
   We aim to cover gas fees for users. Set a new gas policy:

   ![Policy rule image](docs-img/image.png)

   Now, add a rule so our contract uses this policy:

   ![Policy rule image](docs-img/image-2.png)

## Deploy Azure Backend
Open [azure-backend](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/azure-backend) with VS Code and sign in to Azure:

![Azure backend image](docs-img/image-4.png)

Ensure your Function App (here, it's "openfort-playfab") is listed:

![Function App image](docs-img/image-5.png)

In the terminal, run:
```
npm install
```

In the explorer, right-click on a function and select ***Deploy to Function App***:

![Deploy to Function App](docs-img/image-6.png)

Next, choose your Function App:

![Select Function App](docs-img/image-7.png)

Then, click on ***Deploy***:

![Deploy Confirmation](docs-img/image-8.png)

Navigate to your [Azure Portal](https://portal.azure.com/#home) and open your Function App. You should see all the functions listed:

![Functions List](docs-img/image-10.png)

Click on any function and select ***Get Function Url***:

![Get Function URL](docs-img/image-11.png)

Subsequently, add this URL (along with all others) to PlayFab to enable access to our Azure Functions from within PlayFab.

## Set up PlayFab Title

1. #### Register Azure Functions
    Visit the [PlayFab developer dashboard](https://developer.playfab.com/), choose your title, and click on ***Automation***:

    ![Automation Selection](docs-img/image-12.png)

    Our functions are already registered. To do the same, click ***Register function*** and provide the function name along with its URL:

    ![Function Registration](docs-img/image-13.png)

    Repeat this for all deployed functions.

2. #### Install Google Add-on
    Navigate to ***Add-ons*** and choose ***Google***:

    ![Google Add-on Selection](docs-img/image-15.png)

    Then, visit the [Google Cloud APIs Credentials dashboard](https://console.cloud.google.com/apis/credentials) and click on the arrow icon next to your ***OAuth Web Application Client***:

    ![OAuth Web Application Client](docs-img/image-16.png)

    Copy both the ***Client ID*** and ***Client Secret***:

    ![Client Details](docs-img/image-17.png)

    Return to the PlayFab dashboard, paste these details into the ***Google OAuth Client ID*** and ***Google OAuth Client Secret*** fields, and click ***Install Google***:

    ![Google Installation](docs-img/image-18.png)

3. #### Add PlayFab OAuth Redirect URL
    For this, go to the [Google Cloud APIs Credentials dashboard](https://console.cloud.google.com/apis/credentials). Here, select your *Web application*:

    ![Web Application Selection](docs-img/image-19.png)

    In the ***Authorized redirect URIs*** section, add the URL below and click ***Save***:
    ```
    https://oauth.playfab.com/oauth2/google
    ```
    ![Authorized Redirect URIs](docs-img/image-20.png)

## Set up Azure Backend

Our Azure backend requires environment variables from both PlayFab and Openfort. Let's configure them.

1. #### Add Openfort Environment Variables
    - Navigate to the [Azure Portal](https://portal.azure.com/#home) and select your Function App.
    - Under ***Configuration***, click ***New application setting***:
      
      ![Azure Configuration](docs-img/image-21.png)

    - Provide the following details:
      + Name: `OF_API_KEY`
      + Value: [Retrieve the **Secret key**](https://dashboard.openfort.xyz/apikeys)

    - Add another application setting:
      + Name: `OF_NFT_CONTRACT`
      + Value: [Retrieve the **Contract API ID**](https://dashboard.openfort.xyz/assets)

    - And another:
      + Name: `OF_SPONSOR_POLICY`
      + Value: [Retrieve the **Policy API ID**](https://dashboard.openfort.xyz/policies)

    After adding these, your configuration panel should resemble the following. Click ***Save***:
    
    ![Configuration Save](docs-img/image-23.png)

2. #### Add PlayFab Environment Variables
    - Visit the [PlayFab developer dashboard](https://developer.playfab.com/), select your title, and navigate to ***Settings wheel --> Title settings***:

      ![PlayFab Settings](docs-img/image-24.png)

    - In the ***API Features*** section, copy your ***Title ID***:

      ![Title ID](docs-img/image-29.png)

    - Under ***Secret Keys***, note down your ***Secret key***:

      ![Secret Key](docs-img/image-26.png)

    - Return to the [Azure Portal](https://portal.azure.com/#home) and choose your Function App.
    - Under ***Configuration***, select ***New application setting*** and input:
      + Name: `PLAYFAB_TITLE_ID`
      + Value: [Your Title ID]

    - Add another application setting:
      + Name: `PLAYFAB_SECRET_KEY`
      + Value: [Your Secret Key]

    Your configuration panel should now look like the following. Confirm your changes by clicking ***Save***:

    ![PlayFab Configuration Save](docs-img/image-27.png)

## Set up Unity client
For your information, this Unity sample project already comes with:
+ [PlayFab Unity SDK](https://github.com/PlayFab/UnitySDK)
+ [Google Play Games Unity Plugin (v11.01)](https://github.com/playgameservices/play-games-plugin-for-unity) 

Open [unity-client](https://github.com/dpradell-dev/openfort-playfab-unity-sample/tree/main/unity-client) with Unity. We need to do very few things to get it running:

1. #### Set up PlayFab SDK
    In the ***Project*** tab, search for `PlayFabSharedSettings` and set your PlayFab ***Title ID***:

    ![Alt text](docs-img/image-28.png)

2. #### Set up Google Play Games SDK
    You should have the Google Play Games SDK set-up by following the [required tutorial](//TODOlink), but go to ***Window --> Google Play Games --> Setup --> Android setup*** to confirm you have these fields correctly filled:

    ![Alt text](docs-img/image-30.png)

    ![Alt text](docs-img/image-31.png)

    This will let us use Google Play Games to authenticate our users via PlayFab (remember [Google Add-on](https://github.com/dpradell-dev/openfort-playfab-unity-sample#install-google-add-on//TODOlink)) when our game/application is running on Android. If not, we will use PlayFab standard authentication.

## Run the game in Editor
***Play*** the game, choose ***Register*** --> Enter some email and password --> choose ***Register*** again and you will encounter this scene:

![Alt text](docs-img/image-32.png)

Choose ***Mint*** and after some time you will see the representation of your minted NFT:

![Alt text](docs-img/image-33.png)

If you go to your [Openfort Players dashboard](https://dashboard.openfort.xyz/players), you will see a new player has been created. Select it:

![Alt text](docs-img/image-34.png)

There you will see that a `mint` transaction has been completed successfully:

![Alt text](docs-img/image-35.png)

You can also select your **Mumbai Account** and under ***ERC-721 Token Txns*** you'll see the transaction:

![Alt text](docs-img/image-36.png)

![Alt text](docs-img/image-37.png)

## Run the game in Android

If you build and run the game to your Android device, you won't need to register/login manually as Google Play Games will take care of it, making the experience even more frictionless.

## Conclusion
Once you've completed all the steps above, your Unity game should now be integrated with Openfort and PlayFab. Remember to test all functionalities before deploying to ensure a seamless player experience.

If you want to know more in detail what's happening behind the scenes, please watch this [tutorial video]() (coming soon).

For any issues or further queries, consider linking to a contact or support page.
