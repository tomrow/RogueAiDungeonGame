using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCtl : MonoBehaviour
{
    //Control inputs are put here by the aggregator
    public Vector2 move, orbit;
    public bool sprint, resetCamera, lockOn, Atk1, Atk2, Atk3, jump, eHeal;
    GameObject viewModel, attackOrigin;
    public bool faceWithCameraWhenLockingOn;
    bool oldLockOnKeyState, oldEHealKeyState;
    public GameObject animCtl;
    //Labels for player action states
    public enum States
    { NoLockOn = 0, LockedOn = 1, Airborne = 2, LightAttack = 3, HeavyAttack=4, SpecialAttack=5, Attacked=6, Skid=7, Knockback=8, KnockbackGetUp=9, Dead=10, Leap=11, WalkTowardGoal=12, StandOnGoal=13, PauseLookAtTarget=14 }
    public States state = States.NoLockOn;
    public enum CameraModes { Follow, Strafe, Static, FocusPlayerAndBoss, StickBehindPlayer, OrbitTarget }
    public Transform CamOrbitModeTarget;
    float orbitRotate;
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
    Vector3 leftShoulderPosition, rightShoulderPosition;
    //AnimatorStateInfo hair;
    PlayerHealth health;
    public Vector3 leapOrigin, leapTarget;
    float leapTimer;
    float invincibleTimer;
    float invincibleBlinkTimer;
    Vector3 lastGoodGndPosition;
    float lastGoodGndPositionTimer;
    float fallTimer;
    void Start()
    {
        PlayerHealth.canvasObj = transform.Find("/Canvas").gameObject;
        lerpCurrent = Instantiate(Resources.Load("CamLerpPos").GameObject());
        lerpTarget = Instantiate(Resources.Load("CamLerpPos").GameObject());
        animator = animCtl.GetComponent<Animator>();
        leftShoulder = animCtl.transform.Find("skel/root/waist/spine/lshoulder/lhumerus");
        rightShoulder = animCtl.transform.Find("skel/root/waist/spine/rshoulder/rhumerus");
        weaponHand = rightShoulder.Find("rulna/rwrist");
        leftShoulderPosition = leftShoulder.transform.localPosition;
        rightShoulderPosition = rightShoulder.transform.localPosition;
        LoadCharacter();
        handCannonSfx = Resources.Load("sfxEmitters/handCannonSfx").GameObject();
        handCannonSuperSfx = Resources.Load("sfxEmitters/handCannonSuperSfx").GameObject();
        PlayerHealth.thisPlayer = this;
        upperBody = animator.GetNextAnimatorStateInfo(1);
        lowerBody = animator.GetNextAnimatorStateInfo(0);


        try { health = GameObject.Find("CharacterStatus").GetComponent<PlayerHealth>(); }
            //health = FindFirstObjectByType<PlayerHealth>();
        catch 
        { 
            Debug.Log("PlayerHealth obj not found."); 
            health = Instantiate(Resources.Load("CharacterStatus").GameObject()).GetComponent<PlayerHealth>();
            health.gameObject.name = "CharacterStatus";
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        invincibleTimer = invincibleTimer > 0 ? invincibleTimer - Time.fixedDeltaTime : 0; //decrement the invincible timer so it doesnt last forever, and lower clamp it to zero
        invincibleBlinkTimer = invincibleTimer > 0 ? Mathf.Repeat(invincibleBlinkTimer + Time.fixedDeltaTime * 6, 1) : 0; //increment flash timer when invincible, make it wraparound from 1 bcak to 0, and set it to 0 when not invincible
        animCtl.transform.parent.localScale = invincibleBlinkTimer < 0.5 ? Vector3.one : Vector3.zero; // make player visuals flash

        upperBody = animator.GetNextAnimatorStateInfo(1);
        lowerBody = animator.GetNextAnimatorStateInfo(0);
        if (cameraState == cameraStateNext) { PositionCamera(cameraState, lerpCurrent); }
        PositionCamera(cameraStateNext, lerpTarget);
        Camera.main.transform.position = Vector3.Lerp(lerpCurrent.transform.position, lerpTarget.transform.position, camLerpTimer);
        Camera.main.transform.rotation = Quaternion.Lerp(lerpCurrent.transform.rotation, lerpTarget.transform.rotation, camLerpTimer);
        if (cameraState != cameraStateNext)
        {
            if((cameraState != CameraModes.OrbitTarget)&&(cameraStateNext != CameraModes.OrbitTarget)){ camLerpTimer += Time.fixedDeltaTime * 2; }
            else { camLerpTimer += Time.fixedDeltaTime * 1f; }
            if (camLerpTimer >= 1) { camLerpTimer = 0; cameraState = cameraStateNext; }
        }
        if (weapon != null) { animator.SetInteger("weaponType", (int)weapon.weaponType); }
        else { animator.SetInteger("weaponType", 0); }
        
        weaponCoolDown += Time.fixedDeltaTime;
        if (PlayerHealth.health <= 0) { state = States.Dead; }        
        if(state == States.Leap) { leapTimer += Time.deltaTime; }
        else { leapTimer = 0; }
            switch (state)
            {
                case States.NoLockOn:
                    SetGoodGndPos();
                    animator.SetInteger("mode", (int)state);
                    PlayerMovement(6, 2, 1f * (sprint ? runSpeed : walkSpeed), true); //double speed if sprint is held
                    if (lockOn && (oldLockOnKeyState == false)) { checkLockOn(false); }
                    oldLockOnKeyState = lockOn;
                    if (eHeal && (oldEHealKeyState == false)) { PlayerHealth.EmergencyHeal(); }
                    oldEHealKeyState = eHeal;
                    if (lockedOnEnemy != null) { state = States.LockedOn; animator.SetTrigger("battleStance"); }
                    AnimatorUpdateWalking();
                    cameraStateNext = CameraModes.Follow;
                    if (Atk1 && weaponCoolDown >= 0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk1 pressed"); state = States.LightAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }  //fire light attack when button is pressed
                    if (Atk2 && weaponCoolDown >= 0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk2 pressed"); state = States.HeavyAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }
                    break;
                case States.LockedOn:
                    SetGoodGndPos();
                    animator.SetInteger("mode", (int)state);
                    PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                    if (lockedOnEnemy != null) { transform.LookAt(lockedOnEnemy.transform); } //if locked on, face enemy
                    else { state = States.NoLockOn; animator.SetTrigger("noBattleStance"); }  //otherwise, un-lockon
                    if (lockOn && (oldLockOnKeyState == false)) { checkLockOn(false); }
                    oldLockOnKeyState = lockOn;
                    AnimatorUpdateWalking();
                    cameraStateNext = CameraModes.StickBehindPlayer;
                    if (Atk1 && weaponCoolDown >= 0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk1 pressed"); state = States.LightAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }  //fire light attack when button is pressed
                    if (Atk2 && weaponCoolDown >= 0) { weaponCoolDown = 0 - Time.fixedDeltaTime; Debug.Log("Atk2 pressed"); state = States.HeavyAttack; weaponCoolDown = Mathf.Clamp(weaponCoolDown, -10, 0); animator.SetTrigger("battleStance"); }
                    transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                    break;
                case States.Airborne:
                    invincibleTimer = invincibleTimer < Time.fixedDeltaTime ? invincibleTimer : Time.fixedDeltaTime;
                    animCtl.transform.parent.localScale = Vector3.one; //cancel invincible flash during this animation
                    PlayerMovement(6, 2, 1f * (sprint ? runSpeed : walkSpeed), true); //double speed if sprint is held
                    if (lockOn && (lockOn != oldLockOnKeyState)) { checkLockOn(false); }
                    oldLockOnKeyState = lockOn;
                    AnimatorUpdateWalking();
                    animator.SetInteger("mode", 2);
                    cameraStateNext = CameraModes.Follow;
                    airSpd += 0.5f;
                    transform.Translate(Vector3.up * (-1 * airSpd * Time.fixedDeltaTime));
                    GoBackToGndAfterFallTooLong();
                    break;
                case States.LightAttack:
                    animator.SetInteger("mode", (int)state);
                    if (weapon != null) { AttackCreate(weapon.bullet, weapon.hitScan, PlayerHealth.baseAtk + weapon.attackPower, weapon.weaponCoolDownDuration, weapon.firesfx, false); }
                    else
                    {
                        AttackCreate(
                            defaultBullet.GameObject(),
                            true,
                            PlayerHealth.baseAtk,
                            0.4f,
                            handCannonSfx,
                            false);
                    }
                    PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                    transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                    break;
                case States.HeavyAttack:
                    animator.SetInteger("mode", (int)state);
                    if (weapon != null) { AttackCreate(weapon.superBullet, weapon.superHitScan, (PlayerHealth.baseAtk + weapon.attackPower) * 2, weapon.weaponCoolDownDuration * 1.3f, weapon.superFireSfx, true); }
                    else { AttackCreate(defaultBullet, false, 20, 1, handCannonSuperSfx, true); }
                    PlayerMovement(6, 2, 0.8f * (sprint ? runSpeed : walkSpeed), false); //double speed if sprint is held
                    transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, Vector3.up); //fix tilt and roll before drawn to screen
                    break;
                case States.Knockback:
                    animCtl.transform.parent.localScale = Vector3.one; //cancel invincible flash during this animation
                    invincibleTimer = 2;
                    animator.SetInteger("mode", (int)state);
                    knockBackTimer -= Time.fixedDeltaTime;
                    if (knockBackTimer < 0) { state = States.KnockbackGetUp; animator.SetTrigger("knockbackGetUp"); Debug.Log("Getting Up"); }
                    break;
                case States.KnockbackGetUp:
                    invincibleTimer = 4;
                    animator.SetInteger("mode", (int)state);
                    //(animatorStateInfo.IsName("knockBackGetUp") && animatorStateInfo.normalizedTime > 0.9f)
                    Debug.Log(lowerBody.ToString() + lowerBody.normalizedTime.ToString() + lowerBody.IsName("KnockBackGetUp").ToString());
                    if (!lowerBody.IsName("KnockBackGetUp") && !upperBody.IsName("KnockBackGetUp"))
                    { state = States.NoLockOn; Debug.Log("GetUp Complete"); }
                    break;
                case States.Dead:
                    Instantiate(Resources.Load<GameObject>("explosionParticles"), transform.position, Quaternion.identity);
                    FadeOverlay f = Instantiate(Resources.Load<GameObject>("FadeOverlay"), transform.Find("/Canvas")).GetComponent<FadeOverlay>();
                    f.sceneForTransfer = "hubWorld";
                    f.transform.localPosition = Vector3.zero;
                    f.mode = FadeOverlay.Transitions.FadeOut;
                    PlayerHealth.health = PlayerHealth.maxHealth;
                    Destroy(gameObject);
                    break;
                case States.Leap:
                    invincibleTimer = 4;
                    animCtl.transform.parent.localScale = Vector3.one; //cancel invincible flash during this animation
                    animator.SetInteger("mode", 2);
                    transform.position = Vector3.Lerp(leapOrigin, leapTarget, leapTimer);
                    transform.Translate(Vector3.up * Mathf.Sin(leapTimer * Mathf.PI) * 8);
                    if(leapTimer >= 1) { state = States.Airborne; }
                    break;
                case States.WalkTowardGoal:
                    invincibleTimer = 2;
                    animCtl.transform.parent.localScale = Vector3.one; //cancel invincible flash during this animation
                    animator.SetInteger("mode", 0);
                    animator.SetFloat("speed", 1);
                    animator.SetFloat("animSpeed", 1);
                    cameraStateNext = CameraModes.Static;
                    break;
                case States.StandOnGoal:
                    invincibleTimer = 2;
                    animCtl.transform.parent.localScale = Vector3.one; //cancel invincible flash during this animation
                    animator.SetInteger("mode", 0);
                    animator.SetFloat("speed", 0);
                    animator.SetFloat("animSpeed", 1);
                    cameraStateNext = CameraModes.Static;
                    break;
                case States.PauseLookAtTarget:
                    invincibleTimer = 4;
                    animator.SetInteger("mode", 0);
                    animator.SetFloat("speed", 0);
                    animator.SetFloat("animSpeed", 1);
                    cameraStateNext = CameraModes.OrbitTarget;
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
    }

    private void GoBackToGndAfterFallTooLong()
    {
        fallTimer += Time.fixedDeltaTime;
        if (fallTimer > 2)
        {
            invincibleTimer = 0;
            transform.position = lastGoodGndPosition;
            DamageFrom(transform, PlayerHealth.health / 2, 2);
            fallTimer = 0;
            airSpd = 0;
        }
    }

    private void SetGoodGndPos()
    {
        lastGoodGndPositionTimer += Time.fixedDeltaTime;
        if(lastGoodGndPositionTimer > 5) { lastGoodGndPosition = transform.position; lastGoodGndPositionTimer = 0; }
    }

    public void DamageFrom(Transform enemy, int damage, float KnockoutTime)
    {
        if (invincibleTimer > 0) { invincibleTimer++;  return; }
        Debug.Log("Ouch");
        if (PlayerHealth.health <= 0) { state = States.Dead; }
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
            case CameraModes.OrbitTarget:
                orbitRotate += (Mathf.PI / 3) * Time.fixedDeltaTime;
                obj.transform.position = CamOrbitModeTarget.position + (Vector3.up * (camDistFromPlayerSetting * 0.5f));
                obj.transform.position += Vector3.left * (camDistFromPlayerSetting * Mathf.Sin(orbitRotate));
                obj.transform.position += Vector3.forward * (camDistFromPlayerSetting * Mathf.Cos(orbitRotate));
                obj.transform.LookAt(CamOrbitModeTarget);
                if (orbitRotate >= Mathf.PI * 4) { state = States.NoLockOn; orbitRotate = 0; }
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
        if (Physics.Raycast(transform.position + (transform.up*-0.4f), mov.normalized, out rayout, transform.localScale.x * (mov.magnitude+0.2f)))
        {
            transform.position = rayout.point - (transform.localScale.x*(mov.normalized * 0.2f));
            currentHeading = Vector2.zero;
            Debug.Log("walkied into "+rayout.collider.gameObject.name);
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
            { transform.position = rayout.point - (raydir * 0.2f); Debug.Log("pushed out of " + rayout.collider.gameObject.name); }
        } 
    }
    public void SetProportions()
    {
        leftShoulder.transform.localPosition = leftShoulderPosition* (1+shouldersWidth);
        rightShoulder.transform.localPosition = rightShoulderPosition * (1+shouldersWidth);
        /*leftShoulder.Translate(leftShoulder.right * 0-shouldersWidth);
        rightShoulder.Translate(rightShoulder.right * 0-shouldersWidth);*/
    }
    public void LoadCharacter()
    {
        PlayerHealth.body[0] = Math.Clamp(PlayerHealth.body[0], 1, 9); PlayerHealth.body[1] = Math.Clamp(PlayerHealth.body[1], 1, 9); PlayerHealth.body[2] = Math.Clamp(PlayerHealth.body[2], 1, 9); PlayerHealth.body[3] = Math.Clamp(PlayerHealth.body[3], 1, 9);
        foreach (Transform part in animCtl.transform.Find("Heads"))
        { if (part.name == "head" + PlayerHealth.body[0]) { part.gameObject.SetActive(true); } else { part.gameObject.SetActive(false); } }
        foreach (Transform part in animCtl.transform.Find("Arms")) 
        {
            try
            {
                part.Find("lfist" + PlayerHealth.body[1]).gameObject.SetActive(true); part.Find("rfist" + PlayerHealth.body[1]).gameObject.SetActive(true);
                part.Find("lhand" + PlayerHealth.body[1]).gameObject.SetActive(false); part.Find("rhand" + PlayerHealth.body[1]).gameObject.SetActive(false);
            }
            catch { }
            if (part.name == "arms" + PlayerHealth.body[1]) { part.gameObject.SetActive(true); } else { part.gameObject.SetActive(false); } 
        }
        foreach (Transform part in animCtl.transform.Find("torsos"))
        { if (part.name == "torso" + PlayerHealth.body[2]) { part.gameObject.SetActive(true); } else { part.gameObject.SetActive(false); } }
        foreach (Transform part in animCtl.transform.Find("Legs"))
        { if (part.name == "legs" + PlayerHealth.body[3]) { part.gameObject.SetActive(true); } else { part.gameObject.SetActive(false); } }
        shouldersWidth = 0;
        if ((PlayerHealth.body[2] == 3) || (PlayerHealth.body[2] == 4) || (PlayerHealth.body[2]==9)) //big robot torso
        { shouldersWidth = 0.6f; }
        SetProportions();
    }
}
