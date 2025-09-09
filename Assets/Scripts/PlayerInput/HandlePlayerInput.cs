using UnityEngine;
using UnityEngine.InputSystem;

public class HandlePlayerInput : MonoBehaviour
{
    public bool AcceleratePressed { get; private set; }
    public bool DeceleratePressed { get; private set; }
    public bool TurnLeftPressed { get; private set; }
    public bool TurnRightPressed { get; private set; }

    public GameInputActions gameInputActions;

    private void OnEnable()
    {
        if (gameInputActions == null)
        {
            gameInputActions = new GameInputActions();

            gameInputActions.Player.Accelerate.performed += ctx => AcceleratePressed = true;
            gameInputActions.Player.Accelerate.canceled += ctx => AcceleratePressed = false;

            gameInputActions.Player.Decelerate.performed += ctx => DeceleratePressed = true;
            gameInputActions.Player.Decelerate.canceled += ctx => DeceleratePressed = false;

            gameInputActions.Player.TurnLeft.performed += ctx => TurnLeftPressed = true;
            gameInputActions.Player.TurnLeft.canceled += ctx => TurnLeftPressed = false;

            gameInputActions.Player.TurnRight.performed += ctx => TurnRightPressed = true;
            gameInputActions.Player.TurnRight.canceled += ctx => TurnRightPressed = false;
        }

        gameInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        gameInputActions.Player.Disable();
    }

    private void LateUpdate()
    {
        AcceleratePressed = false;
        DeceleratePressed = false;   
    }
}
