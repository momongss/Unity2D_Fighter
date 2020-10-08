using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Portal : MonoBehaviour
{    
    public GameObject ExitObject;

    // Start is called before the first frame update
    void Awake()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)  // Player 
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.transform.position = ExitObject.transform.position;
        }
        else if (collision.gameObject.layer == 9) // Player Bullet
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            bullet.transform.position = ExitObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
