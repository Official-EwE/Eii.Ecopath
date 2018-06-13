using System;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

class Rasterizer
{
    private static float intConversion = 100000000000000.0f;
    private static float sqrt2 = 1.41421356237f;

    public static double[,] Rasterize(List<WeightedLayer> layers, int rasterSize, Rect rasterBounds)
    {
        double[,] output = new double[rasterSize,rasterSize];

        int voffset = (int)rasterBounds.yMin;
        int vsquaresize = (int)(rasterBounds.yMax - rasterBounds.yMin) / rasterSize;
        int hoffset = (int)rasterBounds.xMin;
        int hsquaresize = (int)(rasterBounds.xMax - rasterBounds.xMin) / rasterSize;

        foreach (WeightedLayer layer in layers)
        {
            foreach (List<Vector2> polygon in layer.polygons)
            {
                //Convert to int poly
                List<IntPoint> intpoly = VectorToIntPoint(polygon);

                //Determine squares within bounding box
                long left, right, top, bot;
                GetBounds(intpoly, out left, out right, out top, out bot);
                int xmin = ((int)left - hoffset) / hsquaresize;
                int xmax = ((int)right - hoffset) / hsquaresize;
                int ymin = ((int)bot - voffset) / vsquaresize;
                int ymax = ((int)top - voffset) / vsquaresize;

                //Foreach overlapping square: calculate intersecting area
                for (int x = xmin; x < xmax; x++)
                    for (int y = ymin; y < ymax; y++)
                    {
                        Clipper clipper = new Clipper();

                        //Construct polygon paths (of poly and grid square)
                        clipper.AddPaths(new List<List<IntPoint>>() { intpoly }, PolyType.ptSubject, true);
                        clipper.AddPaths(new List<List<IntPoint>>() { GetSquarePoly(x * hsquaresize, (x + 1) * hsquaresize, y * vsquaresize, (y + 1) * vsquaresize) }, PolyType.ptClip, true);

                        //Calculate intersection
                        List<List<IntPoint>> intersection = new List<List<IntPoint>>();
                        clipper.Execute(ClipType.ctIntersection, intersection);

                        //Calculate part of square that is covered by the intersection and add it to the result
                        if (intersection.Count > 0 && intersection[0].Count > 0)
                            output[x, y] += GetPolygonArea(intersection) / sqrt2 * layer.weight;
                    }
            }
        }

        
        return output;
    }

    /// <summary>
    /// Gets the bounds of a polygon. Returned rect is non-rotated.
    /// </summary>
    public static void GetBounds(List<IntPoint> poly, out long left, out long right, out long top, out long bot)
    {
        left = 0; right = 0; top = 0; bot = 0;
        foreach (IntPoint v in poly)
        {
            if (v.X > right)
                right = v.X;
            else if (v.X < left)
                left = v.X;
            if (v.Y > top)
                top = v.Y;
            else if (v.Y < bot)
                bot = v.Y;
        }
    }

    private static List<IntPoint> VectorToIntPoint(List<Vector2> points)
    {
        List<IntPoint> verts = new List<IntPoint>();

        for (int i = 0; i < points.Count; i++)
        {
            verts.Add(new IntPoint(points[i].x * intConversion, points[i].y * intConversion));
        }

        return verts;
    }

    private static List<IntPoint> GetSquarePoly(long xmin, long xmax, long ymin, long ymax)
    {
        return new List<IntPoint>()
        {
            new IntPoint(ymin, xmin),
            new IntPoint(ymin, xmax),
            new IntPoint(ymax, xmax),
            new IntPoint(ymax, xmin)
        };
    }

    private static double GetPolygonArea(List<List<IntPoint>> polygons)
    {
        double area = 0;
        foreach (List<IntPoint> polygon in polygons)
        {
            for (int i = 0; i < polygon.Count; ++i)
            {
                int j = (i + 1) % polygon.Count;
                area += polygon[i].Y * polygon[j].X - polygon[i].X * polygon[j].Y;
            }
        }
        return Math.Abs(area * 0.5);
    }
}

struct WeightedLayer
{
    public List<List<Vector2>> polygons;
    public double weight;
}

