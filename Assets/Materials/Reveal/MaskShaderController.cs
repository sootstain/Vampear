using UnityEngine;

public class MaskShaderController : MonoBehaviour
{
    public Material mat;

    // Update is called once per frame
    void Update()
    {
        SetValues();
    }

    public void SetValues()
    {
        mat.SetFloat("_MaskRotation", transform.eulerAngles.y);
        mat.SetFloat("_MaskScale", transform.localScale.y);
        mat.SetVector("_ReferencePosition", transform.position);
    }
}
