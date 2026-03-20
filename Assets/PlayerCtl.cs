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
    { NoLockOn = 0, LockedOn = 1, Airborne = 2, LightAttack = 3, HeavyAttack=4, SpecialAttack=5, Attacked=6, Skid=7, Knockback=8, KnockbackGetUp=9, Dead=10}
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
    RaycastHit rayout;
    public float weaponCoolDown; public bool shotFired;
    float dist; //distance calculation between 2 objects for camera use
    bool swordSwingDirection;
    GameObject handCannonSfx, handCannonSuperSfx;
    float airSpd;
    float knockBackTimer;
    AnimatorStateInfo upperBody; 
    AnimatorStateInfo lowerBody;
    public int firingSequence = 0;
    //AnimatorStateInfo hair;
    PlayerHealth health;
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
        PlayerHealth.thisPlayer = this;
        upperBody = animator.GetNextAnimatorStateInfo(1);
        upperBody = animator.GetNextAnimatorStateInfo(0);
        
        try
        {
            health = GameObject.Find("CharacterStatus").GetComponent<PlayerHealth>();
            if (health.marco() != "polo") { Instantiate(Resources.Load("CharacterStatus").GameObject()); health = GameObject.Find("CharacterStatus").GetComponent<PlayerHealth>(); }
        }
        catch { Instantiate(Resources.Load("CharacterStatus").GameObject()); health = GameObject.Find("CharacterStatus").GetComponent<PlayerHealth>(); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (cameraState == cameraStateNext) { PositionCamera(cameraState, lerpCurrent); }
        PositionCamera(cameraStateNext, lerpTarget);
        Camera.main.transform.position = Vector3.Lerp(lerpCurrent.transform.position, lerpTarget.transform.position, camLerpTimer);
        Camera.main.transform.rotation = Quaternion.Lerp(lerpCurrent.transform.rotation, lerpTarget.transform.rotation, camLerpTimer);
        if (cameraState != cameraStateNext)
        {
            camLerpTimer += Time.fixedDeltaTime * 2;
            if (camLerpTimer >= 1) { camLerpTimer = 0; cameraState = cameraStateNext; }
        }
        if (weapon != null) { animator.SetInteger("weaponType", (int)weapon.weaponType); }
        else { animator.SetInteger("weaponType", 0); }
        animator.SetInteger("mode", (int)state);
        weaponCoolDown += Time.fixedDeltaTime;
        switch (state)
        {
            case States.NoLockOn:
                PlayerMovement(6, 2, 1f * (sprint ? runSpeed : walkSpeed), true); //double speed if sprint is held
                if (lockOn && (oldLockOnKeyState == false)) { checkLockOn(false); }
                oldLockOnKeyState = lockOn;
                if (lockedOnEnemy != null) { state = States.LockedOn; animator.SetTrigger("battleStance"); }
                AnimatorUpdateWalking();
                cameraStateNext = CameraModes.Follow;
                if (Atk1 && weaponCoolDown>=0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk1 pressed"); state = States.LightAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }  //fire light attack when button is pressed
                if (Atk2 && weaponCoolDown >= 0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk2 pressed"); state = States.HeavyAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }
                break;
            case States.LockedOn:
                PlayerMovement(6, 2, 0.8f*(sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                if (lockedOnEnemy != null) { transform.LookAt(lockedOnEnemy.transform); } //if locked on, face enemy
                else { state = States.NoLockOn; animator.SetTrigger("noBattleStance"); }  //otherwise, un-lockon
                if (lockOn && (oldLockOnKeyState == false)) { checkLockOn(false); }
                oldLockOnKeyState = lockOn;
                AnimatorUpdateWalking();
                cameraStateNext = CameraModes.StickBehindPlayer;
                if (Atk1 && weaponCoolDown >= 0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk1 pressed"); state = States.LightAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }  //fire light attack when button is pressed
                if (Atk2 && weaponCoolDown >= 0) { weaponCoolDown = 0-Time.fixedDeltaTime; Debug.Log("Atk2 pressed"); state = States.HeavyAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                break;
            case States.Airborne:
                PlayerMovement(6, 2, 1f * (sprint ? runSpeed : walkSpeed), true); //double speed if sprint is held
                if (lockOn && (lockOn != oldLockOnKeyState)) { checkLockOn(false); }
                oldLockOnKeyState = lockOn;
                AnimatorUpdateWalking();
                animator.SetInteger("mode", 2);
                cameraStateNext = CameraModes.Follow;
                airSpd += 0.5f;
                transform.Translate(Vector3.up * (-1 * airSpd * Time.fixedDeltaTime));
                break;
            case States.LightAttack:
                if (weapon != null) { AttackCreate(weapon.bullet, weapon.hitScan, PlayerHealth.baseAtk + weapon.attackPower, weapon.weaponCoolDownDuration, weapon.firesfx, false); }
                else { 
                    AttackCreate(
                        defaultBullet.GameObject(),
                        false,
                        PlayerHealth.baseAtk,
                        0.4f,
                        handCannonSfx, 
                        false);
                }
                PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                break;
            case States.HeavyAttack:
                if (weapon != null) { AttackCreate(weapon.superBullet, weapon.superHitScan, weapon.attackPower * 2, weapon.weaponCoolDownDuration * 1.3f, weapon.superFireSfx, true); }
                else { AttackCreate(defaultBullet, true, 20, 1, handCannonSuperSfx, true); }
                PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                break;
            case States.Knockback:
                knockBackTimer -= Time.fixedDeltaTime;
                if(knockBackTimer < 0) { state = States.KnockbackGetUp; animator.SetTrigger("knockbackGetUp"); }
                break;
            case States.KnockbackGetUp:
                //(animatorStateInfo.IsName("knockBackGetUp") && animatorStateInfo.normalizedTime > 0.9f)
                if (lowerBody.IsName("knockback") && lowerBody.normalizedTime >= 0.99f)
                { state = States.NoLockOn; animator.SetTrigger("noBattleStance"); }
                break;
            default:
                throw new NotImplementedException();
                break;
            }
    }

    public void DamageFrom(Transform enemy, int damage, float KnockoutTime)
    { 
        if (PlayerHealth.health == 0) { state = States.Dead; }
        else if (state != States.Knockback && state != States.KnockbackGetUp) { state = States.Knockback; knockBackTimer = KnockoutTime; PlayerHealth.health -= damage; animator.SetTrigger("knockbackHeavy"); }
        
    }

    void AttackCreate(GameObject bullet, bool hitscan, float power, float cooldownMax, GameObject firesfx, bool heavy)
    {
        Debug.Log("Attacking");
        if (lockedOnEnemy != null){ transform.LookAt(lockedOnEnemy.transform); }
        
        
        //PlayerMovement(6, 2, sprint ? 1.5f : 0.3f, false);
        States returnState = (lockedOnEnemy != null) ? States.LockedOn : States.NoLockOn;
        //float firetime = state == States.HeavyAttack ? cooldownMax - (Time.fixedDeltaTime) : 0;
        float firetime = cooldownMax - (Time.fixedDeltaTime);
        if ((weaponCoolDown < Time.fixedDeltaTime) && !shotFired)
        {
            if (state == States.HeavyAttack) { Instantiate(Resources.Load("heavyCharge").GameObject(), transform.position, transform.rotation).GetComponent<ChargeEffect>().duration = firetime - Time.fixedDeltaTime; }
            else 
            {
                CreateBullet(bullet, hitscan, power, cooldownMax, firesfx);
            }
        }
        if(weaponCoolDown > firetime)
        {
            Debug.Log("Done waiting for timer");
            if(state == States.HeavyAttack) 
            {
                CreateBullet(bullet, hitscan, power, cooldownMax, firesfx);
            }
            shotFired = false;
            if (true)//(!Atk1 && !Atk2)
            {
                firingSequence++;
                Debug.Log("Returning to walking");
                weaponCoolDown = (firingSequence >= 3 ? cooldownMax/-1.2f : 0);
                if (state == States.HeavyAttack) { weaponCoolDown = 0 - cooldownMax; }
                if (firingSequence >= 3) { firingSequence = 0; }
                state = returnState; //weaponCoolDown = -2*Time.fixedDeltaTime;
                
                if (returnState == States.NoLockOn) { animator.SetTrigger("noBattleStance"); }
            }
            else { state = States.LightAttack; } //super terrible hack to prevent shooting 3600 rounds per minute automatic heavy attacks
        }


        //weaponCoolDown += Time.fixedDeltaTime;
    }

    private Projectile CreateBullet(GameObject bullet, bool hitscan, float power, float cooldownMax, GameObject firesfx)
    {
        Projectile newBullet;
        newBullet = Instantiate(bullet, weaponHand.position, transform.rotation).GetComponent<Projectile>();
        newBullet.attackPower = power;
        newBullet.hitscan = hitscan;
        animator.SetBool("swordSwingRandom", swordSwingDirection);
        Instantiate(firesfx, transform.position, Quaternion.identity);
        animator.SetTrigger("attack");  //animator.SetTrigger("attackHeavy");
        swordSwingDirection = !swordSwingDirection;
        shotFired = true;
        newBullet.gameObject.layer = 2;
        return newBullet;
    }
    private void AnimatorUpdateWalking()
    {
        animator.SetFloat("speed", currentHeading.magnitude);
        animator.SetFloat("animSpeed", Mathf.Clamp(currentHeading.magnitude * 2, 0.3f, 5f));
        if (move.x > 0.5) { animator.SetInteger("walkDirection", 1); }
        if (move.x < -0.5) { animator.SetInteger("walkDirection", 3); }
        if (move.y > 0.5) { animator.SetInteger("walkDirection", 0); }
        if (move.y < -0.5) { animator.SetInteger("walkDirection", 2); }
        if (state == States.NoLockOn) { animator.SetInteger("walkDirection", 0); }
    }
    private void PositionCamera(CameraModes mode, GameObject obj)
    {
        switch (mode)
        {
            case CameraModes.Follow:
                obj.transform.Translate(Vector3.right * orbit.x * Time.fixedDeltaTime * 3 * transform.localScale.x);//push it left or right before the LookAt in order to have it rotate
                obj.transform.LookAt(transform);
                dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist > (2.5f * transform.localScale.z))
                { obj.transform.Translate(Vector3.forward * (dist - (camDistFromPlayerSetting* transform.localScale.z)) ); }
                else
                { obj.transform.Translate(Vector3.back * ((camDistFromPlayerSetting* transform.localScale.z) - dist) ); } //maintain a fixed distance from the player
                obj.transform.position = new Vector3(obj.transform.position.x, transform.position.y + ((camDistFromPlayerSetting / 5) * transform.localScale.z), obj.transform.position.z);

                break;
            case CameraModes.Strafe:
                obj.transform.position = transform.position + (obj.transform.forward * (0 - camDistFromPlayerSetting) * transform.localScale.z) + (obj.transform.up * (camDistFromPlayerSetting / 3) * transform.localScale.z);
                break;
            case CameraModes.StickBehindPlayer:
                obj.transform.rotation = transform.rotation;
                obj.transform.position = transform.position + (obj.transform.forward * (0-camDistFromPlayerSetting)* transform.localScale.z) + (obj.transform.up * (camDistFromPlayerSetting / 3)*transform.localScale.z);
                break;
            case CameraModes.Static:
                break;
            case CameraModes.FocusPlayerAndBoss:
                obj.transform.rotation = transform.rotation;
                obj.transform.position = transform.position + (obj.transform.forward * (0 - camDistFromPlayerSetting)*transform.localScale.z) + (obj.transform.up * (camDistFromPlayerSetting / 1)*transform.localScale.z);
                if (lockedOnEnemy!=null) { obj.transform.LookAt(lockedOnEnemy.transform); } else { obj.transform.LookAt(transform); }
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
            currentHeading = Vector2.Lerp(currentHeading, headingTargetRotated, Time.fixedDeltaTime * accel);
            headingTarget = Camera.main.transform.localEulerAngles.y + (Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg);
            while (headingTarget < -180) { headingTarget += 360; } //ensure value is within range
            while (headingTarget > 180) { headingTarget -= 360; }
            if (faceMovementTarget) { transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, headingTarget, 0), (1 - (move.magnitude * 0.6f))); }
            headingTargetRotated = new Vector2(Mathf.Sin(headingTarget * Mathf.Deg2Rad) * multipliedMove.magnitude, Mathf.Cos(headingTarget * Mathf.Deg2Rad) * multipliedMove.magnitude);

        }
        else
        {
            currentHeading = Vector2.Lerp(currentHeading, Vector2.zero, Time.fixedDeltaTime * friction);
            if (currentHeading.magnitude < 0.1)
            { currentHeading = Vector2.zero; }
        }
        Vector3 newPos = transform.position;
        Vector3 mov = Vector3.zero;
        mov += Vector3.right * currentHeading.x * Time.fixedDeltaTime * 3 * transform.localScale.x;
        mov += Vector3.forward * currentHeading.y * Time.fixedDeltaTime * 3 * transform.localScale.z;
        if (Physics.Raycast(transform.position, mov.normalized, out rayout, transform.localScale.x * (mov.magnitude+0.2f)))
        {
            transform.position = rayout.point - (transform.localScale.x*(mov.normalized * 0.2f));
            currentHeading = Vector2.zero;
        }
        else { transform.position += mov; }
        if (Physics.Raycast(transform.position, transform.up * -1f, out rayout, transform.localScale.y * 0.6f))
        {
            //Debug.Log("Ground detected");
            transform.position = rayout.point + transform.up * (transform.localScale.y * 0.5f);
            if (state == States.Airborne) { state = States.NoLockOn; airSpd = 0; }
        }
        else { state = States.Airborne; animator.SetTrigger("airborne"); }
        PlayerCollide();
    }
    void PlayerCollide()
    { 
        for (int i = 0; i < 4; i++)
        {
            Vector3 raydir = transform.TransformVector(new Vector3(Mathf.Sin(i / 2 * Mathf.PI), 0, Mathf.Cos(i / 2 * Mathf.PI)));
            if (Physics.Raycast(transform.position, raydir, out rayout, transform.localScale.x * 0.2f))
            { transform.position = rayout.point - (raydir * 0.2f); }
        } 
    }
}
