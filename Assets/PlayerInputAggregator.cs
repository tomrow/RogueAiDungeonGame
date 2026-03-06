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
    }

    // Update is called once per frame
    void Update()
    { //this code sucks and can be better
        playerCtl.move = moveAction.ReadValue<Vector2>();
        playerCtl.Atk1 = lightAttackAction.IsPressed();
        playerCtl.Atk2 = heavyAttackAction.IsPressed();
        playerCtl.Atk3 = interactAction.IsPressed();
        playerCtl.sprint = sprintAction.IsPressed();
        playerCtl.lockOn = lockOnAction.IsPressed();
        playerCtl.orbit = lookAction.ReadValue<Vector2>();
    }
}
