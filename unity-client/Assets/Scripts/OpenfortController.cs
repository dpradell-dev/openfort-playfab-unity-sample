using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using EntityKey = PlayFab.CloudScriptModels.EntityKey;

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
    
    [System.Serializable]
    public class NftItemList
    {
        public NftItem[] items;
    }
    
    [System.Serializable]
    public class NftItem
    {
        public string assetType;
        public string amount;
        public int tokenId;
        public string address;
        public long lastTransferredAt;
    }

    public UnityEvent OnCreatePlayerErrorEvent;

    public GameObject mintPanel;
    public TextMeshProUGUI statusText;
    
    private string _playerId;
    private string _playerWalletAddress;


    #region AZURE_FUNCTION_CALLERS
    public void PlayFabAuth_OnLoginSuccess_Handler()
    {
        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserReadOnlyData(request, result =>
        {
            if (result.Data != null && result.Data.ContainsKey("OpenfortPlayerId") &&
                result.Data.ContainsKey("PlayerWalletAddress"))
            {
                _playerId = result.Data["OpenfortPlayerId"].Value;
                _playerWalletAddress = result.Data["PlayerWalletAddress"].Value;
                
                GetPlayerNftInventory(_playerId);
            }
            else
            {
                CreatePlayer();
            }
        }, error =>
        {
            // Back to login panel
            OnCreatePlayerErrorEvent?.Invoke();
        });
    }
    
    private void CreatePlayer()
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
        
        PlayFabCloudScriptAPI.ExecuteFunction(request, OnFindTransactionIntentSuccess, OnGeneralError);
    }
    
    private void GetTransactionIntent(string transactionIntentId)
    {
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "GetTransactionIntent",
            FunctionParameter = new
            {
                transactionIntentId
            }
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, OnGetTransactionIntentSuccess, OnGeneralError);
    }
    
    private void GetPlayerNftInventory(string playerId)
    {
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "GetPlayerNftInventory",
            FunctionParameter = new
            {
                playerId
            }
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, OnGetPlayerNftInventorySuccess, OnGeneralError);
    }
    #endregion

    #region SUCCESS_CALLBACK_HANDLERS
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

    private void OnMintNftSuccess(ExecuteFunctionResult result)
    {
        Debug.Log("minted = true");
        GetPlayerNftInventory(_playerId);
    }

    private void OnFindTransactionIntentSuccess(ExecuteFunctionResult result)
    {
        Debug.Log(result.FunctionResult);
        var json = result.FunctionResult.ToString();
        var responseObject = JsonUtility.FromJson<FindTransactionIntentResponse>(json);
        
        GetTransactionIntent(responseObject.id);
    }

    private void OnGetTransactionIntentSuccess(ExecuteFunctionResult result)
    {
        Debug.Log(result.FunctionResult);
        
        var responseObject = JsonUtility.FromJson<GetTransactionIntentResponse>(result.FunctionResult.ToString());
        if (responseObject.minted)
        {
            Debug.Log("Minted is true");
            GetPlayerNftInventory(_playerId);
        }
        else
        {
            // Do something else if false
            Debug.Log("Minted is false");
            GetTransactionIntent(responseObject.id);
        }
    }

    private void OnGetPlayerNftInventorySuccess(ExecuteFunctionResult result)
    {
        Debug.Log(result.FunctionResult);
        var json = result.FunctionResult.ToString();
        List<NftItem> nftItems = JsonConvert.DeserializeObject<List<NftItem>>(json);

        foreach (var nft in nftItems)
        {
            Debug.Log(nft.tokenId);
        }
    }
    #endregion

    #region ERROR_CALLBACK_HANDLERS
    private void OnCreatePlayerError(PlayFabError error)
    {
        Debug.LogError($"Failed to call CreateOpenfortPlayer: {error.GenerateErrorReport()}");
        OnCreatePlayerErrorEvent?.Invoke();
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
    
    private void OnGeneralError(PlayFabError error)
    {
        mintPanel.SetActive(true);
        Debug.LogWarning(error.GenerateErrorReport());
    }
    #endregion
}
