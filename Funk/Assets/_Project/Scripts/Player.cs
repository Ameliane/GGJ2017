using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public enum State
    {
        DEFAULT,
        PAINT,
        SIZE
    }

    [SerializeField]
    float m_BaseSpeed = 10;

    [SerializeField]
    float m_AbilitySpeed = 20;

    float m_Speed;

    [SerializeField]
    float m_TurnSpeed = 80;

    [SerializeField]
    float m_BaseJumpPower = 7;

    [SerializeField]
    float m_AbilityJumpPower = 14;

    float m_JumpPower;

    [SerializeField]
    float m_MaxGroundAngle = 60;
    bool m_IsOnGround = true;

    Rigidbody m_Body = null;

    State m_State;
    SandPainting.SandColor m_Color;

    void Start()
    {
        m_Body = GetComponent<Rigidbody>();

        m_Speed = m_BaseSpeed;
        m_JumpPower = m_BaseJumpPower;
    }

    void Update()
    {
        SwitchState();

        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * m_TurnSpeed * Time.deltaTime);

        if (Input.GetButton("Jump") && m_IsOnGround)
        {
            m_Body.AddForce(Vector3.up * m_JumpPower, ForceMode.VelocityChange);
            m_IsOnGround = false;
        }

        UseAbility(SandPainting.Instance.GetSandColor(transform.position));

        if (m_State == State.PAINT)
            SandPainting.Instance.SetTerrainColor(transform.position, m_Color);
    }

    void FixedUpdate()
    {
        m_Body.AddForce(transform.forward * Input.GetAxisRaw("Vertical") * m_Speed, ForceMode.Acceleration);
        if (m_IsOnGround) m_Body.velocity = Vector3.Scale(m_Body.velocity, new Vector3(0.95f, 1, 0.95f)); //Drag, but without vertical drag!
    }

    void OnCollisionEnter(Collision aHit)
    {
        if (Vector3.Angle(aHit.contacts[0].normal, Vector3.up) < m_MaxGroundAngle)
        {
            if (SandPainting.Instance.GetSandColor(transform.position) == SandPainting.SandColor.BOUNCE)
            {
                m_Body.velocity = aHit.impulse;
            }
            else
            {
                m_IsOnGround = true;
            }
        }
    }

    void SwitchState()
    {
        // Toggle states and sand color
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_State++;
            if (m_State >= State.SIZE)
                m_State = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_Color++;
            if (m_Color >= SandPainting.SandColor.SIZE)
                m_Color = 0;
        }

        // Select states and sans color
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_State = State.DEFAULT;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_State = State.PAINT;
            m_Color = SandPainting.SandColor.DEFAULT;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_State = State.PAINT;
            m_Color = SandPainting.SandColor.BOUNCE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_State = State.PAINT;
            m_Color = SandPainting.SandColor.FAST;
        }

        // Reset and set values
        switch (m_State)
        {
            case State.DEFAULT:
                UIManager.Instance.SetAbility("Ability");
                break;

            case State.PAINT:
                switch (m_Color)
                {
                    case SandPainting.SandColor.DEFAULT:
                        UIManager.Instance.SetAbility("Paint Default");
                        break;

                    case SandPainting.SandColor.BOUNCE:
                        UIManager.Instance.SetAbility("Paint Bounce");
                        break;

                    case SandPainting.SandColor.FAST:
                        UIManager.Instance.SetAbility("Paint Fast");
                        break;

                    case SandPainting.SandColor.SIZE:
                        break;
                    default:
                        break;
                }
                break;

            case State.SIZE:
                break;
            default:
                break;
        }
    }

    void UseAbility(SandPainting.SandColor aColor)
    {
        m_JumpPower = m_BaseJumpPower;
        m_Speed = m_BaseSpeed;

        switch (aColor)
        {
            case SandPainting.SandColor.DEFAULT:
                break;
            case SandPainting.SandColor.BOUNCE:
                // Bounce here
                m_JumpPower = m_AbilityJumpPower;
                break;
            case SandPainting.SandColor.FAST:
                // Fast here
                m_Speed = m_AbilitySpeed;
                break;
            case SandPainting.SandColor.SIZE:
                break;
            default:
                break;
        }
    }
}
