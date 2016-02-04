using UnityEngine;
using System.Collections;

namespace Pubgame
{
    public class PubgameSdk : KnightSingleton<PubgameSdk> 
	{
		public bool ShowDebug = false;
		private static IPubgameSdk _currentPlatform;
		private bool _toggle = false;

        protected override void Init()
		{
			DontDestroyOnLoad(transform.gameObject);
            gameObject.name = "PubgameSdk";
		}
		
		public void InitSDK()
		{
			Debug.Log("PubgameSdk.Init");
#if UNITY_EDITOR
			return;
#endif

#if UNITY_ANDROID
			_currentPlatform = new PubgameSdkAndroid();
#elif UNITY_IPHONE
			_currentPlatform = new PubgameSdkiOS();
#endif
			
			if(_currentPlatform != null)
				_currentPlatform.Init();
			else
				Debug.Log("PubgameSdk not avalible for current platform");
		}
		
		public void Login(System.Action<bool, PubgameLoginResponse> callback)
		{
			Debug.Log("PubgameSdk.Login");
#if UNITY_EDITOR
			return;
#endif
			if(_currentPlatform != null)
				_currentPlatform.Login(callback);			
		}

		public void SetPgToolsActive(bool active)
		{
			Debug.Log("SetPgToolsActive = " + active);
			if(_currentPlatform == null)
				return;

			_currentPlatform.SetPgToolsActive(active);
		}
				
		private void OnApplicationPause(bool paused)
		{
#if UNITY_EDITOR
			return;
#endif
			if(_currentPlatform == null)
				return;

			if(paused)
			{
				_currentPlatform.SetPgToolsActive(false);
			}
			else
			{
				_currentPlatform.SetPgToolsActive(true);
			}
		}
		
		private void OnGUI() 
		{
			if(!ShowDebug)
				return;

			if (GUI.Button(new Rect(10, 10, 200, 80), "Init"))
			{
				Init();
			}

			if(GUI.Button(new Rect(10, 100, 200, 80), "Login"))
			{
				Login(OnLoginResponseTest);
			}

			if(GUI.Button(new Rect(10, 190, 200, 80), "Toggle"))
			{
				SetPgToolsActive(_toggle);
				_toggle = !_toggle;
			}

		}

		private void OnLoginResponseTest(bool isSuccess, PubgameLoginResponse inResponse)
		{
			Debug.Log("Receive callback OnLoginResponse isSuccess = " + isSuccess);
			Debug.Log(string.Format("playerId:{0} token:{1} ", inResponse.PlayerId, inResponse.Token));
		}

	}

}



