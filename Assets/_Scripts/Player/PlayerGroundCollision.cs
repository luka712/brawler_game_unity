using UnityEngine;

public class PlayerGroundCollision : MonoBehaviour {

    private MovePlayer movePlayer;

	// Use this for initialization
	void Start ()
    {
        movePlayer = GetComponentInParent<MovePlayer>();

	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Ground))
        {
            if(collision.contacts[0].point.y >
                collision.gameObject.transform.position.y +
                collision.gameObject.transform.localScale.y * 0.5f)
            {
                movePlayer.ResetJumpState();
            }
            else if(collision.contacts[0].point.y < movePlayer.transform.position.y)
            {
                movePlayer.ResetJumpState();
            }
        }
    }

}
