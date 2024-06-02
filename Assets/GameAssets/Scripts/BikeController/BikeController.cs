using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using static UnityEngine.GraphicsBuffer;

public class BikeController : MonoBehaviour
{
    public bool PC_Controls;

    [Header("Controls")]
    public OVRInput.Axis1D speedBTN = OVRInput.Axis1D.SecondaryIndexTrigger;
    public OVRInput.Axis1D brakesBTN = OVRInput.Axis1D.PrimaryIndexTrigger;
    public OVRInput.RawButton pressButton = OVRInput.RawButton.RHandTrigger;
    public bool engineIsOn;

    [Header("Movement Options")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float currentReverse;
    [SerializeField] private float maxReverse;
    private CharacterController characterController;

    [Header("Ground Options")]
    [SerializeField] private Transform wheel1;
    [SerializeField] private Transform wheel2;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;
    private float notGroundedTimer = 5;

    [Header("Steering Options")]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform steeringCenter;
    [SerializeField] private Transform steeringObject;
    [SerializeField] private float steeringAngle;
    [SerializeField] private float maxSteeringAngle = 30;
    private Vector3 currentRotation;

    [Header("Target Direction Options")]
    [SerializeField] private Transform directionArrow;
    [SerializeField] private TextMeshPro distanceText;

    [Header("Power-ups Options")]
    [SerializeField] private ParticleSystem collectTimeParticle;
    [SerializeField] private ParticleSystem collectSpeedParticle;
    [SerializeField] private ParticleSystem speedEffectParticle;
    [SerializeField] private float nitrousPower;
    [SerializeField] private AnimationCurve nitrousCurve;
    [SerializeField] private float nitrousTime;
    [SerializeField] private Image nitrousImage;
    private bool hasNitrous;
    private float extraSpeed;


    void Start()
    {
        currentRotation.y = transform.eulerAngles.y;
        distanceText.text = "0m";
    }

    void Update()
    {
        float speed;
        float brakes;
        if (PC_Controls)
        {
            speed = Input.GetKey(KeyCode.W)? 1:0;
            brakes = Input.GetKey(KeyCode.S)? 1:0;

            float hInput = Input.GetAxis("Horizontal");
            steeringAngle = hInput * maxSteeringAngle;
        }
        else
        {
            speed = OVRInput.Get(speedBTN);
            brakes = OVRInput.Get(brakesBTN);
            CalculateSteering();
        }

        if (!engineIsOn)
            return;

        //--- Get Package direction
        GetPackageDirection();

        //---- movement
        MoveBike();
       

        //--- check if is grounded
        isGrounded = CheckIsGrounded();


        //-- calculate bike speed
        CalculateSpeed(speed, brakes);

        if (OVRInput.GetDown(pressButton) && hasNitrous)
        {
            StartCoroutine(UseNitrous());
            hasNitrous = false;
        }
        

    }

    //---- stop bike at Game End
    public void StopBike()
    {
        currentSpeed = 0;
        currentReverse = 0;
        engineIsOn= false;
    }

    //-------------------------
    //-- check if grounded to move car and if car fell off map
    //-------------------------
    private bool CheckIsGrounded()
    {
        RaycastHit hit1;
        RaycastHit hit2;
        bool firstWheelHit = Physics.Raycast(wheel1.position, -Vector3.up, out hit1, 1f, groundMask);
        bool secondWheelHit = Physics.Raycast(wheel2.position, -Vector3.up, out hit2, 1f, groundMask);
        if (firstWheelHit || secondWheelHit)
        {
            Debug.DrawRay(wheel1.position, -Vector3.up * hit1.distance, Color.yellow);
            Debug.DrawRay(wheel2.position, -Vector3.up * hit2.distance, Color.yellow);
            Debug.Log("Did Hit");
            notGroundedTimer = 5;
            return true;
        }
        else
        {
            notGroundedTimer-=Time.deltaTime;
            if (notGroundedTimer < 0)
                transform.position = Vector3.zero;
            return false;
        }
    }

    //-------------------------
    //-- Update speed and reverse values
    //-------------------------
    private void CalculateSpeed(float speed, float brakes)
    {
        if (isGrounded)
        {
            if (speed > 0)
            {
                currentSpeed += speed * acceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
            }
            if (brakes > 0)
            {
                currentReverse += math.abs(brakes) * deceleration * Time.deltaTime;
            }
            else
            {
                currentReverse -= acceleration * 2 * Time.deltaTime;
            }
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
            currentReverse -= deceleration * Time.deltaTime;
        }
        

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        currentReverse = math.clamp(currentReverse, 0, maxReverse);
    }


    //-------------------------
    //-- Move and rotate bike according to speed and steering angle
    //-------------------------
    private void MoveBike()
    {
        transform.position += (transform.forward * (currentSpeed + extraSpeed) * Time.deltaTime + transform.forward * -currentReverse * Time.deltaTime);

        if (currentSpeed > 0 || currentReverse > 0)
        {
            currentRotation.y += Time.deltaTime * steeringAngle;
            Quaternion Q = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);
            transform.rotation = Q;
        }
    }

