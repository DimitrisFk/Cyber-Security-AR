using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;

public class PlayFabControls : MonoBehaviour
{
    [SerializeField] GameObject signUpTab, logInTab, startPanel, HUD;
    public Text username, userEmail, userPassword, userEmailLogin, userPasswordLogin, errorSignUp, errorLogin;
    string encryptedPassword;

    string MyPlayfabID;
    public static string dname;

    public void SwitchToSignUpTab()
    {
        signUpTab.SetActive(true);
        logInTab.SetActive(false);
        errorSignUp.text = "";
        errorLogin.text = "";
    }

    public void SwitchToLogInTab()
    {
        signUpTab.SetActive(false);
        logInTab.SetActive(true);
        errorSignUp.text = "";
        errorLogin.text = "";
    }

    string Encrypt(string pass)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        return s.ToString();
    }

    public void SignUp()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail.text, Password = Encrypt(userPassword.text), Username = username.text };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
    }

    public void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        errorSignUp.text = "";
        errorLogin.text = "";
        OnUpdatePlayerName();
        StartGame();
        signUpTab.SetActive(false);
    }

    public void RegisterError(PlayFabError error)
    {
        errorSignUp.text = error.GenerateErrorReport();
    }

    public void LogIn()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmailLogin.text, Password = Encrypt(userPasswordLogin.text) };
        PlayFabClientAPI.LoginWithEmailAddress(request, LogInSuccess, LogInError);
    }

    public void LogInSuccess(LoginResult result)
    {
        GetAccountInfo();
        GetPlayerProfile(MyPlayfabID);
        errorSignUp.text = "";
        errorLogin.text = "";
        logInTab.SetActive(false);
        StartGame();
    }

    public void LogInError(PlayFabError error)
    {
        errorLogin.text = error.GenerateErrorReport();
    }

    void StartGame()
    {
        startPanel.SetActive(false);
        HUD.SetActive(true);
    }

    public void OnUpdatePlayerName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username.text
        }, result =>
        {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }
    //****************************************************************************************************************************************
    void GetPlayerProfile(string playFabId)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result => dname = result.PlayerProfile.DisplayName,
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void GetAccountInfo()
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, Successs, fail);
    }


    public void Successs(GetAccountInfoResult result)
    {

        MyPlayfabID = result.AccountInfo.PlayFabId;

    }


    public void fail(PlayFabError error)
    {

        Debug.LogError(error.GenerateErrorReport());
    }
}
