using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnToggleDebugView;
    public event EventHandler OnRevertLastMove;
    public event EventHandler OnEndTurn;
    public event EventHandler OnFirstWeaponUse;
    public event EventHandler OnSecondWeaponUse;
    public event EventHandler OnRepair;

    private Controls controls;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        controls = new Controls();

        controls.DebugActionMap.Enable();
        controls.DebugActionMap.ToggleDebugView.performed += ToggleDebugView_performed;


        controls.Player.Enable();
        controls.Player.RevertLastMove.performed += RevertLastMove_performed;
        controls.Player.EndTurn.performed += EndTurn_performed;
        controls.Player.FirstWeapon.performed += FirstWeapon_performed;
        controls.Player.SecondWeapon.performed += SecondWeapon_performed;
        controls.Player.Repair.performed += Repair_performed;
    }

    private void Repair_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRepair?.Invoke(this, EventArgs.Empty);
    }

    private void SecondWeapon_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSecondWeaponUse?.Invoke(this, EventArgs.Empty);
    }

    private void FirstWeapon_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFirstWeaponUse?.Invoke(this, EventArgs.Empty);
    }

    private void EndTurn_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnEndTurn?.Invoke(this, EventArgs.Empty);
    }

    private void RevertLastMove_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRevertLastMove?.Invoke(this, EventArgs.Empty);
    }

    private void ToggleDebugView_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnToggleDebugView?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        controls.DebugActionMap.ToggleDebugView.performed -= ToggleDebugView_performed;
        controls.Player.RevertLastMove.performed -= RevertLastMove_performed;
        controls.Player.EndTurn.performed -= EndTurn_performed;
        controls.Player.FirstWeapon.performed -= FirstWeapon_performed;
        controls.Player.SecondWeapon.performed -= SecondWeapon_performed;
        controls.Player.Repair.performed -= Repair_performed;
        controls.Dispose();
    }
}
