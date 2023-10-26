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
        //Queue = FIFO(First input first out) ���� �Էµ� ������ ���� ��µǴ� �ڷᱸ��

        //#.Input Pos
        if(!parentPos.Contains(parent.position)) // parent��ġ�� ������ �������� ������� �ʵ��� ����
            parentPos.Enqueue(parent.position);

        //#.Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position; // ť�� ä������ ������ �θ� ��ġ ���� (���� �������ڸ���)
    }
    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return; //Fire1 ��ư�� ������ �ʾ������� �Ѿ��� ������ �ʰ� ��

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
        curShotDelay += Time.deltaTime; //������ ������ Time.deltaTime�� ��� ���Ͽ� �ð� ���, ������ 
    }

}
