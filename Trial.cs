using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Trial : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private TrialInfoUI trialInfoUI;

    public bool trialActive = false;
    public bool trialComplete = false;

    private float startTime;
    private List<GameObject> spawnedEnemies = new List<GameObject>();  //Keep track of spawned enemies

    private void Start() {
        player.OnTrialActivated += Player_OnTrialActivated;
        StartGame();
    }

    private void Player_OnTrialActivated(object sender, Player.OnTrialActivatedEventArgs e) {
        bool trialActive = e.active; //Access the active state from event args

        if (trialActive) {
            InvokeRepeating("SpawnEnemies", 0, 5);
        } else {
            CancelInvoke("SpawnEnemies");
            DestroySpawnedEnemies();  //Destroy all spawned enemies when the trial is inactive
        }
    }

    private void SpawnEnemies() {
        GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(18, 5, 90), Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.player = player;
        enemy.trial = this;
        SelectedObjectVisual selectedObjectVisual = enemyObject.GetComponent<SelectedObjectVisual>();
        selectedObjectVisual.selectableObject = enemy; // Assign the enemy as the selectable object

        enemy.WhenSpawned();
    }

    private void DestroySpawnedEnemies() {
        foreach (GameObject enemyObject in spawnedEnemies) {
            Destroy(enemyObject);
        }

        spawnedEnemies.Clear();  //Clear the list after destroying the enemies
    }

    public void RemoveEnemyFromList(Enemy enemy) {
        spawnedEnemies.Remove(enemy.gameObject); // Remove the enemy game object from the list
        Debug.Log("enemies in lisT:");
        foreach (var nmy in spawnedEnemies) {
            Debug.Log("hj");
            Debug.Log(nmy);
        }
        if (spawnedEnemies.Count == 0) {
            EndGame();
        }
    }

    public void AddEnemyToList(Enemy enemy) {
        spawnedEnemies.Add(enemy.gameObject); // Remove the enemy game object from the list
    }

    public void StartGame() {
        trialComplete = true;
        startTime = Time.time;
        trialInfoUI.startTime = startTime;
        Debug.Log("STARTING GAME");
    }

    private void EndGame() {
        trialComplete = false;
        float trialTime = Time.time - startTime;
        trialInfoUI.ShowGameOverUI(trialTime);
        player.ResetPos();
        Debug.Log("Ending GAME");
    }


    public void RestartGame() {
        StartGame();
        player.ResetAttributes();
    }

}

