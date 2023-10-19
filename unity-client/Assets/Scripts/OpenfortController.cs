using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

public class OpenfortController : MonoBehaviour
{
    [System.Serializable]
    public class CreatePlayerResponse
    {
        public string playerId;
        public string playerWalletAddress;
    }
    
    [System.Serializable]
    public class FindTransactionIntentResponse
    {
        public string id;
    }
    
    [System.Serializable]
    private class GetTransactionIntentResponse
    {
        public bool minted;
        public string id;
    }

    public GameObject mintPanel;
    
    private string _playerId;
    private string _playerWalletAddress;

    public void CreatePlayer()
    {
        var request = new ExecuteFunctionRequest()
        {
            Entity = new EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType
            },
            FunctionName = "CreateOpenfortPlayer",
            GeneratePlayStreamEvent = true,
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, OnCreatePlayerSuccess, OnCreatePlayerError);
    }
    
    private void OnCreatePlayerSuccess(ExecuteFunctionResult result)
    {
        string json = result.FunctionResult.ToString();
        CreatePlayerResponse response = JsonUtility.FromJson<CreatePlayerResponse>(json);

        // Now you can use response.playerId and response.address
        Debug.Log($"Player ID: {response.playerId}, Player Wallet Address: {response.playerWalletAddress}");

        _playerId = response.playerId;
        _playerWalletAddress = response.playerWalletAddress;
        
        mintPanel.SetActive(true);
    }

    private void OnCreatePlayerError(PlayFabError error)
    {
        // Handle error
        Debug.LogError($"Failed to call CreateOpenfortPlayer: {error.GenerateErrorReport()}");
    }

    public void MintNFT()
    {
        if (string.IsNullOrEmpty(_playerId) || string.IsNullOrEmpty(_playerWalletAddress))
        {
            Debug.LogError("Player ID or Player Wallet Address is null or empty.");
            return;
        }
        
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "MintNFT", // Your Azure function name
            FunctionParameter = new
            {
                playerId = _playerId,
                receiverAddress = _playerWalletAddress
            },
            GeneratePlayStreamEvent = true
        };
        
        PlayFabCloudScriptAPI.ExecuteFunction(request, OnMintNftSuccess, OnMintNftError);
    }

    private void OnMintNftSuccess(ExecuteFunctionResult result)
    {
        Debug.Log("MINTED!");
        //TODO 
    }
    
    private void OnMintNftError(PlayFabError error)
    {
        Debug.Log(error);
        if (error.GenerateErrorReport().Contains("10000ms")) //Timeout, but probably succeeded.
        {
            FindTransactionIntent();
        }
        else
        {
            mintPanel.SetActive(true);
            //TODO status text?
            Debug.LogWarning(error.GenerateErrorReport());
        }
    }
    
    public void FindTransactionIntent()
    {
        if (string.IsNullOrEmpty(_playerId) || string.IsNullOrEmpty(_playerWalletAddress))
        {
            Debug.LogError("Player ID or Player Wallet Address is null or empty.");
            return;
        }
        
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "FindTransactionIntent", // Your Azure function name
            FunctionParameter = new
            {
                playerId = _playerId,
                receiverAddress = _playerWalletAddress
            },
            GeneratePlayStreamEvent = true
        };
        
        PlayFabCloudScriptAPI.ExecuteFunction(request, OnFindTransactionIntentSuccess, OnFindTransactionIntentError);
    }

    private void OnFindTransactionIntentError(PlayFabError error)
    {
        mintPanel.SetActive(true);
        Debug.LogWarning(error.GenerateErrorReport());
    }

    private void OnFindTransactionIntentSuccess(ExecuteFunctionResult result)
    {
        Debug.Log(result.FunctionResult);
        var json = result.FunctionResult.ToString();
        var responseObject = JsonUtility.FromJson<FindTransactionIntentResponse>(json);
        
        GetTransactionIntent(responseObject.id);
    }
    
    //TODO Get NFT?
    private void GetTransactionIntent(string transactionIntentId)
    {
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "GetTransactionIntent",
            FunctionParameter = new
            {
                transactionIntentId = transactionIntentId
            }
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, OnGetTransactionIntentSuccess, OnGetTransactionIntentError);
    }

    private void OnGetTransactionIntentSuccess(ExecuteFunctionResult result)
    {
        Debug.Log(result.FunctionResult);
        
        var responseObject = JsonUtility.FromJson<GetTransactionIntentResponse>(result.FunctionResult.ToString());
        if (responseObject.minted)
        {
            // Do something if true
            Debug.Log("Minted is true");
            //TODO DONE!!!!
        }
        else
        {
            // Do something else if false
            Debug.Log("Minted is false");
            GetTransactionIntent(responseObject.id);
        }
    }

    private void OnGetTransactionIntentError(PlayFabError error)
    {
        mintPanel.SetActive(true);
        Debug.LogWarning(error.GenerateErrorReport());
    }
}