    //-------------------------
    //-- Rotate direction arrow to point at the target package
    //-------------------------
    private void GetPackageDirection()
    {
        if (GameManager.Instance.currentDeliverTarget)
        {
            distanceText.text = (int)Vector3.Distance(transform.position, GameManager.Instance.currentDeliverTarget.transform.position) + "m";

            directionArrow.transform.LookAt(new Vector3(GameManager.Instance.currentDeliverTarget.transform.position.x, directionArrow.position.y, GameManager.Instance.currentDeliverTarget.transform.position.z));
        }
    }

    //-------------------------
    //-- Calculate steering angle accodring to hands location
    //-------------------------
    private void CalculateSteering()
    {
        Vector3 rightHandPos = new Vector3(rightHand.position.x, steeringCenter.position.y, rightHand.position.z);
        Vector3 leftHandPos = new Vector3(leftHand.position.x, steeringCenter.position.y, leftHand.position.z);

        Vector3 steeringDirection = rightHandPos - leftHandPos;
        Debug.DrawRay(steeringCenter.position, steeringDirection, Color.yellow);
        steeringAngle = math.sign(Vector3.Cross(steeringCenter.right, steeringDirection).y) * Vector3.Angle(steeringCenter.right, steeringDirection);
        steeringAngle = math.clamp(steeringAngle, -maxSteeringAngle, maxSteeringAngle);
        steeringObject.localEulerAngles = new Vector3(0, steeringAngle, 0);
    }

    //-------------------------
    //-- Player collected packages and powerups
    //-------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeliverTarget"))
        {
            print("Package delivered");
            GameManager.Instance.DeliverPackage();
            AudioManager.Instance.PlaySFX("DeliverPackage");
        }
        else if (other.CompareTag("PowerUp"))
        {
            PowerUp tempPow = other.gameObject.GetComponent<PowerUp>();
            AudioManager.Instance.PlaySFX("Collect");
            switch (tempPow.powerUpType)
            {
                case PowerUpType.Time:
                    GameManager.Instance.IncreaseTime();
                    collectTimeParticle.Play();
                    break;
                case PowerUpType.Missile:
                    break;
                case PowerUpType.Speed:
                    hasNitrous = true;
                    collectSpeedParticle.Play();
                    nitrousImage.color = Color.white;
                    break;
                default:
                    break;
            }
            tempPow.DestroyPowerUp();
        }
    }

    //-------------------------
    //-- user speed boost power-up
    //-------------------------
    IEnumerator UseNitrous()
    {
        speedEffectParticle.Play();
        nitrousImage.color = Color.white / 2;
        AudioManager.Instance.PlaySFX("Nitrous");
        float timer = 0;
        while (timer < nitrousTime)
        {
            extraSpeed = Mathf.Lerp(0, nitrousPower, nitrousCurve.Evaluate(timer / nitrousTime));
            timer += Time.deltaTime;
            yield return null;
        }
        speedEffectParticle.Stop();
        extraSpeed = 0;
    }


}
