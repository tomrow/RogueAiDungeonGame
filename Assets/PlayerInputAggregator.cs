using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAggregator : MonoBehaviour
{
    PlayerCtl playerCtl;

    InputAction moveAction;
    InputAction lookAction;
    InputAction sprintAction;
    InputAction lightAttackAction;
    InputAction heavyAttackAction;
    InputAction interactAction;
    InputAction lockOnAction;
    InputAction eHealAction;
    public static bool inputEnabled = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCtl = GetComponent<PlayerCtl>();
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        lightAttackAction = InputSystem.actions.FindAction("AttackLight");
        heavyAttackAction = InputSystem.actions.FindAction("AttackHeavy");
        interactAction = InputSystem.actions.FindAction("Interact");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        lockOnAction = InputSystem.actions.FindAction("LockOn");
        eHealAction = InputSystem.actions.FindAction("EmergencyHeal");
    }

    // Update is called once per frame
    void FixedUpdate()
    { //this code sucks and can be better

        playerCtl.move = PlayerInputAggregator.inputEnabled ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        playerCtl.Atk1 = PlayerInputAggregator.inputEnabled ? lightAttackAction.IsPressed() : false;
        playerCtl.Atk2 = PlayerInputAggregator.inputEnabled ? heavyAttackAction.IsPressed() : false;
        playerCtl.Atk3 = PlayerInputAggregator.inputEnabled ? interactAction.IsPressed() : false;
        playerCtl.sprint = PlayerInputAggregator.inputEnabled ? sprintAction.IsPressed() : false;
        playerCtl.lockOn = PlayerInputAggregator.inputEnabled ? lockOnAction.IsPressed() : false;
        playerCtl.orbit = PlayerInputAggregator.inputEnabled ? lookAction.ReadValue<Vector2>()*-1 : Vector2.zero;
        playerCtl.eHeal = PlayerInputAggregator.inputEnabled ? eHealAction.IsPressed() : false;
    }
}
