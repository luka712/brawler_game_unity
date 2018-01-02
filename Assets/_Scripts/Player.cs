using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private int health;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Health = 100;
    }

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if(health > 100)
            {
                health = 100;
            }
            else if(health < 0)
            {
                health = 0;
            }
        }
    }
    
    public void AddDamage(int damage, Vector2 feedback)
    {
        Health -= damage;
        rigidBody.AddForce(feedback);
    }

    public void AddHealth(int health)
    {
        Health += health; 
    }

    public void SetPosition(Vector3 position)
    {
        this.gameObject.transform.position = position;
    }
}
