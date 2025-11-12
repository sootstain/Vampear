using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class Attack
{
    //Just for now, dumb method
    [field: SerializeField] public string AnimationName { get; private set; }
    [field: SerializeField] public float TransitionDuration { get; private set; }
    [field: SerializeField] public int ComboIndex { get; private set; } = -1;
    [field: SerializeField] public float ComboAttackTime { get; private set; } //to start playing next combo anim

    [field: SerializeField] public float ForceTime { get; private set; }
    
    [field: SerializeField] public float ForceStrength { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Knockback { get; private set; }
    [field: SerializeField] public List<ParticleSystem> VisualEffect { get; private set; }

    public void PlayVFX()
    {
        foreach (var x in VisualEffect)
        {
            x.Play();
        }
    }
    
    public void StopVFX()
    {
        foreach (var x in VisualEffect)
        {
            x.Stop();
        }
    }
}
