using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCtl : MonoBehaviour
{
    //Control inputs are put here by the aggregator
    public Vector2 move, orbit;
    public bool sprint, resetCamera, lockOn, Atk1, Atk2, Atk3, jump;
    GameObject viewModel, attackOrigin;
    public bool faceWithCameraWhenLockingOn;
    bool oldLockOnKeyState;
    public GameObject animCtl;
    //Labels for player action states
    public enum States
    { NoLockOn = 0, LockedOn = 1, Airborne = 2, LightAttack = 3, HeavyAttack=4, SpecialAttack=5, Attacked=6, Skid=7}
    public States state = States.NoLockOn;
    public enum CameraModes { Follow, Strafe, Static, FocusPlayerAndBoss, StickBehindPlayer}
    public CameraModes cameraState, cameraStateNext;
    float camLerpTimer;
    // Weapon type labels to identify which animations and attack boxes to use. This will be set in the parameter script for each weapon.
    public enum WeaponTypes
    { PalmGun = 0, HandGun=1, Rifle=2, Sword=3, Knife=4, Bazooka=5}

    //momentum parameters for current movement
    float headingTarget;
    [SerializeField]float lastMagnitude;
    [SerializeField]Vector2 headingTargetRotated, currentHeading, multipliedMove;
    public float camDistFromPlayerSetting=2.5f;
    public float walkSpeed = 0.7f;
    public float runSpeed=2f;
    //GameObjects to be handled
    public GameObject lockedOnEnemy, lockOnCheckerPrefab;
    GameObject lockOnChecker, lerpCurrent, lerpTarget;
    Animator animator;
    public float shouldersWidth;
    Transform leftShoulder, rightShoulder;
    public GameObject defaultBullet;
    public Transform weaponHand;
    public WeaponClass weapon;
    
    public float weaponCoolDown;
    float dist; //distance calculation between 2 objects for camera use
    bool swordSwingDirection;
    GameObject handCannonSfx, handCannonSuperSfx;
    void Start()
    {
        lerpCurrent = Instantiate(Resources.Load("CamLerpPos").GameObject());
        lerpTarget = Instantiate(Resources.Load("CamLerpPos").GameObject());
        animator = animCtl.GetComponent<Animator>();
        leftShoulder = animCtl.transform.Find("skel/root/waist/spine/lshoulder/lhumerus");
        rightShoulder = animCtl.transform.Find("skel/root/waist/spine/rshoulder/rhumerus");
        weaponHand = rightShoulder.Find("rulna/rwrist");
        leftShoulder.Translate(Vector3.left * shouldersWidth);
        rightShoulder.Translate(Vector3.left * shouldersWidth);
        handCannonSfx = Resources.Load("sfxEmitters/handCannonSfx").GameObject();
        handCannonSuperSfx = Resources.Load("sfxEmitters/handCannonSuperSfx").GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraState == cameraStateNext) { PositionCamera(cameraState, lerpCurrent); }
        PositionCamera(cameraStateNext, lerpTarget);
        Camera.main.transform.position = Vector3.Lerp(lerpCurrent.transform.position, lerpTarget.transform.position, camLerpTimer);
        Camera.main.transform.rotation = Quaternion.Lerp(lerpCurrent.transform.rotation, lerpTarget.transform.rotation, camLerpTimer);
        if (cameraState != cameraStateNext)
        {
            camLerpTimer += Time.deltaTime * 2;
            if (camLerpTimer >= 1) { camLerpTimer = 0; cameraState = cameraStateNext; }
        }
        if (weapon != null) { animator.SetInteger("weaponType", (int)weapon.weaponType); }
        else { animator.SetInteger("weaponType", 0); }
        animator.SetInteger("mode", (int)state);
        switch (state)
        {
            case States.NoLockOn:
                PlayerMovement(6, 2, 1f * (sprint ? runSpeed : walkSpeed), true); //double speed if sprint is held
                if (lockOn && (lockOn != oldLockOnKeyState)) { checkLockOn(false); }
                oldLockOnKeyState = lockOn;
                if (lockedOnEnemy != null) { state = States.LockedOn; animator.SetTrigger("battleStance"); }
                AnimatorUpdateWalking();
                cameraStateNext = CameraModes.Follow;
                if (Atk1) { Debug.Log("Atk1 pressed"); state = States.LightAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); }  //fire light attack when button is pressed
                if (Atk2) { Debug.Log("Atk2 pressed"); state = States.HeavyAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); }
                break;
            case States.LockedOn:
                PlayerMovement(6, 2, 0.8f*(sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                if (lockedOnEnemy != null) { transform.LookAt(lockedOnEnemy.transform); } //if locked on, face enemy
                else { state = States.NoLockOn; animator.SetTrigger("noBattleStance"); }  //otherwise, un-lockon
                if (lockOn && (lockOn != oldLockOnKeyState)) { checkLockOn(false); }
                oldLockOnKeyState = lockOn;
                AnimatorUpdateWalking();
                cameraStateNext = CameraModes.StickBehindPlayer;
                if (Atk1) { Debug.Log("Atk1 pressed"); state = States.LightAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); }  //fire light attack when button is pressed
                if (Atk2) { Debug.Log("Atk2 pressed"); state = States.HeavyAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); }
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                break;
            case States.LightAttack:
                if (weapon != null) { AttackCreate(weapon.bullet, weapon.hitScan, weapon.attackPower, weapon.weaponCoolDownDuration, weapon.firesfx); }
                else { 
                    AttackCreate(
                        defaultBullet.GameObject(),
                        true,
                        10,
                        0.3f,
                        handCannonSfx);
                }
                PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                break;
            case States.HeavyAttack:
                if (weapon != null) { AttackCreate(weapon.superBullet, weapon.superHitScan, weapon.attackPower * 2, weapon.weaponCoolDownDuration * 1.3f, weapon.superFireSfx); }
                else { AttackCreate(defaultBullet, true, 20, 1, handCannonSuperSfx); }
                PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                break;
            default:
                throw new NotImplementedException();
                break;
            }
    }



    void AttackCreate(GameObject bullet, bool hitscan, float power, float cooldownMax, GameObject firesfx)
    {
        Debug.Log("Attacking");
        if (lockedOnEnemy != null){ transform.LookAt(lockedOnEnemy.transform); }
        
        Projectile newBullet;
        //PlayerMovement(6, 2, sprint ? 1.5f : 0.3f, false);
        States returnState = (lockedOnEnemy != null) ? States.LockedOn : States.NoLockOn;
        float firetime = state == States.HeavyAttack ? cooldownMax - (Time.deltaTime) : 0;
        if (weaponCoolDown < Time.deltaTime)
        {
            if (state == States.HeavyAttack) { Instantiate(Resources.Load("heavyCharge").GameObject(), transform.position, transform.rotation).GetComponent<ChargeEffect>().duration = firetime - Time.deltaTime; }
            else 
            {
                newBullet = Instantiate(bullet, weaponHand.position, transform.rotation).GetComponent<Projectile>();
                newBullet.attackPower = power;
                newBullet.hitscan = hitscan;
                animator.SetBool("swordSwingRandom", swordSwingDirection);
                Instantiate(firesfx, transform.position, Quaternion.identity);
                animator.SetTrigger("attack");
                swordSwingDirection = !swordSwingDirection;
            }
        }
        if(weaponCoolDown > firetime)
        {
            Debug.Log("Done waiting for timer");
            if(state == States.HeavyAttack) 
            {
                newBullet = Instantiate(bullet, weaponHand.position, transform.rotation).GetComponent<Projectile>();
                newBullet.attackPower = power;
                newBullet.hitscan = hitscan;
                animator.SetBool("swordSwingRandom", swordSwingDirection);
                Instantiate(firesfx, transform.position, Quaternion.identity);
                animator.SetTrigger("attack");  //animator.SetTrigger("attackHeavy");
                swordSwingDirection = !swordSwingDirection;
            }
            Debug.Log("Returning to walking");
            state = States.LockedOn; //weaponCoolDown = -2*Time.deltaTime;
        }


        weaponCoolDown += Time.deltaTime;
    }
    private void AnimatorUpdateWalking()
    {
        animator.SetFloat("speed", currentHeading.magnitude);
        animator.SetFloat("animSpeed", Mathf.Clamp(currentHeading.magnitude * 2, 0.3f, 5f));
    }

    private void PositionCamera(CameraModes mode, GameObject obj)
    {
        switch (mode)
        {
            case CameraModes.Follow:
                obj.transform.Translate(Vector3.right * orbit.x * Time.deltaTime * 3);//push it left or right before the LookAt in order to have it rotate
                obj.transform.LookAt(transform);
                dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist > 2.5f)
                { obj.transform.Translate(Vector3.forward * (dist - camDistFromPlayerSetting)); }
                else
                { obj.transform.Translate(Vector3.back * (camDistFromPlayerSetting - dist)); } //maintain a fixed distance from the player
                obj.transform.position = new Vector3(obj.transform.position.x, transform.position.y + (camDistFromPlayerSetting / 5), obj.transform.position.z);

                break;
            case CameraModes.Strafe:
                obj.transform.position = transform.position + (obj.transform.forward * (0 - camDistFromPlayerSetting)) + (obj.transform.up * (camDistFromPlayerSetting / 3));
                break;
            case CameraModes.StickBehindPlayer:
                obj.transform.rotation = transform.rotation;
                obj.transform.position = transform.position + (obj.transform.forward * (0-camDistFromPlayerSetting)) + (obj.transform.up * (camDistFromPlayerSetting / 3));
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

    private void checkLockOn(bool automaticLockOn)
    {
        lockOnChecker = Instantiate(lockOnCheckerPrefab, transform.position, transform.rotation);
        lockOnChecker.transform.localScale = Vector3.one * 640;
        lockOnChecker.GetComponent<LockOnChecker>().playerCtl = this;
        lockOnChecker.GetComponent<LockOnChecker>().autoLockOn = automaticLockOn;
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
