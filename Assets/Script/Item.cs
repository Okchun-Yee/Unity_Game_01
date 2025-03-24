using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum itemType { itemAmmo, itemCoin, itemGrenade, itemHeart, itemWeapon };
    public itemType type;
    public int value;

    Rigidbody rigid;
    BoxCollider boxCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") {
            rigid.isKinematic = true;
            boxCollider.enabled = false;
        }
    }
}
