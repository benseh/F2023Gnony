using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    public int life;
    public int score;
    public float speed;
    public int maxPower; // 파워 아이템에 제한을 둠
    public int power; //아이템 먹으면 총알이 더많이 발사
    public int maxBoom;
    public int boom;
    public float maxShotDelay; //총알 발사 딜레이 로직을 위한 변수 생성(최대, 현재), 클수록 총알 발사 간격 커짐
    public float curShotDelay;

    //총알 프리펩을 저장할 변수 생성
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit; // 동시에 2개에게 피격당했을 시, 목숨 두개 깎임 방지
    public bool isBoomTime;

    public GameObject[] followers;

    Animator anim;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Reload();
        Boom();
    }
    void Move()
    {
        //방향 값 추출
        float h = Input.GetAxisRaw("Horizontal");
        //플래그 변수를 사용하여 경계 이상 넘지 못하도록 값 제한
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        //플레이어 위치에 현재위치, 다음위치 가져오기
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        //애니메이션 parameters
        if (Input.GetButtonDown("Horizontal") ||
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }
    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return; //Fire1 버튼을 누르지 않았을때는 총알이 나오지 않게 함

        if (curShotDelay < maxShotDelay)
            return; 

        switch (power)
        {
            case 1://Power One
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 총 쏨
                break;
            case 2://Power Two
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                //총알이 두배로 발사되는데, 위치 살짝 조정함
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 총 쏨   
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 총 쏨
                break;
            default: // case 3,4,5,6 포함
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f; 
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;

                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 총 쏨   
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 총 쏨
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // 총 쏨
                break;
        }


        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime; //딜레이 변수에 Time.deltaTime을 계속 더하여 시간 계산, 재장전 
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2"))
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom); 

        //#1. Effect visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 2f); // 폭탄 스프라이트를 invoke로 시간차 비활성화

        //#2. Remove enemy
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        //태그로 장면의 모든 오브젝트를 추출
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            } 
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        //#3. Remove enemy bullets
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");

        //태그로 장면의 모든 오브젝트를 추출
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }
        }

    }

    //경계 충돌 떨림 보정
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border") //border에 태그와 isTrigger설정
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit)
                return;

            isHit = true;

            life--;
            gameManager.UpdateLifeIcon(life);

            if(life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
        }

        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;

                case "Coin":
                    score += 1000;
                    break;

                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }

                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void AddFollower()
    {
        if (power == 4)
            followers[0].SetActive(true);
        else if(power == 5)
            followers[1].SetActive(true);
        else if (power == 6)
            followers[2].SetActive(true);
    }

    //border와 충돌하지 않을 때 플래그 지우기
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
