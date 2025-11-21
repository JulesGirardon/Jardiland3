using System.Collections.Generic;
using UnityEngine;

// Cette classe abstraite définit les propriétés communes à toutes les plantations dans le jeu.
// Cela permet d'ajouter facilement de nouveaux types de plantations en créant des classes dérivées.
public abstract class IPlantation : ScriptableObject
{
    [Header("Growth Prefabs")]
    [Tooltip("List of prefabs representing each growth stage")]
    public List<GameObject> prefabs; // Liste des préfabriqués représentant chaque étape de croissance.
    
    [Header("Growth Duration")]
    [Tooltip("Duration in seconds for each growth step")]
    public float growthDurationForSteps = 10f; // Durée en secondes pour chaque étape de croissance.
    
    [Header("Explosion Settings")]
    [Tooltip("Indicates if the plantation explodes after growth")]
    public float timeBeforeExplosion = 5f; // Temps avant l'explosion après la croissance complète.
    
    [Header("Score Value")]
    [Tooltip("Score awarded upon successful harvest")]
    public int scoreValue = 10;
    
    [Header("Animation")]
    [Tooltip("Animation clip for living state")]
    public AnimationClip livingAnimationClip;
    [Tooltip("Animation clip for explosion effect")]
    public AnimationClip explosionAnimationClip;
    
    [Header("Inventory Item")]
    [Tooltip("GameObject show in bottom of the screen")]
    public GameObject inventoryItemPrefab; // Objet représentant l'élément dans l'inventaire. C'est le panneau dans la scène
}