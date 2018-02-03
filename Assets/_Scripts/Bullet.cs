using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rigBody;

    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float timeToExpire = 3f;

    public int Group { get; set; }

    // Use this for initialization
    void Awake()
    {
        rigBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void Fire(Vector2 one)
    {
        this.gameObject.SetActive(true);
        rigBody.velocity = one * speed;
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeToExpire);
        this.gameObject.SetActive(false);
    }
}
