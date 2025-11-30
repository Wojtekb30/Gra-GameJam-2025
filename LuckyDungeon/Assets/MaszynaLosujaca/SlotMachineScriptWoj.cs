using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

[Serializable]
public class IntArrayEvent : UnityEvent<int[]> { }

public class SlotMachineScriptWoj : MonoBehaviour
{
    // --- SINGLETON GUARD ---
    private static SlotMachineScriptWoj activeInstance;

    [Header("Singleton")]
    [Tooltip("If true the slot machine GameObject will be destroyed after the player accepts. " +
             "If false (default) the machine will be hidden and kept in the scene so other enemies/objects that reference it won't lose the reference.")]
    public bool destroyOnAccept = false;

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

    [Header("Accept Events")]
    public UnityEvent OnAccept;
    public IntArrayEvent OnAcceptResult;

    [Header("Other")]
    public CanvasGroup canvasGroup;
    public Sprite emptySprite;

    private Image slot1Image;
    private Image slot2Image;
    private Image slot3Image;

    private Transform slot1Transform;
    private Transform slot2Transform;
    private Transform slot3Transform;

    // PlayerTime reference (may be null)
    private PlayerTime pt;

    private static readonly string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Awake()
    {
        // --- Singleton guard: only allow one instance at a time ---
        if (activeInstance != null && activeInstance != this)
        {
            Debug.LogWarning($"[{name}] Another SlotMachineScriptWoj is already active on '{activeInstance.name}'. Destroying this new instance to avoid duplicates.");
            Destroy(gameObject);
            return;
        }

        activeInstance = this;

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

        // Initial time subtraction (kept from earlier script)
        pt?.SubtractTime(20);

        ApplyVisibility();
    }

    void OnDestroy()
    {
        // Clear static reference when destroyed so a new instance can become active later
        if (activeInstance == this)
            activeInstance = null;
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
        if (canvasGroup == null) return;

        canvasGroup.alpha = rootVisible ? 1 : 0;
        canvasGroup.interactable = rootVisible;
        canvasGroup.blocksRaycasts = rootVisible;

        if (slot1Transform) slot1Transform.gameObject.SetActive(slot1visible);
        if (slot2Transform) slot2Transform.gameObject.SetActive(slot2visible);
        if (slot3Transform) slot3Transform.gameObject.SetActive(slot3visible);
    }

    public void TriggerSpin()
    {
        // Prevent starting a spin on a destroyed/invalid singleton
        if (activeInstance != this)
            return;

        // Ensure the machine is active in case it was hidden previously
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    // Helper: immediately randomize and display (returns the int[] result)
    int[] DoRandomizeAndDisplay()
    {
        int r1 = UnityEngine.Random.Range(1, symbolCount + 1);
        int r2 = UnityEngine.Random.Range(1, symbolCount + 1);
        int r3 = UnityEngine.Random.Range(1, symbolCount + 1);

        int[] results = { r1, r2, r3 };

        SetSlotSprite(slot1Image, r1);
        SetSlotSprite(slot2Image, r2);
        SetSlotSprite(slot3Image, r3);

        return results;
    }

    IEnumerator SpinRoutine()
    {
        // Start visible and run the first "reveal"
        rootVisible = true;
        slot1visible = slot2visible = slot3visible = true;
        ApplyVisibility();

        yield return new WaitForSeconds(prePickDelay);

        int[] results = DoRandomizeAndDisplay();

        // Notify listeners about the spin result
        OnSpinResult?.Invoke(results);
        SpinResultEvent?.Invoke(results);

        // Now wait for player input. R = reroll, E = accept.
        bool accepted = false;
        while (!accepted)
        {
            // Reroll pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Subtract time for every reroll
                pt?.SubtractTime(20);

                // Play the "hide old -> show animation -> reveal new" flow

                // 1) Hide current display immediately (simulate hide animation)
                if (emptySprite)
                {
                    if (slot1Image) slot1Image.sprite = emptySprite;
                    if (slot2Image) slot2Image.sprite = emptySprite;
                    if (slot3Image) slot3Image.sprite = emptySprite;
                }

                rootVisible = false;
                slot1visible = slot2visible = slot3visible = false;
                ApplyVisibility();

                // small pause to make hide readable
                yield return new WaitForSeconds(0.15f);

                // 2) Show UI again and play pre-pick delay (the "animation")
                rootVisible = true;
                slot1visible = slot2visible = slot3visible = true;
                ApplyVisibility();

                yield return new WaitForSeconds(prePickDelay);

                // 3) Randomize & display new results
                results = DoRandomizeAndDisplay();

                // Notify listeners about the new spin result
                OnSpinResult?.Invoke(results);
                SpinResultEvent?.Invoke(results);

                // After reveal, leave visible so player can press R again or E to accept.
            }
            // Accept pressed
            else if (Input.GetKeyDown(KeyCode.E))
            {
                // Notify accept listeners
                OnAcceptResult?.Invoke(results);
                OnAccept?.Invoke();
                accepted = true;
                break;
            }

            yield return null;
        }

        // After accept: hide sprites and UI
        if (emptySprite)
        {
            if (slot1Image) slot1Image.sprite = emptySprite;
            if (slot2Image) slot2Image.sprite = emptySprite;
            if (slot3Image) slot3Image.sprite = emptySprite;
        }

        rootVisible = false;
        slot1visible = slot2visible = slot3visible = false;
        ApplyVisibility();

        // --- IMPORTANT: either destroy OR hide-and-keep ---
        if (destroyOnAccept)
        {
            // allow OnDestroy to clear the static instance
            Destroy(gameObject);
        }
        else
        {
            // Keep the instance so other enemies that hold a reference don't see it go null.
            // We leave activeInstance pointing to this object so inspector references remain valid.
            // Deactivate the GameObject to hide it from scene until reused.
            gameObject.SetActive(false);
        }
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
