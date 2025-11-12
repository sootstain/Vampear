using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairFollow : MonoBehaviour
{
    private RectTransform crosshairRect;
    private Canvas canvas;

    void Start()
    {
        crosshairRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
    }

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            crosshairRect.position = mousePosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePosition,
                canvas.worldCamera,
                out localPoint
            );
            crosshairRect.localPosition = localPoint;
        }
    }
}