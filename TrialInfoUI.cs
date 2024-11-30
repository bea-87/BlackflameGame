using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrialInfoUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject trialStartedTextObject;
    [SerializeField] private GameObject trialEndedTextObject;
    [SerializeField] private TextMeshProUGUI timerText; //textmeshpro
    [SerializeField] private TextMeshProUGUI gameOverUI;
    [SerializeField] public GameObject restartButton;
    [SerializeField] private Trial trial;
    public float startTime;


    private void Start() {
        player.OnTrialActivated += Player_OnTrialActivated;
    }

    private void Update() {
        if (!trial.trialComplete) {
            float elapsedTime = Time.time - startTime;
            UpdateTimerText(elapsedTime);
        }
    }

    private void UpdateTimerText(float elapsedTime) {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = "Time: " + timerString;
    }


    private void Player_OnTrialActivated(object sender, Player.OnTrialActivatedEventArgs e) {
        bool trialActive = e.active; // Access the active state from event args

        if (trialActive) {
            StartCoroutine(ShowTextForSeconds(trialStartedTextObject, 1f)); // Show trial started text for 1 second
        } else {
            StartCoroutine(ShowTextForSeconds(trialEndedTextObject, 1f)); // Show trial ended text for 1 second
        }
    }

    private IEnumerator ShowTextForSeconds(GameObject textObject, float duration) {
        textObject.SetActive(true);

        yield return new WaitForSeconds(duration); // Wait for specified duration

        textObject.SetActive(false);
    }

    public void ShowGameOverUI(float trialTime) {
        gameOverUI.SetActive(true);
        restartButton.SetActive(true);
        // DISPLAY TRIAL TIME
    }
}
