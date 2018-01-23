using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividedSprite
{
    [SerializeField]
    private string layer = "DividedSprites";

    [SerializeField]
    private string sortingLayer = "Foreground";

    private GameObject _this;
    private GameObject parent;
    private SpriteRenderer rend;
    private BoxCollider2D coll;
    private Rigidbody2D rigidBody;

    // added for fine tunning details.
    private const float ForceMultiplier = 500f;

    // gravity
    private const float Gravity = 0.2f;
    private const float FadeOutSpeed = 20f;
    private const float PixelsPerUnit = 32f;
    private float lifeTime = 0;
    private bool isActive = false;


    // Use this for initialization
    public DividedSprite(GameObject parent)
    {
        _this = new GameObject();
        this.parent = parent;
        rend = _this.AddComponent<SpriteRenderer>();
        coll = _this.AddComponent<BoxCollider2D>();
        rigidBody = _this.AddComponent<Rigidbody2D>();
        rigidBody.gravityScale = Gravity;
        rigidBody.gameObject.layer = LayerMask.NameToLayer(layer);
    }

    // Update is called once per frame
    public void Update(float passedTime)
    {
        if (isActive && lifeTime > 0)
        {
            // multiplication is faster then division. So 0.001f.
            lifeTime -= Time.deltaTime * FadeOutSpeed * 0.01f;
            rend.color = rend.color.SetAlpha(lifeTime);
            if (lifeTime <= 0)
            {
                _this.gameObject.SetActive(false);
            }
        }
    }

    public virtual void CreateSprite(SpriteRenderer parentRenderer, Rect sourceRect, Vector2 offset)
    {
        _this.SetActive(true);
        isActive = true;
        lifeTime = 1f;
        rend.size = parentRenderer.size;

        // TODO: this part was throwing exceptions, check if this handling is ok, test game sprite divider
        if (sourceRect.x + sourceRect.width > parentRenderer.sprite.texture.width)
        {
            sourceRect.width = parentRenderer.sprite.texture.width - sourceRect.x;
        }
        if (sourceRect.y + sourceRect.height > parentRenderer.sprite.texture.height)
        {
            sourceRect.height = parentRenderer.sprite.texture.height - sourceRect.y;
        }
        rend.sprite = Sprite.Create(parentRenderer.sprite.texture, sourceRect, Vector2.zero, PixelsPerUnit);

        rend.color = parentRenderer.color;
        rend.sortingLayerName = sortingLayer;

        var parentPosition = parent.transform.position;
        _this.transform.position =
            new Vector3(parentPosition.x + offset.x / PixelsPerUnit, parentPosition.y + offset.y / PixelsPerUnit, parentPosition.z);

        // Set collision box.
        coll.offset = new Vector2(sourceRect.width * .5f / PixelsPerUnit, sourceRect.height * .5f / PixelsPerUnit);
        coll.size = new Vector2(sourceRect.width / PixelsPerUnit, sourceRect.height / PixelsPerUnit);
    }

    public virtual void AddForce(Vector2 direction, float minForce, float maxForce)
    {
        rigidBody.AddForce(new Vector2(direction.x * Random.Range(minForce, maxForce),
            direction.y * Random.Range(minForce, maxForce)) * ForceMultiplier);
    }

}
