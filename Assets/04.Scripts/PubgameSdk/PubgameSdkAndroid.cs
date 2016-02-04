using UnityEngine;
using System.Collections;

namespace Pubgame
{
	public class PubgameEventListener : AndroidJavaProxy
	{
		private PubgameSdkAndroid _target;
		
		public PubgameEventListener(PubgameSdkAndroid inTarget) : base("com.pubgame.unityplugin.PubgameEventListener")
		{
			_target = inTarget;
		}

		public void onLoginResponse(int inResultCode, string inPlayerId, string inToken)
		{
			_target.onLoginResponse(inResultCode, inPlayerId, inToken);
		}

		public string toString()
		{
			return "PubgameEventListener";
		}
	}
	
	public class PubgameSdkAndroid : IPubgameSdk
	{
		private AndroidJavaObject _unityActivity;
		private AndroidJavaObject _unityPlugin;
		private PubgameEventListener _eventListener;
		private System.Action<bool, PubgameLoginResponse> _loginResponse;
		
		public void Init()
		{
			_eventListener = new PubgameEventListener(this);
			_unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			_unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => 
			                                                             {
				_unityPlugin = new AndroidJavaObject("com.pubgame.unityplugin.PubgameUnityPlugin");
				_unityPlugin.Call("init", _unityActivity, _eventListener);
			}));

		}

		public void Login(System.Action<bool, PubgameLoginResponse> callback)
		{
			if(!IsInit())
				return;

			_loginResponse = callback;
			
			_unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => 
			                                                             {
				_unityPlugin.Call("login");
			}));
		}
		
		public void onLoginResponse(int inResultCode, string inPlayerId, string inToken)
		{
			Debug.Log(string.Format("onLoginSuccess inPlayerId:{0} inToken:{1}", inPlayerId, inToken));
			var response = new PubgameLoginResponse();
			response.ResultCode = inResultCode;
			response.PlayerId = inPlayerId;
			response.Token = inToken;

			_loginResponse(true, response);
		}

		public void SetPgToolsActive(bool isActive)
		{
			if(!IsInit())
				return;

			_unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => 
			                                                             {
				_unityPlugin.Call("setPgToolActive", isActive);
			}));
		}

		private bool IsInit()
		{
			if(_unityActivity == null)
			{
				Debug.LogWarning("IsInit unityActivity is null");
				return false;
			}

			if(_unityPlugin == null)
			{
				Debug.LogWarning("IsInit _unityPlugin is null");
				return false;
			}
			return true;
		}
	}
}


