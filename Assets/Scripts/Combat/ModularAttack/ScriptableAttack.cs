using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Vampear/Attack")]
public class ScriptableAttack : ScriptableObject
{
    public string AnimationName; //could maybe put animation instead, but think this might be better for performance..
    public float TransitionDuration;
    public int ComboIndex;
    public float ForceTime;
    public float ForceStrength;
    public float Damage;
    public float Knockback;
    public VisualEffect VFX;

    void OnEnable()
    {
        VFX.Play();
    }
}


