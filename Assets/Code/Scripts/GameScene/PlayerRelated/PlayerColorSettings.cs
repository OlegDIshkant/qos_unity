using UnityEngine;

/// <summary>
/// Позволяет установить цвет игрока.
/// </summary>
public class PlayerColorSettings : MonoBehaviour
{

    [SerializeField]
    private Material _yellow;
    [SerializeField]
    private Material _red;
    [SerializeField]
    private Material _green;
    [SerializeField]
    private Material _blue;

    [SerializeField]
    private Renderer _materialRenderer;
    [SerializeField]
    private int _targetMaterialIntex;


    public void SetColor(PlayerAppearence.Colors color)
    {
        var mat = color switch
        {
            PlayerAppearence.Colors.BLUE => _blue,
            PlayerAppearence.Colors.RED => _red,
            PlayerAppearence.Colors.YELOW => _yellow,
            PlayerAppearence.Colors.GREEN => _green,
            _ => throw new System.Exception($"Unknown color : '{color}'.")
        };

        var tmp = _materialRenderer.materials;
        tmp[_targetMaterialIntex] = mat;
        _materialRenderer.materials = tmp;
    }
}