using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

[Serializable]
public class IntArrayEvent : UnityEvent<int[]> { }

public class SlotMachineScriptWoj : MonoBehaviour
{
    public PlayerTime playerTime;               // assign in inspector (optional)

    [Header("Symbol Settings")]
    [Tooltip("How many symbols can be randomly selected (1..symbolCount). Must match number of sprites.")]
    public int symbolCount = 5;

    [Tooltip("Sprite list must contain at least 'symbolCount' sprites.")]
    public Sprite[] slotSprites;

    [Tooltip("Load fallback sprites from Resources/Sprites/A, B, C, ... if missing.")]
    public bool useResourcesFallback = true;

    [Header("Visibility")]
    public bool rootVisible = true;
    public bool slot1visible = true;
    public bool slot2visible = true;
    public bool slot3visible = true;

    [Header("Timing")]
    public float prePickDelay = 2f;
    public float postResultDelay = 4f;

    [Header("Events")]
    public IntArrayEvent OnSpinResult;
    public event Action<int[]> SpinResultEvent;

    [Header("Other")]
    public CanvasGroup canvasGroup;
    public Sprite emptySprite;

    private Image slot1Image;
    private Image slot2Image;
    private Image slot3Image;

    private Transform slot1Transform;
    private Transform slot2Transform;
    private Transform slot3Transform;

    // <-- the corrected reference
    private PlayerTime pt;

    private static readonly string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Awake()
    {
        // Resolve PlayerTime reference
        if (playerTime != null)
            pt = playerTime;
        else
        {
            // Try to find it on a parent of this GameObject
            pt = GetComponentInParent<PlayerTime>();
            if (pt == null)
                Debug.LogWarning($"{name}: No PlayerTime component found. Time subtraction will be skipped.");
        }

        // Ensure CanvasGroup exists
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Slot references
        slot1Transform = transform.Find("Slot1");
        slot2Transform = transform.Find("Slot2");
        slot3Transform = transform.Find("Slot3");

        if (slot1Transform) slot1Image = slot1Transform.GetComponent<Image>();
        if (slot2Transform) slot2Image = slot2Transform.GetComponent<Image>();
        if (slot3Transform) slot3Image = slot3Transform.GetComponent<Image>();

        if (useResourcesFallback)
            EnsureSprites();

        ApplyVisibility();
    }

    /// <summary>
    /// Ensures slotSprites has at least 'symbolCount' sprites.
    /// Loads missing ones from Resources/Sprites/A, B, C...
    /// </summary>
    void EnsureSprites()
    {
        if (symbolCount < 1) symbolCount = 1;
        if (slotSprites == null) slotSprites = new Sprite[symbolCount];

        // Expand array if too small
        if (slotSprites.Length < symbolCount)
        {
            Sprite[] expanded = new Sprite[symbolCount];
            for (int i = 0; i < slotSprites.Length; i++)
                expanded[i] = slotSprites[i];
            slotSprites = expanded;
        }

        for (int i = 0; i < symbolCount; i++)
        {
            if (slotSprites[i] != null) continue;

            // Load A, B, C, D...
            char letter = letters[Mathf.Clamp(i, 0, letters.Length - 1)];
            slotSprites[i] = Resources.Load<Sprite>($"Sprites/{letter}");
        }
    }

    void ApplyVisibility()
    {
        canvasGroup.alpha = rootVisible ? 1 : 0;
        canvasGroup.interactable = rootVisible;
        canvasGroup.blocksRaycasts = rootVisible;

        if (slot1Transform) slot1Transform.gameObject.SetActive(slot1visible);
        if (slot2Transform) slot2Transform.gameObject.SetActive(slot2visible);
        if (slot3Transform) slot3Transform.gameObject.SetActive(slot3visible);
    }

    public void TriggerSpin()
    {
        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        rootVisible = true;
        slot1visible = slot2visible = slot3visible = true;
        ApplyVisibility();

        yield return new WaitForSeconds(prePickDelay);

        // 1..symbolCount (inclusive)
        int r1 = UnityEngine.Random.Range(1, symbolCount + 1);
        int r2 = UnityEngine.Random.Range(1, symbolCount + 1);
        int r3 = UnityEngine.Random.Range(1, symbolCount + 1);

        int[] results = { r1, r2, r3 };

        SetSlotSprite(slot1Image, r1);
        SetSlotSprite(slot2Image, r2);
        SetSlotSprite(slot3Image, r3);

        OnSpinResult?.Invoke(results);
        SpinResultEvent?.Invoke(results);

        yield return new WaitForSeconds(postResultDelay);

        if (emptySprite)
        {
            if (slot1Image) slot1Image.sprite = emptySprite;
            if (slot2Image) slot2Image.sprite = emptySprite;
            if (slot3Image) slot3Image.sprite = emptySprite;
        }

        // Only subtract time if we actually resolved a PlayerTime component
        pt.SubtractTime(20);

        rootVisible = false;
        slot1visible = slot2visible = slot3visible = false;
        ApplyVisibility();
    }

    void SetSlotSprite(Image img, int number)
    {
        if (!img) return;
        if (slotSprites == null || slotSprites.Length == 0)
        {
            Debug.LogWarning("SlotMachine: No sprites assigned.");
            return;
        }

        int index = Mathf.Clamp(number - 1, 0, slotSprites.Length - 1);
        img.sprite = slotSprites[index] ?? emptySprite;
    }

#if UNITY_EDITOR
    [ContextMenu("Test TriggerSpin")]
    void EditorTest() => TriggerSpin();
#endif
}
