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
    public delegate void AnimationMoveEvent(Vector3 position, Vector3 velocity, Quaternion quaternion);
    public class PlayerAnim : MonoBehaviour
    {
        private PlayableGraph m_graph;
        private Mixer m_mixer;
        private Animator m_animator;
        private Dictionary<string, int> m_animMap2Int;

        private Dictionary<string, AnimBehaviour> m_animMap;
        private Dictionary<string, BlendTree1D> m_blendTree1DMap;
        private Dictionary<string, BlendTree2D> m_blendTree2DMap;

        private IdleAnim m_idleAnim;
        private AnimMixAnimator m_animMixAnimator;

        public AnimationClip[] idleClips;
        // public AnimationClip walkClip;
        // public AnimationClip runClip;

        public BlendClip1D[] moveClips;
        public BlendClip2D[] moveClip2Ds;
        public AnimationClip runStopClip;
        public AnimationClip walkStopClip;
        public AnimationClip runTurn180Clip;

        private Coroutine m_animationCroosFadeCallback;
        private Coroutine m_TransitionToCallback;
        private bool m_rootmotion;
        private CharacterController m_characterController;
        private Transform m_modle;
        private PlayerMotion m_motion;

        public void Init(PlayerMotion playerMotion)
        {
            m_graph = PlayableGraph.Create();
            m_mixer = new Mixer(m_graph);
            m_animMap2Int = new Dictionary<string, int>();

            m_animMap = new Dictionary<string, AnimBehaviour>();
            m_blendTree1DMap = new Dictionary<string, BlendTree1D>();
            m_blendTree2DMap = new Dictionary<string, BlendTree2D>();

            m_motion = playerMotion;
            m_animator = playerMotion.Animator;
            m_characterController = playerMotion.CharactorController;
            m_modle = playerMotion.Modle;

            //添加状态
            m_idleAnim = new IdleAnim(m_graph, idleClips, 5f, 0.5f);
            AddState("Idle", m_idleAnim);
            AddBlendTree1D("MoveBlendTree", moveClips, 0.1f);
            AddAnimator("Animator", 0.2f);
            AddBlendTree2D("MoveBlendTree2D", moveClip2Ds, 0.1f);

            AddAnimUnit("RunStop", runStopClip, 0.2f);
            AddAnimUnit("RunTurn180", runTurn180Clip, 0.2f);

            AnimHelper.SetOutput(m_graph, m_animator, m_mixer);
            AnimHelper.Start(m_graph);
        }

        private void Update()
        {
            #region  Test
            // if (Input.GetKeyDown(KeyCode.C))
            // {
            //     TransitionTo("RunTurn180", () =>
            //     {
            //         TransitionTo("Idle");
            //         PreventRootMotion();
            //     });
            //     ApplayRootMotion();
            // }
            #endregion
        }

        private void OnDestroy()
        {
            m_graph.Destroy();
        }

        #region  过渡
        public void TransitionTo(string name, Action callback = null, float enterTIme = -1f, float animationClipLength = -1f)
        {
            if (enterTIme != -1f)
                SetEnterTime(name, enterTIme);
            TransitionTo(name);
            if (m_TransitionToCallback != null)
                StopCoroutine(m_TransitionToCallback);
            if (m_animationCroosFadeCallback != null)
                StopCoroutine(m_animationCroosFadeCallback);
            if (callback != null)
            {
                if (animationClipLength != -1f)
                    m_TransitionToCallback = StartCoroutine(TransitionToEnumerator(name, animationClipLength, callback));
                else
                    m_TransitionToCallback = StartCoroutine(TransitionToEnumerator(name, GetAnimLength(name), callback));
            }
        }

        private void SetEnterTime(string name, float enterTIme)
        {
            m_animMap[name].SetEnterTime(enterTIme);
        }

        private void TransitionTo(string name)
        {
            m_mixer.TransitionTo(m_animMap2Int[name]);
        }

        private IEnumerator TransitionToEnumerator(string name, float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
            yield break;
        }
        #endregion

        #region  添加动画节点
        //添加Animator
        private void AddAnimator(string name, float enterTIme)
        {
            m_animMixAnimator = new AnimMixAnimator(m_graph, m_animator, 0.2f);
            AddState(name, m_animMixAnimator);
        }

        //添加BlendTree1D
        public void AddBlendTree1D(string name, BlendClip1D[] blendTree1DClips, float enterTime)
        {
            BlendTree1D blendTree1D = new BlendTree1D(m_graph, blendTree1DClips, 0.2f);
            m_blendTree1DMap.Add(name, blendTree1D);
            AddState(name, blendTree1D);
        }

        //添加BlendTree2D
        public void AddBlendTree2D(string name, BlendClip2D[] blendTree2DClips, float enterTime)
        {
            BlendTree2D blendTree2D = new BlendTree2D(m_graph, blendTree2DClips, 0.2f);
            m_blendTree2DMap.Add(name, blendTree2D);
            AddState(name, blendTree2D);
        }

        //添加单个动画
        public void AddAnimUnit(string name, AnimationClip animationClip, float enterTime)
        {
            if (!m_animMap2Int.TryGetValue(name, out int index))
            {
                var anim = new AnimUnit(m_graph, animationClip, enterTime);
                AddState(name, anim);
            }
        }

        // 向根混合器添加状态
        public void AddState(string name, AnimBehaviour behaviour)
        {
            m_mixer.AddInput(behaviour);
            m_animMap2Int.Add(name, m_mixer.inputCount - 1);
            m_animMap.Add(name, behaviour);
        }
        #endregion

        #region BlendTree
        public void BlendTree1DSetValue(string name, float value)
        {
            if (m_blendTree1DMap.TryGetValue(name, out BlendTree1D blendTree1D))
            {
                blendTree1D.SetValue(value);
            }
        }

        public void BlendClipTree2DSetPointer(string name, Vector2 value)
        {
            if (m_blendTree2DMap.TryGetValue(name, out BlendTree2D blendTree2D))
            {
                blendTree2D.SetPointer(value);
            }
        }
        #endregion

        #region  AnimatonMixer
        public void AnimatorCroosFade(string stateName, float transitionDuration, Action callback = null)
        {
            TransitionTo("Animator");
            m_animMixAnimator.CrossFade(stateName, transitionDuration);
            if (m_TransitionToCallback != null)
                StopCoroutine(m_TransitionToCallback);
            if (m_animationCroosFadeCallback != null)
                StopCoroutine(m_animationCroosFadeCallback);
            if (callback != null)
            {
                m_animationCroosFadeCallback = StartCoroutine(AnimationCroosFadeEnumerator(m_animMixAnimator.GetAnimLength(0), callback));
            }
        }
        private IEnumerator AnimationCroosFadeEnumerator(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
        public void AnimatorSetFloat(string name, float value) { m_animMixAnimator.SetFloat(name, value); }
        public void AnimatorSetBool(string name, bool value) { m_animMixAnimator.SetBool(name, value); }
        public void AnimatorSetTrigger(string name) { m_animMixAnimator.SetTrigger(name); }
        public float AnimatorGetAnimLength(int layer) { return m_animMixAnimator.GetAnimLength(layer); }
        #endregion

        #region  RootMotion
        private void OnAnimatorMove()
        {
            if (m_rootmotion)
            {
                Move(m_animator.deltaPosition, m_animator.velocity, m_animator.deltaRotation);
            }
        }


        public void ApplayRootMotion()
        {
            m_rootmotion = true;
        }

        public void PreventRootMotion()
        {
            m_rootmotion = false;
        }

        protected void Move(Vector3 deltaPosition, Vector3 velocity, Quaternion deltaRotation)
        {
            // 设置速度
            m_characterController.Move(deltaPosition);

            // 通过将刚体的角度设置为当前角度加上增量旋转
            m_modle.transform.rotation *= deltaRotation;
        }
        #endregion

        #region  协程
        public Coroutine AnimStartCoroutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }

        public void AnimStopCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        #endregion

        //获取当前动画播放长度
        public float GetAnimLength(string name)
        {
            if (m_animMap.TryGetValue(name, out AnimBehaviour anim))
            {
                return anim.GetAnimLength();
            }
            return 0;
        }
    }
}