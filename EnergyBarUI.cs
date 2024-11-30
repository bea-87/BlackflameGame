using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image bar;

    void Update()
    {
        bar.fillAmount = player.returnPlayerEnergy() / player.returnPlayerMaxEnergy();
    }
}
