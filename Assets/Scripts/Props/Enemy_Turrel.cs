using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Turrel : MonoBehaviour
{
    public float shot_delay = 1f;
    public float rotation_speed = 1f;
    public float limit_angle = -1;

    float start_angle = 0f;
    float last_time_shot = 0f;

    Color[] colors = new Color[]{Color.white, Color.red, Color.green, Color.blue, Color.yellow, Color.black };

    // Start is called before the first frame update
    void Start()
    {
        start_angle = transform.localRotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (BOT.is_paused || BOT.pause) { last_time_shot += Time.deltaTime; return; }

        transform.Rotate(0f, rotation_speed, 0f);
        if (limit_angle > 0f && Mathf.Abs(Mathf.DeltaAngle(start_angle, transform.localRotation.eulerAngles.y)) >= limit_angle) rotation_speed *= -1f;

        if (BOT.script_thread != null && BOT.script_thread.IsAlive) {
            //if (last_time_shot + shot_delay >= Time.time) { //THAT was funny - ball every frame
            if (Time.time >= last_time_shot + shot_delay) {
                last_time_shot = Time.time;

                var g = Instantiate(transform.GetChild(2).gameObject);
                g.transform.position = transform.GetChild(2).transform.position;

                int r = Random.Range(0, colors.Length);
                var p = g.GetComponent<Projectile>();
                p.color = colors[r]; p.speed = -transform.forward * 0.01f;

                g.SetActive(true);
            }
        }
    }
}
