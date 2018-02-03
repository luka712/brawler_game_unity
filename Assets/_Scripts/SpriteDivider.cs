using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpriteDivider : MonoBehaviour
{
    [SerializeField]
    private Vector2 pass = new Vector2(3, 3);

    private SpriteRenderer spriteRenderer;
    private Player player;
    private MovePlayer movePlayer;
    private List<Collider2D> playerColliders;
    private List<DividedSprite> gameObjects = new List<DividedSprite>();

    private Vector2 teleportDestination;

    // Use this for initialization
    private void Start()
    {
        player = GetComponent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movePlayer = GetComponent<MovePlayer>();
        playerColliders = GetComponentsInChildren<Collider2D>().ToList();
        for (int x = 0; x < pass.x * pass.y; x++)
        {
            gameObjects.Add(new DividedSprite(this.gameObject));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        gameObjects.ForEach(x => x.Update(Time.deltaTime));
        if (player.CanTeleport == false && player.IsTeleporting)
        {
            if (gameObjects.Any(x => x.State == State.TeleportAnimation))
            {
                return;
            }

            gameObjects.ForEach(x => x.Active(false));
            playerColliders.ForEach(x => x.enabled = true);
            spriteRenderer.color = spriteRenderer.color.SetAlpha(1f);
            player.SetPosition(teleportDestination.ToVector3());
            player.IsTeleporting = false;
        }
    }

    private IEnumerable<DividedSprite> GetGameObjectsWithSprites()
    {
        var totalSpriteSize = new Vector2(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);

        var xPass = totalSpriteSize.x / pass.x;
        var yPass = totalSpriteSize.y / pass.y;

        int i = 0;
        for (var x = 0f; x < totalSpriteSize.x; x += xPass)
        {
            for (var y = 0f; y < totalSpriteSize.y; y += yPass)
            {
                var dividedSprite = gameObjects[i++];
                float xPos = -(xPass * pass.x * .5f) + x;
                float yPos = -(yPass * pass.y * .5f) + y;
                dividedSprite.CreateSprite(this.spriteRenderer, new Rect(x, y, xPass, yPass), new Vector2(xPos, yPos));
                yield return dividedSprite;
            }
        }
    }

    public void Divide(Vector2 direction, float minForce = 0.1f, float maxForce = 0.5f)
    {
        var spriteObjects = GetGameObjectsWithSprites();
        foreach (var spriteObj in spriteObjects)
        {
            spriteObj.DeathAnimation(direction, minForce, maxForce);
        }
    }

    public void Teleport(Vector2 position)
    {
        if (player.CanTeleport)
        {
            player.CanTeleport = false;
            var spriteObjects = GetGameObjectsWithSprites().ToList();
            player.IsTeleporting = true;
            playerColliders.ForEach(x => x.enabled = false);
            spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
            foreach (var spriteObj in spriteObjects)
            {
                spriteObj.Teleport(position + spriteObj.Position - player.transform.position.ToVector2(), .5f);
            }
            teleportDestination = position;
        }
    }
}
