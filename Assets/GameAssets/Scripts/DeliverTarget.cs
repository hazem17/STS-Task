using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverTarget : MonoBehaviour
{
    [SerializeField] private GameObject triggerObject;
    [SerializeField] private Transform mapPinpoint;
    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        triggerObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        mapPinpoint.localEulerAngles = new Vector3(90, 0, -GameManager.Instance.BikeController.transform.eulerAngles.y);
    }

    //-------------------------
    //-- Trigger Deliver Target
    //-------------------------
    public void TriggerTarget(bool state)
    {
        triggerObject.SetActive(state);
    }
}
