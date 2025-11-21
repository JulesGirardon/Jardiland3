using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [Tooltip("Final height to which objects will rise before scene transition.")]
    public float finalHeight = 50f;       

    [Tooltip("Duration for each object's rise animation.")]
    public float durationPerObject = 2f;  
    
    [Tooltip("Maximum random delay before each object's rise animation starts.")]
    public float maxRandomDelay = 1f;     
    
    [Tooltip("Easing type for the rise animation.")]
    public Ease easeType = Ease.OutBounce;
    
    [Tooltip("Name of the scene to load after the animation.")]
    public string sceneToLoad = "SceneGame";
    
    [Header("Game Over Settings")]
    [Tooltip("Game Over UI GameObject.")]
    public GameObject gameOver;
    
    [Tooltip("Game Over Text UI element.")]
    public TextMeshProUGUI gameOverText;

    [Tooltip("Game Time Text UI element.")]
    public TextMeshProUGUI gameTimeText;
    
    [Header("Settings Menu")]
    [Tooltip("The parent GameObject of the settings menu UI.")]
    public GameObject settingsMenuParent;
    
    [Tooltip("List of other UI GameObjects to disable when the pause menu is active.")]
    public List<GameObject> otherUIIsToDisable;

    [Header("Best Time Settings")]
    [Tooltip("Best Time Text UI element.")]
    public TextMeshProUGUI bestTime;
    
    private bool _isSettingsOpen;
    
    public void Start()
    {
        _isSettingsOpen = false;
        settingsMenuParent.SetActive(false);
        
        if (GameManager.Instance.IsGameOver())
        {
            SoundManager.Instance.PlayFinishSound();
            gameOver.SetActive(true);
            gameOverText.text = GameManager.Instance.GetScore().ToString();
            gameTimeText.text = ConvertTimeToString(GameManager.Instance.GetGameTime());
        }
    }
    
    public void LoadSceneGame()
    {
        StartCoroutine(RiseObjectsAndLoad());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    /// <summary>
    /// Fonction qui fait l'inverse qu'au lancement.
    /// Elle permet de faire tomber les objets avant de charger la scène de jeu.
    /// J'ai fait ça car je trouvais ça stylé. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RiseObjectsAndLoad()
    {
        SoundManager.Instance.PlayClickSound();

        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            canvas.enabled = false;
        }
        
        Transform[] allTransforms = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);

        // Pour un effet plus esthétique, on trie les objets par position Y croissante
        // Comme ca les éléments du bas montent en premier
        var sortedTransforms = allTransforms
            .Where(t => t.gameObject != this.gameObject && t.GetComponent<Camera>() == null)
            .OrderBy(t => t.position.y);

        float longestDelay = 0f;

        foreach (Transform t in sortedTransforms)
        {
            float randomDelay = Random.Range(0f, maxRandomDelay);
            Vector3 start = t.position;
            Vector3 end = new Vector3(start.x, -finalHeight, start.z);

            LMotion.Create(start, end, durationPerObject)
                .WithEase(easeType)
                .WithDelay(randomDelay)
                .BindToPosition(t);

            if (randomDelay > longestDelay) longestDelay = randomDelay;
        }

        yield return new WaitForSeconds(durationPerObject + longestDelay);
        gameOver.SetActive(false);
        SceneManager.LoadScene(sceneToLoad);
    }
    
    private string ConvertTimeToString(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:D2}:{seconds:D2}";
    }
    
    /// <summary>
    /// Passage en mode pause / dé-pause du menu des paramètres.
    /// </summary>
    public void Settings()
    {
        _isSettingsOpen = !_isSettingsOpen;
        settingsMenuParent.SetActive(_isSettingsOpen);
        
        float bestTimeInPlayerPrefs = PlayerPrefs.GetFloat("BestTime", 0);
        bestTime.text = ConvertTimeToString(bestTimeInPlayerPrefs);
        
        foreach (GameObject ui in otherUIIsToDisable)
        {
            if (ui != null)
            {
                ui.SetActive(!_isSettingsOpen);
            }
        }
        
        if (GameManager.Instance.IsGameOver())
        {
            gameOver.SetActive(!_isSettingsOpen);
        }
    }
}