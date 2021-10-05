using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public Animator animator;

    public bool isStartAnim;
    public DudeAnimation startAnim;

    public bool isDead;

    public enum DudeAnimation
    {
        girl_idle, girl_walk, girl_sit, girl_lie, girl_fall,
        girl_finish, girl_finish_fail, girl_finish_victory,
        man_idle_1, man_idle_2, man_idle_3,
        man_walk_bring, man_run_bring, man_fall, man_walk,
        man_idle_sit, man_finish_victory, man_finish_fail, man_run,
        girl_walk_20, girl_walk_40, girl_walk_70, girl_walk_100,
        kiss, girl_enemy_walk, girl_enemy_throw, girl_stumble,
        girl_enemy_attack, girl_stairs_up, girl_pipe, girl_jump, girl_spin, flying
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (isStartAnim)
            PlayAnimation(startAnim);
    }

    public void PlayAnimation(DudeAnimation anim, float speed = 1)
    {
        PlayAnimation(anim.ToString(), speed);
    }

    private void PlayAnimation(string s, float speed = 1)
    {
        if (isDead) return;

        animator.SetTrigger(s);
        animator.speed = speed;

        //Debug.Log(s);
    }

}
