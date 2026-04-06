using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ARInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnPerformTap;
    [SerializeField] private InputActionReference tapAction;
    private void OnEnable()
    {
        tapAction.action.started += OnTapTriggered;
        tapAction.action.Enable();
    }
    private void OnDisable()
    {
        tapAction.action.started -= OnTapTriggered;
        tapAction.action.Disable();
    }
    private void OnTapTriggered(InputAction.CallbackContext context)
    {
        if (Pointer.current != null)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            OnPerformTap?.Invoke(screenPosition);
        }
    }

}
