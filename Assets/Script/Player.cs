using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public class Palyer : MonoBehaviour
    {
        public float speed;
        float hAxis;
        float vAxis;
        bool walkDown;  //walk
        bool jmpDown;   //jump
        bool iDown = false;     //e : ??????? ?????? ?
        bool swapDown_1;    // ???? ???? 1, 2, 3
        bool swapDown_2;
        bool swapDown_3;
        bool gDown;

        //???? ???? ????
        bool fDown;
        bool rDown;
        bool isReload = false;
        float fireDelay; //???? ??????
        bool isFireReady = true;   //???? ??? ??? ????
        public Camera followCamera;
        bool isDamage;
        public bool isShop;

        //Item ????
        public int ammo;
        public int coin;
        public int health;
        public int hasGrenades;

        //Item ???? ????
        public int ammoMax;
        public int coinMax;
        public int healthMax;
        public int hasGrenadesMax;

        public GameObject[] weapons;    //???? ??????? ?��
        public bool[] hasWeapons;       //???? ???? (?��??? ?��)
        public GameObject[] Grenades;

        //??? ???? - ????? ???
        public GameObject grenadeObj;


        bool isJump = false;    //jump ???? ???
        bool isDodge = false;   //Dodge ???? ???
        bool isSwap = false;    //swap ???????

        GameObject nearObj; //????? ?????? ????
        Weapon equipweapon; //???? ???????? ?????? ????


        public Vector3 moveVec; //about Player Move
        public Vector3 dodgeVec; //about Player Move
        Rigidbody rigid;
        Animator anim;
        MeshRenderer[] meshes;
        //?? ???? ???? ????
        bool isBorder = false;

        // Start is called before the first frame update
        void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            meshes = GetComponentsInChildren<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            getInput();
            move();
            turn();
            jump();
            Dodge();
            Interation();
            itemSwap();
            Attack();
            Reload();
            Grenade();
        }

        void getInput()
        {
            hAxis = Input.GetAxisRaw("Horizontal"); //GetAxisRaw = Axis???? ?????? ?????? ???
            vAxis = Input.GetAxisRaw("Vertical");   //????? Input Manage???? ???????.
            walkDown = Input.GetButtonDown("Walk");
            jmpDown = Input.GetButtonDown("Jump");
            iDown = Input.GetButtonDown("Interation");  //???? ???
            swapDown_1 = Input.GetButtonDown("Swap_1");
            swapDown_2 = Input.GetButtonDown("Swap_2");
            swapDown_3 = Input.GetButtonDown("Swap_3");
            fDown = Input.GetButton("Fire1");  //???? ? = ???�J ???? ??? 
            rDown = Input.GetButtonDown("Reload"); //????
            gDown = Input.GetButtonDown("Fire2"); //??? ???? ? = ???�J ?????? ???
        }

        void move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized; /*normalized ???? ?�O?? ???? ???? ????? ??? ???? ?�O?? ????? ???????? ???? ?? ????? ????*/
            if (isDodge) { moveVec = dodgeVec; }
            if (isSwap || !isFireReady || isReload || isShop) { moveVec = Vector3.zero; }
            if (!isBorder) {
                transform.position += 
                    moveVec * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime; 
            }

            anim.SetBool("is_Run", moveVec != Vector3.zero);
            anim.SetBool("is_Walk", walkDown);
        }

        void turn()
        {
            transform.LookAt(transform.position + moveVec);  //?????? ????? ????? ?????????? ???

            //???�J?? ???? ???
            if(fDown)
            {
                Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 100))
                {
                    Vector3 nextVec = rayHit.point - transform.position;
                    nextVec.y = 0;
                    transform.LookAt(transform.position + nextVec);
                }
            }
        }

        void jump()
        {
            if (jmpDown&& moveVec==Vector3.zero && !isJump && !isDodge && !isSwap && !isReload && !isShop)
            {
                rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
                anim.SetBool("is_Jump", true);
                anim.SetTrigger("doJump");
                isJump = true;
            }
        }

        void Attack()
        {
            if (equipweapon == null) return;

            fireDelay += Time.deltaTime;
            isFireReady = equipweapon.rate < fireDelay;

            if (fDown && isFireReady && !isDodge && !isSwap && !isShop) {
                equipweapon.Use();
                anim.SetTrigger(equipweapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
                fireDelay = 0;
            }
        }

        void Grenade()
        {
            if (hasGrenades == 0) return;
            if (gDown && !isSwap && !isReload &&!isShop)
            {
                Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 100))
                {
                    Vector3 nextVec = rayHit.point - transform.position;
                    nextVec.y = 2;

                    GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                    Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                    rigidGrenade.AddForce(nextVec * 1.2f, ForceMode.Impulse);
                    rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                    hasGrenades--;
                    Grenades[hasGrenades].SetActive(false);
                }
            }
        }
        void Reload()
        {
            if (equipweapon == null) 
                return;
            if (equipweapon.type == Weapon.Type.Melee)
                return;
            if (ammo == 0)
                return;
            if(rDown && !isJump && !isSwap && !isDodge && isFireReady && !isShop)
            {
                Debug.Log("Reloading");
                isReload = true;
                anim.SetTrigger("doReload");

                Invoke("ReloadOut", 2f);
            }
            else { Debug.Log("Reloading False");}
        }
        void ReloadOut()
        {
            //???? ??????? 
            int reAmmo = 
                ammo + equipweapon.curAmmo < equipweapon.maxAmmo 
                ? ammo : equipweapon.maxAmmo - equipweapon.curAmmo;
            equipweapon.curAmmo += reAmmo;
            ammo -= reAmmo;

            isReload = false;
        }

        void Dodge()
        {
            if (jmpDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap) 
            {
                dodgeVec = moveVec;
                speed *= 2;
                anim.SetBool("is_Jump", true);
                anim.SetTrigger("doDodge");
                isDodge = true;

                Invoke("DodgeOut",0.5f);
            }
        }
        void DodgeOut()
        {
            speed *= 0.5f;
            isDodge = false;
        }

        int equipweaponIndex = -1;
        void itemSwap()
        {
            if(swapDown_1 && (!hasWeapons[0] || equipweaponIndex == 0)) { return; }
            if (swapDown_2 && (!hasWeapons[1] || equipweaponIndex == 1)) { return; }
            if (swapDown_3 && (!hasWeapons[2] || equipweaponIndex == 2)) { return; }

            int weaponIndex = -1;
            if (swapDown_1) weaponIndex = 0;
            if (swapDown_2) weaponIndex = 1;
            if (swapDown_3) weaponIndex = 2;

            if ((swapDown_1 || swapDown_2 || swapDown_3) && isJump!= true && isDodge != true )
            {
                if(equipweapon!=null) equipweapon.gameObject.SetActive(false);
                equipweaponIndex = weaponIndex;
                equipweapon = weapons[weaponIndex].GetComponent<Weapon>();
                equipweapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap");
                isSwap = true;

                Invoke("SwapOut", 0.5f);
            }
        }
        void SwapOut() { isSwap = false; }
        void Interation()
        {
            if(iDown && nearObj != null && isJump != true)
            {
                if (nearObj.tag == "weapon")
                {
                    //Debug.Log("check");
                    Item item = nearObj.GetComponent<Item>();
                    int weaponIndex = item.value;
                    
                    hasWeapons[weaponIndex] = true; //?��??? ?��?? ????

                    Destroy(nearObj);   //???? ?????? 
                }
                else if (nearObj.tag == "Shop")
                {
                    //Debug.Log("check");
                    Shop shop = nearObj.GetComponent<Shop>();
                    shop.Enter(this);
                    isShop = true;
                }
            }
        }
        void FreezeRotation()
        {
            rigid.angularVelocity = Vector3.zero;

        }
        void StopToWall()
        {
            Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
            isBorder = Physics.Raycast(transform.position, moveVec, 5, LayerMask.GetMask("Wall"));
        }
        private void FixedUpdate()
        {
            FreezeRotation();
            StopToWall();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                anim.SetBool("is_Jump", false);
                isJump = false;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if(other.tag == "item")
            {
                Item item = other.GetComponent<Item>();
                switch(item.type)
                {
                    case Item.itemType.itemAmmo:
                        ammo += item.value;
                        if (ammo > ammoMax) ammo = ammoMax;
                        break;
                    case Item.itemType.itemCoin:
                        coin += item.value;
                        if (coin > coinMax) coin = coinMax;
                        break;
                    case Item.itemType.itemHeart:
                        health += item.value;
                        if (health > healthMax) health = healthMax;
                        break;
                    case Item.itemType.itemGrenade:
                        if (hasGrenades == hasGrenadesMax)
                        {
                            Debug.Log("Max Grenades");
                            return;
                        }
                        Debug.Log("Take Grenades");
                        Grenades[hasGrenades].SetActive(true);
                        hasGrenades += item.value;
                        break;
                }
                Destroy(other.gameObject);
            }
            else if(other.tag == "EnemyBullet"){
                if(!isDamage){
                    Bullet enemybullet = other.GetComponent<Bullet>();
                    health -= enemybullet.damage;

                    bool isBossAttack = other.name == "BossMelee Area";
                    StartCoroutine(onDamage(isBossAttack));
                }

                if(other.GetComponent<Rigidbody>() != null) { Destroy(other.gameObject); }
            }
        }

        IEnumerator onDamage(bool isBossAtk)
        {
            isDamage = true;
            foreach(MeshRenderer mesh in meshes){
                mesh.material.color = Color.yellow;
            }
            if (isBossAtk){
                rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(1f);

            isDamage = false;
            foreach(MeshRenderer mesh in meshes){
                mesh.material.color = Color.white;
            }
            if (isBossAtk){
                rigid.velocity = Vector3.zero;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.tag=="weapon" || other.tag == "Shop") 
            {
                nearObj = other.gameObject;
            }

            //Debug.Log(nearObj.name);
        }
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "weapon") nearObj = null;
            else if (other.tag == "Shop") {
                Shop shop = nearObj.GetComponent<Shop>();
                isShop = false;
                shop.Exit();
                
                nearObj = null;
            }
        }
    }
}
