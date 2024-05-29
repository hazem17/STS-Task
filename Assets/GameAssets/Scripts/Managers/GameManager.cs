using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [Header("Game Options")]
    [SerializeField] private BikeController bikeController;
    [SerializeField] private int playerScore;
    [SerializeField] private bool gameStarted;

    [Header("Packages Options")]
    [SerializeField] private List<DeliverTarget> deliverTargetArray;
    public DeliverTarget currentDeliverTarget;
    [SerializeField] private int baseDeliverScore;

    [Header("Time Options")]
    [SerializeField] private float gameTime;
    [SerializeField] private float currentTime;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        currentTime = gameTime;
        UIController.Instance.SetTimer(currentTime);
        UIController.Instance.UpdateScore(playerScore);

        yield return new WaitForSeconds(3);
        StartCoroutine(CountdownTimer());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            currentTime -= Time.deltaTime;
            UIController.Instance.SetTimer(currentTime);
            if (currentTime <= 0)
            {
                gameStarted = false;
                UIController.Instance.SetTimer(0);
                print("End Game");
                bikeController.engineIsOn = false;
            }
        }
    }

    public void DeliverPackage()
    {
        playerScore += baseDeliverScore;
        UIController.Instance.UpdateScore(playerScore);

        DeliverTarget temp = currentDeliverTarget;
        temp.TriggerTarget(false);

        currentDeliverTarget = deliverTargetArray[Random.Range(0, deliverTargetArray.Count)];
        currentDeliverTarget.TriggerTarget(true);
        deliverTargetArray.Remove(currentDeliverTarget);
        deliverTargetArray.Add(temp);
    }

    IEnumerator CountdownTimer()
    {
        for (int i = 3; i >= 0; i--)
        {
            string displayText = i.ToString();
            if (i == 0)
            {
                displayText = "GO!!!";
            }
            UIController.Instance.DisplayMessage(displayText);
            yield return new WaitForSeconds(2);
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
    }
}
