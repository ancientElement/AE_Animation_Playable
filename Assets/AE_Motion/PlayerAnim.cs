using AE_Animation_Playable;
using AE_Motion.Anim;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Collections;
using System;

namespace AE_Motion
{
    public class PlayerAnim : MonoBehaviour
    {
        private AEAnimController m_animController;
        public AnimationClip[] idleClips;
        public BlendClip1D[] moveClips;
        public BlendClip2D[] moveClip2Ds;
        public AnimationClip runStopClip;
        public AnimationClip walkStopClip;
        public AnimationClip runTurn180Clip;

        public void Init(PlayerMotion playerMotion)
        {
            m_animController = new AEAnimController(this, playerMotion.Animator, playerMotion.Modle, playerMotion.Body);

            //添加状态
            var m_idleAnim = new IdleAnim(m_animController.Graph, idleClips, 5f, 0.5f);
            m_animController.AddState("Idle", m_idleAnim);

            m_animController.AddBlendTree1D("MoveBlendTree", moveClips, 0.1f);
            m_animController.AddAnimator("Animator", 0.2f);
            m_animController.AddBlendTree2D("MoveBlendTree2D", moveClip2Ds, 0.1f);

            m_animController.AddAnimUnit("RunStop", runStopClip, 0.2f);
            m_animController.AddAnimUnit("RunTurn180", runTurn180Clip, 0.2f);

            m_animController.Start();
        }

        private void OnAnimatorMove() {
            m_animController.OnAnimatorMove();
        }

        private void OnDestroy() {
            m_animController.Stop();
        }
    
        public void ApplayRootMotion()
        {
            m_animController.ApplayRootMotion();
        }
        public void PreventRootMotion()
        {
            m_animController.PreventRootMotion();
        }
        
        public void TransitionTo(string name, Action callback = null, float enterTIme = -1f, float animationClipLength = -1f)
        {
            m_animController.TransitionTo(name, callback, enterTIme, animationClipLength);
        }
        public void AnimatorCroosFade(string stateName, float transitionDuration, Action callback = null)
        {
            m_animController.AnimatorCroosFade(stateName, transitionDuration, callback);
        }
        public void BlendTree1DSetValue(string name, float value)
        {
            m_animController.BlendTree1DSetValue(name, value);
        }
    }
}