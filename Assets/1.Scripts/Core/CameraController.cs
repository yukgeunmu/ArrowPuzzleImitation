using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private float zoomSpeed = 5f;

    [SerializeField]
    private float minZoom = 3f;

    [SerializeField]
    private float maxZoom = 20f;

    [SerializeField]
    private float padding = 2f;

    [SerializeField]
    private float dragSpeed = 0.01f;

    [SerializeField]
    private float dragThreshold = 10f;

    private Vector2 touchStartPos;
    private bool isTouchDragging;

    private float defaultZoom;

    private Vector2 lastPointerPosition;

    public bool IsDragging => isTouchDragging;

    private bool isDragging;

    private float minX;
    private float maxX;

    private float minY;
    private float maxY;


    private void Start()
    {
        Manager.Instance.Stage.OnStageLoaded += FitToGrid;
    }

    private void Update()
    {
        HandleMouseZoom();
        HandlePinchZoom();

        if (targetCamera.orthographicSize <= defaultZoom)
        {
            HandleMouseDrag();
            HandleTouchDrag();
        }
    }

    public void FitToGrid(int width, int height)
    {

        float aspect = (float)Screen.width / Screen.height;

        float verticalSize = height * 0.5f + padding;

        float horizontalSize = (width * 0.5f + padding) / aspect;

        targetCamera.orthographicSize = Mathf.Max( verticalSize, horizontalSize);

        defaultZoom = targetCamera.orthographicSize;

        minZoom = defaultZoom * 0.5f;
        maxZoom = defaultZoom * 2f;

        minX = -width * 0.5f;
        maxX = width * 0.5f;

        minY = -height * 0.5f;
        maxY = height * 0.5f;

        targetCamera.transform.position = new Vector3( 0f, 0f, targetCamera.transform.position.z);
    }

    private void HandleMouseZoom()
    {
        if (Mouse.current == null)
            return;

        float scroll =
            Mouse.current.scroll.ReadValue().y;

        if (Mathf.Approximately(scroll, 0))
            return;

        targetCamera.orthographicSize -=
            scroll * zoomSpeed;

        targetCamera.orthographicSize =
            Mathf.Clamp(
                targetCamera.orthographicSize,
                minZoom,
                maxZoom);
    }

    private void HandlePinchZoom()
    {
        if (Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;

        if (touches.Count < 2)
            return;

        if (!touches[0].isInProgress ||
            !touches[1].isInProgress)
            return;

        Vector2 currentPos0 =
            touches[0].position.ReadValue();

        Vector2 currentPos1 =
            touches[1].position.ReadValue();

        Vector2 prevPos0 =
            currentPos0 -
            touches[0].delta.ReadValue();

        Vector2 prevPos1 =
            currentPos1 -
            touches[1].delta.ReadValue();

        float previousDistance =
            Vector2.Distance(
                prevPos0,
                prevPos1);

        float currentDistance =
            Vector2.Distance(
                currentPos0,
                currentPos1);

        float delta =
            currentDistance -
            previousDistance;

        targetCamera.orthographicSize -=
            delta * zoomSpeed;

        targetCamera.orthographicSize =
            Mathf.Clamp(
                targetCamera.orthographicSize,
                minZoom,
                maxZoom);
    }

    private void HandleMouseDrag()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastPointerPosition =
                Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        Vector2 currentPos =
            Mouse.current.position.ReadValue();

        Vector2 delta =
            currentPos - lastPointerPosition;

        MoveCamera(delta);

        lastPointerPosition = currentPos;
    }

    private void MoveCamera(Vector2 delta)
    {
        Vector3 move =
            new Vector3(
                -delta.x,
                -delta.y,
                0f);

        move *= dragSpeed;

        Vector3 targetPos =
            targetCamera.transform.position + move;

        targetPos.x =
            Mathf.Clamp(
                targetPos.x,
                minX,
                maxX);

        targetPos.y =
            Mathf.Clamp(
                targetPos.y,
                minY,
                maxY);

        targetCamera.transform.position =
            targetPos;
    }

    private void HandleTouchDrag()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.isPressed)
            return;

        // 터치 시작
        if (touch.press.wasPressedThisFrame)
        {
            touchStartPos = touch.position.ReadValue();
            isTouchDragging = false;
            return;
        }

        Vector2 currentPos =
            touch.position.ReadValue();

        float distance =
            Vector2.Distance(
                touchStartPos,
                currentPos);

        if (distance > dragThreshold)
        {
            isTouchDragging = true;
        }

        if (!isTouchDragging)
            return;

        MoveCamera(
            touch.delta.ReadValue());
    }
}