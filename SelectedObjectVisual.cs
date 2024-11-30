using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectVisual : MonoBehaviour
{
    [SerializeField] public ISelectableObject selectableObject;
    [SerializeField] public GameObject[] visualGameObjectArray;


    private void Start() {
        Player.Instance.OnSelectedObjectChanged += Player_OnSelectedObjectChanged;
    }

    private void Player_OnSelectedObjectChanged(object sender, Player.OnSelectedObjectChangedEventArgs e) {
        if (e.selectedObject == selectableObject) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            if (visualGameObject != null) {
                visualGameObject.SetActive(true);
            }
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            if (visualGameObject != null) {
                visualGameObject.SetActive(false);
            }
        }
    }

}

