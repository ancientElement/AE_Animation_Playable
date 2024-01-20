using AE_FSM;
using AE_Motion;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private PlayerMotion m_playerMotion;
    public PlayerMotion Motion => m_playerMotion;

    public Transform playerTansfrom;
    public PlayerLocomotionContext playerLocomotion;

    private void Start()
    {
        m_playerMotion = GetComponent<PlayerMotion>();
        m_playerMotion.Init(playerTansfrom,
                            GetComponent<Animator>(),
                            GetComponent<CharacterController>(),
                            GetComponent<PlayerAnim>(),
                            GetComponent<FSMController>(),
                            GetComponent<PlayerParam>(),
                            GetComponent<PlayerSensor>(),
                            playerLocomotion);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
    }
}