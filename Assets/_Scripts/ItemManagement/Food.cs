using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public float nourishment = .5f;
    public Taste primaryTaste = Taste.None;
    public Taste secondaryTaste = Taste.None;

    public enum Taste
    {
        None,
        Sweet,
        Savoury,
        Sour,
        Salty,
        Spicy
    }

    public enum Trait
    {
        Creamy,
        Fresh,
        Healthy
    }
}
