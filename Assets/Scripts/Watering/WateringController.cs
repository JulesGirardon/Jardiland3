using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WateringController : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Input action reference for watering controller")]
    public InputActionReference wateringInputActionRef;
    
    [Tooltip("Particle system for watered ground plantation effect")]
    public ParticleSystem wateredGroundPlantation;
    
    private GameObject _activeGroundPlantation;
    
    void OnEnable()
    {
        wateringInputActionRef.action.performed += OnWateringPerformed;
    }
    
    void OnDisable()
    {
        wateringInputActionRef.action.performed -= OnWateringPerformed;
    }
    
    private void OnWateringPerformed(InputAction.CallbackContext context)
    {
        GameObject activeGroundPlantation = GameManager.Instance.GetActiveGroundPlantation();
        
        List<PlantationData> plantations = GameManager.Instance.GetPlantedGroundPlantations();
        
        // Avant d'arroser, on verifie que la plantation active est bien une plantation au sol
        if (activeGroundPlantation 
            && activeGroundPlantation.CompareTag(Constants.GroundPlantationTag)
            )
        {
            foreach (PlantationData plantation in plantations)
            {
                if (plantation.GroundPlantation == activeGroundPlantation)
                {
                    _activeGroundPlantation = activeGroundPlantation;
                    StartCoroutine(WateringSequence(plantation));
                }
            }
        }
    }
    
    private IEnumerator WateringSequence(PlantationData plantationData)
    {
        // On verifie qu'aucun arrosage n'est en cours pour éviter de lancer des arrossages multiples
        if (GameManager.Instance.IsWateringInProgress()) yield break;
        GameManager.Instance.SetWateringInProgress(true);
        
        Vector3 position = plantationData.GroundPlantation.transform.position;

        PlayWateredGroundPlantationEffect();
        SoundManager.Instance.PlayWaterSound();
        
        // On attends la fin de l'animation d'arrosage avant de remplacer la plantation par une plantation arrosée
        yield return new WaitForSeconds(wateredGroundPlantation.main.duration);
        
        GameObject wateredGroundPlantationInstance = 
            Instantiate(GameManager.Instance.groundPlantationWatered, new Vector3(position.x, position.y, position.z), Quaternion.identity);
        wateredGroundPlantationInstance.transform.SetParent(GameManager.Instance.parentGarden.transform);
        
        // On remplace la plantation au sol par une plantation au sol arrosée
        GameManager.Instance.RemoveWateredGroundPlantation(_activeGroundPlantation);
        Destroy(_activeGroundPlantation);
        GameManager.Instance.SetActiveGroundPlantation(wateredGroundPlantationInstance);

        plantationData.GroundPlantation = wateredGroundPlantationInstance;
        
        // On ajoute la nouvelle plantation arrosée à la liste des plantations arrosées
        GameManager.Instance.AddWateredGroundPlantation(wateredGroundPlantationInstance);
        
        plantationData.Seed.transform.SetParent(wateredGroundPlantationInstance.transform);
        GrowthController growthController = wateredGroundPlantationInstance.AddComponent<GrowthController>();
        
        // On change la plantation de la plantationData pour la nouvelle plantation arrosée
        growthController.SetPlantationData(plantationData);
        growthController.LaunchGrowth();

        GameManager.Instance.SetWateringInProgress(false);
    }
    
    private void PlayWateredGroundPlantationEffect()
    {
        wateredGroundPlantation.Play();
    }
}
