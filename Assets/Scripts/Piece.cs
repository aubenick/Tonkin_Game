using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private string color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetColor()
    {
        return color;
    }

    public void SetColor(string c)
    {
        color = c;
    }

    public bool IsWhite()
    {
        if (color == "White")
            return true;

        return false;
    }
}
