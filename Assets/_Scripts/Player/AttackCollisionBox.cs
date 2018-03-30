#define LOGGER

using UnityEngine;



public class AttackCollisionBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
#if LOGGER
        Debug.Log("Attack_Collider");
#endif
    }
}
