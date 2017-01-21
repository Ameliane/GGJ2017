using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField]
    float m_Speed = 10;

    [SerializeField]
    float m_TurnSpeed = 80;

    [SerializeField]
    float m_JumpPower = 7;

    [SerializeField]
    float m_MaxGroundAngle = 60;
    bool m_IsOnGround = true;

    Rigidbody m_Body = null;

	void Start ()
    {
        m_Body = GetComponent<Rigidbody>();
	}

    void Update ()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * m_TurnSpeed * Time.deltaTime);

        if (Input.GetButton("Jump") && m_IsOnGround)
        {
            m_Body.AddForce(Vector3.up * m_JumpPower, ForceMode.VelocityChange);
            m_IsOnGround = false;
        }
    }
	
	void FixedUpdate ()
    {
        m_Body.AddForce(transform.forward * Input.GetAxisRaw("Vertical") * m_Speed, ForceMode.Acceleration);
        m_Body.velocity = Vector3.Scale(m_Body.velocity, new Vector3(0.95f, 1, 0.95f)); //Drag, but without vertical drag!
    }

    void OnCollisionEnter(Collision aHit)
    {
        if (Vector3.Angle(aHit.contacts[0].normal, Vector3.up) < m_MaxGroundAngle)
        {
            m_IsOnGround = true;
        }
    }
}
