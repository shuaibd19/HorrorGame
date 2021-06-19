using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLandscapeGenerator : AbstractLandscapeMeshGenerator
{
    [SerializeField] private float uvScale = 1;
    [SerializeField] private Gradient gradient;
    [SerializeField] private float gradMax = 5;
    [SerializeField] private float gradMin = -2;


    protected override void setMeshNums()
    {
        //number of vertices in x direction multiplied by the number in z
        numVertices = (xResolution + 1) * (zResolution + 1);
        //This is 3 ints per geometric triangle * 2 geometric triangles per square * the number of triangles needed in the x direction
        //* in the z direction
        numTriangles = 6 * xResolution * zResolution;
    }

    protected override void SetVertices()
    {
        float xx, y, zz = 0;
        Vector3[] vs = new Vector3[numVertices];

        NoiseGenerator noise = new NoiseGenerator(octaves, lacunarity, gain, perlinScale);

        for (int z = 0; z <= zResolution; z++)
        {
            for (int x = 0; x <= xResolution; x++)
            {
                xx = ((float)x / xResolution) * meshScale;
                zz = ((float)z / zResolution) * meshScale;
                y = yScale * noise.GetFractalNoise(xx, zz);
                y = FallOff((float)x, y, (float)z);

                vertices.Add(new Vector3(xx, y, zz));
            }

        }
    }

    protected override void SetTriangles()
    {
        int triCount = 0;
        for (int z = 0; z < zResolution; z++)
        {
            for (int x = 0; x < xResolution; x++)
            {
                triangles.Add(triCount);
                triangles.Add(triCount + xResolution + 1);
                triangles.Add(triCount + 1);

                triangles.Add(triCount + 1);
                triangles.Add(triCount + xResolution + 1);
                triangles.Add(triCount + xResolution + 2);

                triCount++;
            }
            triCount++;
        }
    }

    protected override void SetUVs()
    {

        for (int z = 0; z <= zResolution; z++)
        {
            for (int x = 0; x <= xResolution; x++)
            {
                uvs.Add(new Vector2(x / (uvScale * xResolution), z / (uvScale * zResolution)));
            }
        }
    }

    protected override void SetNormals()
    {
        SetGeneralNormals();
    }

    protected override void SetTangents()
    {
        SetGeneralTangents();
    }

    protected override void SetVertexColours()
    {
        float diff = gradMax - gradMin;
        for (int i = 0; i < numVertices; i++)
        {
            vertexColours.Add(gradient.Evaluate((vertices[i].y - gradMin) / diff));
        }
    }
}
