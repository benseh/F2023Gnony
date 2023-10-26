using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   //총알 데미지
    public int dmg;
    public bool isRotate; // 회전하는 총알 만들기

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * 10);
    }
    //총알 사라지기
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }
}
