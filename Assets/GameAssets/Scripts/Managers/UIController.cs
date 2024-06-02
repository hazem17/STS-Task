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
    [SerializeField] private UI_Interaction UI_RayCaster;

    [Header("Text Message Options")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private AnimationCurve messageAlphaCurveAppear;
    [SerializeField] private AnimationCurve messageAlphaCurveDisappear;
    [SerializeField] private AnimationCurve messageMoveCurve;
    [SerializeField] private CanvasGroup messageTextCanvasGroup;
    [SerializeField] private Transform messageTextObject;

    [Header("Main Panel Options")]
    [SerializeField] private CanvasGroup mainPanelCanvas;
    [SerializeField] private CanvasGroup controlsPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject finalScorePanel;
    [SerializeField] private GameObject startMenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        SetTimer(235);
        messageTextCanvasGroup.alpha = 0;
        controlsPanel.alpha = 0;
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

    public void ToggleControls(bool state)
    {
        StartCoroutine(ToggleCanvasGroupAnimated(controlsPanel, state));
    }

    public void ShowEndScreen(int score)
    {
        mainPanelCanvas.gameObject.SetActive(true);
        finalScoreText.text = score.ToString();
        finalScorePanel.SetActive(true);
        startMenuPanel.SetActive(false);
        ToggleMainMen(true);
        UI_RayCaster.gameObject.SetActive(true);
    }

    public void ToggleMainMen(bool state)
    {
        StartCoroutine(ToggleCanvasGroupAnimated(mainPanelCanvas, state));
    }

    IEnumerator ToggleCanvasGroupAnimated(CanvasGroup canvasGroup, bool state)
    {
        AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float timer = 0;
        float init = state ? 0 : 1;
        float target = state ? 1 : 0;
        float animTime = 1f;
        while (timer < animTime)
        {
            canvasGroup.alpha =  Mathf.Lerp(init, target, animCurve.Evaluate(timer / animTime));
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = target;
        if (!state)
        {
            canvasGroup.gameObject.SetActive(false);
            UI_RayCaster.gameObject.SetActive(false);
        }

    }
}
