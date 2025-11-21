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
    
    /// <summary>
    /// Setter pour l'objet de la plantation au sol active.
    /// </summary>
    /// <param name="target">La plantation à mettre en active</param>
    public void SetActiveGroundPlantation(GameObject target)
    {
        _activeGroundPlantation = target;
    }
    
    /// <summary>
    /// Getter pour l'objet de la plantation au sol active.
    /// </summary>
    /// <returns></returns>
    public GameObject GetActiveGroundPlantation()
    {
        return _activeGroundPlantation;
    }
    
    /// <summary>
    /// Fonction qui ajoute un sol planté à la liste des plantations plantées.
    /// </summary>
    /// <param name="plantationData">Les informations complètes du sol (graines, types de cultures...)</param>
    public void AddPlantedGroundPlantation(PlantationData plantationData)
    {
        _plantedGroundPlantations.Add(plantationData);
    }

    /// <summary>
    /// Getter pour la liste des plantations plantées.
    /// </summary>
    /// <returns>La liste</returns>
    public List<PlantationData> GetPlantedGroundPlantations()
    {
        return _plantedGroundPlantations;
    }
    
    /// <summary>
    /// Fonction qui ajoute un sol arrosé à la liste des plantations arrosées.
    /// </summary>
    /// <param name="groundPlantation">Le sol a ajouter</param>
    public void AddWateredGroundPlantation(GameObject groundPlantation)
    {
        _wateredGroundPlantations.Add(groundPlantation);
    }
    
    /// <summary>
    /// Supprime un sol arrosé de la liste des plantations arrosées.
    /// </summary>
    /// <param name="groundPlantation">Le sol a supprimer</param>
    public void RemoveWateredGroundPlantation(GameObject groundPlantation)
    {
        _wateredGroundPlantations.Remove(groundPlantation); 
    }
    
    /// <summary>
    /// Fonction qui renvoie la plantation active
    /// </summary>
    /// <returns>Les informations de la plantation</returns>
    public IPlantation GetActivePlantation()
    {
        return _activeIPlantation;
    }
    
    /// <summary>
    /// Fonction qui indique si un arrosage est en cours
    /// Cette fonction est appelé pour éviter d'arroser plusieurs plantations en même temps
    /// Elle est appelé dans le WateringController
    /// </summary>
    /// <returns>Le booléan qui indique si un arrosage est en cours</returns>
    public bool IsWateringInProgress()
    {
        return _wateringInProgress;
    }

    /// <summary>
    /// Fonction qui définit si un arrosage est en cours
    /// </summary>
    /// <param name="inProgress">Le booléan qui indique si l'arrosage est en cours</param>
    public void SetWateringInProgress(bool inProgress)
    {
        _wateringInProgress = inProgress;
    }
    
    /// <summary>
    /// Met à jour le texte du score affiché à l'écran.
    /// </summary>
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
    
    /// <summary>
    /// Augmente le score du joueur.
    /// </summary>
    /// <param name="amount">Le score a ajouter</param>
    public void IncreaseScore(int amount)
    {
        _score += amount;
        
        // Débloque une nouvelle plantation si le score le permet
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

    /// <summary>
    /// Permet de savoir si le jeu est terminé.
    /// Utilisé dans le retour au menu principal pour afficher le temps final.
    /// </summary>
    /// <returns>Le booléan indiquant si le jeu est fini</returns>
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
    
    /// <summary>
    /// Récupère le score actuel.
    /// </summary>
    /// <returns>Le score</returns>
    public int GetScore()
    {
        return _score;
    }
    
    /// <summary>
    /// Récupère le temps de jeu écoulé.
    /// </summary>
    /// <returns>Le temps de jeu</returns>
    public float GetGameTime()
    {
        return gameTime;
    }

    /// <summary>
    /// Est utilisé pour changer le panneau qui est dans la scène du jeu.
    /// Ce panneau indique la graine actuellement sélectionnée.
    /// Pourquoi dans le jeu ? Parce que je trouvais ça plus jolie
    /// </summary>
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
