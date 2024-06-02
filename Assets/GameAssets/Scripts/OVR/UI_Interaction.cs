using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UI_Interaction : MonoBehaviour
{
    public OVRInput.RawButton pressButton = OVRInput.RawButton.RHandTrigger;

    [Header("Line Renderer")]
    private LineRenderer lineRenderer;
    [SerializeField] private LayerMask hitMask;

    CustomButtonUI selectedButton;
    private bool isPressing;
    public float rayLength = 100;
    public bool isEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
            return;

        //-------------------------
        //-- Raycasting logic: hover, press and normal, trigger button actions
        //-------------------------
        if (OVRInput.GetDown(pressButton))
        {
            if (selectedButton != null)
            {
                selectedButton.PressButton();
                isPressing = true;
            }
        }
        else if (OVRInput.GetUp(pressButton))
        {
            isPressing = false;
            if (selectedButton != null)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, rayLength, hitMask))
                {
                    if (selectedButton == hit.collider.GetComponent<CustomButtonUI>())
                    {
                        selectedButton.HoverButton();
                    }
                    else
                    {
                        selectedButton.NormalButton();
                    }
                }

            }
        }
        else
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            lineRenderer.SetPosition(0, transform.position);

            if (!isPressing)
            {
                if (Physics.Raycast(ray, out hit, rayLength, hitMask))
                {
                    if (selectedButton != null)
                    {
                        selectedButton.NormalButton();
                    }
                    lineRenderer.SetPosition(1, hit.point);
                    selectedButton = hit.collider.GetComponent<CustomButtonUI>();
                    selectedButton.HoverButton();
                }
                else if (selectedButton != null)
                {
                    lineRenderer.SetPosition(1, transform.position + transform.forward * rayLength);
                    selectedButton.NormalButton();
                    selectedButton = null;
                }
                else
                {
                    // Nothing was hit, deactive the last interactive item.
                    lineRenderer.SetPosition(1, transform.position + transform.forward * rayLength);
                }
            }
            else
            {
                lineRenderer.SetPosition(1, transform.position + transform.forward * rayLength);

            }
        }
    }

    //-------------------------
    //-- toggle raycasting
    //-------------------------
    public void ToggleRay(bool state)
    {
        if (state)
        {
            isEnabled = true;
            lineRenderer.enabled = true;
            isPressing = false;
            selectedButton = null;
        }
        else
        {
            isEnabled = false;
            lineRenderer.enabled = false;
            isPressing = false;
            selectedButton = null;
        }
    }
}
