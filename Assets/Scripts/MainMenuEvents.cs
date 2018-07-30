/*
 * Copyright (C) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

// Mainmenu events.
using UnityEngine.SceneManagement;


public class MainMenuEvents : MonoBehaviour
{
	public float fadeSpeed = 1.5f; 
	public Image mFader;
     Text signinButtonText;
     Text authStatus;
	bool toBlack = false;
	bool toClear = true;

    void Awake() {
        mFader.color = Color.black;
        mFader.gameObject.SetActive(true);
        toClear = true;
    }

	void Start() {

        GameObject startButton = GameObject.Find("startButton");
        signinButtonText =
            GameObject.Find("signInButton").GetComponentInChildren<Text>();
        authStatus = GameObject.Find("authStatus").GetComponent<Text>();
        EventSystem.current.firstSelectedGameObject = startButton;

        // ADD Play Game Services init code here.
        //create client configuration
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        //enable debugging output
        PlayGamesPlatform.DebugLogEnabled = true;
        //initialize and activate platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        //try silent sign in
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
	}

    public void SignIn()
    {
        Debug.Log("signin button clicked");
        if(!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            PlayGamesPlatform.Instance.SignOut();
            signinButtonText.text = "Sign in";
            authStatus.text = "";
        }
    }

    public void SignInCallback(bool success)
    {
        if(success)
        {
            Debug.Log("signed in");
            signinButtonText.text = "Sign out";
            authStatus.text = "Signed in as " + Social.localUser.userName;

        }else
        {
            Debug.Log("sign in failed");
            signinButtonText.text = "Sign in";
            authStatus.text = "Signed in failed";

        }
    }

    void LateUpdate()
    {
        if (toBlack) {
            FadeToBlack();
        }
        else if (toClear) {
            FadeToClear();
        }
    }

	public void Play ()
	{
		Debug.Log ("Playing!!");
		toBlack = true;
		// Make sure the texture is enabled.
		mFader.gameObject.SetActive(true);
		
		// Start fading towards black.
		FadeToBlack();

		FadeController fader = gameObject.GetComponentInChildren<FadeController>();
		if (fader != null) {
			fader.FadeToLevel(()=>SceneManager.LoadScene("GameScene"));
		}
		else {
            SceneManager.LoadScene("GameScene");
		}
		

	}

	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and black.
		mFader.color = Color.Lerp(mFader.color, Color.clear, fadeSpeed * Time.deltaTime);
		// If the screen is almost black...
		if(mFader.color.a <= 0.05f) {

			toClear = false;
			mFader.gameObject.SetActive(false);
			
		}
	}

	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		mFader.color = Color.Lerp(mFader.color, Color.black, fadeSpeed * Time.deltaTime);
		// If the screen is almost black...
		if(mFader.color.a >= 0.95f) {
			// ... reload the level.
			toBlack = false;

		}
	}


}
