using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlantationController : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Input action reference for plantation controller")]
    public InputActionReference plantationInputActionRef;
    
    [Header("Plantation Settings")]
    [Tooltip("Ground plantation object prefab")]
    public GameObject seedPrefab;
    
    public void OnEnable()
    {
        plantationInputActionRef.action.performed += OnPlantationPerformed;
    }
    
    public void OnDisable()
    {
        plantationInputActionRef.action.performed -= OnPlantationPerformed;
    }
    
    private void OnPlantationPerformed(InputAction.CallbackContext context)
    {
        GameObject activeGroundPlantation = GameManager.Instance.GetActiveGroundPlantation();
        
        // On vérifie que la plantation active est valide et qu'elle n'a pas déjà été plantée
        if (activeGroundPlantation 
            && activeGroundPlantation.CompareTag(Constants.GroundPlantationTag))
        {
            foreach (PlantationData plantation in GameManager.Instance.GetPlantedGroundPlantations())
            {
                if (plantation.GroundPlantation == activeGroundPlantation)
                {
                    return;
                }
            }
            
            Vector3 position = activeGroundPlantation.transform.position;

            GameObject seedPrefabInstance = Instantiate(seedPrefab,
                new Vector3(position.x, position.y + Constants.AdditionalGrowthHeight, position.z), Quaternion.identity);
            seedPrefabInstance.name = seedPrefab.name;
            seedPrefabInstance.transform.SetParent(activeGroundPlantation.transform);
            
            // PlantationData permet de stocker les informations sur la plantation
            // Comme le type de plantation car la graine est un prefab générique utilisé pour toutes les plantations
            // On crée donc une instance de PlantationData pour stocker ces informations
            PlantationData plantationData = new PlantationData(
                seedPrefabInstance,
                GameManager.Instance.GetActivePlantation(),
                activeGroundPlantation
            );
            
            GameManager.Instance.AddPlantedGroundPlantation(plantationData);
            SoundManager.Instance.PlayPlantSound();
        }
    }
}
