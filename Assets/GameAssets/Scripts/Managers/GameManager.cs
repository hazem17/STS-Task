using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    [Header("Game Options")]
    [SerializeField] private BikeController bikeController;
    [SerializeField] private int playerScore;
    [SerializeField] private bool gameStarted;
    [SerializeField] private PowerUpSpawner powerUpSpawner;

    [Header("Packages Options")]
    [SerializeField] private List<DeliverTarget> deliverTargetArray;
    public DeliverTarget currentDeliverTarget;
    [SerializeField] private int baseDeliverScore;

    [Header("Time Options")]
    [SerializeField] private float gameTime;
    [SerializeField] private float currentTime;

    public BikeController BikeController { get => bikeController; }

    // Start is called before the first frame update
    void Start()
    {
        currentTime = gameTime;
        UIController.Instance.SetTimer(currentTime);
        UIController.Instance.UpdateScore(playerScore);
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownTimer());
        UIController.Instance.ToggleMainMen(false);
    }

    void Update()
    {
        if (gameStarted)
        {
            currentTime -= Time.deltaTime;
            UIController.Instance.SetTimer(currentTime);
            if (currentTime <= 0)
            {
                EndGame();
            }
        }

        if (Input.GetKey(KeyCode.F))
        {
            Time.timeScale = 5;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void EndGame()
    {
        gameStarted = false;
        UIController.Instance.SetTimer(0);
        print("End Game");
        bikeController.StopBike();
        UIController.Instance.DisplayMessage("Times Up!!!");
        UIController.Instance.ShowEndScreen(playerScore);
        AudioManager.Instance.SwitchMusic("MenuMusic");
    }

    public void DeliverPackage()
    {
        playerScore += baseDeliverScore;
        UIController.Instance.UpdateScore(playerScore);
        UIController.Instance.DisplayMessage("Package Delivered");

        DeliverTarget temp = currentDeliverTarget;
        temp.TriggerTarget(false);

        currentDeliverTarget = deliverTargetArray[Random.Range(0, deliverTargetArray.Count)];
        currentDeliverTarget.TriggerTarget(true);
        deliverTargetArray.Remove(currentDeliverTarget);
        deliverTargetArray.Add(temp);
    }

    IEnumerator CountdownTimer()
    {
        AudioManager.Instance.SwitchMusic("GameMusic");
        for (int i = 3; i >= 0; i--)
        {
            string displayText = i.ToString();
            if (i == 0)
            {
                displayText = "GO!!!";
                AudioManager.Instance.PlaySFX("Start");
            }
            else
            {
                AudioManager.Instance.PlaySFX("Countdown");
            }
            UIController.Instance.DisplayMessage(displayText);
            yield return new WaitForSeconds(1.55f);
        }
        StartGame();
    }

    private void StartGame()
    {
        gameStarted = true;
        bikeController.engineIsOn = true;
        currentDeliverTarget = deliverTargetArray[Random.Range(0, deliverTargetArray.Count)];
        currentDeliverTarget.TriggerTarget(true);
        deliverTargetArray.Remove(currentDeliverTarget);
        powerUpSpawner.StartSpawning();
       
    }
    public void IncreaseTime()
    {
        currentTime += 10;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
