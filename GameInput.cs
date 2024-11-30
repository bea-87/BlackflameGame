using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnEnforceAction;

    public event EventHandler OnRunAction;

    public event EventHandler OnPunchAction;

    public event EventHandler OnPickUpAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Enforce.performed += Enforce_Performed;
        playerInputActions.Player.PickUp.performed += PickUp_Performed;
        playerInputActions.Player.Run.performed += Run_Performed;
        playerInputActions.Player.Punch.performed += Punch_Performed;
    }

    private void Enforce_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnEnforceAction?.Invoke(this, EventArgs.Empty);
    }

    private void Run_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnRunAction?.Invoke(this, EventArgs.Empty);
    }

    private void Punch_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPunchAction?.Invoke(this, EventArgs.Empty);
    }

    private void PickUp_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPickUpAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;
        return inputVector;
    }
}
