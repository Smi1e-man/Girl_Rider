using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    
    public Animator animator;
    //public AnimDudeBoom animDudeBoom;

    public enum DudeAnimation { girl_idle , girl_walk , girl_sit , girl_lie , girl_fall ,
                                RandomManIdle , man_idle_1, man_idle_2 , man_idle_3 , man_walk_bring , man_run_bring , man_fall , man_walk}

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(DudeAnimation anim, float speed = 1)
    {
        switch (anim)
        {
            case DudeAnimation.RandomManIdle:
                List<DudeAnimation> animList = new List<DudeAnimation>() { DudeAnimation.man_idle_1, DudeAnimation.man_idle_2, DudeAnimation.man_idle_3};
                PlayAnimation(animList[Random.Range(0, animList.Count)].ToString(), speed);
                break;

            default:
                PlayAnimation(anim.ToString(), speed);
                break;
        }
    }

    private void PlayAnimation(string s, float speed = 1)
    {
        animator.SetTrigger(s);
        //animator.speed = speed;
    }

    //public void RunDeathAnimation(TweenCallback callback = null)
    //{
    //    //animDudeBoom.RunAnimation(0, callback);
    //    animDudeBoom.Play();
    //}
    
}
