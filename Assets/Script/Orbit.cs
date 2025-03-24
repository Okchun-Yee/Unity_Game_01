using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;
    void Start()
    {
        offset = transform.position - target.position;  //플레이어와 오브젝트 사이의 차이값
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(
            target.position,
            Vector3.up,
            orbitSpeed * Time.deltaTime * 10);
        offset = transform.position - target.position;
    }
}
