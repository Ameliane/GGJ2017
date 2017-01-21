using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    Transform m_FollowObject = null;

    [SerializeField]
    float m_Height = 2;

    [SerializeField]
    float m_MaxDistance = 10;

    [SerializeField]
    float m_MinDistance = 5;

    [Range(1, 10)]
    [SerializeField]
    float m_Stiffness = 3;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        Vector3 targetPos = m_FollowObject.position - m_FollowObject.forward * Vector3.Distance(transform.position, m_FollowObject.position);

        float dist = Vector3.Distance(targetPos, m_FollowObject.position);
        if (dist > m_MaxDistance)
        {
            targetPos = Vector3.MoveTowards(targetPos, m_FollowObject.position, dist - m_MaxDistance);
        }
        else if (dist < m_MinDistance)
        {
            targetPos = Vector3.MoveTowards(targetPos, transform.TransformPoint(-transform.InverseTransformPoint(m_FollowObject.position)), m_MinDistance - dist);
        }

        targetPos.y = m_FollowObject.position.y + m_Height;
        transform.position = Vector3.Lerp(transform.position, targetPos, m_Stiffness * Time.deltaTime);
        transform.LookAt(m_FollowObject);
	}
}
