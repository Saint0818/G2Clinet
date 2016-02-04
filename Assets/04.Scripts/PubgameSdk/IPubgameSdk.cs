using UnityEngine;
using System.Collections;

namespace Pubgame
{
	public interface IPubgameSdk
	{
		void Init();
		void Login(System.Action<bool, PubgameLoginResponse> callback);
		void SetPgToolsActive(bool isActive);
	}
}

