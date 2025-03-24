using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;


    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    //원거리 공격 관련 변수
    public Transform bulletPos;
    public GameObject bullet; //생성 위치
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); //0.1f(초) 대기 
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f); //0.2f(초) 대기 
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f); //0.3f(초) 대기 
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        //발사 #1
        GameObject intanBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intanBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;

        //탄피 배출 
        GameObject intanCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intanCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-5, -2) + Vector3.up * Random.Range(2, 3);   //탄피에 가해질 힘

        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
