using System;
using UnityEngine;

public class PlayerPointerController : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Pointer object to indicate ground position")]
    public GameObject playerPointer;
    
    [Tooltip("Speed at which the player pointer rotates")]
    public float playerPointerRotationSpeed = 50f;
    
    private GameObject _playerPointerInstance;
    
    void OnTriggerEnter(Collider other)
    {
        // On lance une interaction si on entre en collision avec une plantation
        if (other.CompareTag(Constants.GroundPlantationTag) || other.CompareTag(Constants.WateredGroundPlantationTag))
        {
            GameManager.Instance.SetActiveGroundPlantation(other.gameObject);
            
            GrowthController growthController = other.gameObject.GetComponent<GrowthController>();
            if (growthController)
            {
                growthController.IsReadyToRecolt();
            }
            
            MovePlayerPointer(other.gameObject);
        }
        else
        {
            DeletePlayerPointer();
        }
    }

    void Update()
    {
        if (_playerPointerInstance)
        {
            _playerPointerInstance.transform.Rotate(Vector3.up, playerPointerRotationSpeed * Time.deltaTime);
        }
    }
    
    private void MovePlayerPointer(GameObject target)
    {
        // Le pointeur permet de visualiser quelle plantation est active
        // C'est a dire sur quelle plantation le joueur va interagir
        if (_playerPointerInstance == null) {
            _playerPointerInstance = Instantiate(playerPointer);
        }
        
        Vector3 position = target.transform.position;
        _playerPointerInstance.transform.position = new Vector3(position.x, Constants.AdditionalPointerHeight, position.z);
    }
    
    private void DeletePlayerPointer()
    {
        Destroy(_playerPointerInstance);
    }
}
