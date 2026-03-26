using NUnit.Framework;
using System.Diagnostics;
using UnityEngine;

public class CharacterCreationMenuSelectionHighlighter : MonoBehaviour
{
    public int part;
    Transform category;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Trace.Assert(part > 0);
        Trace.Assert(part < 4);
        category = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = category.Find(PlayerHealth.body[part].ToString()).position;
    }
}
