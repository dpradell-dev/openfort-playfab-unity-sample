using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayFabAuth : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject choosePanel;
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Login")]
    public TMP_InputField lEmail;
    public TMP_InputField lPassword;
    
    [Header("Register")]
    public TMP_InputField rEmail;
    public TMP_InputField rPassword;
    
    [Header("Other")]
    public TextMeshProUGUI statusText;

    [Header("Events")]
    public UnityEvent OnLoginSuccess;

    #region PUBLIC_METHODS
    public void LoginUser()
    {
        if(!ValidateInput(lEmail.text, lEmail.text)) return;
        
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = lEmail.text,
            Password = lPassword.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, LoginSuccess, OnError);
        loginPanel.SetActive(false);
    }
    
    public void RegisterUser()
    {
        if(!ValidateInput(rEmail.text, rPassword.text)) return;
        
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = rEmail.text,
            Password = rPassword.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, OnError);
        registerPanel.SetActive(false);
    }
    #endregion

    #region CALLBACK_HANDLERS
    void LoginSuccess(LoginResult result)
    {
        Debug.Log("Successfully logged in!");
        OnLoginSuccess?.Invoke();
    }
    void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Successfully registered user!");
        //OnRegisterSuccess?.Invoke();

        lEmail.text = rEmail.text;
        lPassword.text = rPassword.text;
        
        LoginUser();
    }

    void OnError(PlayFabError error)
    {
        choosePanel.SetActive(true);
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region PRIVATE_METHODS
    bool ValidateInput(string email, string pswd)
    {
        if(string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            Debug.LogError("Invalid email address.");
            return false;
        }

        if(string.IsNullOrEmpty(pswd) || pswd.Length < 6)
        {
            Debug.LogError("Password must be at least 6 characters.");
            return false;
        }

        return true;
    }
    #endregion
}