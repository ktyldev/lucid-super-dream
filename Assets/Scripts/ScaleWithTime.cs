using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using TMPro;
using UnityEngine;

public class ScaleWithTime : MonoBehaviour
{
    [SerializeField] private SerialFloat timeSince;

    [SerializeField] private float scale = 1.5f;
    [SerializeField] private float ySquish = -0.15f;
    [SerializeField] private float characterSpacing = 10f;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _text.characterSpacing = timeSince * timeSince * characterSpacing;
        transform.localScale = new Vector3(1 + timeSince*timeSince * scale, 1 + timeSince*timeSince * ySquish, 1);
    }
}
