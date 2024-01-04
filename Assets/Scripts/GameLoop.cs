using AE_FSM;
using AE_Motion;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private PlayerMotion m_playerMotion;
    
    public PlayerLocomotionContext playerLocomotion;

    private void Start()
    {
        m_playerMotion = new PlayerMotion(transform, GetComponent<Animator>(), GetComponent<CharacterController>(), GetComponent<Rigidbody>(), GetComponent<PlayerAnim>(), GetComponent<FSMController>(), GetComponent<PlayerParam>(), GetComponent<PlayerSensor>(), playerLocomotion);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
    }
}