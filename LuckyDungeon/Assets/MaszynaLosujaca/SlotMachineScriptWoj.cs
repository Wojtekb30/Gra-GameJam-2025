using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

[Serializable]
public class IntArrayEvent : UnityEvent<int[]> { }

public class SlotMachineScriptWoj : MonoBehaviour
{
    [Header("Visibility (can be controlled at runtime)")]
    // rootVisible now controls CanvasGroup alpha / interactability instead of SetActive
    public bool rootVisible = true;
    public bool slot1visible = true;
    public bool slot2visible = true;
    public bool slot3visible = true;

    [Header("Slot sprites (prefer assigning in Inspector)")]
    // index 0 => A, 1 => B, 2 => C
    public Sprite[] slotSprites = new Sprite[3];

    [Tooltip("If true and slotSprites elements are null, script will try to load Sprites from Resources/Sprites/A/B/C")]
    public bool useResourcesFallback = true;

    [Header("Timing")]
    public float prePickDelay = 2f; // wait before picking random values
    public float postResultDelay = 4f; // wait after calling event before hiding

    [Header("Optional Events")]
    // UnityEvent visible in Inspector; delivers the int[] results (each element is 1..3)
    public IntArrayEvent OnSpinResult;

    // C# event for code subscriptions
    public event Action<int[]> SpinResultEvent;

    // CanvasGroup used to hide/show UI without deactivating the GameObject
    [Tooltip("Optional: a CanvasGroup on this GameObject. If null the script will add one automatically.")]
    public CanvasGroup canvasGroup;

    public Sprite emptySprite;

    // internal references
    Image slot1Image;
    Image slot2Image;
    Image slot3Image;

    Transform slot1Transform;
    Transform slot2Transform;
    Transform slot3Transform;

    void Awake()
    {
        // Ensure CanvasGroup exists (so we never deactivate the root GameObject)
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // add one so hiding doesn't disable the gameobject
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // find child transforms
        slot1Transform = transform.Find("Slot1");
        slot2Transform = transform.Find("Slot2");
        slot3Transform = transform.Find("Slot3");

        if (slot1Transform != null) slot1Image = slot1Transform.GetComponent<Image>();
        if (slot2Transform != null) slot2Image = slot2Transform.GetComponent<Image>();
        if (slot3Transform != null) slot3Image = slot3Transform.GetComponent<Image>();

        // resources fallback: try to load A, B, C if inspector sprites are not assigned
        if (useResourcesFallback)
        {
            TryLoadResourcesFallback();
        }

        // Apply initial visibility booleans
        ApplyVisibility();
    }

    void TryLoadResourcesFallback()
    {
        // only load missing sprites
        if (slotSprites == null || slotSprites.Length < 3)
            slotSprites = new Sprite[3];

        if (slotSprites[0] == null)
            slotSprites[0] = Resources.Load<Sprite>("Sprites/A"); // Resources/Sprites/A.png
        if (slotSprites[1] == null)
            slotSprites[1] = Resources.Load<Sprite>("Sprites/B"); // Resources/Sprites/B.png
        if (slotSprites[2] == null)
            slotSprites[2] = Resources.Load<Sprite>("Sprites/C"); // Resources/Sprites/C.png
    }

    void ApplyVisibility()
    {
        // Important: never disable the root GameObject here.
        // Use CanvasGroup to make the UI invisible and non-interactable instead.

        if (canvasGroup != null)
        {
            if (rootVisible)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        // Child slots can still be enabled/disabled as needed (safe while root remains active)
        if (slot1Transform != null)
            slot1Transform.gameObject.SetActive(slot1visible);
        if (slot2Transform != null)
            slot2Transform.gameObject.SetActive(slot2visible);
        if (slot3Transform != null)
            slot3Transform.gameObject.SetActive(slot3visible);
    }

    /// <summary>
    /// Public method to trigger the full sequence.
    /// Call this from UI Button onClick, another script, etc.
    /// </summary>
    public void TriggerSpin()
    {
        // If a previous coroutine might be running, stop it to ensure deterministic behaviour
        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        // 1) Make the canvas and its child elements visible (without deactivating GameObject)
        rootVisible = true;
        slot1visible = slot2visible = slot3visible = true;
        ApplyVisibility();

        // 2) Wait prePickDelay seconds
        yield return new WaitForSeconds(prePickDelay);

        // 3) Pick 3 random numbers from 1 to 3 (inclusive)
        int r1 = UnityEngine.Random.Range(1, 4); // 1..3
        int r2 = UnityEngine.Random.Range(1, 4);
        int r3 = UnityEngine.Random.Range(1, 4);

        int[] results = new int[] { r1, r2, r3 };

        // 4) Set each slot image to A/B/C depending on the random number
        SetSlotSprite(slot1Image, r1);
        SetSlotSprite(slot2Image, r2);
        SetSlotSprite(slot3Image, r3);

        // 5) Call other (dummy) event and give it results.
        // Raise UnityEvent for inspector listeners
        if (OnSpinResult != null)
            OnSpinResult.Invoke(results);

        // Raise C# event for code listeners
        SpinResultEvent?.Invoke(results);

        // 6) Wait postResultDelay seconds
        yield return new WaitForSeconds(postResultDelay);

        // 7) Make the canvas and its children invisible (via CanvasGroup, not SetActive)
        slot1Image.sprite = emptySprite;
        slot2Image.sprite = emptySprite;
        slot3Image.sprite = emptySprite;
        rootVisible = false;
        slot1visible = slot2visible = slot3visible = false;
        ApplyVisibility();
    }

    void SetSlotSprite(Image img, int number)
    {
        if (img == null) return;

        // number in 1..3 maps to slotSprites index 0..2
        int index = Mathf.Clamp(number - 1, 0, 2);
        if (slotSprites != null && index < slotSprites.Length && slotSprites[index] != null)
        {
            img.sprite = slotSprites[index];
            // ensure native size or preserve aspect as needed -- here we leave as-is
            return;
        }

        // fallback: try to load from resources by name
        string resourceName = number == 1 ? "Sprites/A" : number == 2 ? "Sprites/B" : "Sprites/C";
        Sprite s = Resources.Load<Sprite>(resourceName);
        if (s != null)
            img.sprite = s;
        else
            Debug.LogWarning($"SlotMachineScriptWoj: sprite for number {number} not found (index {index}). Assign in inspector or place under Resources/{resourceName}.png");
    }

#if UNITY_EDITOR
    // Editor helper: draw a button in inspector would be possible with a custom editor, but here's a simple quick way to test via script
    [ContextMenu("Test TriggerSpin")]
    void EditorTest() => TriggerSpin();
#endif
}
