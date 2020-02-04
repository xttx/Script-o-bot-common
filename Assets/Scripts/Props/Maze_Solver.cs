using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Maze_Solver : MonoBehaviour
{

    bool solving_in_process = false;

    float anim_rot_speed = 0.15f;
    float anim_move_speed = 0.2f;

    bool move_mode = true;
    bool wait = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Engine.check_key(Engine.Key.Maze_Solver)) solving_in_process = true;
        if (!solving_in_process || wait) return;

        if (move_mode) {
            //Debug.Log("Move");
            wait = true;
            move_mode = false;
            transform.DOMove(transform.position + transform.forward, anim_move_speed).OnComplete(()=> wait = false);
        } else {
            //Если справа дырка - лезем в дырку
            if (!Physics.Raycast(transform.position, transform.right, 1f)) {
                wait = true;
                var new_rot2 = transform.rotation.eulerAngles + new Vector3(0f, 90f, 0f);
                transform.DOLocalRotate(new_rot2, anim_rot_speed).OnComplete(()=> { wait = false; move_mode = true; });
                return; 
            }

            //Если справа дырки нет, но можно вперёд - идём вперёд
            if (!Physics.Raycast(transform.position, transform.forward, 1f)) {
                move_mode = true; return;
            }

            //Если и вперёд нельзя - тыкаемся влево
            if (!Physics.Raycast(transform.position, -transform.right, 1f)) {
                wait = true;
                var new_rot2 = transform.rotation.eulerAngles + new Vector3(0f, -90f, 0f);
                transform.DOLocalRotate(new_rot2, anim_rot_speed).OnComplete(()=> { wait = false; move_mode = true; });
                return; 
            }

            //Если в тупике - то разворачиваемся
            wait = true;
            var new_rot3 = transform.rotation.eulerAngles + new Vector3(0f, -180f, 0f);
            transform.DOLocalRotate(new_rot3, anim_rot_speed).OnComplete(()=> { wait = false; move_mode = true; });
        }
    }
}
