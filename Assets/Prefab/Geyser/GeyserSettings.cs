using UnityEngine;

[CreateAssetMenu(fileName = "GeyserSettings", menuName = "Settings/Geyser Settings")]
public class GeyserSettings : ScriptableObject
{
    [Header("Push Settings")]
    [Tooltip("Базовая сила толчка (acceleration). Настраивай для нужного расстояния полёта.")]
    public float pushForce = 15f;

    [Header("Pyramid Zone Settings")]
    [Tooltip("Высота пирамидальной зоны толчка.")]
    public float pyramidHeight = 8f;
    [Tooltip("Радиус широкой базы на конце зоны.")]
    public float baseRadius = 4f;
}