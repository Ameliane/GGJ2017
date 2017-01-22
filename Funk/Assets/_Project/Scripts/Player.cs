using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public enum Ability
    {
        DEFAULT,
        BOUNCE,
        FAST,
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

    Ability m_Ability;
    SandPainting.SandColor m_Color;

    void Start()
    {
        m_Body = GetComponent<Rigidbody>();

        m_Speed = m_BaseSpeed;
        m_JumpPower = m_BaseJumpPower;
        
        GameManager.Instance.Register(this);
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

        //UseAbility(SandPainting.Instance.GetSandColor(transform.position));
        //
        //if (m_State == State.PAINT)
        //    SandPainting.Instance.SetTerrainColor(transform.position, m_Color);
    }

    void FixedUpdate()
    {
        m_Body.AddForce(transform.forward * Input.GetAxisRaw("Vertical") * m_Speed, ForceMode.Acceleration);
        m_Body.velocity = Vector3.Scale(m_Body.velocity, new Vector3(0.98f, 1, 0.98f)); //Drag, but without vertical drag!
    }

    void OnCollisionEnter(Collision aHit)
    {
        if (Vector3.Angle(aHit.contacts[0].normal, Vector3.up) < m_MaxGroundAngle)
        {
            if (m_Ability == Ability.BOUNCE)
            {
                m_Body.velocity = aHit.impulse;
            }
            else
            {
                m_IsOnGround = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Death":
                GameManager.Instance.End(false);
                break;

            case "Win":
                GameManager.Instance.End(true);
                break;

            case "Checkpoint":
                GameManager.Instance.SetCheckPoint(transform.position);
                break;

            case "Collectible":
                GameManager.Instance.Collect();
                other.gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }

    void SwitchState()
    {
        if (GameManager.Instance.m_IsPaused)
            return;

        // Toggle states and sand color
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_Ability++;
            if (m_Ability >= Ability.SIZE)
                m_Ability = 0;
        }

        // Select states and sans color
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_Ability = Ability.DEFAULT;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_Ability = Ability.BOUNCE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_Ability = Ability.FAST;
        }

        // Set UI
        switch (m_Ability)
        {
            case Ability.DEFAULT:
                UIManager.Instance.SetAbility("No Ability");
                break;

            case Ability.BOUNCE:
                UIManager.Instance.SetAbility("Bounce");
                break;

            case Ability.FAST:
                UIManager.Instance.SetAbility("Fast");
                break;

            default:
                break;
        }

        UseAbility();
    }
    public void ResetAbility()
    {
        m_Ability = Ability.DEFAULT;
    }

    void UseAbility()
    {
        m_JumpPower = m_BaseJumpPower;
        m_Speed = m_BaseSpeed;

        switch (m_Ability)
        {
            case Ability.DEFAULT:
                break;
            case Ability.BOUNCE:
                // Bounce here
                m_JumpPower = m_AbilityJumpPower;
                break;
            case Ability.FAST:
                // Fast here
                m_Speed = m_AbilitySpeed;
                break;
            case Ability.SIZE:
                break;
            default:
                break;
        }
    }
}
