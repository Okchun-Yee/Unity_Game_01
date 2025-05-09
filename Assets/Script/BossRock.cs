using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShot;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    void Update()
    {
        
    }

    IEnumerator GainPowerTimer(){
        yield return new WaitForSeconds(2.2f);
        isShot= true;
    }
    IEnumerator GainPower(){
        while(!isShot){
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);

            yield return null;
        }
    }
}
