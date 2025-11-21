using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Panel Animator")]
    [Tooltip("Animator component of the panel to animate on hover.")]
    public Animator panelAnimator;
    
    [Tooltip("Name of the animation to play on hover.")]
    public string hoverAnimationName = "HoverAnimation";

    [Tooltip("Name of the idle animation.")]
    public string idleAnimationName = "Idle";

    [Header("Button Text")]
    [Tooltip("Text component of the button.")]
    public TextMeshProUGUI text;
    
    [Tooltip("Initial color of the text.")]
    public Color initialColor = Color.white;
    
    [Tooltip("Color of the text on hover.")]
    public Color hoverColor = Color.grey;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text) text.color = hoverColor;
        
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlayHoverSound();
        }

        if (panelAnimator && !string.IsNullOrEmpty(hoverAnimationName))
        {
            panelAnimator.Play(hoverAnimationName, 0, 0f);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (text) text.color = initialColor;
        
        if (panelAnimator && !string.IsNullOrEmpty(idleAnimationName))
        {
            panelAnimator.Play(idleAnimationName, 0, 0f);
        }
    }
}