using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using static UnityEngine.GraphicsBuffer;

public class BikeController : MonoBehaviour
{
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;

    WheelControl[] wheels;
    Rigidbody rigidBody;


    [SerializeField] private float currentSpeed;
    [SerializeField] private float acc;
    [SerializeField] private float steeringSpeed;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    [Header("Steering Options")]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform rightHandRef;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform leftHandRef;
    [SerializeField] private Transform steeringCenter;
    [SerializeField] private Transform steeringObject;


    [Header("Direction Options")]
    [SerializeField] private Transform directionArrow;
    [SerializeField] private TextMeshPro distanceText;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
       // rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();
    }

    // Update is called once per frame
    void Update()
    {
        rightHandRef.position = new Vector3(rightHand.position.x, steeringCenter.position.y, rightHand.position.z);
        leftHandRef.position = new Vector3(leftHand.position.x, steeringCenter.position.y, leftHand.position.z);

        Vector3 steeringDirection = rightHandRef.position - leftHandRef.position;
        Debug.DrawRay(steeringCenter.position, steeringDirection , Color.yellow);
        float angle = math.sign(Vector3.Cross(steeringCenter.right, steeringDirection).y) * Vector3.Angle(steeringCenter.right, steeringDirection);
        print(angle);
        steeringObject.localEulerAngles = new Vector3(0, math.clamp(angle,-30,30), 0);

        float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");

        if (GameManager.Instance.currentDeliverTarget)
        {
            distanceText.text = (int)Vector3.Distance(transform.position, GameManager.Instance.currentDeliverTarget.transform.position) + "m";

            directionArrow.transform.LookAt(new Vector3(GameManager.Instance.currentDeliverTarget.transform.position.x, directionArrow.position.y, GameManager.Instance.currentDeliverTarget.transform.position.z));
        }

        //// Calculate current speed in relation to the forward direction of the car
        //// (this returns a negative number when traveling backwards)
        //float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);


        //// Calculate how close the car is to top speed
        //// as a number from zero to one
        //float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        //// Use that to calculate how much torque is available 
        //// (zero torque at top speed)
        //float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        //// …and to calculate how much to steer 
        //// (the car steers more gently at top speed)
        //float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        //// Check whether the user input is in the same direction 
        //// as the car's velocity
        //bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        //foreach (var wheel in wheels)
        //{
        //    // Apply steering to Wheel colliders that have "Steerable" enabled
        //    if (wheel.steerable)
        //    {
        //        wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
        //    }

        //    if (isAccelerating)
        //    {
        //        // Apply torque to Wheel colliders that have "Motorized" enabled
        //        if (wheel.motorized)
        //        {
        //            wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
        //        }
        //        wheel.WheelCollider.brakeTorque = 0;
        //    }
        //    else
        //    {
        //        // If the user is trying to go in the opposite direction
        //        // apply brakes to all wheels
        //        wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
        //        wheel.WheelCollider.motorTorque = 0;
        //    }
        //}

        //RaycastHit hit;
        //// Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.4f, groundMask))
        //{
        //    Debug.DrawRay(transform.position, -Vector3.up * hit.distance, Color.yellow);
        //    Debug.Log("Did Hit");
        //    isGrounded = true;
        //}
        //else
        //{
        //    isGrounded = false;
        //}

        currentSpeed = Mathf.Clamp(currentSpeed + vInput * acc * Time.deltaTime, 0, maxSpeed);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (rigidBody.velocity.magnitude > 0)
        {
            transform.Rotate(transform.up, Time.deltaTime * angle);
        }
        if (isGrounded)
        {
            //currentSpeed = Mathf.Clamp(currentSpeed + vInput * acc * Time.deltaTime, 0, maxSpeed);
            //rigidBody.velocity = transform.forward * currentSpeed;
            //transform.Rotate(transform.up, hInput * Time.deltaTime * steeringSpeed);
            
        }
        else
        {
            //rigidBody.velocity = Vector3.zero;
        }
        
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeliverTarget"))
        {
            print("Package delivered");
        }
    }


}
