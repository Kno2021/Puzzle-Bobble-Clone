using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, I_Input
{
    Input_Actions _inputActions;
    
    Vector2 _rotationkeyValues;
    public Vector2 RotationKeyValues { get { return _rotationkeyValues; } }
    bool _firePressed;
    public bool FirePressed { get { return _firePressed; } }

    private void Awake()
    {
        //_inputActions = new Input_Actions();
        //_inputActions.Player.MoveTarget.performed += x => _rotationkeyValues = x.ReadValue<Vector2>();
        //_inputActions.Player.Fire.started += x => _firePressed = x.ReadValueAsButton();
        //_inputActions.Player.Fire.canceled += x => _firePressed = x.ReadValueAsButton();
        //_inputActions.Player.Enable();
    }
    private void Start()
    {
        _inputActions.Player.MoveTarget.performed += x => _rotationkeyValues = x.ReadValue<Vector2>();
        _inputActions.Player.Fire.started += x => _firePressed = x.ReadValueAsButton();
        _inputActions.Player.Fire.canceled += x => _firePressed = x.ReadValueAsButton();
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    public void SetInputActions(Input_Actions inputActions)
    {
        if(_inputActions == null)
            _inputActions = inputActions;
    }
}
