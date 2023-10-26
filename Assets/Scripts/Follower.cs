using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    void Watch()
    {
        //Queue = FIFO(First input first out) 먼저 입력된 구조가 먼저 출력되는 자료구조

        //#.Input Pos
        if(!parentPos.Contains(parent.position)) // parent위치가 가만히 있을때는 따라오지 않도록 저장
            parentPos.Enqueue(parent.position);

        //#.Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position; // 큐가 채워지기 전까진 부모 위치 적용 (게임 시작하자마자)
    }
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return; //Fire1 버튼을 누르지 않았을때는 총알이 나오지 않게 함

        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime; //딜레이 변수에 Time.deltaTime을 계속 더하여 시간 계산, 재장전 
    }

}
