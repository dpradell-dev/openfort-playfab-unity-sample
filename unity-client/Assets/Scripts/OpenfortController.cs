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
        //TODO
        Debug.Log(error);
    }

    private void OnFindTransactionIntentSuccess(ExecuteFunctionResult result)
    {
        Debug.Log(result.FunctionResult);
        
        //TODO GetTransactionIntent
    }
}
