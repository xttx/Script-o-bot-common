using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool captured = false;
    public Color color = Color.white;
    public Vector3 speed = Vector3.zero;

    GameObject thread_safe_gameobject;

    // Start is called before the first frame update
    void Start()
    {
        thread_safe_gameobject = gameObject;

        Engine.Level_Projectiles.Add(this);
        GetComponent<MeshRenderer>().material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (BOT.is_paused || BOT.pause) return;
        transform.position += speed;
    }

    public Vector3 position {
        get {
            return thread_safe_gameobject.transform2().position;
        }
    }
}
