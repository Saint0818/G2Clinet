using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameJoystick : ETCJoystick {
    private float speed;
    protected override void SetActivated (){
        base.SetActivated();

		if (activated) {
			GetComponent<Image>().color = new Color32(255, 255, 255, 150);
			thumb.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        } else {
            GetComponent<Image>().color = new Color32(255, 255, 255, 150);
            thumb.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        }
    }

    public void AttachPlayer(PlayerBehaviour player) {
        onMoveStart.AddListener(player.OnJoystickStart);
        onMove.AddListener(player.OnJoystickMove);
        onMoveEnd.AddListener(player.OnJoystickMoveEnd);
		onTouchUp.AddListener(ShowJoystick);

        onMoveStart.AddListener(OnTutorial);
        onMove.AddListener(OnTutorialMove);
        onTouchStart.AddListener(OnTutorial);
    }

    public void OnTutorialMove(Vector2 v) {
        if (UITutorial.Visible) 
           UITutorial.Get.OnTutorial();
    }

    public void OnTutorial() {
        if (UITutorial.Visible) 
            UITutorial.Get.OnTutorial();
    }

	public void SetJoystickType (JoystickType type) {
		joystickType = type;
	}

	public void ShowJoystick () {
		visible = true;
		cachedRectTransform.anchoredPosition = new Vector3(-245, -130f, 0);
	}

	public Canvas GetCanvas {
		get {return cachedRootCanvas;}
	}

	public RectTransform GetTranform {
		get {return cachedRectTransform;}
	}

    public float Speed {
        get {
            return speed;
        }
        set {
            speed = value;
            axisX.speed = value;
            axisY.speed = value;
        }
    }
}
