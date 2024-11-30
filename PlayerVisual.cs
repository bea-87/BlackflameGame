using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject VisualEnforce;
    [SerializeField] private Player player;

    //Use start not Awake for events cause it happens after
    void Start()
    {
        player.OnStateChanged += Player_OnStateChanged;
    }

    private void Player_OnStateChanged(object sender, System.EventArgs e) {
        if (player.returnPlayerState() != 1) {
            Hide();
        } else {
            Show();
        }
    }

    private void Show() {
        VisualEnforce.SetActive(true);
    }

    private void Hide() {
        VisualEnforce.SetActive(false);
    }
}
