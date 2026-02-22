using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCtl : MonoBehaviour
{
    //Control inputs are put here by the aggregator
    public Vector2 move, orbit;
    public bool sprint, resetCamera, lockOn, Atk1, Atk2, Atk3, jump;
    GameObject viewModel;
    GameObject attackOrigin;
    public bool faceWithCameraWhenLockingOn;

    //Labels for player action states
    public enum States
    { NoLockOn, LockedOn, Airborne, LightAttack, HeavyAttack, SpecialAttack, Attacked}
    public States state = States.NoLockOn;

    // Weapon type labels to identify which animations and attack boxes to use. This will be set in the parameter script for each weapon.
    public enum WeaponTypes
    { PalmGun, HandGun, Rifle, Sword, Knife, Bazooka}

    //momentum parameters for current movement
    float headingTarget;
    [SerializeField]float lastMagnitude;
    Vector2 headingTargetRotated;
    Vector2 currentHeading;
    Vector2 multipliedMove;


    //GameObjects to be handled
    public GameObject lockedOnEnemy;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.LookAt(transform);
        switch(state)
        { 
            case States.NoLockOn:
                PlayerMovement(6,2,sprint ? 2f : 0.6f, true); //double speed if sprint is held
                if (lockOn) { state= States.LockedOn; }
                break;
            case States.LockedOn:
                PlayerMovement(6, 2, sprint ? 1.5f : 0.3f, false); //double speed if sprint is held
                if (lockedOnEnemy != null)
                { transform.LookAt(lockedOnEnemy.transform); }
                else if (faceWithCameraWhenLockingOn)
                { transform.rotation = Camera.main.transform.rotation; }
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //do not tilt pitch or roll
                if (!lockOn && lockedOnEnemy == null) { state = States.NoLockOn; }
                break;
            default:
                throw new NotImplementedException();
        }
        
    }


    void PlayerMovement(float friction, float accel, float multiplier, bool faceMovementTarget) 
    {
        multipliedMove = move * multiplier;
        if (move.magnitude > 0.1 && currentHeading.magnitude <= multipliedMove.magnitude )
        {
            currentHeading = Vector2.Lerp(currentHeading, headingTargetRotated, Time.deltaTime * accel);
            headingTarget = Camera.main.transform.localEulerAngles.y + (Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg);
            while (headingTarget < -180) { headingTarget += 360; } //ensure value is within range
            while (headingTarget > 180) { headingTarget -= 360; }
            if (faceMovementTarget) { transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, headingTarget, 0), (1 - (move.magnitude * 0.6f))); }
            headingTargetRotated = new Vector2(Mathf.Sin(headingTarget * Mathf.Deg2Rad) * multipliedMove.magnitude, Mathf.Cos(headingTarget * Mathf.Deg2Rad) * multipliedMove.magnitude);

        }
        else
        {
            currentHeading = Vector2.Lerp(currentHeading, Vector2.zero, Time.deltaTime * friction);
            if (currentHeading.magnitude < 0.1)
            { currentHeading = Vector2.zero; }
        }
        transform.position += Vector3.right * currentHeading.x * Time.deltaTime * 3;
        transform.position += Vector3.forward * currentHeading.y * Time.deltaTime * 3;

    }
}
