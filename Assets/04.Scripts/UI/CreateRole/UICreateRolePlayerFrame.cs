using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRolePlayerFrame : MonoBehaviour
{
    public GameObject RemoveButton;
    public UISprite Position;

    [HideInInspector]
    public string[] PosSpriteNames;

	[UsedImplicitly]
	private void Start()
    {
	    Clear();
	}

    [UsedImplicitly]
    // Update is called once per frame
    private void Update()
    {
	
	}

    public void Clear()
    {
        RemoveButton.SetActive(false);
        Position.gameObject.SetActive(false);
    }

    public void SetPlayer(TGreatPlayer player)
    {
        RemoveButton.SetActive(true);

        Position.gameObject.SetActive(true);
        Position.spriteName = PosSpriteNames[player.BodyType];
    }
}
