using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   //�Ѿ� ������
    public int dmg;
    public bool isRotate; // ȸ���ϴ� �Ѿ� �����

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * 10);
    }
    //�Ѿ� �������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }
}
