using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

public class OpenfortController : MonoBehaviour
{
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
        // Handle success
        Debug.Log("Successfully called CreateOpenfortPlayer");
        // If the function returns data, you can access it with:
        // var returnedData = result.FunctionResult;
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
