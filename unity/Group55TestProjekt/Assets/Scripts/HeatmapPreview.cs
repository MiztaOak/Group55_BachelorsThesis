using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapPreview : MonoBehaviour {

    private Camera preview;
    [SerializeField] private RenderTexture texture;

    // Start is called before the first frame update
    void Start()
    {
        preview = GetComponent<Camera>();
        preview.targetTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
