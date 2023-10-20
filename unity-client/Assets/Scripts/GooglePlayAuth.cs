using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.Events;

public class GooglePlayAuth : MonoBehaviour
{
    public UnityEvent<string> OnGooglePlayAuthSuccess;
    public UnityEvent OnGooglePlayAuthError;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Authenticate();
        }
        else
        {
            Debug.Log("Application.Platform is not Android. We go ahead with PlayFab standard login.");
            OnGooglePlayAuthError?.Invoke();
        }
    }

    public void Authenticate()
    {
        PlayGamesPlatform.Activate();
        
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play successful.");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, authCode =>
                {
                    Debug.Log($"Auth code is {authCode}");
                    OnGooglePlayAuthSuccess?.Invoke(authCode);
                });
            }
            else
            {
                Debug.LogError("Failed to retrieve Google Play auth code.");
                OnGooglePlayAuthError?.Invoke();
            }
        });
    }
}
