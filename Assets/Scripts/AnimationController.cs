using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    
    public SkeletonAnimation      Controller;

    public List<AnimationReferenceAsset> Animations;

    private string currentAnimation;

    void Start()
    {
        
    }
    

    public void SetState(string animationName, bool loop = false)
    {
        if (animationName.Equals(currentAnimation))
        {
            return;
        }
        
        Controller.AnimationState.SetAnimation(0, animationName, loop);

        currentAnimation = animationName;
    }

    public void SetTrigger(string animationName)
    {
        if (animationName.Equals(currentAnimation))
        {
            return;
        }
        Controller.AnimationState.SetAnimation(0, animationName, false);
        currentAnimation = animationName;
    }



}
