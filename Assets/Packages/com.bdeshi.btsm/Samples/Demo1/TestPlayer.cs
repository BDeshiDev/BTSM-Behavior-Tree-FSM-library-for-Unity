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
        // Use this for initialization
        void Awake()
        {
            moveComponent = GetComponent<BasicMoveComponent>();
        }

        private void OnEnable()
        {
            moveAction.action.performed += handleMovePerformed;
            moveAction.action.canceled += handleMoveCancelled;
        }

        private void OnDisable()
        {
            moveAction.action.performed -= handleMovePerformed;
            moveAction.action.canceled -= handleMoveCancelled;
        }

        private void handleMoveCancelled(InputAction.CallbackContext context)
        {
            moveComponent.MoveInputNextFrame = Vector3.zero;
        }

        private void handleMovePerformed(InputAction.CallbackContext context)
        {
            moveComponent.MoveInputNextFrame = context.ReadValue<Vector2>();
        }

    }
}