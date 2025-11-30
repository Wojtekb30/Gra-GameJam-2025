// GameUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }

    [Header("Panels")] 
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Shared UI")]
    public TextMeshProUGUI resultText; // optional shared text used by both panels (or leave null and put text inside each panel)
    public Button restartButton;       // hook up the Restart button (can be on either panel)
    public Button restartButton2;
    [Header("Options")]
    public bool pauseOnEnd = true;     // set to true to freeze time when game ends




    public PlayerHealth phptr;
    public PlayerMana pmptr;
    public PlayerStamina staminaptr;
    public PlayerTime timeptr;
    public GoldTextScript1 goldptr;





    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (restartButton != null) restartButton.onClick.AddListener(RestartScene);
        if (restartButton2 != null) restartButton2.onClick.AddListener(RestartScene);

        // Try to subscribe automatically to player/time if present (Inspector assignment is preferable)
        PlayerHealth ph = FindObjectOfType<PlayerHealth>();
        if (ph != null) ph.OnDied += HandlePlayerDied;

        PlayerTime pt = FindObjectOfType<PlayerTime>();
        if (pt != null) pt.OnTimeExpired += HandleTimeExpired;
    }

    void HandlePlayerDied() => ShowLose("You lost!");

    void HandleTimeExpired() => ShowLose("Time's up!");

    public void ShowLose(string message = "You lost!")
    {
        if (pauseOnEnd) Time.timeScale = 0f;
        if (resultText != null) resultText.text = message;
        if (losePanel != null) losePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowWin(string message = "You won!")
    {
        if (pauseOnEnd) Time.timeScale = 0f;
        if (resultText != null) resultText.text = message;
        if (winPanel != null) winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartScene()
    {

        phptr.currentHealth = 100;
        pmptr.currentMana = 100;
        staminaptr.currentStamina = 100;

        int readTime = timeptr.GetCurrentTime();
        if (readTime>120) {
            readTime = 120;
        }

        timeptr.AddTime(120-readTime);
        goldptr.SetGold(0);



        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
