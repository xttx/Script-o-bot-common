using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor_RandomPotatoes : MonoBehaviour
{
    public int min = 1;
    public int max = 4;

    public Vector3 size_min = new Vector3(0.075f, 0.075f, 0.075f);
    public Vector3 size_max = new Vector3(0.12f, 0.12f, 0.12f);

    List<GameObject> instantiated = new List<GameObject>();
    [HideInInspector] public int[,] potatoes = new int[,]{};

    //void OnEnable() { RandomizePotatoes(); }

    public void RandomizePotatoes() {
        for(int i = 0; i < instantiated.Count; i++) { Destroy(instantiated[i]); }
        instantiated.Clear();

        var Start_Cell = transform.localPosition - new Vector3((transform.localScale.x / 2f) - 0.5f, 0f, (transform.localScale.z / 2f) - 0.5f );
        var potato = transform.GetChild(0).gameObject;
        Vector2Int sz = new Vector2Int(Mathf.RoundToInt(transform.localScale.x), Mathf.RoundToInt(transform.localScale.z));
        potatoes = new int[sz.x,sz.y];
        for (int x = 0; x < sz.x; x++) {
            for (int z = 0; z < sz.y; z++) {
                int r = Random.Range(min, max + 1);
                potatoes[x,z] = r;
                Vector3 cell = Start_Cell + new Vector3(x, 0f, z);

                for (int c = 0; c < r; c++) {
                    var offset = new Vector2 (Random.Range(0f, 0.3f), Random.Range(0f, 0.3f));
                    var new_potato = Instantiate(potato, transform.parent);
                    new_potato.transform.localPosition = new Vector3(cell.x + offset.x, 0f, cell.z + offset.y);
                    new_potato.transform.localScale = new Vector3( Random.Range(size_min.x, size_max.x), Random.Range(size_min.y, size_max.y), Random.Range(size_min.z, size_max.z) );
                    new_potato.SetActive(true);
                    instantiated.Add(new_potato);
                }
            }
        }
    }

    public int GetCellPotatoCount() {
        var bot_pos = BOT.bot_obj.transform.position;
        if (bot_pos.y < transform.position.y + 0.5f) return 0;
        if (bot_pos.y > transform.position.y + 1f) return 0;
        
        var cell = bot_pos - transform.position + (transform.localScale / 2f) - (Vector3.one / 2f);
        int x = Mathf.RoundToInt(cell.x);
        int y = Mathf.RoundToInt(cell.z);
        //Debug.Log("cell = " + cell + ", x = " + x + ", y = " + y + ", potatoes_arr_dimensions = " + potatoes.GetUpperBound(0) + "x" + potatoes.GetUpperBound(1));
        if (x < 0 || y < 0) return 0;
        if (potatoes.GetUpperBound(0) < x) return 0;
        if (potatoes.GetUpperBound(1) < y) return 0;

        return potatoes[x,y];
    }
}
