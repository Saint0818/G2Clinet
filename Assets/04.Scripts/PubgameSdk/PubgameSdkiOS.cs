using UnityEngine;
using System.Collections;

namespace Pubgame
{
	public class PubgameSdkiOS : IPubgameSdk
	{
		private System.Action<bool, PubgameLoginResponse> _loginResponse;

		public void Init()
		{
			Debug.Log("PubgameSdkiOS.Init");
		}
		
		public void Login(System.Action<bool, PubgameLoginResponse> callback)
		{
			_loginResponse = callback;
			if(_loginResponse != null)
			{
				_loginResponse(true, new PubgameLoginResponse());
			}
		}
	
		public void SetPgToolsActive(bool isActive)
		{
		}
	}

}
