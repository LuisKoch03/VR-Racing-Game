using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform gasPedal;
    public Transform brakePedal;

    public float pedalPressDistance = 0.05f; 
    public float pedalMoveSpeed = 5f;

    private Vector3 gasPedalStartPos;
    private Vector3 brakePedalStartPos;


    public Transform steeringWheel;
    public float wheelTurnAngle = 270f;

    public ParticleSystem rearLeftSmoke;
    public ParticleSystem rearRightSmoke;

    public float motorForce = 1500f;
    public float steeringAngle = 30f;

    public WheelCollider ColliderFrontRight, ColliderBackRight; 
    public WheelCollider ColliderFrontLeft, ColliderBackLeft;
    public Transform WheelFrontRight, WheelBackRight;
    public Transform WheelFrontLeft, WheelBackLeft;

    public float brakeForce = 3000f;
    private bool isBraking;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;

    private Rigidbody rb;

    public float centerOfMassY = -0.5f;
    public float antiRollForce = 5000f;

    private void Start()
    {
         if (gasPedal != null)
            gasPedalStartPos = gasPedal.localPosition;

        if (brakePedal != null)
            brakePedalStartPos = brakePedal.localPosition;

        rb = GetComponent<Rigidbody>();

        Vector3 com = rb.centerOfMass;
        com.y = centerOfMassY;
        rb.centerOfMass = com;

        AdjustWheelFriction(ColliderFrontLeft, 2f);
        AdjustWheelFriction(ColliderFrontRight, 2f);
        AdjustWheelFriction(ColliderBackLeft, 2f);
        AdjustWheelFriction(ColliderBackRight, 2f);
    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        Brake();
        UpdateWheels();
        UpdatePedals();
        ApplyAntiRoll(ColliderFrontLeft, ColliderFrontRight);
        ApplyAntiRoll(ColliderBackLeft, ColliderBackRight);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void Steer()
    {
        currentSteerAngle = steeringAngle * horizontalInput;
        ColliderFrontLeft.steerAngle = currentSteerAngle;
        ColliderFrontRight.steerAngle = currentSteerAngle;
        if (steeringWheel != null)
        {
            float wheelZ = -currentSteerAngle / steeringAngle * wheelTurnAngle;
            steeringWheel.localRotation = Quaternion.Euler(0f, 0f, wheelZ);
        }
    }

    private void Accelerate()
    {
        ColliderFrontLeft.motorTorque = verticalInput * motorForce;
        ColliderFrontRight.motorTorque = verticalInput * motorForce;
    }

    private void Brake()
{
    float appliedBrakeForce = isBraking ? brakeForce : 0f;

    ColliderBackLeft.brakeTorque = appliedBrakeForce;
    ColliderBackRight.brakeTorque = appliedBrakeForce;

    ColliderFrontLeft.brakeTorque = 0f;
    ColliderFrontRight.brakeTorque = 0f;

    float driftGrip = isBraking ? 1.2f : 2f;

    AdjustWheelFriction(ColliderBackLeft, driftGrip);
    AdjustWheelFriction(ColliderBackRight, driftGrip);

    if (isBraking)
    {
        if (!rearLeftSmoke.isPlaying) rearLeftSmoke.Play();
        if (!rearRightSmoke.isPlaying) rearRightSmoke.Play();
    }
    else
    {
        rearLeftSmoke.Stop();
        rearRightSmoke.Stop();
    }
}


    private void UpdateWheels()
    {
        UpdateWheelPose(ColliderFrontLeft, WheelFrontLeft);
        UpdateWheelPose(ColliderFrontRight, WheelFrontRight);
        UpdateWheelPose(ColliderBackLeft, WheelBackLeft);
        UpdateWheelPose(ColliderBackRight, WheelBackRight);
    }

    private void UpdateWheelPose(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        trans.position = pos;
        trans.rotation = rot;
    }

    private void AdjustWheelFriction(WheelCollider wheel, float stiffness)
    {
        WheelFrictionCurve sideFriction = wheel.sidewaysFriction;
        sideFriction.stiffness = stiffness;
        wheel.sidewaysFriction = sideFriction;
    }

    private void ApplyAntiRoll(WheelCollider left, WheelCollider right)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = left.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-left.transform.InverseTransformPoint(hit.point).y - left.radius) / left.suspensionDistance;

        bool groundedR = right.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-right.transform.InverseTransformPoint(hit.point).y - right.radius) / right.suspensionDistance;

        float force = (travelL - travelR) * antiRollForce;

        if (groundedL)
            rb.AddForceAtPosition(left.transform.up * -force, left.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(right.transform.up * force, right.transform.position);
    }
    private void UpdatePedals()
    {
        float gasInput = Mathf.Clamp01(Input.GetAxis("Vertical"));
        bool isBraking = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Space);

        if (gasPedal != null)
        {
            Vector3 targetPos = gasPedalStartPos + Vector3.down * pedalPressDistance * gasInput;
            gasPedal.localPosition = Vector3.Lerp(gasPedal.localPosition, targetPos, Time.deltaTime * pedalMoveSpeed);
        }

        if (brakePedal != null)
        {
            float brakeAmount = isBraking ? 1f : 0f;
            Vector3 targetPos = brakePedalStartPos + Vector3.down * pedalPressDistance * brakeAmount;
            brakePedal.localPosition = Vector3.Lerp(brakePedal.localPosition, targetPos, Time.deltaTime * pedalMoveSpeed);
        }
    }
}

