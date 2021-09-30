using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public ComputeShader shader;
    public float initThreshold=0.3f;
    public Color cellColor, emptyColor;

    RenderTexture tex1, tex2;
    int kernelCSInit, kernelCSMain;
    bool isOddFrame;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Screen Width : " + Screen.width);
        Debug.Log("Screen Height : " + Screen.height);

        tex1 = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
        tex1.enableRandomWrite = true;
        tex1.Create();
        tex2 = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
        tex2.enableRandomWrite = true;
        tex2.Create();

        kernelCSInit = shader.FindKernel("CSInit");
        kernelCSMain = shader.FindKernel("CSMain");

        shader.SetFloat("randomSeed", 0);
        shader.SetFloat("initThreshold", initThreshold);
        shader.SetVector("cellColor", cellColor);
        shader.SetVector("emptyColor", emptyColor);

        shader.SetTexture(kernelCSInit, "tex1", tex1);
        shader.SetTexture(kernelCSInit, "tex2", tex2);
        shader.Dispatch(kernelCSInit,tex1.width,tex1.height,1);

        shader.SetTexture(kernelCSMain, "tex1", tex1);
        shader.SetTexture(kernelCSMain, "tex2", tex2);

        isOddFrame = true;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        // Debug.Log($"isOddFrame: {isOddFrame}.");
        shader.SetBool("isOddFrame", isOddFrame);
        shader.Dispatch(kernelCSMain,tex1.width,tex1.height,1);
        if (isOddFrame) {
            Graphics.Blit(tex1, dest);
        } else {
            Graphics.Blit(tex2, dest);
        }
        isOddFrame = !isOddFrame;
    }
}
