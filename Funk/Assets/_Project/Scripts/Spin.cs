using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
    [SerializeField]
    float m_SpinSpeed = 10;

    [SerializeField]
    Vector3 m_LocalSpinAxis = Vector3.up;

	void Update ()
    {
        transform.Rotate(m_LocalSpinAxis, m_SpinSpeed * Time.deltaTime);
	}
}
