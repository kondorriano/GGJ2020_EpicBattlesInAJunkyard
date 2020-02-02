using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{ 
    RectTransform rect;
    Vector2 initAnchoredPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        initAnchoredPos = rect.anchoredPosition;
    }

    void Update()
    {
        rect.anchoredPosition = initAnchoredPos + new Vector2(Random.Range(-8, 8), Random.Range(-8, 8));
    }
}
