using System;
using AE_FSM;
using UnityEngine;

namespace AE_Motion
{
    [Serializable]
    public class PlayerLocomotionContext
    {
        //行走速度
        public float walkSpeed = 1.81728f;
        //奔跑速度
        public float runSpeed = 5.547339f;
        //跳跃高度
        public float jumpHeight = 5f;
        //旋转速度
        public float rotateSpeed = 520f;
        //浮点比较值
        public float zero = 1e-6f;
        //转身结束的角度
        public float turnStopAngle = 30f;
        //转身开始的角度
        public float turnStartAngle = 160f;
        //最大加速度
        public float maxAcceleration = 10f;
        public float tmep1;
    }

    public class PlayerMotion : MonoBehaviour
    {
        public PlayerParam Param { get; private set; }
        public FSMController FSMController { get; private set; }
        public PlayerAnim Anim { get; private set; }
        public PlayerSensor Sensor { get; private set; }
        public Transform Modle { get; private set; }
        public CharacterController CharactorController { get; private set; }
        public Animator Animator { get; private set; }
        public PlayerLocomotionContext LocomtionCtx { get; private set; }

        public void Init(Transform modle, Animator animator, CharacterController characterController, PlayerAnim anim, FSMController fSMController, PlayerParam playerParam, PlayerSensor sensor, PlayerLocomotionContext playerLocomotionContext)
        {
            Modle = modle;
            Anim = anim;
            FSMController = fSMController;
            Param = playerParam;
            Sensor = sensor;
            LocomtionCtx = playerLocomotionContext;
            CharactorController = characterController;
            Animator = animator;

            Anim.Init(this);
            Param.Init(this);
            Sensor.Init(this);
            FSMController.Init();
        }
    }
}