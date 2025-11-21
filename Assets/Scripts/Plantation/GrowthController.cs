using System.Collections;
using UnityEngine;

public class GrowthController : MonoBehaviour
{
    private PlantationData _plantationData;
    private int _currentStep;
    private GameObject _currentInstance;

    private Animator _animator;
    private bool _isReadyToRecolt = false;
    private bool _explosionEnded = false;

    public void SetPlantationData(PlantationData plantationData)
    {
        _plantationData = plantationData;
    }

    public void LaunchGrowth()
    {
        StartCoroutine(Growth());
    }

    private IEnumerator Growth()
    {
        _currentStep = 0;

        // On lance une boucle sur chaque étape de croissance de la plantation
        while (_currentStep < _plantationData.Plantation.prefabs.Count)
        {
            // On attends la durée de croissance pour chaque étape
            yield return new WaitForSeconds(_plantationData.Plantation.growthDurationForSteps);

            // On instancie le prefab correspondant à l'étape de croissance actuelle
            Vector3 spawnPosition = _plantationData.GroundPlantation.transform.position + new Vector3(0, Constants.AdditionalGrowthHeight, 0);

            // On détruit l'instance précédente si elle existe
            // car a la première étape, il n'y a pas d'instance précédente
            if (_currentInstance)
                Destroy(_currentInstance);

            GameObject prefab = _plantationData.Plantation.prefabs[_currentStep];
            _currentInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);
            // On parent l'instance au sol de la plantation
            _currentInstance.transform.SetParent(_plantationData.GroundPlantation.transform);

            // On détruit la graine uniquement à la première étape
            if (_currentStep == 0 && _plantationData.Seed)
                Destroy(_plantationData.Seed);

            _currentStep++;
        }

        _isReadyToRecolt = true;
        SoundManager.Instance.PlayPopSound();
        _animator = _currentInstance.GetComponent<Animator>();

        yield return null;
        
        _animator.Play(_plantationData.Plantation.livingAnimationClip.name, 0, 0f);

        StartCoroutine(WaitBeforeExplosion());
    }

    private IEnumerator WaitBeforeExplosion()
    {
        yield return new WaitForSeconds(_plantationData.Plantation.timeBeforeExplosion);

        if (!_explosionEnded)
        {
            if (_animator)
            {
                _animator.Play(_plantationData.Plantation.explosionAnimationClip.name, 0, 0f);
                float clipLength = _plantationData.Plantation.explosionAnimationClip.length;
                yield return new WaitForSeconds(clipLength);
            }

            SoundManager.Instance.PlayExplosionSound();
            OnExplosionEnd();
        }
    }

    public void IsReadyToRecolt()
    {
        if (_isReadyToRecolt && !_explosionEnded)
        {
            GameManager.Instance.IncreaseScore(_plantationData.Plantation.scoreValue);
            SoundManager.Instance.PlayRecoltingSound();
            ResetGroundPlantation();
        }
    }

    public void OnExplosionEnd()
    {
        if (!_explosionEnded)
        {
            _explosionEnded = true;
            ResetGroundPlantation();
        }
    }

    private void ResetGroundPlantation()
    {
        GameManager.Instance.RemoveWateredGroundPlantation(_plantationData.GroundPlantation);

        if (_currentInstance)
            Destroy(_currentInstance);

        GameObject newGround = Instantiate(GameManager.Instance.groundPlantationUnwatered, _plantationData.GroundPlantation.transform.position, Quaternion.identity);
        newGround.transform.SetParent(GameManager.Instance.parentGarden.transform);

        Destroy(_plantationData.GroundPlantation);
        Destroy(this);
    }
}
