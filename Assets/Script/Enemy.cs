using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;

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
        if (isChase)
        {
            nav.SetDestination(target.position);
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
    private void FixedUpdate()
    {
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