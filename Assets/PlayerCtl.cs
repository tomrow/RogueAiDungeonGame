using System;
using Unity.VisualScripting;
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
    bool oldLockOnKeyState;
    public GameObject animCtl;
    //Labels for player action states
    public enum States
    { NoLockOn, LockedOn, Airborne, LightAttack, HeavyAttack, SpecialAttack, Attacked}
    public States state = States.NoLockOn;
    public enum CameraModes { Follow, Strafe, Static, FocusPlayerAndBoss, StickBehindPlayer}
    public CameraModes cameraState, cameraStateNext;
    float camLerpTimer;
    // Weapon type labels to identify which animations and attack boxes to use. This will be set in the parameter script for each weapon.
    public enum WeaponTypes
    { PalmGun, HandGun, Rifle, Sword, Knife, Bazooka}

    //momentum parameters for current movement
    float headingTarget;
    [SerializeField]float lastMagnitude;
    Vector2 headingTargetRotated;
    [SerializeField]Vector2 currentHeading;
    Vector2 multipliedMove;
    RaycastHit cameraRayOut;
    public float camDistFromPlayerSetting=2.5f;
    //GameObjects to be handled
    public GameObject lockedOnEnemy;
    public GameObject lockOnCheckerPrefab;
    GameObject lockOnChecker;
    Vector3 directionFromPlayerToTarget;
    GameObject lerpCurrent, lerpTarget;
    Animator animator;

    float dist; //distance calculation between 2 objects for camera use
    void Start()
    {
        lerpCurrent = Instantiate(Resources.Load("CamLerpPos").GameObject());
        lerpTarget = Instantiate(Resources.Load("CamLerpPos").GameObject());
        animator = animCtl.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PositionCamera(cameraState, lerpCurrent);
        PositionCamera(cameraStateNext, lerpTarget);
        Camera.main.transform.position = Vector3.Lerp(lerpCurrent.transform.position, lerpTarget.transform.position, camLerpTimer);
        Camera.main.transform.rotation = Quaternion.Lerp(lerpCurrent.transform.rotation, lerpTarget.transform.rotation, camLerpTimer);
        if (cameraState != cameraStateNext)
        {
            camLerpTimer += Time.deltaTime * 2;
            if (camLerpTimer >= 1) { camLerpTimer = 0; cameraState = cameraStateNext; }
        }
        switch (state)
        { 
            case States.NoLockOn:
                
                PlayerMovement(6,2,sprint ? 2f : 0.6f, true); //double speed if sprint is held
                if (lockOn && (lockOn != oldLockOnKeyState)) { checkLockOn(); }
                oldLockOnKeyState = lockOn;
                if (lockedOnEnemy != null)
                { 
                    state = States.LockedOn;
                    animator.SetTrigger("battleStance");
                }
                animator.SetInteger("mode", 0);  //set animation to walking/idle at current speed
                animator.SetFloat("speed", currentHeading.magnitude);
                animator.SetFloat("animSpeed", Mathf.Clamp(currentHeading.magnitude*2, 0.3f, 5f));
                //Debug.Log(currentHeading.magnitude);
                break;
            case States.LockedOn:
                PlayerMovement(6, 2, sprint ? 1.5f : 0.3f, false); //double speed if sprint is held
                if (lockedOnEnemy != null)
                { transform.LookAt(lockedOnEnemy.transform); }
                else
                { 
                    state = States.NoLockOn;
                    animator.SetTrigger("noBattleStance");
                }
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //do not tilt pitch or roll
                if (lockOn && (lockOn != oldLockOnKeyState)) { checkLockOn(); }
                oldLockOnKeyState = lockOn;
                animator.SetInteger("mode", 0);  //set animation to walking/idle at current speed
                animator.SetFloat("speed", currentHeading.magnitude);
                animator.SetFloat("animSpeed", Mathf.Clamp(currentHeading.magnitude * 2, 0.3f, 5f));
                break;
            default:
                throw new NotImplementedException();
        }
        
    }

    private void PositionCamera(CameraModes mode, GameObject obj)
    {
        switch (mode)
        {
            case CameraModes.Follow:
                obj.transform.Translate(Vector3.right * orbit.x * Time.deltaTime);//push it left or right before the LookAt in order to have it rotate
                obj.transform.LookAt(transform);
                dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist > 2.5f)
                { obj.transform.Translate(Vector3.forward * (dist - camDistFromPlayerSetting)); }
                else
                { obj.transform.Translate(Vector3.back * (camDistFromPlayerSetting - dist)); } //maintain a fixed distance from the player
                obj.transform.position = new Vector3(obj.transform.position.x, transform.position.y + (camDistFromPlayerSetting / 5), obj.transform.position.z);

                break;
            case CameraModes.Strafe:
                obj.transform.position = transform.position + (obj.transform.forward * (0 - camDistFromPlayerSetting)) + (obj.transform.up * (camDistFromPlayerSetting / 5));
                break;
            case CameraModes.StickBehindPlayer:
                obj.transform.rotation = transform.rotation;
                obj.transform.position = transform.position + (obj.transform.forward * (0-camDistFromPlayerSetting)) + (obj.transform.up * (camDistFromPlayerSetting / 5));
                break;
            case CameraModes.Static:
                break;
            case CameraModes.FocusPlayerAndBoss:
                obj.transform.position = lockedOnEnemy.transform.position;
                obj.transform.Translate(Vector3.up * camDistFromPlayerSetting / 2);
                obj.transform.LookAt(transform.position);
                break;
        }
    }

    private void checkLockOn()
    {
        lockOnChecker = Instantiate(lockOnCheckerPrefab, transform.position, transform.rotation);
        lockOnChecker.transform.localScale = Vector3.one * 640;
        lockOnChecker.GetComponent<LockOnChecker>().playerCtl = this;
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
