using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShipController : MonoBehaviour
{
    private Rigidbody rb;
    public ShipControls controls;
    public GameObject Rudder;
    public Rigidbody PropellerRB;
    public Text uiThrustText;
    HingeJoint rudderHinge;
    BuoyancyScript buoyancyRef;

    public float rotationspeed = 0.1f;
    public float acceleration = 0.001f;

    private float lastRot = 0;

    ShipControls.MoveActions ShipMoveAction;

    public Vector3 lastVelocity = Vector3.zero;
    public float currentSpeed = 0;
    public float maxSpeed = 10000;


    Vector2 input;
    

    // Update is called once per frame
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new ShipControls();
        ShipMoveAction = controls.Move;
        buoyancyRef = GetComponent<BuoyancyScript>();
        acceleration *= buoyancyRef.shipType.mass;

        ShipMoveAction.WASD.performed += context => input = context.ReadValue<Vector2>();
        ShipMoveAction.WASD.canceled += context => input = Vector2.zero;

        ShipMoveAction.Quit.performed += context => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        rudderHinge = Rudder.GetComponent<HingeJoint>();
    }
    private void Update()
    {
        SetSpeed();
        Turn();
        if (input.x == 0)
        {
            NormalizeRudder();
        }
    }

    public void FixedUpdate()
    {
        Thrust();
    }
    private void SetSpeed()
    {
        currentSpeed += input.y;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
        input.y = 0;
        uiThrustText.text = "Thrust: " + currentSpeed;
    }

    private void Turn()
    {
        float targetRot = Mathf.Clamp(lastRot + (input.x * -1), rudderHinge.limits.min, rudderHinge.limits.max);
        lastRot = targetRot;
        
        JointSpring rudderChange = rudderHinge.spring;
        rudderChange.targetPosition = targetRot;
        
        rudderHinge.spring = rudderChange;

    }
    private void Thrust()
    {

        //Rudder.GetComponent<Rigidbody>().AddForce(Vector3.up * (input.y * currentSpeed));
        if (WaveHandler.instance.DistanceToWater(PropellerRB.transform.position, GameManager.secondsSinceStart) >= 0)
        {
            Vector3 tempVelocity = new Vector3(lastVelocity.x, rb.velocity.y, lastVelocity.z);

            rb.velocity = tempVelocity;

            return;
        }
        else
        {
            lastVelocity = rb.velocity;
        }
        Vector3 dir = Rudder.transform.TransformDirection(Vector3.up);
        PropellerRB.AddForce(dir * currentSpeed * acceleration);
        //Debug.DrawRay(Rudder.transform.position, Rudder.transform.up * acceleration * input.y, Color.red);
        //Debug.Log(input);
    }

    private void NormalizeRudder()
    {
        lastRot = Mathf.Lerp(lastRot, 0, 0.2f);
    }

    

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();   
    }
}
