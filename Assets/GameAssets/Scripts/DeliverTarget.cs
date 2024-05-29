using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverTarget : MonoBehaviour
{
    [SerializeField] private GameObject triggerObject;
    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        triggerObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerTarget(bool state)
    {
        triggerObject.SetActive(state);
    }
}
