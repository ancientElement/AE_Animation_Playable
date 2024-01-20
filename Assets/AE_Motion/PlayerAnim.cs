using AE_Animation_Playable;
using UnityEngine;
using System;

namespace AE_Motion
{
    public class PlayerAnim : MonoBehaviour
    {
        private AEAnimController m_animController;

        public SingleAnimClipSO[] singleAnimClips;
        public BlendTree1DSO[] blendTree1Ds;
        public BlendTree2DSO[] blendTree2Ds;
        public CustomAnimBehaviourSO[] customAnimBehaviours;

        public void Init(PlayerMotion playerMotion)
        {
            m_animController = new AEAnimController(this, playerMotion.Animator, playerMotion.Modle, playerMotion.CharactorController);

            for (int i = 0; i < singleAnimClips?.Length; i++)
            {
                m_animController.AddAnimUnit(singleAnimClips[i]?.name, singleAnimClips[i]?.AnimationClip, 0.2f);
            }
            for (int i = 0; i < blendTree1Ds?.Length; i++)
            {
                m_animController.AddBlendTree1D(blendTree1Ds[i]?.name, blendTree1Ds[i]?.BlendClip1DClips, 0.1f);
            }
            for (int i = 0; i < blendTree2Ds?.Length; i++)
            {
                m_animController.AddBlendTree2D(blendTree2Ds[i]?.name, blendTree2Ds[i]?.BlendClip2DClips, 0.1f);
            }
            for (int i = 0; i < customAnimBehaviours?.Length; i++)
            {
                customAnimBehaviours[i].Init(m_animController.Graph);
                m_animController.AddState(customAnimBehaviours[i]?.name, customAnimBehaviours[i]?.AnimBehaviour);
            }

            m_animController.AddAnimator("Animator", 0.2f);
            m_animController.Start();
        }

        private void OnAnimatorMove()
        {
            m_animController.OnAnimatorMove();
        }

        private void OnDestroy()
        {
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
        public float BlendTree1DGetValue(string name)
        {
            return m_animController.BlendTree1DGetValue(name);
        }
    }
}