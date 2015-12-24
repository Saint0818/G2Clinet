using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameJoystick : ETCJoystick {
    private float speed;
    protected override void SetActivated (){
        base.SetActivated();

        if (activated) {
            GetComponent<Image>().color = Color.white;
            thumb.GetComponent<Image>().color = Color.white;
        } else {
            GetComponent<Image>().color = new Color32(255, 255, 255, 150);
            thumb.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        }
    }

    public void AttachPlayer(PlayerBehaviour player) {
        onMoveStart.AddListener(player.OnJoystickStart);
        onMove.AddListener(player.OnJoystickMove);
        onMoveEnd.AddListener(player.OnJoystickMoveEnd);
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
