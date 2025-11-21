using System.Collections;
using System.Linq;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class RiseObjectOnStart : MonoBehaviour
{
    [Header("Rise Animation Settings")]
    [Tooltip("Starting Y offset for the rise animation.")]
    public float startYOffset = -50f;
    
    [Tooltip("Duration of the rise animation per object.")]
    public float durationPerObject = 2f;  
    
    [Tooltip("Maximum random delay before the rise animation starts.")]
    public float maxRandomDelay = 1f;     
    
    [Tooltip("Easing type for the rise animation.")]
    public Ease easeType = Ease.OutBounce;

    private Canvas[] _canvas;
    
    void Start()
    {
        _canvas = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        SetCanvasEnabled(false);
        StartCoroutine(HandleRiseAndDestroy());
    }

    private IEnumerator HandleRiseAndDestroy()
    {
        yield return StartCoroutine(RiseObjects());

        SetCanvasEnabled(true);

        Destroy(gameObject);
    }
    
    private IEnumerator RiseObjects()
    {
        // Récupération de tous les objets de la scène
        Transform[] allTransforms = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);

        // Exclusion de la caméra car je ne veux pas qu'elle bouge
        var filteredTransforms = allTransforms
            .Where(t => t.gameObject != this.gameObject && t.GetComponent<Camera>() == null);

        float longestDelay = 0f;

        foreach (Transform t in filteredTransforms)
        {
            Vector3 initialLocalPos = t.localPosition;
            Vector3 startLocalPos = new Vector3(initialLocalPos.x, initialLocalPos.y + startYOffset, initialLocalPos.z);

            t.localPosition = startLocalPos;

            float randomDelay = Random.Range(0f, maxRandomDelay);
            if (randomDelay > longestDelay) longestDelay = randomDelay;

            LMotion.Create(startLocalPos, initialLocalPos, durationPerObject)
                .WithEase(easeType)
                .WithDelay(randomDelay)
                .BindToLocalPosition(t);
        }

        // On attends la fin du dernier mouvement pour continuer
        yield return new WaitForSeconds(durationPerObject + longestDelay);
    }
    
    private void SetCanvasEnabled(bool enabled)
    {
        foreach (Canvas c in _canvas)
        {
            c.enabled = enabled;
        }
    }
}
