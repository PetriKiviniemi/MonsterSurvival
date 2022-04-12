using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int bulletDamage;
    public float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnTime - Time.time > 8)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Enemy"))
            collision.transform.gameObject.GetComponent<ZombieScript>().takeDamage(bulletDamage);
        Destroy(gameObject);
    }
}
