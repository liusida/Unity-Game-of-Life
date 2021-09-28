using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject virtualScreen; // A 3D Quad
    public ComputeShader shader; // The main compute shader

    public int width = 1024;
    public int height = 768;
    public float randomSeed = 0.1f;
    
    [Range(0.001f, 0.999f)]
    public float initThreshold = 0.5f;


    Material mat;
    RenderTexture tex;
    int kernelUpdate;
    uint stepCount;

    // Start is called before the first frame update
    void Start()
    {
        // Create a new material
        // link the material to virtual screen.
        mat = new Material(Shader.Find("Standard"));
        virtualScreen.GetComponent<MeshRenderer>().material = mat;

        // Create a new render texture.
        tex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        tex.enableRandomWrite = true;
        tex.Create();

        // set texture of the material to be the render texture
        mat.SetTexture("_MainTex", tex);

        // pass texture to compute shader
        int kernelInit = shader.FindKernel("Init");

        shader.SetFloat("initThreshold", initThreshold);
        shader.SetFloat("randomSeed", randomSeed);
        shader.SetTexture(kernelInit, "Result", tex);
        shader.Dispatch(kernelInit, width, height, 1);

        kernelUpdate = shader.FindKernel("Update");
        shader.SetTexture(kernelUpdate, "Result", tex);

        stepCount=0;
    }

    // Update is called once per frame
    void Update()
    {
        // run shader
        shader.Dispatch(kernelUpdate, width, height, 1);
        stepCount ++;
        Debug.Log($"stepCount: {stepCount};");
    }
}
