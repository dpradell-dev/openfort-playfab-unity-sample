using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

public class OpenfortController : MonoBehaviour
{
    [System.Serializable]
    public class OpenfortResponse
    {
        public string playerId;
        public string address;
    }

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
        OpenfortResponse response = JsonUtility.FromJson<OpenfortResponse>(json);

        // Now you can use response.playerId and response.address
        Debug.Log($"Player ID: {response.playerId}, Address: {response.address}");
    }

    private void OnCreatePlayerError(PlayFabError error)
    {
        // Handle error
        Debug.LogError($"Failed to call CreateOpenfortPlayer: {error.GenerateErrorReport()}");
    }

    public void ClaimSomething()
    {
        //TODO
    }
}
