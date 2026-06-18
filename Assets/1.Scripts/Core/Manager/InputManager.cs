using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    private Camera mainCam;

    private CameraController mainCamController;

    private PlayerInputActions inputActions;

    public void Init()
    {
        inputActions = new PlayerInputActions();
        mainCam = Camera.main;
        mainCamController = mainCam.GetComponent<CameraController>();
    }

    public void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Click.performed += OnClick;
    }

    public void OnDisable()
    {
        inputActions.Player.Click.performed -= OnClick;

        inputActions.Disable();
    }


    private void OnClick(InputAction.CallbackContext ctx)
    {

        if (Manager.Instance.UI.HasPopup)
            return;

        if (mainCamController.IsDragging)
            return;

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