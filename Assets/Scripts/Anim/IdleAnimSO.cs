using AE_Animation_Playable;
using UnityEngine;
using UnityEngine.Playables;

namespace AE_Motion.Anim
{
    [CreateAssetMenu(fileName = "IdleAnimSO", menuName = "AE_Animation_Playable/Custom/IdleAnimSO")]
    public class IdleAnimSO : CustomAnimBehaviourSO
    {
        public AnimationClip[] idleClips;
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        public override void Init(PlayableGraph graph)
        {
            AnimBehaviour = new IdleAnim(graph, idleClips, 5f, 0.5f);
        }
    }
}