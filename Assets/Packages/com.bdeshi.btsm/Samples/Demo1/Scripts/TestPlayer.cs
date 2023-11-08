using Bdeshi.BTSM.Samples.Demo1;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Bdeshi.BTSM.Samples.Demo1
{
    [RequireComponent(typeof(BasicMoveComponent))]
    public class TestPlayer : MonoBehaviour
    {
        public InputActionReference moveAction; 
        BasicMoveComponent moveComponent;
        Vector3 moveInput;
        // Use this for initialization
        void Awake()
        {
            moveComponent = GetComponent<BasicMoveComponent>();
        }

        private void OnEnable()
        {
            moveAction.asset.Enable();
            moveAction.action.performed += handleMovePerformed;
            moveAction.action.canceled += handleMoveCancelled;
        }

        private void OnDisable()
        {
            moveAction.asset.Disable();

            moveAction.action.performed -= handleMovePerformed;
            moveAction.action.canceled -= handleMoveCancelled;
        }

        private void handleMoveCancelled(InputAction.CallbackContext context)
        {
            moveInput = Vector3.zero;
        }

        private void handleMovePerformed(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            moveInput = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }

        private void Update()
        {
            moveComponent.MoveInputNextFrame = moveInput;
        }

    }
}