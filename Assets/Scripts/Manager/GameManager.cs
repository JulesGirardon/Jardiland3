using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LitMotion;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private GameObject _activeGroundPlantation;
    
    private List<PlantationData> _plantedGroundPlantations;
    private List<GameObject> _wateredGroundPlantations;

    private IPlantation _activeIPlantation;
    
    [Header("Plantations")]
    [SerializeField] 
    private List<IPlantation> availablePlantations;
    public List<IPlantation> AvailablePlantations => availablePlantations;
    
    private bool _wateringInProgress = false;
    
    [Tooltip("Parent object for inventory items")]
    public GameObject inventoryParent;
    private GameObject _inventory;
    
    [Header("Watering Settings")]
    [Tooltip("Watered ground plantation object prefab")]
    public GameObject groundPlantationWatered;
    
    [Tooltip("Unwatered ground plantation object prefab")]
    public GameObject groundPlantationUnwatered;
    
    [Tooltip("Parent object for garden elements")]
    public GameObject parentGarden;
    
    [Header("Score Settings")]
    [Tooltip("Score Text UI element")]
    public TextMeshProUGUI scoreText;
    
    private int _score = 0;
    
    [Tooltip("Maximum score achievable")]
    public int maxScore = 10;

    private bool _gameIsOver = false;
    
    [Header("Input Actions")]
    [Tooltip("Input Action Reference for changing active plantation")]
    public InputActionReference changeActivePlantationActionRef;
    
    [Header("Plantation Unlock Settings")]
    [Tooltip("Number of steps required to deblock a new plantation")]
    public int scoreStepToDeblockNewPlantation = 10;
    private int _numberOfDeblockedPlantations = 1;
    
    private float gameTime = 0f;
    
    private void OnEnable()
    {
        changeActivePlantationActionRef.action.performed += OnChangeActivePlantationPerformed;
    }

    private void OnDisable()
    {
        changeActivePlantationActionRef.action.performed -= OnChangeActivePlantationPerformed;
    }
    
    private void OnChangeActivePlantationPerformed(InputAction.CallbackContext obj)
    {
        // Permet de changer la plantation active.
        // On change uniquement sur les graines débloquer
        if (_score >= scoreStepToDeblockNewPlantation * (_numberOfDeblockedPlantations - 1))
        {
            int currentIndex = availablePlantations.IndexOf(_activeIPlantation);
            int nextIndex = (currentIndex + 1) % _numberOfDeblockedPlantations;
            _activeIPlantation = availablePlantations[nextIndex];

            InstantiateInventoryItem();
            SoundManager.Instance.PlayChangeInventorySound();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Ces deux listes permettent d'éviter de réarroser les plantations déjà arrosées
        // ou de replanter sur des emplacements déjà plantés.
        _plantedGroundPlantations = new List<PlantationData>();
        _wateredGroundPlantations = new List<GameObject>();
        
        if (availablePlantations.Count > 0)
        {
            _activeIPlantation = availablePlantations[0];
            InstantiateInventoryItem();
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    public void SetActiveGroundPlantation(GameObject target)
    {
        _activeGroundPlantation = target;
    }
    
    public GameObject GetActiveGroundPlantation()
    {
        return _activeGroundPlantation;
    }
    
    public void DestroyActiveGroundPlantation()
    {
        if (_activeGroundPlantation)
        {
            DestroyActiveGroundPlantation();
        }
    }
    
    public void AddPlantedGroundPlantation(PlantationData plantationData)
    {
        _plantedGroundPlantations.Add(plantationData);
    }

    public List<PlantationData> GetPlantedGroundPlantations()
    {
        return _plantedGroundPlantations;
    }

    public void RemovePlantedGroundPlantation(PlantationData plantationData)
    {
        _plantedGroundPlantations.Remove(plantationData); 
    }
    
    public void AddWateredGroundPlantation(GameObject groundPlantation)
    {
        _wateredGroundPlantations.Add(groundPlantation);
    }
    
    public List<GameObject> GetWateredGroundPlantations()
    {
        return _wateredGroundPlantations;
    }
    
    public void RemoveWateredGroundPlantation(GameObject groundPlantation)
    {
        _wateredGroundPlantations.Remove(groundPlantation); 
    }
    
    public void SetActivePlantation(IPlantation iPlantation)
    {
        _activeIPlantation = iPlantation;
    }
    
    public IPlantation GetActivePlantation()
    {
        return _activeIPlantation;
    }
    
    public bool IsWateringInProgress()
    {
        return _wateringInProgress;
    }
    
    public void SetWateringInProgress(bool inProgress)
    {
        _wateringInProgress = inProgress;
    }
    
    private void UpdateScoreText()
    {
        // Mise a jour du texte quand on récolte une plantation
        if (scoreText)
        {
            LMotion
                .Create(int.Parse(scoreText.text), _score, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(x => scoreText.text = Mathf.RoundToInt(x).ToString());
        }
    }
    
    public void IncreaseScore(int amount)
    {
        _score += amount;
        if (_score + amount >= scoreStepToDeblockNewPlantation * _numberOfDeblockedPlantations && 
            _numberOfDeblockedPlantations < availablePlantations.Count)
        {
            _numberOfDeblockedPlantations++;
        }
        
        UpdateScoreText();

        if (_score >= maxScore)
        {
            GameIsOver();
        }
    }

    public bool IsGameOver()
    {
        return _gameIsOver;
    }
    
    private void GameIsOver()
    {
        _gameIsOver = true;
        
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (gameTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", gameTime);
        }
        
        SceneManager.LoadScene("Menu");
    }
    
    public int GetScore()
    {
        return _score;
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }

    private void InstantiateInventoryItem()
    {
        if (_inventory)
        {
            Destroy(_inventory);
        }
        _inventory = Instantiate(_activeIPlantation.inventoryItemPrefab, inventoryParent.transform);
        // Rotation par défaut car le panneau n'est pas bien tourné
        _inventory.transform.Rotate(new Vector3(0, -140f, 0));
        _inventory.transform.localScale = new Vector3(3f, 3f, 3f);
    }


}
