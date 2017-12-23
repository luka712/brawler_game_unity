using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDivider : MonoBehaviour
{
    [SerializeField]
    private Vector2 pass = new Vector2(3, 3);


    [SerializeField]
    private Vector2 defaultSpriteSize = new Vector2(64, 64);

    [SerializeField]
    private Vector2 defaultSpriteOffset = new Vector2(0f, 0f);

    private SpriteRenderer spriteRenderer;
    private List<DividedSprite> gameObjects = new List<DividedSprite>();


    // Use this for initialization
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int x = 0; x < pass.x * pass.y; x++)
        {
            gameObjects.Add(new DividedSprite(this.gameObject));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        gameObjects.ForEach(x => x.Update(Time.deltaTime));
    }

    private IEnumerable<DividedSprite> GetGameObjectsWithSprites()
    {
        
        var totalSpriteSize = new Vector2(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);
        if(defaultSpriteSize != Vector2.zero)
        {
            totalSpriteSize = defaultSpriteSize;
        }

        var xPass = totalSpriteSize.x / pass.x;
        var yPass = totalSpriteSize.y / pass.y;

        int i = 0;
        for(var x = 0f; x < totalSpriteSize.x; x += xPass)
        {
            for(var y = 0f; y < totalSpriteSize.y; y += yPass)
            {
                var dividedSprite = gameObjects[i++];
                float xPos = -(xPass * pass.x * .5f) + x;
                float yPos = -(yPass * pass.y * .5f) + y;
                dividedSprite.CreateSprite(this.spriteRenderer,
                    new Rect(x + defaultSpriteOffset.x, y + defaultSpriteOffset.y, xPass, yPass),
                    new Vector2(xPos, yPos));
                yield return dividedSprite;
            }
        }
    }

    public void Divide(Vector2 direction, float minForce = 0.1f, float maxForce = 0.5f)
    {
        var spriteObjects = GetGameObjectsWithSprites();
        foreach (var spriteObj in spriteObjects)
        {
            spriteObj.AddForce(direction, minForce, maxForce);
        }
    }
}
