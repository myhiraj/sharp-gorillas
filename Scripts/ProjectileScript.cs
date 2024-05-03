using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTerrain;

public class ProjectileScript : MonoBehaviour
{
    public GameObject controller;
    Rigidbody2D rb;
    Collider2D collider;

    private void Awake()
    {
        controller = GameObject.Find("GameController");
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        DisableCollider();
        Invoke("EnableCollider", 0.1f);

        Invoke("KillProjectile", 60f);
    }

    void DisableCollider()
    {
        collider.enabled = false;
    }

    void EnableCollider()
    {
        collider.enabled = true;
    }

    public void Launch(float angle, float velocity)
    {
        angle *= Mathf.Deg2Rad;

        float x_velocity = Mathf.Cos(angle) * velocity;
        float y_velocity = Mathf.Sin(angle) * velocity;

        rb.velocity = new Vector2(x_velocity, y_velocity);
    }

    public void Update()
    {
        WindDirection windDirection = controller.GetComponent<GameLogic>().wind;
        int windSpeed = controller.GetComponent<GameLogic>().windSpeed;

        if (windDirection == WindDirection.LEFT) { rb.AddForce(Vector2.left * windSpeed * 0.01f) ; }
        else if (windDirection == WindDirection.RIGHT) { rb.AddForce(Vector2.right * windSpeed * 0.01f); }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);

        GameObject other_object = collision.gameObject;
        if (other_object.name == "GorillaOne" || other_object.name == "GorillaTwo")
        {
            controller.GetComponent<GameLogic>().doScore(other_object);
            Destroy(other_object);
            controller.GetComponent<GameLogic>().Reset();
        }
    }

    void KillProjectile()
    {
        Destroy(gameObject);
    }
}
