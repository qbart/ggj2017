using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    public Sprite[] spriteUp;
    public Sprite[] spriteReel;
    public Sprite[] spriteDown;

    public void setFrame(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
