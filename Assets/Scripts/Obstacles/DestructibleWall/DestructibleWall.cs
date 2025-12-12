using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("Parts")]
    public GameObject[] wallParts; // Заполни в инспекторе или авто-найдёт детей

    [Header("Explosion")]
    public float explosionForce = 12f;
    public float explosionRadius = 5f;
    public float upwardsModifier = 2f;

    [Header("Collider")]
    public Collider wallCollider; // Основной коллайдер стены (если отдельный)

    private Vector3[] originalLocalPositions;
    private Quaternion[] originalLocalRotations;
    private Vector3[] originalLocalScales;
    public bool isDestroyed = false;

    private void Awake()
    {
        // Авто-находим части, если не заполнены
        if (wallParts == null || wallParts.Length == 0)
        {
            wallParts = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                wallParts[i] = transform.GetChild(i).gameObject;
            }
        }

        // Сохраняем исходные данные
        originalLocalPositions = new Vector3[wallParts.Length];
        originalLocalRotations = new Quaternion[wallParts.Length];
        originalLocalScales = new Vector3[wallParts.Length];

        for (int i = 0; i < wallParts.Length; i++)
        {
            if (wallParts[i] == null) continue;
            Transform t = wallParts[i].transform;
            originalLocalPositions[i] = t.localPosition;
            originalLocalRotations[i] = t.localRotation;
            originalLocalScales[i] = t.localScale;
        }
    }

    private void Start()
    {
        // КРИТИЧЕСКИЙ ФИКС: сбрасываем стену при загрузке уровня
        ResetWall();

        // Включаем коллайдер, если задан
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }
    }

    private void Update()
    {
        // Для теста: R = перезагрузить стену
        if (isDestroyed && Input.GetKeyDown(KeyCode.R))
        {
            ResetWall();
        }
    }

    public void DestroyWall()
    {
        if (isDestroyed) return;
        Explode(explosionForce, transform.position);
    }

    public void DestroyWallFromSlam(Vector3 impactPoint)
    {
        if (isDestroyed) return;
        Explode(explosionForce * 1.3f, impactPoint);
    }

    private void Explode(float force, Vector3 center)
    {
        isDestroyed = true;

        // Отключаем коллайдеры стены
        if (wallCollider != null)
            wallCollider.enabled = false;
        Collider mainCol = GetComponent<Collider>();
        if (mainCol != null)
            mainCol.enabled = false;

        foreach (GameObject part in wallParts)
        {
            if (part == null) continue;

            // ФИКС scale: принудительно восстанавливаем перед физикой
            Transform t = part.transform;
            int index = System.Array.IndexOf(wallParts, part);
            t.localScale = originalLocalScales[index];

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb == null)
                rb = part.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Взрыв
            rb.AddExplosionForce(force, center, explosionRadius, upwardsModifier, ForceMode.Impulse);

            // Слой игнора для игрока
            part.layer = LayerMask.NameToLayer("IgnorePlayer");
        }
    }

    public void ResetWall()
    {
        isDestroyed = false;

        // Включаем коллайдеры
        if (wallCollider != null)
            wallCollider.enabled = true;
        Collider mainCol = GetComponent<Collider>();
        if (mainCol != null)
            mainCol.enabled = true;

        for (int i = 0; i < wallParts.Length; i++)
        {
            GameObject part = wallParts[i];
            if (part == null) continue;

            Transform t = part.transform;
            t.localPosition = originalLocalPositions[i];
            t.localRotation = originalLocalRotations[i];
            t.localScale = originalLocalScales[i];

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            part.layer = LayerMask.NameToLayer("Default");
        }
    }
}