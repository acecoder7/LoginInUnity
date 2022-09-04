using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private string authenticateEndpoint = "http://127.0.0.1:13756/account";

    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    
    public void OnLoginClick()
    {
        alertText.text = "Signing in...";
        loginButton.interactable = false;

        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {

        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if(username.Length<3 || username.Length > 25)
        {
            alertText.text = "Invalid username";
            loginButton.interactable = true;
            yield break;
        }
        if(password.Length<3 || password.Length > 25)
        {
            alertText.text = "Invalid password";
            loginButton.interactable = true;
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get($"{authenticateEndpoint}?rUsername={username}&rPassword={password}");
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }
            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text != "Invalid credential") //login success?
            {
                loginButton.interactable = false;
                GameAccount returnedAccount = JsonUtility.FromJson<GameAccount>(request.downloadHandler.text);
                alertText.text = "Welcome " + returnedAccount.username;
            }
            else
            {
                alertText.text = "Invalid credentials...";
                loginButton.interactable = true;
            }

        }
        else
        {
            alertText.text = "Error connecting to the server...";
            loginButton.interactable = true;
        }

        yield return null;
    }

    
}