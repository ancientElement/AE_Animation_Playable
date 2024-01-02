using AE_Motion;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private PlayerMotion m_Motion;

    public int contactNum;
    public Vector3 contactNormal;
    public Vector3 desiredVelocity;
    public Transform playerInputSpace;

    public void Init(PlayerMotion playerMotion)
    {
        m_Motion = playerMotion;
    }

    private void FixedUpdate()
    {
        contactNormal = Vector3.zero;
        contactNum = 0;
    }

    private void Update() {
        Debug.DrawRay(transform.position,desiredVelocity,Color.red);
        Debug.DrawRay(transform.position,transform.forward,Color.yellow);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ModifyContacts(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        ModifyContacts(collision);
    }

    private void ModifyContacts(Collision collision)
    {
        // 确保发生碰撞
        if (collision.contacts.Length > 0)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                // 获取第一个接触点的法线
                contactNormal += collision.contacts[i].normal;
                contactNum++;
                // 输出法线信息
            }
            contactNormal.Normalize();
#if UNITY_EDITOR
            DrawRay(transform.position, contactNormal, Color.red);
#endif
        }
    }


#if UNITY_EDITOR
    private void DrawRay(Vector3 start, Vector3 direction, Color color)
    {
        Debug.DrawRay(start, direction, color);
    }
#endif
}
