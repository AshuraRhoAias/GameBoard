using UnityEditor;

/// <summary>
/// Interfaz para todos los drawers de efecto.
/// </summary>
public interface IEffectDrawer
{
    /// <param name="effectProperty">
    /// SerializedProperty de la sub-clase de datos,
    /// por ejemplo prop.FindPropertyRelative("defeatEffect").
    /// </param>
    void Draw(SerializedProperty effectProperty);
}
