using UnityEngine;
using System.Collections;

public class getRealDisplayScreens : MonoBehaviour {

    public Color color = Color.white;
    public bool depthTest = true;

    void Update()
    {
        int nodeCount = getReal3D.Plugin.getNodeCount();
        for (int i = 0; i < nodeCount; ++i )
        {
            int instanceCount = getReal3D.Plugin.getNodeConfigCount(i);
            for (int j = 0; j < instanceCount; ++j)
            {
                int screenCount = getReal3D.Plugin.getScreenCount(i,j);
                for (int k = 0; k < screenCount; ++k)
                {
                    Vector3[] corners = getReal3D.Plugin.getScreenCoordinates((uint)i, (uint)j, (uint)k);
                    Debug.DrawLine(transform.TransformPoint(corners[0]), transform.TransformPoint(corners[1]), color, 0.0f, depthTest);
                    Debug.DrawLine(transform.TransformPoint(corners[1]), transform.TransformPoint(corners[2]), color, 0.0f, depthTest);
                    Debug.DrawLine(transform.TransformPoint(corners[2]), transform.TransformPoint(corners[3]), color, 0.0f, depthTest);
                    Debug.DrawLine(transform.TransformPoint(corners[3]), transform.TransformPoint(corners[0]), color, 0.0f, depthTest);
                }
            }
        }
    }
}
