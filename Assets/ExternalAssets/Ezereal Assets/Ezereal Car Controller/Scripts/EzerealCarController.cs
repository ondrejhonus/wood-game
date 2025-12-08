using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace Ezereal
{
    public class EzerealCarController : MonoBehaviour
    {
        [Header("Ezereal References")]
        [SerializeField] EzerealLightController ezerealLightController;
        [SerializeField] EzerealSoundController ezerealSoundController;
        [SerializeField] EzerealWheelFrictionController ezerealWheelFrictionController;

        [Header("References")]
        public Rigidbody vehicleRB;
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;
        WheelCollider[] wheels;

        [SerializeField] Transform frontLeftWheelMesh;
        [SerializeField] Transform frontRightWheelMesh;
        [SerializeField] Transform rearLeftWheelMesh;
        [SerializeField] Transform rearRightWheelMesh;

        [SerializeField] Transform steeringWheel;

        [SerializeField] TMP_Text currentGearTMP_UI;
        [SerializeField] TMP_Text currentGearTMP_Dashboard;

        [SerializeField] TMP_Text currentSpeedTMP_UI;
        [SerializeField] TMP_Text currentSpeedTMP_Dashboard;
        [SerializeField] Slider accelerationSlider;

        [Header("Settings")]
        public bool isStarted = true;

        public float maxForwardSpeed = 100f; 
        public float maxReverseSpeed = 30f; 
        public float horsePower = 1000f; 
        public float brakePower = 2000f; 
        public float handbrakeForce = 3000f; 
        public float maxSteerAngle = 30f; 
        public float steeringSpeed = 5f; 
        public float stopThreshold = 1f; 
        public float decelerationSpeed = 0.5f; 
        public float maxSteeringWheelRotation = 360f; 

        [Header("Drive Type")]
        public DriveTypes driveType = DriveTypes.RWD;

        [Header("Gearbox")]
        // We keep this variable for the UI, but we control it automatically now
        public AutomaticGears currentGear = AutomaticGears.Drive;

        [Header("Debug Info")]
        public bool stationary = true;
        [SerializeField] float currentSpeed = 0f;
        [SerializeField] float currentAccelerationValue = 0f;
        [SerializeField] float currentBrakeValue = 0f; // This is now REVERSE input
        [SerializeField] float currentHandbrakeValue = 0f;
        [SerializeField] float currentSteerAngle = 0f;
        [SerializeField] float targetSteerAngle = 0f;
        [SerializeField] float FrontLeftWheelRPM = 0f;
        [SerializeField] float FrontRightWheelRPM = 0f;
        [SerializeField] float RearLeftWheelRPM = 0f;
        [SerializeField] float RearRightWheelRPM = 0f;

        [SerializeField] float speedFactor = 0f; 

        private void Awake()
        {
            wheels = new WheelCollider[]
            {
                frontLeftWheelCollider,
                frontRightWheelCollider,
                rearLeftWheelCollider,
                rearRightWheelCollider,
            };

            // Safety Checks
            if (vehicleRB == null) Debug.LogError("VehicleRB reference is missing!");

            // Initialization
            if (isStarted)
            {
                if (ezerealLightController != null) ezerealLightController.MiscLightsOn();
                if (ezerealSoundController != null) ezerealSoundController.TurnOnEngineSound();
            }
            
            // Force Gear Text to D on start
            UpdateGearText("D");
        }

        // --- INPUTS ---

        void OnStartCar()
        {
            isStarted = !isStarted;

            if (isStarted)
            {
                if (ezerealLightController != null) ezerealLightController.MiscLightsOn();
                if (ezerealSoundController != null) ezerealSoundController.TurnOnEngineSound();
            }
            else
            {
                if (ezerealLightController != null) ezerealLightController.AllLightsOff();
                if (ezerealSoundController != null) ezerealSoundController.TurnOffEngineSound();
                
                // Cut power
                ResetWheelTorque();
            }
        }

        void OnAccelerate(InputValue accelerationValue)
        {
            // Usually W
            currentAccelerationValue = accelerationValue.Get<float>();
        }

        void OnBrake(InputValue brakeValue)
        {
            // Usually S - WE NOW USE THIS AS REVERSE THROTTLE
            currentBrakeValue = brakeValue.Get<float>();
        }

        void OnHandbrake(InputValue handbrakeValue)
        {
            // Spacebar - THIS IS NOW THE ONLY WAY TO "BRAKE" without Reversing
            currentHandbrakeValue = handbrakeValue.Get<float>();

            if (isStarted)
            {
                if (currentHandbrakeValue > 0)
                {
                    if (ezerealWheelFrictionController != null) ezerealWheelFrictionController.StartDrifting(currentHandbrakeValue);
                    if (ezerealLightController != null) ezerealLightController.HandbrakeLightOn();
                }
                else
                {
                    if (ezerealWheelFrictionController != null) ezerealWheelFrictionController.StopDrifting();
                    if (ezerealLightController != null) ezerealLightController.HandbrakeLightOff();
                }
            }
        }

        void OnSteer(InputValue turnValue)
        {
            targetSteerAngle = turnValue.Get<float>() * maxSteerAngle;
        }

        // --- PHYSICS LOGIC ---

        void AccelerationAndReverseLogic()
        {
            if (!isStarted) return;

            // 1. CHECK IF REVERSING (Holding S)
            if (currentBrakeValue > 0.1f) 
            {
                // --- REVERSE MODE ---
                if(currentGear != AutomaticGears.Reverse)
                {
                    currentGear = AutomaticGears.Reverse;
                    UpdateGearText("R");
                    if (ezerealLightController != null) ezerealLightController.ReverseLightsOn();
                }

                // Apply Reverse Torque (Negative Horsepower)
                float reverseForce = -currentBrakeValue * horsePower;

                // Speed Limit Check for Reverse
                if (currentSpeed > -maxReverseSpeed)
                {
                    ApplyTorque(reverseForce);
                }
                else
                {
                    ResetWheelTorque();
                }

                // Update Slider UI
                accelerationSlider.value = currentBrakeValue;
            }
            // 2. ELSE DRIVE FORWARD (Holding W or Idling)
            else 
            {
                // --- DRIVE MODE ---
                if (currentGear != AutomaticGears.Drive)
                {
                    currentGear = AutomaticGears.Drive;
                    UpdateGearText("D");
                    if (ezerealLightController != null) ezerealLightController.ReverseLightsOff();
                }

                // Calculate Top Speed Factor
                speedFactor = Mathf.InverseLerp(0, maxForwardSpeed, currentSpeed);
                float availableTorque = Mathf.Lerp(horsePower, 0, speedFactor);
                
                // Apply Forward Torque
                if (currentAccelerationValue > 0f && currentSpeed < maxForwardSpeed)
                {
                    ApplyTorque(availableTorque * currentAccelerationValue);
                }
                else
                {
                    ResetWheelTorque();
                }

                // Update Slider UI
                accelerationSlider.value = Mathf.Lerp(accelerationSlider.value, currentAccelerationValue, Time.deltaTime * 15f);
            }
        }

        // Helper function to apply torque based on Drive Type
        void ApplyTorque(float torque)
        {
            if (driveType == DriveTypes.RWD)
            {
                rearLeftWheelCollider.motorTorque = torque;
                rearRightWheelCollider.motorTorque = torque;
                frontLeftWheelCollider.motorTorque = 0;
                frontRightWheelCollider.motorTorque = 0;
            }
            else if (driveType == DriveTypes.FWD)
            {
                frontLeftWheelCollider.motorTorque = torque;
                frontRightWheelCollider.motorTorque = torque;
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }
            else // AWD
            {
                frontLeftWheelCollider.motorTorque = torque;
                frontRightWheelCollider.motorTorque = torque;
                rearLeftWheelCollider.motorTorque = torque;
                rearRightWheelCollider.motorTorque = torque;
            }
        }

        void ResetWheelTorque()
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            rearLeftWheelCollider.motorTorque = 0;
            rearRightWheelCollider.motorTorque = 0;
        }

        void Handbraking()
        {
            // Only the Handbrake (Space) applies actual BrakeTorque now.
            // S key applies negative MotorTorque (Reverse), which acts as a brake anyway if moving forward.
            
            if (currentHandbrakeValue > 0f)
            {
                rearLeftWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
                rearRightWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
                
                // Cut motor power while handbraking
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }
            else
            {
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void Steering()
        {
            float adjustedspeedFactor = Mathf.InverseLerp(20, maxForwardSpeed, currentSpeed); 
            float adjustedTurnAngle = targetSteerAngle * (1 - adjustedspeedFactor); 
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, adjustedTurnAngle, Time.deltaTime * steeringSpeed);

            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            UpdateWheel(frontLeftWheelCollider, frontLeftWheelMesh);
            UpdateWheel(frontRightWheelCollider, frontRightWheelMesh);
            UpdateWheel(rearLeftWheelCollider, rearLeftWheelMesh);
            UpdateWheel(rearRightWheelCollider, rearRightWheelMesh);
        }

        void Slowdown()
        {
            if (vehicleRB != null)
            {
                // Drag effect when no inputs are pressed
                if (currentAccelerationValue == 0 && currentBrakeValue == 0 && currentHandbrakeValue == 0)
                {
#if UNITY_6000_0_OR_NEWER
                    vehicleRB.linearVelocity = Vector3.Lerp(vehicleRB.linearVelocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#else
                    vehicleRB.velocity = Vector3.Lerp(vehicleRB.velocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#endif
                }
            }
        }

        private void FixedUpdate()
        {
            AccelerationAndReverseLogic(); // Combined Logic
            Handbraking();
            Steering();
            Slowdown();
            RotateSteeringWheel();
            CheckStationary();
            CalculateSpeed();
            UpdateDebugRPMs();
        }

        // --- HELPERS ---

        void CheckStationary()
        {
            if (Mathf.Abs(frontLeftWheelCollider.rpm) < stopThreshold &&
                Mathf.Abs(frontRightWheelCollider.rpm) < stopThreshold)
            {
                stationary = true;
            }
            else
            {
                stationary = false;
            }
        }

        void CalculateSpeed()
        {
            if (vehicleRB != null) 
            {
#if UNITY_6000_0_OR_NEWER
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.linearVelocity) * 3.6f;
#else
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.velocity) * 3.6f;
#endif
                UpdateSpeedText(currentSpeed);
            }
        }

        void UpdateDebugRPMs()
        {
            FrontLeftWheelRPM = frontLeftWheelCollider.rpm;
            FrontRightWheelRPM = frontRightWheelCollider.rpm;
            RearLeftWheelRPM = rearLeftWheelCollider.rpm;
            RearRightWheelRPM = rearRightWheelCollider.rpm;
        }

        private void UpdateWheel(WheelCollider col, Transform mesh)
        {
            col.GetWorldPose(out Vector3 position, out Quaternion rotation);
            mesh.SetPositionAndRotation(position, rotation);
        }

        void RotateSteeringWheel()
        {
            float currentXAngle = steeringWheel.transform.localEulerAngles.x; 
            float normalizedSteerAngle = Mathf.Clamp(frontLeftWheelCollider.steerAngle, -maxSteerAngle, maxSteerAngle);
            float rotation = Mathf.Lerp(maxSteeringWheelRotation, -maxSteeringWheelRotation, (normalizedSteerAngle + maxSteerAngle) / (2 * maxSteerAngle));
            steeringWheel.localRotation = Quaternion.Euler(currentXAngle, 0, rotation);
        }

        void UpdateGearText(string gear)
        {
            if(currentGearTMP_UI != null) currentGearTMP_UI.text = gear;
            if(currentGearTMP_Dashboard != null) currentGearTMP_Dashboard.text = gear;
        }

        void UpdateSpeedText(float speed)
        {
            speed = Mathf.Abs(speed);
            if(currentSpeedTMP_UI != null) currentSpeedTMP_UI.text = speed.ToString("F0");
            if(currentSpeedTMP_Dashboard != null) currentSpeedTMP_Dashboard.text = speed.ToString("F0");
        }

        public bool InAir()
        {
            foreach (WheelCollider wheel in wheels)
            {
                if (wheel.GetGroundHit(out _)) return false;
            }
            return true;
        }
    }
}