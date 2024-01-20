using AE_Motion;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private PlayerMotion m_motion;

    //接触点
    public int contactNum;
    //接触法线
    public Vector3 contactNormal;
    //输入空间
    public Transform playerInputSpace;
    //当前速度
    public float speed;
    //期望速度
    public Vector3 velocity;

    //重力
    public float grivaty = 9.8f;
    //重力方向
    public Vector3 grivatyUp = new Vector3(0, 1, 0);

    //地面检测半径
    [SerializeField] float groundDetectionRadius;

    //最大地面角度
    [SerializeField] float maxGroundAngle = 25f;
    //地面角度余弦值
    float minGroundDotProduct;
    private int groundContactCount;
    public bool OnGround => groundContactCount > 0;

    public void Init(PlayerMotion playerMotion)
    {
        m_motion = playerMotion;

        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void FixedUpdate()
    {
        contactNormal = Vector3.zero;
        contactNum = 0;

        // 使用OverlapSphere检测碰撞
        Collider[] colliders = Physics.OverlapSphere(transform.position, groundDetectionRadius);

    }

    private void Update()
    {
        Debug.DrawRay(transform.position, velocity, Color.red);
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);
    }

    private void ModifyContacts(Collision collision)
    {
        // 确保发生碰撞
        if (collision.contacts.Length > 0)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(grivatyUp, normal);
                if (upDot >= minGroundDotProduct)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                }
            }
            contactNormal.Normalize();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, contactNormal);
    }
#endif
}
