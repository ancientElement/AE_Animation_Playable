using AE_Animation_Playable;
using UnityEngine;
using UnityEngine.Playables;

namespace AE_Motion.Anim
{

    public class IdleAnim : AnimBehaviour
    {
        private float timer;
        public float TimeToPlay;

        private Mixer m_mixer;
        private PlayableGraph m_grapha;
        private RandomSelector m_randomSelector;
        private AnimUnit m_idle;


        public IdleAnim(PlayableGraph graph, AnimationClip[] animationClips, float timeToPlay = 10f, float enterTime = 0f) : base(graph, enterTime)
        {
            m_grapha = graph;
            TimeToPlay = timeToPlay;
            timer = timeToPlay;

            m_randomSelector = new RandomSelector(m_grapha, enterTime);

            m_idle = new AnimUnit(m_grapha, animationClips[0], enterTime);

            for (int i = 1; i < animationClips.Length; i++)
            {
                m_randomSelector.AddInput(animationClips[i], enterTime);
            }
            m_mixer = new Mixer(m_grapha);

            Playable playable = m_mixer.GetAdaptrtPlayable();
            m_adapterPlayable.AddInput(playable, 0, 1f);

            m_mixer.AddInput(m_idle);
            m_mixer.AddInput(m_randomSelector);
        }

        public override void Enable()
        {
            base.Enable();
            m_adapterPlayable.SetTime(0f);
            m_adapterPlayable.Play();
            m_mixer.Enable();
        }

        public override void Excute(Playable playable, FrameData info)
        {
            base.Excute(playable, info);
            timer -= info.deltaTime;
            if (timer <= 0)
            {
                timer = TimeToPlay;
                m_randomSelector.Select();
                m_mixer.TransitionTo(1);
            }

            if (m_randomSelector.remainTime == 0f && !m_mixer.isTransition && m_mixer.currentIndex != 0)
            {
                m_mixer.TransitionTo(0);
            }
        }

        public override void Disable()
        {
            base.Disable();
            m_mixer.Disable();
            m_adapterPlayable.Pause();
            m_randomSelector.Disable();
            m_idle.Disable();
        }
    }
}