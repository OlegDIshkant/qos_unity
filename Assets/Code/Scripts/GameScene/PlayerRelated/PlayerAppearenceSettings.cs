using System;
using UnityEngine;


/// <summary>
/// —крипт, предоставл€ющий методы дл€ настройки внешности персонажа.
/// </summary>
public class PlayerAppearenceSettings : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Object armbeePrefab;
    [SerializeField]
    private UnityEngine.Object birdPrefab;


    public void SetUp(PlayerAppearence playerAppearence)
    {
        var prototype = playerAppearence.Type switch
        {
            PlayerAppearence.Types.ARMBEE => armbeePrefab,
            PlayerAppearence.Types.BIRD => birdPrefab,
            _ => throw new Exception($"Unknown type '{playerAppearence.Type}'.")
        };

        var go = (GameObject)UnityEngine.Object.Instantiate(prototype, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(this.transform, worldPositionStays: false);

        go.GetComponent<PlayerColorSettings>().SetColor(playerAppearence.Color);
    }

}

