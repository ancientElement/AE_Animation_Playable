using System.Collections;
using UnityEngine;

namespace AE_Motion
{
    public class PlayerParam : MonoBehaviour
    {
        private PlayerMotion m_motion;

        public PlayerInputAction m_inputActions;

        public Vector2 moveInput;
        public Vector2 mouseInput;

        public bool run;
        public bool lastRun;
        private Coroutine stopLastRunCoroutine;

        public bool jump;
        public bool climb;

        public void Init(PlayerMotion motion)
        {
            m_motion = motion;

            m_inputActions = new PlayerInputAction();
            m_inputActions.Enable();

            m_inputActions.Simple.Run.performed += (context) => { run = true; lastRun = true; };
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

            jump = m_inputActions.Simple.Jump.WasPressedThisFrame();

            GiveParamsToFSM();
        }

        private void OnDestroy()
        {
            m_inputActions.Disable();
        }

        /// 将参数给状态机
        private void GiveParamsToFSM()
        {
            if (m_motion == null) return;
            m_motion.FSMController.SetFloat("moveInputMagnitude", moveInput.magnitude);
        }

        private IEnumerator StopLastRun()
        {
            yield return new WaitForSeconds(1f);
            lastRun = false;
        }
    }
}