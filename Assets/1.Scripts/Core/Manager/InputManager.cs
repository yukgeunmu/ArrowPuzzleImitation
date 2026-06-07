using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera mainCam;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;

        inputActions.Disable();
    }

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos =  inputActions.Player.Position.ReadValue<Vector2>();

        Vector2 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null)
            return;

        if (hit.collider.TryGetComponent<ArrowBlock>(out ArrowBlock arrow))
        {
            arrow.TryMove();
        }
    }
}