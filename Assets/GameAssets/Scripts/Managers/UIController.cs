using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Mathematics;

public class UIController : Singleton<UIController>
{
    [Header("UI Options")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Text Message Options")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private AnimationCurve messageAlphaCurveAppear;
    [SerializeField] private AnimationCurve messageAlphaCurveDisappear;
    [SerializeField] private AnimationCurve messageMoveCurve;
    [SerializeField] private CanvasGroup messageTextCanvasGroup;
    [SerializeField] private Transform messageTextObject;
    // Start is called before the first frame update
    void Start()
    {
        SetTimer(235);
        messageTextCanvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimer(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void DisplayMessage(string message)
    {
        messageText.text = message;
        StartCoroutine(AnimateTextMessage(0.5f));
    }
    IEnumerator AnimateTextMessage(float animTime)
    {
        float timer = 0;
        while (timer < animTime)
        {
            messageTextCanvasGroup.alpha = math.lerp(0, 1, messageAlphaCurveAppear.Evaluate(timer / animTime));
            messageTextObject.localPosition = new Vector2(math.lerp(800, 0, messageMoveCurve.Evaluate(timer / animTime)), messageTextObject.localPosition.y); 

            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        timer = 0;
        while (timer < animTime)
        {
            messageTextCanvasGroup.alpha = math.lerp(1, 0, messageAlphaCurveDisappear.Evaluate(timer / animTime));
            messageTextObject.localPosition = new Vector2(math.lerp(0, -800, messageMoveCurve.Evaluate(timer / animTime)), messageTextObject.localPosition.y);

            timer += Time.deltaTime;
            yield return null;
        }
        messageTextCanvasGroup.alpha = 0;
    }
}
