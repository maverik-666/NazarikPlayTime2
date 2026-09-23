namespace Project
{
    using System;
    using UnityEngine.InputSystem;

    public class ClickListener
    {
        private readonly InputActionReference _clickActionReference;

        private bool _isPressed = false;

        public event Action Pressed;
        public event Action Released;
        
        public bool IsPressed => _isPressed;
        
        public ClickListener(InputActionReference clickActionRef)
        {
            _clickActionReference = clickActionRef;
            
            _clickActionReference.action.Enable();
            _clickActionReference.action.started += OnPressPerformed;
            _clickActionReference.action.canceled += OnReleasePerformed;
        }
        
        private void OnPressPerformed(InputAction.CallbackContext ctx)
        {
            Pressed?.Invoke();
            _isPressed = true;
        }

        private void OnReleasePerformed(InputAction.CallbackContext ctx)
        {
            Released?.Invoke();
            _isPressed = false;
        }
        
        public void Dispose()
        {
            _clickActionReference.action.started -= OnPressPerformed;
            _clickActionReference.action.canceled -= OnReleasePerformed;
            _clickActionReference.action.Disable();
        }
    }
}