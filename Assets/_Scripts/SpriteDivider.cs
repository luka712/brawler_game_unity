using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface ISpriteDivider
{
    DividedSprite[] DividedSprites { get; }
    void RenderDividedSprites();
}

public class SpriteDivider : MonoBehaviour, ISpriteDivider
{
    #region Editor Variables

    public Vector2 _pass = new Vector2(3, 3);

    #endregion

    #region Fields

    #endregion

    private SpriteRenderer spriteRenderer;
    private ITeleportObjectInterface objectToTeleport;
    private List<Collider2D> playerColliders;


    private Vector2 teleportDestination;

    #region Properties

    public DividedSprite[] DividedSprites { get; private set; }

    #endregion

    private void Awake()
    {
        DividedSprites = new DividedSprite[(int)(_pass.x * _pass.y)];
        for (int i = 0; i < DividedSprites.Length; i++)
        {
            DividedSprites[i] = new DividedSprite(this.gameObject);
        }
    }

    private void Start()
    {
        objectToTeleport = GetComponent<ITeleportObjectInterface>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerColliders = GetComponentsInChildren<Collider2D>().ToList();
    }

    public void RenderDividedSprites()
    {
        var totalSpriteSize = new Vector2(spriteRenderer.sprite.textureRect.width, spriteRenderer.sprite.textureRect.height);

        var xPass = totalSpriteSize.x / _pass.x;
        var yPass = totalSpriteSize.y / _pass.y;

        int i = 0;
        for (var x = 0f; x < totalSpriteSize.x; x += xPass)
        {
            for (var y = 0f; y < totalSpriteSize.y; y += yPass)
            {
                var dividedSprite = DividedSprites[i++];
                float xPos = -(xPass * _pass.x * .5f) + x;
                float yPos = -(yPass * _pass.y * .5f) + y;
                dividedSprite.CreateSprite(this.spriteRenderer, new Rect(x, y, xPass, yPass), new Vector2(xPos, yPos));
            }
        }
    }

    //public void Divide(Vector2 direction, float minForce = 0.1f, float maxForce = 0.5f)
    //{
    //    var spriteObjects = RenderDividedSprites();
    //    foreach (var spriteObj in spriteObjects)
    //    {
    //        spriteObj.DeathAnimation(direction, minForce, maxForce);
    //    }
    //}

}
