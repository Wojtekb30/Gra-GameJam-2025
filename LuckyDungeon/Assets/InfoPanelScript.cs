using UnityEngine;

public class InfoPanelScript : MonoBehaviour
{
    [Tooltip("Movement speed in canvas units (increase for faster).")]
    public float moveSpeed = 200f;

    private RectTransform rect;         // this panel's RectTransform
    private RectTransform canvasRect;   // the root canvas RectTransform
    private bool isMoving = true;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        Canvas parentCanvas = GetComponentInParent<Canvas>();

        if (rect == null)
        {
            Debug.LogError("[InfoPanelScript] No RectTransform found. Attach this script to a UI element (panel).");
            enabled = false;
            return;
        }

        if (parentCanvas == null)
        {
            Debug.LogError("[InfoPanelScript] No Canvas found in parents. Panel must be child of a Canvas.");
            enabled = false;
            return;
        }

        canvasRect = parentCanvas.GetComponent<RectTransform>();

        // Place the panel so its right edge is flush with the canvas right edge (fully visible)
        // anchoredPosition is in local canvas units where center is (0,0).
        float canvasHalfWidth = canvasRect.rect.width * 0.5f;
        float panelWidth = rect.rect.width * rect.lossyScale.x;
        float rightEdgeOffset = panelWidth * (1f - rect.pivot.x);

        float startX = canvasHalfWidth - rightEdgeOffset;
        rect.anchoredPosition = new Vector2(startX, rect.anchoredPosition.y);

        // start moving immediately
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        // Move left in local anchoredPosition space
        rect.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

        // Compute the panel left edge in anchored coordinates (account for scale)
        float panelWidth = rect.rect.width * rect.lossyScale.x;
        float leftEdge = rect.anchoredPosition.x - panelWidth * rect.pivot.x;

        // Canvas left boundary in anchored coordinates is -canvasHalfWidth
        float canvasHalfWidth = canvasRect.rect.width * 0.5f;
        if (leftEdge < -canvasHalfWidth)
        {
            // fully off-screen -> deactivate
            gameObject.SetActive(false);
            isMoving = false;
        }
    }
}
