using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    /*
    public Animator animator;
    public AnimDudeBoom animDudeBoom;

    public enum DudeAnimation { RandomIdle, idle, idle_bored, idle_sad, walk, run, fear, RandomDance, dance_1, dance_2, dance_3, dance_4, dance_5, dance_6, RandomSit, sit_1, sit_2, laying_1, laying_2, finish_bow, RandomMutantIdle, mutant_idle_1, mutant_idle_2, mutant_idle_3, mutant_idle_4}

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(DudeAnimation anim, float speed = 1)
    {
        switch (anim)
        {
            case DudeAnimation.RandomIdle:
                List<DudeAnimation> animList = new List<DudeAnimation>() { DudeAnimation.idle, DudeAnimation.idle_bored, DudeAnimation.idle_sad};
                PlayAnimation(animList[Random.Range(0, animList.Count)].ToString(), speed);
                break;

            case DudeAnimation.RandomDance:
                List<DudeAnimation> animList2 = new List<DudeAnimation>() { DudeAnimation.dance_1, DudeAnimation.dance_2, DudeAnimation.dance_3, DudeAnimation.dance_4, DudeAnimation.dance_5, DudeAnimation.dance_6 };
                PlayAnimation(animList2[Random.Range(0, animList2.Count)].ToString(), speed);
                break;

            case DudeAnimation.RandomSit:
                List<DudeAnimation> animList3 = new List<DudeAnimation>() { DudeAnimation.sit_1, DudeAnimation.sit_2};
                PlayAnimation(animList3[Random.Range(0, animList3.Count)].ToString(), speed);
                break;

            case DudeAnimation.RandomMutantIdle:
                List<DudeAnimation> animList4 = new List<DudeAnimation>() { DudeAnimation.mutant_idle_1, DudeAnimation.mutant_idle_2, DudeAnimation.mutant_idle_3, DudeAnimation.mutant_idle_4};
                PlayAnimation(animList4[Random.Range(0, animList4.Count)].ToString(), speed);
                break;

            default:
                PlayAnimation(anim.ToString(), speed);
                break;
        }
    }

    private void PlayAnimation(string s, float speed = 1)
    {
        animator.SetTrigger(s);
        animator.speed = speed;
    }

    public void RunDeathAnimation(TweenCallback callback = null)
    {
        //animDudeBoom.RunAnimation(0, callback);
        animDudeBoom.Play();
    }
    */
}
