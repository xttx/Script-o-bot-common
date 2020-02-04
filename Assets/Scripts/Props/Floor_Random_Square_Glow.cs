using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor_Random_Square_Glow : MonoBehaviour
{

    Material mat;
    Texture2D texture;
    int half_border_size = 14;
    int square_size = 793;
    int skip = 0;

    List<Color[]> pattern = new List<Color[]>();

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        texture = new Texture2D(512, 512);
        texture.Apply();
        mat.SetTexture ("_EmissionMap", texture);
        mat.SetColor ("_EmissionColor", Color.white);

        //From bottom to up
        pattern.Add(new Color[]{Color.green, Color.yellow, Color.red, Color.green, Color.blue});
        pattern.Add(new Color[]{Color.yellow, Color.blue, Color.green, Color.red, Color.green});
        pattern.Add(new Color[]{Color.red, Color.green, Color.red, Color.green, Color.red});
        pattern.Add(new Color[]{Color.green, Color.yellow, Color.blue, Color.red, Color.yellow});
        pattern.Add(new Color[]{Color.blue, Color.red, Color.green, Color.yellow, Color.blue});
    }

    // Update is called once per frame
    void Update()
    {
        skip += 1;
        if (skip < 5) return;
        skip = 0;

        //reset
        for (int x = 0; x <= 512; x++)
            for (int y = 0; y <= 512; y++)
                texture.SetPixel(x, y, new Color(0f, 0f, 0f, 1f));

        int col_num = Random.Range(1, 5);
        int row_num = Random.Range(1, 5);

        int x_from = half_border_size + (half_border_size * 2 * (col_num-1)) + (square_size * (col_num-1));
        x_from = (int)Mathf.Round(x_from / 8);
        int y_from = half_border_size + (half_border_size * 2 * (row_num-1)) + (square_size * (row_num-1));
        y_from = (int)Mathf.Round(y_from / 8);

        int x_to = x_from + (int)Mathf.Round(square_size / 8);
        int y_to = y_from + (int)Mathf.Round(square_size / 8);
        for (int x = x_from; x <= x_to; x++)
        {
            for (int y = y_from; y <= y_to; y++)
            {
                Color c = pattern[row_num-1][col_num-1];
                texture.SetPixel(x, y, new Color(c.r * 2f, c.g * 2f, c.b * 2f, 1f));
            }
        }
        texture.Apply();
    }
}
