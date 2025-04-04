using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookvec;
    Vector3 tauntVec;
    public bool isLook;


    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); 
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        nav.isStopped = true;

        StartCoroutine(Think());
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead){
            StopAllCoroutines();
            return; //EXIT
        }
        if(isLook){
            float h =Input.GetAxisRaw("Horizontal");
            float v =Input.GetAxisRaw("Vertical");
            lookvec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookvec);
        }
        else {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = true;
        int ranAction = Random.Range(0, 5);
        switch (ranAction){
            case 0:
            case 1:
                //미사일 발사 패턴턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //돌 굴리기 패턴턴
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }
    }
    IEnumerator MissileShot(){
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissilA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissilA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissilB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissilB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Think());
    }
    IEnumerator RockShot(){
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);

        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt(){
        tauntVec = target.position + lookvec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}
