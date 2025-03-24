using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;   //Player.cs 파일의 멤버 사용하기

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
        transform.LookAt(transform.position);
    }
}
