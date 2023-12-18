using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    /// <summary>
    /// 网格点位坐标
    /// </summary>
    public  Vector3[] vectors;
    /// <summary>
    /// 点位顺序（参照顺时针三角排列）
    /// </summary>
    public int[] triangles;
    /// <summary>
    /// uv坐标
    /// </summary>
    public Vector2[] uvs;

    private Color[] colors;
    public Gradient gradient;

    public int xSize = 20;
    public int zSize = 20;

    public float firstParlin;
    public float secondParlin;  
    public float thirdParlin;

    public float ParlinScaleY;

    public float TextureScale;
    // Start is called before the first frame update
    void Start()
    {
        mesh =new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //StartCoroutine(CreateShape());
    }
    [ContextMenu("创建")]
    public void CreatOnEdit()
    {
        CreateShape();

    }
    void Update ()
    {
        UpdateMesh();
    }
    /// <summary>
    /// 创建平面
    /// </summary>
    void  CreateShape()
    {
        vectors = new Vector3[(xSize+1)*(zSize+1)];
        //生成网格
        for (int i=0,z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y=Mathf.PerlinNoise(x* firstParlin, z* firstParlin)  * Mathf.PerlinNoise(x* secondParlin, z* secondParlin) *Mathf.PerlinNoise(x * thirdParlin, z * thirdParlin) * ParlinScaleY;
                vectors[i] = new Vector3(x, y, z);
                i++;
            }
        }
       
        triangles=new int[xSize*zSize*6];
        //连接三角面
        int tris = 0;
        int vert = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 4] = vert + 1;
                triangles[tris + 5] = vert + xSize + 1;
                triangles[tris + 3] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        uvs = new Vector2[vectors.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x/xSize* TextureScale, (float)z/zSize* TextureScale);
                i++;
            }
        }

        colors = new Color[vectors.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = vectors[i].y/ ParlinScaleY;
                colors[i] = gradient.Evaluate(0.5f);
                i++;
            }
        }

    }
    /// <summary>
    /// 更新网格 
    /// </summary>
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vectors;
        mesh.triangles=triangles;
        mesh.uv = uvs;
        //mesh.colors = colors;
        mesh.RecalculateNormals();
    }
    private void OnDrawGizmos()
    {
        if (vectors==null)
        {
            return;
        }
        for (int i = 0; i < vectors.Length; i++)
        {
           // Gizmos.DrawSphere(vectors[i], 0.1f);
        }
    }
}
