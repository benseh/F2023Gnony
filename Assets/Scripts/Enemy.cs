using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom; // 아이템 프리팹을 저장할 변수 생성

    public GameObject player;
    public ObjectManager objectManager;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount; // 패턴 흐름에 필요한 변수 생성

    SpriteRenderer spriteRenderer;
    Animator anim;
    Rigidbody2D rigid;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyName == "B")
            anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                health = 3000;
                Invoke("Stop", 2);
                break;
            case "L":
                health = 40;
                break;
            case "M":
                health = 10;
                break;
            case "S":
                health = 3;
                break;
        }
    }

    void Stop()
    {
        if (!gameObject.activeSelf)
            return; // stop 함수가 두번 사용되지 않도록 함

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1; //현재 패턴이 패턴 개수를 넘기면 0으로 돌아오는 로직 작성
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireForward() //앞으로 4발 발사
    {
        //#. Fire 4 bullet forward 
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        //#. Pattern counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireForward", 2); 
        else
            Invoke("Think", 3); // 각 패턴별 횟수를 실행하고 다음 패턴으로 넘어가도록 구현
    }

    void FireShot() //플레이어 방향으로 5개 샷건
    {
        for(int index=0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f)); //???
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
        
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3);
    }

    void FireArc() //부채 모양으로 발사
    {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity; // rotation 0로 만들기

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

            curPatternCount++;

            if (curPatternCount < maxPatternCount[patternIndex])
                Invoke("FireArc", 0.15f);
            else
                Invoke("Think", 3);
    }

    void FireAround() //원 형태로 전체 공격
    {
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB; //패턴 횟수에 따라 생성되는 총알 갯수 조절로 난이도 상승

        for (int index=0; index < roundNum; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity; //rotation 0로 만들기

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum)
                                       , Mathf.Sin(Mathf.PI * 2 * index / roundNum)); //생성되는 총알의 순번을 활용하여 방향 결정
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }
       
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 0.7f);
        else
            Invoke("Think", 3);
    }
    void Update()
    {
        if (enemyName == "B")
            return;
        Fire();
        Reload();
    }
    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
                    return; // 아이템 다중 생성 방지

        health -= dmg; 
        if(enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }
        

        if(health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore; // Enemy가 파괴될 때, player의 score 추가

            //#.Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10); // boss는 아이템을 떨어트리지 않음
            // 30, 30, 20, 20의 확률로 아이템 드랍   
            if(ran < 3)
            {// Not Item 30%
                Debug.Log("Not Item");
            }
            else if (ran < 6)
            {
                // Coin 30%
                GameObject itemCoin = objectManager.MakeObj("itemCoin");
                itemCoin.transform.position = transform.position;             
            }
            else if (ran < 8)
            {
                // Power 20%
                GameObject itemPower = objectManager.MakeObj("itemPower");
                itemPower.transform.position = transform.position;               
            }
            else if (ran < 10)
            {
                // Boom 20%
                GameObject itemBoom = objectManager.MakeObj("itemBoom");
                itemBoom.transform.position = transform.position;
            }
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //맵 이탈 시 삭제
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        //총알 피격 시 데미지
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
