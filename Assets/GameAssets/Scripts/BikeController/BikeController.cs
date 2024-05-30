using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using static UnityEngine.GraphicsBuffer;

public class BikeController : MonoBehaviour
{
    public bool PC_Controls;
    //public OVRInput.RawButton brakesButton = OVRInput.RawButton.LIndexTrigger;
    public OVRInput.Axis1D speedBTN = OVRInput.Axis1D.SecondaryIndexTrigger;
    public OVRInput.Axis1D brakesBTN = OVRInput.Axis1D.PrimaryIndexTrigger;
    private Rigidbody rigidBody;

    public bool engineIsOn;

    [Header("Movement Options")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float currentReverse;
    [SerializeField] private float maxReverse;

    [Header("Ground Options")]
    [SerializeField] private Transform wheel1;
    [SerializeField] private Transform wheel2;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    [Header("Steering Options")]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform steeringCenter;
    [SerializeField] private Transform steeringObject;
    [SerializeField] private float steeringAngle;
    [SerializeField] private float maxSteeringAngle = 30;

    [Header("Target Direction Options")]
    [SerializeField] private Transform directionArrow;
    [SerializeField] private TextMeshPro distanceText;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        distanceText.text = "0m";
    }

    // Update is called once per frame
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

        //--- calculate steering 
        
        

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
        

    }

    public void StopBike()
    {
        currentSpeed = 0;
        currentReverse = 0;
        engineIsOn= false;
    }

    private bool CheckIsGrounded()
    {
        RaycastHit hit1;
        RaycastHit hit2;
        bool firstWheelHit = Physics.Raycast(wheel1.position, -Vector3.up, out hit1, 0.4f, groundMask);
        bool secondWheelHit = Physics.Raycast(wheel2.position, -Vector3.up, out hit2, 0.4f, groundMask);
        if (firstWheelHit || secondWheelHit)
        {
            Debug.DrawRay(wheel1.position, -Vector3.up * hit1.distance, Color.yellow);
            Debug.DrawRay(wheel2.position, -Vector3.up * hit2.distance, Color.yellow);
            Debug.Log("Did Hit");
            return true;

        }
        else
        {
            return false;
        }
    }

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

    private void MoveBike()
    {
        transform.position += (transform.forward * currentSpeed * Time.deltaTime + transform.forward * -currentReverse * Time.deltaTime);

        if (rigidBody.velocity.magnitude > 0)
        {
            transform.Rotate(transform.up, Time.deltaTime * steeringAngle);
        }
    }

    private void GetPackageDirection()
    {
        if (GameManager.Instance.currentDeliverTarget)
        {
            distanceText.text = (int)Vector3.Distance(transform.position, GameManager.Instance.currentDeliverTarget.transform.position) + "m";

            directionArrow.transform.LookAt(new Vector3(GameManager.Instance.currentDeliverTarget.transform.position.x, directionArrow.position.y, GameManager.Instance.currentDeliverTarget.transform.position.z));
        }
    }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeliverTarget"))
        {
            print("Package delivered");
            GameManager.Instance.DeliverPackage();
        }
        else if (other.CompareTag("PowerUp"))
        {
            PowerUp tempPow = other.gameObject.GetComponent<PowerUp>();
            switch (tempPow.powerUpType)
            {
                case PowerUpType.Time:
                    GameManager.Instance.IncreaseTime();
                    break;
                case PowerUpType.Missile:
                    break;
                case PowerUpType.Speed:
                    break;
                default:
                    break;
            }
            tempPow.DestroyPowerUp();
        }
    }


}
