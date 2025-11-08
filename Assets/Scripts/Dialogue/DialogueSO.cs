using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vampear/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public string Context;
    public List<DialogueLine> Dialogue;
}
