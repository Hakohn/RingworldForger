using UnityEngine;

[CreateAssetMenu(menuName = "Ringworld Forger/Ring Material Palette", fileName = "New Ringworld Material Palette")]
public class RingMaterialPaletteSO : ScriptableObject
{
    public Material[] outer = null;
    public Material[] ocean = null;
    public Material[] continent = null;
}
