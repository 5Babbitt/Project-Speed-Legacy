using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CarController : MonoBehaviour
{
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    public GameObject bananaPrefab;
    public GameObject missilePrefab;
    public Transform carTransform;
    private Rigidbody rb;
    private Camera cam;
    private Transform camObject;
    [SerializeField] private Transform centreOfMass;

    public bool barrierActive = false;
    public float carSpeed;


    private bool hasBoost = false;
    private bool hasBanana = false;
    private bool hasBarrier = false;
    private bool hasMissile = false;
    private bool isBoosted = false;
    private bool isSlowed = false;    
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private bool isUsingItem;
    private float speedBoostTimer;
    private float barrierTimer;
    private float mass = -0.9f;
    private bool isAccelerating;
    //private Vector3 lastPosition ;
    //private Vector3 lastVelocity;
    //private Vector3 lastAcceleration;
    

    [SerializeField] private float motorForce;
    [SerializeField] private float defaultSpeed;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftCol;
    [SerializeField] private WheelCollider frontRightCol;
    [SerializeField] private WheelCollider backLeftCol;
    [SerializeField] private WheelCollider backRightCol;

    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform backLeftTransform;
    [SerializeField] private Transform backRightTransform;

    [SerializeField] private PhotonView photonView;

    [SerializeField] private TrackCheckpoints trackCheckpoints;
    private List<CheckpointSingle> checkpointSingles;


    //private void Awake()
    //{
    //    Vector3 position = transform.position;
    //    Vector3 velocity = Vector3.zero;
    //    Vector3 acceleration = Vector3.zero;
    //}
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = gameObject.GetComponent<Rigidbody>();
        //rb.centerOfMass = new Vector3(0f, mass, 0f);
        rb.centerOfMass = centreOfMass.localPosition;

        cam = GetComponentInChildren<Camera>();
        camObject = transform.GetChild(2);

        trackCheckpoints = Transform.FindObjectOfType<TrackCheckpoints>();

        checkpointSingles = trackCheckpoints.checkpointSingleList;
    }

    private void FixedUpdate()
    {
        carSpeed = rb.velocity.magnitude;

        if (photonView.IsMine)
        {
            Timers();
            GetInput();
            HandleMotor();
            HandleSteering();
            HandleUsingItem();
            UpdateWheels();
        }
        else if(!photonView.IsMine)
        {
            camObject.gameObject.SetActive(false);
        }
        //SpeedFOV();
    }

    //private bool CheckAcceleration() 
    //{
    //    Vector3 position = transform.position;
    //    Vector3 velocity = (position - lastPosition) / Time.deltaTime;
    //    Vector3 acceleration = (velocity - lastVelocity) / Time.deltaTime;
    //    if(acceleration.magnitude > lastAcceleration.magnitude)
    //    {
    //        return true;
    //        Debug.Log("Accelerating");
    //    }
    //    else
    //    {
    //        return false;
    //        Debug.Log("Decelerating");
    //    }
    //    lastAcceleration = acceleration;
    //    lastVelocity = velocity;
    //    lastPosition = position;
    //}
    private void GetInput()
    {
        horizontalInput = Input.GetAxis(Horizontal);
        verticalInput = Input.GetAxis(Vertical);
        isBreaking = Input.GetKey(KeyCode.Space);
        isUsingItem = Input.GetKey(KeyCode.E);

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetVehicle();
        }

    }

    private void HandleMotor()
    {
        backLeftCol.motorTorque = verticalInput * motorForce;
        backRightCol.motorTorque = verticalInput * motorForce;
        //frontLeftCol.motorTorque = verticalInput * motorForce;
        //frontRightCol.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking || (Mathf.Abs(verticalInput) < 0.05) ? breakForce : 0f;

        BreakingActive();       
    }
    private void BreakingActive()
    {
        frontLeftCol.brakeTorque = currentbreakForce;
        frontRightCol.brakeTorque = currentbreakForce;
        backLeftCol.brakeTorque = currentbreakForce;
        backRightCol.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftCol.steerAngle = currentSteerAngle;
        frontRightCol.steerAngle = currentSteerAngle;
        frontLeftCol.motorTorque = horizontalInput * motorForce * 2;
        frontRightCol.motorTorque = horizontalInput * -motorForce * 2;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftCol, frontLeftTransform);
        UpdateSingleWheel(frontRightCol, frontRightTransform);
        UpdateSingleWheel(backLeftCol, backLeftTransform);
        UpdateSingleWheel(backRightCol, backRightTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void Timers()
    {
        if (isBoosted && speedBoostTimer < Time.time) // speed boost timer
        {
            setSpeed(defaultSpeed);
            isBoosted = false;
        }
        if (barrierActive && barrierTimer < Time.time)
        {
            barrierActive = false;
        }
    }

    private void setSpeed(float speed)
    {
        motorForce = speed;
    }

    public void ActivateBoost()
    {
        float boostDuration = 3f;
        speedBoostTimer = Time.time + boostDuration;
        isBoosted = true;
        setSpeed(defaultSpeed * 2f);
        hasBoost = false;
    }

    private void ActivateBanana()
    {
        Instantiate(bananaPrefab, transform.position - new Vector3(0, 0, 3), Quaternion.identity);
        hasBanana = false;
    }
    private void ActivateBarrier()
    {
        barrierActive = true;
        float boostDuration = 3f;
        barrierTimer = Time.time + boostDuration;
        hasBarrier = false;
    }

    private void ActivateMissile()
    {
        Debug.Log("shooting missile");
        GameObject missile = Instantiate(missilePrefab, carTransform.position + new Vector3(0,1,0) + (carTransform.forward*4) , carTransform.rotation * Quaternion.Euler(90f, 0f, 0f));
        Rigidbody missileRigidbody = missile.GetComponent<Rigidbody>();
        missileRigidbody.velocity = carTransform.forward * 10f;
        hasMissile = false;
    }

    public void GiveBoost()
    {
        hasBoost = true;
    }

    public void GiveBanana()
    {
        hasBanana = true;
    }

    public void GiveBarrier()
    {
        hasBarrier = true;
    }

    public void GiveMissile()
    {
        hasMissile = true;
    }

    private void HandleUsingItem()
    {
        if (hasBoost && isUsingItem)
        {
            ActivateBoost();
        } else if (hasBanana && isUsingItem)
        {
            ActivateBanana();
         }
        else if (hasBarrier && isUsingItem)
        {
            ActivateBarrier();
        }
        else if (hasMissile && isUsingItem)
        {
            ActivateMissile();
        }
    }

    public void Slip()
    {
        if (!barrierActive)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void ResetVehicle()
    {
        if(photonView.IsMine)
        {
            int currentCheckpoint = trackCheckpoints.nextCheckpointSingleIndexList[trackCheckpoints.GetCarTransformList().IndexOf(this.transform)] - 1;
            Vector3 checkpointSpawnPosition = checkpointSingles[currentCheckpoint].spawnPoint.position;

            transform.position = checkpointSpawnPosition;
            transform.forward = checkpointSingles[currentCheckpoint].transform.right;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            return;
        }
    }

    //public void SpeedFOV()
    //{
    //    if(carSpeed >= 30 && CheckAcceleration())
    //    {
    //        cam.fieldOfView += (carSpeed - 30) / 8;
    //        if(cam.fieldOfView >= 80)
    //        {
    //            cam.fieldOfView = 80;
    //        }
    //    }
    //    else if(carSpeed >= 30 && CheckAcceleration() == false)
    //    {
    //        cam.fieldOfView -= (carSpeed - 30) / 4;
    //    }
    //}
}
