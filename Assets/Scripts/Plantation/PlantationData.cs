using UnityEngine;

public class PlantationData
{
    // On enregistre la graine car on va la supprimer du sol
    // une fois que la première partie de l'arrosage est faite
    public GameObject Seed; 
    
    // On garde une référence à la plantation pour accéder à ses données
    public IPlantation Plantation;
    
    // On garde une référence à l'objet de la plantation
    // car on peut planter une graine et après la retirer du sol quand on arrose
    public GameObject GroundPlantation;

    public PlantationData(GameObject seed, IPlantation plantation, GameObject groundPlantation)
    {
        Seed = seed;
        Plantation = plantation;
        GroundPlantation = groundPlantation;
    }
}