using System;
using System.Collections.Generic;
using UnityEngine;


public enum DialogueType
{
    VisualNovelStyle,
    ShortDSStyle
}

[CreateAssetMenu(menuName = "Vampear/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public string Context;
    public DialogueType DialogueType;
    public List<DialogueLine> Dialogue;
}
