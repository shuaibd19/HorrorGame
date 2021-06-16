using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// With thanks to Gregory Schlomoff, who wrote this implementation and
// released it into the public domain, and to Robert Bridson of the
// University of British Columbia, for developing the efficient
// algorithm that this code implements.

// http://gregschlom.com/devlog/2014/06/29/Poisson-disc-sampling-Unity.html
// http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf

//Buttfield - Addison, Paris; Manning, Jon; Nugent, Tim.Unity Game Development Cookbook (pp. 450-451). O 'Reilly Media. Kindle Edition. 


//Generates a distribution of 2d points that aren't too lcose to each other 
//operates in in O(N) time
public class PoissonDiscSampler
{
    //maximum number of attemps before marking sampler as inactive
    private const int k = 30;

    //the rectangle in which the points will be places
    private readonly Rect rect;

    //radius squared
    private readonly float radius2;

    //the cell size of the grid of points
    private readonly float cellSize;

    //the grid of points
    private Vector2[,] grid;

    //the list of locations near which we're trying to add new points to
    private List<Vector2> activeSamples = new List<Vector2>();

    //create a smapler with the following parameters
    /// - width: each sample's x coord will be between [0, width]
    /// - height: each sample's y coord will be between [0, height]
    /// - radius: each sample will be at least `radius` units away from 
    /// any other sample, and at most 2 * `radius`
    /// 
    public PoissonDiscSampler(float width, float height, float radius)
    {
        rect = new Rect(0, 0, width, height);
        radius2 = Mathf.Pow(radius, 2.0f);
        cellSize = radius / Mathf.Sqrt(2);
        grid = new Vector2[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];
    }

    //return a lazy sequence of samples
    public IEnumerable<Vector2> Samples()
    {
        //first sample if chosen randomly
        Vector2 firstSample = new Vector2(Random.value * rect.width, Random.value * rect.height);
        yield return AddSample(firstSample);

        while(activeSamples.Count > 0)
        {
            //pick a random active sample
            int i = (int)Random.value * activeSamples.Count;
            Vector2 sample = activeSamples[i];

            //try `k` random candidates between [radius, 2 * radius] from that sample
            bool found = false;
            for(int j = 0; j < k; ++j)
            {
                float angle = 2 * Mathf.PI * Random.value;

                float r = Mathf.Sqrt(Random.value * 3 * (2 * radius2));

                Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                //accept cadidates if it's inside the rect and farther than 2 * radius to any existing sample
                if(rect.Contains(candidate) && IsFarEnough(candidate))
                {
                    found = true;
                    yield return AddSample(candidate);
                    break;
                }
            }

            //if we could not find a valid candidate after k attempts,
            //remove this sample form the active samples queue
            if(!found)
            {
                activeSamples[i] = activeSamples[activeSamples.Count - 1];
                activeSamples.RemoveAt(activeSamples.Count - 1);
            }
        }

    }

    private bool IsFarEnough(Vector2 sample)
    {
        GridPos pos = new GridPos(sample, cellSize);

        int xmin = Mathf.Max(pos.x - 2, 0);
        int ymin = Mathf.Max(pos.y - 2, 0);
        int xmax = Mathf.Min(pos.x + 2, grid.GetLength(0) - 1);
        int ymax = Mathf.Min(pos.y + 2, grid.GetLength(1) - 1);

        for (int y = ymin; y <= ymax; y++)
        {
            for (int x = xmin; x <= xmax; x++)
            {
                Vector2 s = grid[x, y];
                if(s != Vector2.zero)
                {
                    Vector2 d = s - sample;
                    if (d.x * d.x + d.y * d.y < radius2) return false;
                }
            }
        }
        return true;

        ///Note: we use the zero vector to deonte an unfilled cell in 
        ///the grid. this means that if we were to randomly pcik (0, 0)
        ///as a sample, it would be ignored for the purposes of proximity-testing 
        ///and we might end up with another sample too close from (0, 0). This is a 
        ///very minor issue
    }

    //adds the sample to the active samples queue and the grid before returning it
    private Vector2 AddSample(Vector2 sample)
    {
        activeSamples.Add(sample);
        GridPos pos = new GridPos(sample, cellSize);
        grid[pos.x, pos.y] = sample;
        return sample;
    }

    //helper struct to calcualte the x and y indices of a sample in the grid
    private struct GridPos
    {
        public int x;
        public int y;

        //constructor
        public GridPos(Vector2 sample, float cellSize)
        {
            x = (int)(sample.x / cellSize);
            y = (int)(sample.y / cellSize);
        }
    }
}
