using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type {A, B, C};
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    //meleeAttack
    public BoxCollider meleeArea;
    public bool isAttack;
    Rigidbody rigid;
    BoxCollider boxCollider;

    //???? ???
    Material mat;

    NavMeshAgent nav;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>(); 
        boxCollider = GetComponent<BoxCollider>();
        //?????? ??? ????????? ???????? ???? ??? ??? -> ???? ?????? ????????
        mat = GetComponentInChildren<MeshRenderer>().material;
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        Invoke("ChaseStart", 2f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(onDamge(reactVec, false));
            //Debug.Log("Melee " + curHealth + " Damge : " + weapon.damage);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            
            Destroy(other.gameObject);
            StartCoroutine(onDamge(reactVec, false));
            //Debug.Log("Range " + curHealth + " Damge : " + bullet.damage);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine (onDamge(reactVec, true));
    }



    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

    }
    void Targerting(){
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType){
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0;
                targetRange = 0;
                break;
        }
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if(rayHits.Length > 0 && !isAttack){
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack(){
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        
        switch (enemyType){
            case Type.A:
                yield return new WaitForSeconds(0.6f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1.6f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(0.8f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.2f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:

                break;
        }
        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    private void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }


    IEnumerator onDamge(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
            reactVec += Vector3.up * 0.7f;
            rigid.AddForce(reactVec * 1.7f, ForceMode.Impulse);
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if(isGrenade) {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 4);
        }
    }
}