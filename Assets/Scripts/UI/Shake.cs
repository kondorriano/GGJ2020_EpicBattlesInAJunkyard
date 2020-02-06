using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{ 
    RectTransform rect;
    Vector2 initAnchoredPos;
    Quaternion initRotation;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        initAnchoredPos = rect.anchoredPosition;
        initRotation = rect.rotation;
    }

    void Update()
    {
        rect.anchoredPosition = initAnchoredPos + new Vector2(Random.Range(-8, 8), Random.Range(-8, 8));
        rect.rotation = initRotation * Quaternion.Euler(0, 0, Random.Range(-1, 1));
    }
}
