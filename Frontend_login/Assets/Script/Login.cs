using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private string LoginEndpoint = "http://127.0.0.1:70707/account/login";
    [SerializeField] private string CreateEndpoint = "http://127.0.0.1:70707/account/create";

    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    
    public void OnLoginClick()
    {
        alertText.text = "Signing in...";
        ActivateButtons(false);

        StartCoroutine(TryLogin());
    }

    public void OnCreateClick()
    {
        alertText.text = "Creating account...";
        ActivateButtons(false);

        StartCoroutine(TryCreate());
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

        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);

        UnityWebRequest request = UnityWebRequest.Post(LoginEndpoint, form);
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
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (response.code == 0) //login success?
            {
                ActivateButtons(false);
                alertText.text = "Welcome ";
            }
            else
            {
                switch (response.code)
                {
                    case 1:
                        alertText.text = "Invalid crededential";
                        ActivateButtons(true);
                        break;
                    default:
                        alertText.text = "Corruption detected";
                        ActivateButtons(false);
                        break;
                }
            }

        }
        else
        {
            alertText.text = "Error connecting to the server...";
            ActivateButtons(true);
        }

        yield return null;
    }

    private IEnumerator TryCreate()
    {

        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (username.Length < 3 || username.Length > 25)
        {
            alertText.text = "Invalid username";
            ActivateButtons(true);
            yield break;
        }
        if (password.Length < 3 || password.Length > 25)
        {
            alertText.text = "Invalid password";
            ActivateButtons(true);
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);

        UnityWebRequest request = UnityWebRequest.Post(CreateEndpoint, form);
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
            CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);
            if (response.code == 0) //login success?
            {
                alertText.text = "Account has been created";
                ActivateButtons(true);
            }
            else
            {
                switch (response.code)
                {
                    case 1:
                        alertText.text = "Invalid crededential";
                        break;
                    case 2:
                        alertText.text = "Username is alreqady taken";
                        break;
                    default:
                        alertText.text = "Corruption detected";
                        break;
                }
            }

        }
        else
        {
            alertText.text = "Error connecting to the server...";
        }
        ActivateButtons(true);

        yield return null;
    }

    private void ActivateButtons(bool toggle)
    {
        loginButton.interactable = toggle;
        createButton.interactable = toggle;
    }
}
