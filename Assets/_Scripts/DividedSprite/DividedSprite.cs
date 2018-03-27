
using UnityEngine;

public enum State { None, DeathAnimation, TeleportAnimation }

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

    public float pixelsPerUnit = 8f;

    // gravity
    private const float Gravity = 0.2f;
    private const float FadeOutSpeed = 20f;

    private float lifeTime = 0;
    private bool isActive = false;

    // teleport
    public Vector2 teleportDestination;
    private Vector2 randomDirection;
    private float randomDirectionForce = 0f;
    private const float RandomDirectionForceScalar = 0.5f;
    private float teleportSpeed = 0.1f;

    public State State { get; private set; }

    public Vector2 Position { get { return _this.transform.position.ToVector2(); } }

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
        if (isActive && lifeTime > 0 && State == State.DeathAnimation)
        {
            // multiplication is faster then division. So 0.001f.
            lifeTime -= Time.deltaTime * FadeOutSpeed * 0.01f;
            rend.color = rend.color.SetAlpha(lifeTime);
            if (lifeTime <= 0)
            {
                State = State.None;
                _this.gameObject.SetActive(false);
            }
        }
        else if (State == State.TeleportAnimation)
        {
            // arbitrary random direction
            _this.transform.position += randomDirection.ToVector3() * randomDirectionForce;
            randomDirectionForce *= 0.9f;

            // normal direction
            var direction = teleportDestination - _this.transform.position.ToVector2();
            if (direction.magnitude < .5f)
            {
                State = State.None;
                _this.transform.position = teleportDestination;
            }
            else
            {
                direction.Normalize();
                _this.transform.position += direction.ToVector3() * teleportSpeed;
                _this.transform.Rotate(Vector3.forward, 0f);
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
        rend.sprite = Sprite.Create(parentRenderer.sprite.texture, sourceRect, Vector2.zero, pixelsPerUnit);

        rend.color = parentRenderer.color;
        rend.sortingLayerName = sortingLayer;

        var parentPosition = parentRenderer.gameObject.transform.position;
        _this.transform.position =
            new Vector3(parentPosition.x + offset.x / pixelsPerUnit, parentPosition.y + offset.y / pixelsPerUnit, parentPosition.z);

        // Set collision box.
        coll.offset = new Vector2(sourceRect.width * .5f / pixelsPerUnit, sourceRect.height * .5f / pixelsPerUnit);
        coll.size = new Vector2(sourceRect.width / pixelsPerUnit, sourceRect.height / pixelsPerUnit);
    }

    public virtual void DeathAnimation(Vector2 direction, float minForce, float maxForce)
    {
        _this.SetActive(true);
        State = State.DeathAnimation;
        rigidBody.gravityScale = Gravity;
        coll.enabled = true;
        AddForce(direction, minForce, maxForce);
    }

    public virtual void AddForce(Vector2 direction, float minForce, float maxForce)
    {
        rigidBody.AddForce(new Vector2(direction.x * Random.Range(minForce, maxForce),
            direction.y * Random.Range(minForce, maxForce)) * ForceMultiplier);
    }

    public virtual void Teleport(Vector2 destination, float teleportSpeed)
    {
        _this.SetActive(true);
        State = State.TeleportAnimation;
        coll.enabled = false;
        rigidBody.bodyType = RigidbodyType2D.Static;
        rend.color = rend.color.SetAlpha(1f);
        randomDirectionForce = RandomDirectionForceScalar;
        this.teleportSpeed = teleportSpeed;
        randomDirection = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        randomDirection.Normalize();
        teleportDestination = destination;
    }

    public void Active(bool active)
    {
        _this.SetActive(active);
    }
}
