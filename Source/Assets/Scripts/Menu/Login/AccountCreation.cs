using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountCreation : MonoBehaviour
{
	private const string RegisterUrl = ".../register.php";

	[SerializeField] private InputField Username = null;
	[SerializeField] private InputField Password = null;
	[SerializeField] private InputField PasswordRepeat = null;
	[SerializeField] private InputField Email = null;
	[SerializeField] private Button CreateAccountButton = null;

	/// <summary>
	/// Checks Field for correct information.
	/// </summary>
	public void OnCreateAccount()
	{
		if (Username.text == string.Empty || Password.text == string.Empty ||
			PasswordRepeat.text == string.Empty || Email.text == string.Empty)
		{
			Debug.Log("Please fill out all fields!");
		}
		else if (!Password.text.Equals(PasswordRepeat.text))
		{
			Debug.Log("Passwords do not match!");
		}
		else
		{
			StartCoroutine(TryCreateAccount(Username.text, Password.text, Email.text));
			
			if (CreateAccountButton)
			{
				CreateAccountButton.interactable = false;
			}
		}
	}

	/// <summary>
	/// Creates account with given data
	/// </summary>
	/// <param name="username">Entered from user</param>
	/// <param name="password">Entered from user</param>
	/// <param name="email">Entered from user</param>
	IEnumerator TryCreateAccount(string username, string password, string email)
	{
		WWWForm form = new WWWForm();
		form.AddField("usernamePost", username);
		form.AddField("passwordPost", password);
		form.AddField("emailPost", email);

		Debug.Log("please wait a moment..");
		using (UnityWebRequest www = UnityWebRequest.Post(RegisterUrl, form))
		{
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError(www.error);
			}
			else
			{
				EvaluateBody(www);
			}
		}
	}

	/// <summary>
	/// Checks if Account creation were successful or not. 
	/// </summary>
	/// <param name="www"></param>
	private void EvaluateBody(UnityWebRequest www)
	{
		var body = www.downloadHandler.text;

		switch (body)
		{
			case "01":
				Debug.Log("Account created.");
				break;
			case "00":
			{
				Debug.Log("Account already Exists!");

				if (CreateAccountButton)
				{
					CreateAccountButton.interactable = true;
				}

				break;
			}

			default:
			{
				Debug.Log(body);

				if (CreateAccountButton)
				{
					CreateAccountButton.interactable = true;
				}

				break;
			}
		}
	}
}