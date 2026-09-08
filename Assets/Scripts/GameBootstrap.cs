using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        GameMapBuilder.Build();
    }
}