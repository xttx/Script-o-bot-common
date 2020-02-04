using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Floor_RandomWidth : MonoBehaviour, Engine.ISetable, Engine.IResetable
{
    public BoxCollider col = null;
    public int min_width = 1;
    public int max_width = 10;

    public UnityEvent OnRandomize = null;

    List<GameObject> obj = new List<GameObject>();

    void OnEnable() { Set(); }

    public void Set(){
        for(int i = 0; i < obj.Count; i++) { Destroy(obj[i]); }
        obj.Clear();

        var pos = col.transform.position;
        int w = Random.Range(min_width, max_width + 1);
        var inst = transform.GetChild(0).gameObject;
        for (int i = 0; i < w; i++) { 
            var new_go = Instantiate(inst, transform);
            new_go.transform.position = new Vector3(i+1f, pos.y, pos.z);
            obj.Add(new_go);
        }

        if (col != null) {
            float x = ((float)(w + 1) / 2f) - 0.5f;
            col.transform.localScale = new Vector3(w+1, 1f, 1f);
            col.transform.localPosition = new Vector3(x, pos.y, pos.z);
        }

        if (OnRandomize != null) OnRandomize.Invoke();
    }
    public void Reset(){
        
    }
}
