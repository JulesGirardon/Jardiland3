using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Parameters")]
    
    [Tooltip("Movement speed of the player")]
    [Range(0f, 10f)]
    public float moveSpeed = 1f;
    
    [Header("Input Action References")]
    
    [Tooltip("Reference to the Input Action for movement")]
    public InputActionReference moveActionRef;

    private void OnEnable()
    {
        moveActionRef.action.Enable();
    }
    
    private void Update()
    {
        if (GameManager.Instance.IsWateringInProgress()) return;
        
        Vector2 stickDirection = moveActionRef.action.ReadValue<Vector2>();
        Vector3 droneDirection = new Vector3(stickDirection.x * moveSpeed, 0, stickDirection.y * moveSpeed);
        
        transform.Translate(droneDirection * Time.deltaTime);
    }
}
