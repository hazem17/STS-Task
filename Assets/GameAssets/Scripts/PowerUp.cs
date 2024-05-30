using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum PowerUpType
{
    Time,
    Missile,
    Speed
}
public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    [SerializeField] private Transform powerUpIcon;
    private Collider col;

    [Header("Float Options")]
    [SerializeField] private float mag;
    [SerializeField] private float frq;
    private Vector3 initLocation;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        StartCoroutine(AnimateIcon());
        initLocation = transform.position;
    }
    void Update()
    {
        transform.position = initLocation + new Vector3(0, math.sin(Time.time * frq) * mag, 0);
    }
    public void DestroyPowerUp()
    {
        StartCoroutine(AnimatePowerUp(false));
        col.enabled = false;
    }
    IEnumerator AnimatePowerUp(bool state)
    {
        float init = state ? 0 : 1;
        float target = state ? 1 : 0;
        AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float animTime = 0.5f;
        float timer = 0;
        while (timer < animTime)
        {
            transform.localScale = math.lerp(init, target, animCurve.Evaluate(timer / animTime)) * Vector3.one;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = target * Vector3.one;

        if (!state)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator AnimateIcon()
    {
        AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float timer = 0;
        float animTime = 1f;
        while (timer < animTime)
        {
            powerUpIcon.localEulerAngles = new Vector3(0, math.lerp(0, 359, animCurve.Evaluate(timer / animTime)),0);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(AnimateIcon());
    }
}
