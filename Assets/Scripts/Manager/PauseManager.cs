using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [Tooltip("The parent GameObject of the pause menu UI.")]
    public GameObject pauseMenuParent;
    
    [Tooltip("List of other UI GameObjects to disable when the pause menu is active.")]
    public List<GameObject> otherUIIsToDisable;
    
    [Tooltip("The parent GameObject of the settings menu UI.")]
    public GameObject settingsMenuParent;
    
    [Tooltip("The parent GameObject of the main menu UI.")]
    public GameObject menuMenuParent;
    
    [Header("Input Actions")]
    [Tooltip("Reference to the input action for pausing the game.")]
    public InputActionReference pauseActionRef;
    
    private bool _isPause;
    private bool _isSettingsOpen;
    
    private void Start()
    {
        if (pauseMenuParent != null)
        {
            pauseMenuParent.SetActive(false);
        }
        _isPause = false;
        
        if (settingsMenuParent != null)
        {
            settingsMenuParent.SetActive(false);
        }
        _isSettingsOpen = false;

        if (menuMenuParent != null)
        {
            menuMenuParent.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        pauseActionRef.action.performed += OnPausePerformed;
    }
    
    private void OnDisable()
    {
        pauseActionRef.action.performed -= OnPausePerformed;
    }

    public void Pause()
    {
        SwitchPauseState();
    }

    public void Settings()
    {
        SwitchSettingsState();
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (_isSettingsOpen)
        {
            _isSettingsOpen = false;
            _isPause = false;
            settingsMenuParent.SetActive(false);
            pauseMenuParent.SetActive(false);
            menuMenuParent.SetActive(false);
            return;
        }
        
        SwitchPauseState();
    }
    
    private void SwitchPauseState()
    {
        _isPause = !_isPause;
        pauseMenuParent.SetActive(_isPause);
        menuMenuParent.SetActive(_isPause);
        
        foreach (GameObject ui in otherUIIsToDisable)
        {
            if (ui != null)
            {
                ui.SetActive(!_isPause);
            }
        }
    }
    
    private void SwitchSettingsState()
    {
        _isSettingsOpen = !_isSettingsOpen;
        settingsMenuParent.SetActive(_isSettingsOpen);
        menuMenuParent.SetActive(!_isSettingsOpen);
    }
}
