using System.Collections;
using System.Collections.Generic;
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
        bool iDown = false;     //e : 오브젝트 상호작용 키
        bool swapDown_1;    // 무기 스왑 1, 2, 3
        bool swapDown_2;
        bool swapDown_3;
        bool gDown;

        //공격 변수 모음
        bool fDown;
        bool rDown;
        bool isReload = false;
        float fireDealy; //공격 딜레이
        bool isFireReady = true;   //공격 준비 완료 변수
        public Camera followCamera;


        //Item 변수
        public int ammo;
        public int coin;
        public int health;
        public int hasGreandes;

        //Item 변수 최대치
        public int ammoMax;
        public int coinMax;
        public int healthMax;
        public int hasGreandesMax;

        public GameObject[] weapons;    //무기 오브젝트 배열
        public bool[] hasWeapons;       //무기 소지 (인벤토리 배열)
        public GameObject[] Greandes;

        //특수 공격 - 수류탄 투척
        public GameObject greandeObj;


        bool isJump = false;    //jump 상태 확인
        bool isDodge = false;   //Dodge 상태 확인
        bool isSwap = false;    //swap 상태확인

        GameObject nearObj; //가까운 아이템 변수
        Weapon equipweapon; //현재 장착중인 아이템 변수


        public Vector3 moveVec; //about Player Move
        public Vector3 dodgeVec; //about Player Move
        Rigidbody rigid;
        Animator anim;

        //벽 관통 방지 변수
        bool isBorder = false;

        // Start is called before the first frame update
        void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
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
            hAxis = Input.GetAxisRaw("Horizontal"); //GetAxisRaw = Axis값을 정수로 반환하는 함수
            vAxis = Input.GetAxisRaw("Vertical");   //입력은 Input Manage에서 관리한다.
            walkDown = Input.GetButtonDown("Walk");
            jmpDown = Input.GetButtonDown("Jump");
            iDown = Input.GetButtonDown("Interation");  //무기 먹기
            swapDown_1 = Input.GetButtonDown("Swap_1");
            swapDown_2 = Input.GetButtonDown("Swap_2");
            swapDown_3 = Input.GetButtonDown("Swap_3");
            fDown = Input.GetButton("Fire1");  //공격 키 = 마우스 왼쪽 클릭 
            rDown = Input.GetButtonDown("Reload"); //장전
            gDown = Input.GetButtonDown("Fire2"); //투척 공격 키 = 마우스 오른쪽 클릭
        }

        void move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized; /*normalized 사용시 대각선 방향도 같은 속도로 이동 원래 대각선 이동시 피타고라스로 인해 더 큰값을 가짐*/
            if (isDodge) { moveVec = dodgeVec; }
            if (isSwap || !isFireReady || isReload) { moveVec = Vector3.zero; }
            if (!isBorder) {
                transform.position += 
                    moveVec * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime; 
            }

            anim.SetBool("is_Run", moveVec != Vector3.zero);
            anim.SetBool("is_Walk", walkDown);
        }

        void turn()
        {
            transform.LookAt(transform.position + moveVec);  //지정된 벡터를 향해서 회전시켜주는 함수

            //마우스에 따른 회전
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
            if (jmpDown&& moveVec==Vector3.zero && !isJump && !isDodge && !isSwap && !isReload)
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

            fireDealy += Time.deltaTime;
            isFireReady = equipweapon.rate < fireDealy;

            if (fDown && isFireReady && !isDodge && !isSwap) {
                equipweapon.Use();
                anim.SetTrigger(equipweapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
                fireDealy = 0;
            }
        }

        void Grenade()
        {
            if (hasGreandes == 0) return;
            if (gDown && !isSwap && !isReload)
            {
                Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 100))
                {
                    Vector3 nextVec = rayHit.point - transform.position;
                    nextVec.y = 2;

                    GameObject instantGrenade = Instantiate(greandeObj, transform.position, transform.rotation);
                    Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                    rigidGrenade.AddForce(nextVec * 1.2f, ForceMode.Impulse);
                    rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                    hasGreandes--;
                    Greandes[hasGreandes].SetActive(false);
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
            if(rDown && !isJump && !isSwap && !isDodge && isFireReady)
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
            //잔여탄 고려하기 
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

        int equipweapomIndex = -1;
        void itemSwap()
        {
            if(swapDown_1 && (!hasWeapons[0] || equipweapomIndex == 0)) { return; }
            if (swapDown_2 && (!hasWeapons[1] || equipweapomIndex == 1)) { return; }
            if (swapDown_3 && (!hasWeapons[2] || equipweapomIndex == 2)) { return; }

            int weaponIndex = -1;
            if (swapDown_1) weaponIndex = 0;
            if (swapDown_2) weaponIndex = 1;
            if (swapDown_3) weaponIndex = 2;

            if ((swapDown_1 || swapDown_2 || swapDown_3) && isJump!= true && isDodge != true )
            {
                if(equipweapon!=null) equipweapon.gameObject.SetActive(false);
                equipweapomIndex = weaponIndex;
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
                    
                    hasWeapons[weaponIndex] = true; //인벤토리 배열에 저장

                    Destroy(nearObj);   //먹은 아이템 
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
                        if (hasGreandes == hasGreandesMax)
                        {
                            Debug.Log("Max Greandes");
                            return;
                        }
                        Debug.Log("Take Greandes");
                        Greandes[hasGreandes].SetActive(true);
                        hasGreandes += item.value;
                        break;
                }
                Destroy(other.gameObject);
            }
        }
        void OnTriggerStay(Collider other)
        {
            if(other.tag=="weapon") nearObj = other.gameObject;

            //Debug.Log(nearObj.name);
        }
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "weapon") nearObj = null;
        }
    }
}
