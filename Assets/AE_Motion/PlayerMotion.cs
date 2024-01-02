using System;
using AE_FSM;
using UnityEngine;

namespace AE_Motion
{
    [Serializable]
    public class PlayerLocomotionContext
    {
        public float walkSpeed = 1.81728f;
        public float walkRotateSpeed = 2.430124f;
        public float runSpeed = 5.547339f;
        public float runRoateSpeed = 3.92699f;
        public float rotateSpeed = 520f;
        public float currentTurnSpeed;
        public float currentSpeed;
        /// <summary>
        /// 浮点比较值
        /// </summary>
        public float zero = 1e-6f;
    }

    public class PlayerMotion
    {
        public PlayerParam Param { get; private set; }
        public FSMController FSMController { get; private set; }
        public PlayerAnim Anim { get; private set; }
        public PlayerSensor Sensor { get; private set; }
        public Transform Modle { get; private set; }
        public CharacterController CharactorController { get; private set; }
        public Animator Animator { get; private set; }
        public PlayerLocomotionContext LocomtionCtx { get; private set; }

        public PlayerMotion(Transform modle, Animator animator, CharacterController characterController, PlayerAnim anim, FSMController fSMController, PlayerParam playerParam, PlayerSensor sensor, PlayerLocomotionContext playerLocomotionContext)
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
            FSMController.Init(this);
            Param.Init(this);
            Sensor.Init(this);
        }
    }
}