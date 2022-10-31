using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Spirit> wildSpirits;

    public Spirit GetRandomWildSpirit()
    {
        //When called, picks random spirit and returns it
        var wildSpirit = wildSpirits[Random.Range(0, wildSpirits.Count)];
        return wildSpirit;
    }
}
