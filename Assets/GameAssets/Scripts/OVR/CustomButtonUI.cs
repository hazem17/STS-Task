using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CustomButtonUI : MonoBehaviour
{

    private Image buttonImage;
    [SerializeField] private Sprite normalImg;
    [SerializeField] private Sprite highlightImg;
    [SerializeField] private Sprite pressImg;

    public UnityEvent buttonAction;

    private BoxCollider buttonCollider;
    // Start is called before the first frame update
    void Start()
    {
        buttonCollider = gameObject.AddComponent<BoxCollider>();

        if (GetComponent<RectTransform>() != null)
        {
            print(GetComponent<RectTransform>().rect.size.x);
            buttonCollider.size = new Vector3(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height, 0.02f);
        }
        else
        {
            buttonCollider.size = Vector3.one;
        }
        buttonCollider.isTrigger = true;
        buttonImage = GetComponent<Image>();
        normalImg = buttonImage.sprite;
    }

    //-------------------------
    //-- Toggle Button collision
    //-------------------------
    private void Toggle(bool state)
    {
        buttonCollider.enabled = state;
    }

    //-------------------------
    //-- Change Button State: normal, hover and press
    //-------------------------
    public void NormalButton()
    {
        if (buttonCollider.enabled)
            buttonImage.sprite = normalImg;
    }
    public void HoverButton()
    {
        if (buttonCollider.enabled)
            buttonImage.sprite = highlightImg;
    }

    public void PressButton()
    {
        if (buttonCollider.enabled)
            buttonImage.sprite = pressImg;
        buttonAction.Invoke();
        AudioManager.Instance.PlaySFX("Click");
    }
}
