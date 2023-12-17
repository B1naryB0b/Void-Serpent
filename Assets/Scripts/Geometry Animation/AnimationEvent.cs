using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : ScriptableObject
{
    [SerializeField] private bool loopAnimation;

    AnimationTrigger trigger;

    public List<AnimationElement> animationElements;



    
}
