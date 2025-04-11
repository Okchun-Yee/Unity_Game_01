using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;
    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;
    Palyer enterPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter(Palyer player){
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }
    public void Exit(){
        enterPlayer.isShop = false;
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }
    public void Buy(int index){
        int price = itemPrice[index];
        if(price > enterPlayer.coin){
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        enterPlayer.coin -= price;
        Vector3 randVec = 
        Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);

        Instantiate(itemObj[index], itemPos[index].position + randVec, itemPos[index].rotation);
    }
    IEnumerator Talk(){
        talkText.text = talkData[1];

        yield return new WaitForSeconds(2f);

        talkText.text = talkData[0];
    }
}
