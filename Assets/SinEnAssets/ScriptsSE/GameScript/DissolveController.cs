using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DissolveController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] skinnedMaterials;
    public float dissolveRate = 0.0125f;
    
    void Start()
    {
        if (skinnedMeshRenderer != null)
        {
            skinnedMaterials = skinnedMeshRenderer.materials;
        }
    }

    public IEnumerator DissolveEffect()
    {
        if (skinnedMaterials.Length >0)
        {
            float counter = 0;
            while (skinnedMaterials[0].GetFloat("_Dissolve_Amount") < 1)
            {
                counter += dissolveRate;
                for (int i=0; i<skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_Dissolve_Amount", counter);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
