using System.Collections;
using UnityEngine;

namespace AE_Motion
{
    public class PlayerParam : MonoBehaviour
    {
        private PlayerMotion m_motion;
        //InputAction文件
        public PlayerInputAction m_inputActions;
        //移动方向输入
        public Vector2 moveInput;
        //视角方向输入
        public Vector2 mouseInput;
        //奔跑输入
        public bool run;
        public bool lastRun;
        private Coroutine stopLastRunCoroutine;
        //跳跃
        public bool jump;
        public bool climb;

        public void Init(PlayerMotion motion)
        {
            m_motion = motion;

            m_inputActions = new PlayerInputAction();
            m_inputActions.Enable();

            m_inputActions.Simple.Run.performed += (context) =>
            {
                run = true;
                if (stopLastRunCoroutine != null) StopCoroutine(stopLastRunCoroutine);
                lastRun = true;
            };
            m_inputActions.Simple.Run.canceled += (context) =>
            {
                run = false;
                if (stopLastRunCoroutine != null) StopCoroutine(stopLastRunCoroutine);
                stopLastRunCoroutine = StartCoroutine(StopLastRun());
            };

            m_inputActions.Simple.Climb.performed += (context) => climb = true;
            m_inputActions.Simple.Climb.canceled += (context) => climb = false;
        }

        public void Update()
        {
            moveInput = Vector2.Lerp(moveInput, m_inputActions.Simple.Move.ReadValue<Vector2>(), 0.5f);

            mouseInput = m_inputActions.Simple.MousesXY.ReadValue<Vector2>();

            jump |= m_inputActions.Simple.Jump.WasPressedThisFrame();

            GiveParamsToFSM();
        }

        private void OnDestroy()
        {
            m_inputActions.Disable();
        }

        /// <summary>
        /// 将参数给状态机
        /// </summary>
        private void GiveParamsToFSM()
        {
            if (m_motion == null) return;
            m_motion.FSMController.SetFloat("moveInputMagnitude", moveInput.magnitude);
            m_motion.FSMController.SetBool("jump", jump);
        }
        /// <summary>
        /// 记录上一秒是否在奔跑
        /// </summary>
        /// <returns></returns>
        private IEnumerator StopLastRun()
        {
            yield return new WaitForSeconds(1f);
            lastRun = false;
        }
    }
}