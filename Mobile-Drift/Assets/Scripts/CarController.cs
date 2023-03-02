using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour {

    // Settings
    public float MoveSpeed = 50;
    public float MaxSpeed = 15;
    public float Drag = 0.98f;
    public float SteerAngle = 20;
    public float Traction = 1;
    public float ControllsGravity = 0.1f;
    public float ControllsSensitivity = 0.3f;
    public float DriftRadius = 15;
    public float StartDriftDistance = 30;
    public float DriftAngle = 10;
    public LayerMask WaypointTrigger;

    // Variables
    private Vector3 MoveForce;
    private Waypoint wp;
    private float Vertical;
    private float Horizontal;
    private GameObject ActiveWaypoint = null;
    private bool ClosestPass;
    public bool Turn = true; //true - right, false - left
    [SerializeField] WaypointManager waypointManager;
    [SerializeField] Transform Looker;
    enum State { MoveToWaypoint, StartDrift, Drift };
    State state = State.MoveToWaypoint;
    private void OnEnable()
    {
        TouchManager.OnClicked += FindActive;
    }
    private void OnDisable()
    {
        TouchManager.OnClicked -= FindActive;
    }
    void Update() {
        switch(state){

            case State.MoveToWaypoint:
                if(ActiveWaypoint == null) {
                    FindActive();
                    break;
                }

                if(Vector3.Distance(ActiveWaypoint.transform.position, transform.position) < StartDriftDistance) {
                    state = State.StartDrift;
                }

                Looker.LookAt(ActiveWaypoint.transform.position);
                if(Mathf.Abs(transform.eulerAngles.y - Looker.eulerAngles.y) > 180)
                    ClosestPass=false;
                else
                    ClosestPass=true;
                if(transform.eulerAngles.y - Looker.eulerAngles.y > 0) {
                    if(Horizontal < 0 && !ClosestPass) Horizontal = 0;
                    if(Horizontal > 0 && ClosestPass) Horizontal = 0;
                    Horizontal += (ClosestPass ? -ControllsSensitivity : ControllsSensitivity) * Time.deltaTime;
                }else {
                    if(Horizontal < 0 && ClosestPass) Horizontal = 0;
                    if(Horizontal > 0 && !ClosestPass) Horizontal = 0;
                    Horizontal += (ClosestPass ? ControllsSensitivity : -ControllsSensitivity) * Time.deltaTime;
                }
                Vertical = 1;
            break;

            case State.StartDrift:
                if(ActiveWaypoint == null) {
                    FindActive();
                    break;
                }
                //Debug.Log(Vector3.Distance(ActiveWaypoint.transform.position, transform.position));
                Looker.LookAt(ActiveWaypoint.transform.position);
                if(Mathf.Abs(transform.eulerAngles.y - Looker.eulerAngles.y + (Turn ? 45 : 45)) > 180)
                    ClosestPass=false;
                else
                    ClosestPass=true;
                if(transform.eulerAngles.y - Looker.eulerAngles.y + (Turn ? 45 : -45) > 0) {
                    if(Horizontal < 0 && !ClosestPass) Horizontal = 0;
                    if(Horizontal > 0 && ClosestPass) Horizontal = 0;
                    Horizontal += (ClosestPass ? -ControllsSensitivity : ControllsSensitivity) * Time.deltaTime;
                }else {
                    if(Horizontal < 0 && ClosestPass) Horizontal = 0;
                    if(Horizontal > 0 && !ClosestPass) Horizontal = 0;
                    Horizontal += (ClosestPass ? ControllsSensitivity : -ControllsSensitivity) * Time.deltaTime;
                }
                Vertical = 0.6f;
                if(Vector3.Distance(ActiveWaypoint.transform.position, transform.position) < DriftRadius)
                    state = State.Drift;
            break;

            case State.Drift:
                if(ActiveWaypoint == null) {
                    FindActive();
                    break;
                }
                Looker.LookAt(ActiveWaypoint.transform.position);
                if(Mathf.Abs(transform.eulerAngles.y - Looker.eulerAngles.y + (Turn ? DriftAngle : - DriftAngle)) > 180)
                    ClosestPass=false;
                else
                    ClosestPass=true;
                if(transform.eulerAngles.y - Looker.eulerAngles.y + (Turn ? DriftAngle : - DriftAngle) > 0) {
                    if(Horizontal < 0 && !ClosestPass) Horizontal = 0;
                    if(Horizontal > 0 && ClosestPass) Horizontal = 0;
                    Horizontal += (ClosestPass ? -ControllsSensitivity : ControllsSensitivity) * Time.deltaTime;
                }else {
                    if(Horizontal < 0 && ClosestPass) Horizontal = 0;
                    if(Horizontal > 0 && !ClosestPass) Horizontal = 0;
                    Horizontal += (ClosestPass ? ControllsSensitivity : -ControllsSensitivity) * Time.deltaTime;
                }
                Vertical = 1;
            break;
        }
        Mathf.Clamp(Vertical, -1, 1);
        Mathf.Clamp(Horizontal, -1, 1);
        // Moving
        MoveForce += transform.forward * MoveSpeed * Vertical * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        // Steering
        transform.Rotate(Vector3.up * Horizontal * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        Debug.DrawRay(Looker.position, Looker.forward * 3, Color.red);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;

        if(Mathf.Abs(Vertical) < ControllsGravity * Time.deltaTime) {
            Vertical = 0;
        }else if(Vertical > 0) {
            Vertical -= ControllsGravity * Time.deltaTime;
        }else if(Vertical < 0) {
            Vertical += ControllsGravity * Time.deltaTime;
        }

        if(Mathf.Abs(Horizontal) < ControllsGravity * Time.deltaTime) {
            Horizontal = 0;
        }else if(Horizontal > 0) {
            Horizontal -= ControllsGravity * Time.deltaTime;
        }else if(Horizontal < 0) {
            Horizontal += ControllsGravity * Time.deltaTime;
        }

    }
    private void FindActive(){
        if(ActiveWaypoint == null) {
            ActiveWaypoint = waypointManager.FindClosestTo(transform);
        }else {
            ActiveWaypoint = waypointManager.FindLastPaced();
        }
        state = State.MoveToWaypoint;
        Turn = !Turn;
    }
    private void OnTriggerEnter(Collider other){
        if(WaypointTrigger == (WaypointTrigger | (1 << other.gameObject.layer))) {
            wp = other.GetComponentInParent<Waypoint>();
            wp.triggered = true;
            wp.WhoTriggered = this.transform;
        }
        /*switch(state) {
            case State.MoveToWaypoint:
                if((WaypointTrigger == (WaypointTrigger | (1 << other.gameObject.layer))) && (GameObject.ReferenceEquals(other.transform.parent.gameObject, ActiveWaypoint))){
                    //waypointManager.Waypoints.Remove(ActiveWaypoint);
                    //Destroy(ActiveWaypoint);
                    //FindActive();
                    state = State.StartDrift;
                } 
            break;

            case State.Drift:
            break;
        }
        */
    }
}
