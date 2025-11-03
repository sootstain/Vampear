using System;
using UnityEngine;

[Serializable]
public class Attack
{
    //Just for now, dumb method
    [field: SerializeField] public string AnimationName { get; private set; }
    [field: SerializeField] public float TransitionDuration { get; private set; }
    [field: SerializeField] public int ComboIndex { get; private set; } = -1;
    [field: SerializeField] public float ComboAttackTime { get; private set; } //to start playing next combo anim


}
