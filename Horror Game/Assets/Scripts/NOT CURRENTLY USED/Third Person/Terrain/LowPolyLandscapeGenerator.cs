using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPolyLandscapeGenerator : AbstractLandscapeMeshGenerator
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private float gradMax = 5;
    [SerializeField] private float gradMin = -2;
    protected override void setMeshNums()
    {
        numTriangles = 6 * xResolution * zResolution;
        numVertices = numTriangles;
    }

    protected override void SetVertices()
    {
        NoiseGenerator noise = new NoiseGenerator(octaves, lacunarity, gain, perlinScale);

        int xx = 0;
        int zz = 0;
        bool isBottomTriangle = false;

        for (int vertexIndex = 0; vertexIndex < numVertices; vertexIndex++)
        {

            //increment xx and zz appropriately
            //check if it's a bottom or top of a triangle
            if (isNewRow(vertexIndex))
            {
                isBottomTriangle = !isBottomTriangle;
            }
            //increase xx by one when it's a new position
            if (!isNewRow(vertexIndex))
            {
                if (isBottomTriangle)
                {
                    if (vertexIndex % 3 == 1)
                    {
                        xx++;
                    }
                }
                else
                {
                    if (vertexIndex % 3 == 2)
                    {
                        xx++;
                    }
                }
            }
            //increase zz by one when it's a new row. Reset xx to zero
            if (isNewRow(vertexIndex))
            {
                //reset xx on new new row
                xx = 0;

                //actually go up a level
                if (!isBottomTriangle)
                {
                    zz++;
                }
            }

            float xVal = ((float)xx / xResolution) * meshScale;
            float zVal = ((float)zz / zResolution) * meshScale;
            float y = yScale * noise.GetFractalNoise(xVal, zVal);
            y = FallOff((float)xx, y, (float)zz);

            vertices.Add(new Vector3(xVal, y, zVal));
        }
    }

    private bool isNewRow(int vertexIndex)
    {
        return vertexIndex % (3 * xResolution) == 0;
    }

    protected override void SetTriangles()
    {
        int triCount = 0;
        for (int z = 0; z < zResolution; z++)
        {
            for (int x = 0; x < xResolution; x++)
            {
                triangles.Add(triCount);
                triangles.Add(triCount + 3 * xResolution);
                triangles.Add(triCount + 1);

                triangles.Add(triCount + 2);
                triangles.Add(triCount + 3 * xResolution + 1);
                triangles.Add(triCount + 3 * xResolution + 2);

                triCount += 3;
            }
            triCount += 3 * xResolution;
        }
    }

    protected override void SetUVs()
    {
    }

    protected override void SetNormals()
    {
    }

    protected override void SetTangents()
    {
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
