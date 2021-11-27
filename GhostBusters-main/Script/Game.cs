using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class WeightedRandomBag<T>
{
    private struct Entry
    {
        public double accumulatedWeight;

        public T item;
    }

    private List<Entry> entries = new List<Entry>();

    private double accumulatedWeight;

    private System.Random rand = new System.Random();

    public void AddEntry(T item, double weight)
    {
        accumulatedWeight += weight;
        entries
            .Add(new Entry {
                item = item,
                accumulatedWeight = accumulatedWeight
            });
    }

    public T GetRandom()
    {
        double r = rand.NextDouble() * accumulatedWeight;

        foreach (Entry entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }
}

public class Game : MonoBehaviour
{
    public int height = 6;
    public int width = 6;
    public Tile[,] grid;
    public GameObject[,] Textgrid;
    public double probabilitycount = 2;
    public int gx;
    public int gy;
    public int lastcheckedX;
    public int lastcheckedY;
    public int NumberOfTiles = 0;
    public int NumberOfClicks = 0;

    public String displayed_color(int DistanceFromGhost)
    {
        WeightedRandomBag<String> color_noise = new WeightedRandomBag<String>();

        if (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        {
            color_noise.AddEntry("yellow", 0.6);
            color_noise.AddEntry("orange", 0.150);
            color_noise.AddEntry("red", 0.050);
            color_noise.AddEntry("green", 0.200);

            return color_noise.GetRandom();
        }
        else if (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        {
            color_noise.AddEntry("yellow", 0.150);
            color_noise.AddEntry("orange", 0.600);
            color_noise.AddEntry("red", 0.2);
            color_noise.AddEntry("green", 0.050);

            return color_noise.GetRandom();
        }
        else if (DistanceFromGhost == 0)
        {
            color_noise.AddEntry("yellow", 0.10);
            color_noise.AddEntry("orange", 0.200);
            color_noise.AddEntry("red", 0.6500);
            color_noise.AddEntry("green", 0.050);

            return color_noise.GetRandom();
        }
        else if (DistanceFromGhost >= 5)
        {
            color_noise.AddEntry("yellow", 0.250);
            color_noise.AddEntry("orange", 0.100);
            color_noise.AddEntry("red", 0.050);
            color_noise.AddEntry("green", 0.600);

            return color_noise.GetRandom();
        }
        else
            return "green";
    }

    public float JointTableProbability(string color, int DistanceFromGhost)
    {
        //Table 1
        if (
            color.Equals("yellow") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.6f;
        if (
            color.Equals("red") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.05f;
        if (
            color.Equals("green") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.2f;
        if (
            color.Equals("orange") &&
            (DistanceFromGhost == 3 || DistanceFromGhost == 4)
        ) return 0.15f;

        //Table2
        if (
            color.Equals("yellow") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.15f;
        if (
            color.Equals("red") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.2f;
        if (
            color.Equals("green") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.05f;
        if (
            color.Equals("orange") &&
            (DistanceFromGhost == 1 || DistanceFromGhost == 2)
        ) return 0.6f;

        //Table3
        if (color.Equals("yellow") && DistanceFromGhost >= 5) return 0.25f;
        if (color.Equals("red") && DistanceFromGhost >= 5) return 0.05f;
        if (color.Equals("green") && DistanceFromGhost >= 5) return 0.6f;
        if (color.Equals("orange") && DistanceFromGhost >= 5) return 0.1f;

        //Table4
        if (color.Equals("red") && DistanceFromGhost == 0) return 0.6500f;
        if (color.Equals("yellow") && DistanceFromGhost == 0) return 0.10f;
        if (color.Equals("green") && DistanceFromGhost == 0) return 0.05f;
        if (color.Equals("orange") && DistanceFromGhost == 0) return 0.2f;

        return 0;
    }

    void Start()
    {
        this.grid = new Tile[6, 6];
        this.Textgrid = new GameObject[6, 6];
        PlaceGhost();
    }

    public void CheckInputGrid()
    {
        int
            Distance = 0,
            DistanceX = 0,
            DistanceY = 0;

        if (Input.GetButtonDown("Fire1"))
        {

            Vector3 mousePosition =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int a = Mathf.RoundToInt(mousePosition.x);
            int b = Mathf.RoundToInt(mousePosition.y);
            if (a > 6 || b > 6 || a < 0 || b < 0)
            {
                return;
            }
            NumberOfClicks++;
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mousePosition.x);
            int y = Mathf.RoundToInt(mousePosition.y);
            lastcheckedX = x;
            lastcheckedY = y;

            DistanceX = Math.Abs(lastcheckedX - gx);

            DistanceY = Math.Abs(lastcheckedY - gy);

            Distance = DistanceX + DistanceY;

            Tile tile = grid[x, y];

            String colorD = displayed_color(Distance);

            float sum = 0;
            float sum2 = 0;

            if (tile.iscovered == true)
            {
                //int G = NumberOfTiles;
                for (int yy = 0; yy < 6; yy++)
                {
                    for (int xx = 0; xx < 6; xx++)
                    {
                        // if (xx != lastcheckedX && yy != lastcheckedY)
                        {
                            DistanceX = Math.Abs(xx - gx);

                            DistanceY = Math.Abs(yy - gy);

                            Distance = DistanceX + DistanceY;

                            float prob = float.Parse(grid[xx, yy].probability.text);
                            String color = displayed_color(Distance);
                            
                            //float noise = JointTableProbability(displayed_color(Distance), Distance);
                            if (NumberOfClicks == 1) {
                                prob = 0.027f;
                            }
                            grid[xx, yy].probability.text =
                                Math
                                    .Round( prob *
                                    JointTableProbability(color,
                                    Distance),4)
                                    .ToString();

                            sum += float.Parse(grid[xx, yy].probability.text);
                        }
                    }
                }
                Debug.Log("sum");
                Debug.Log(sum);
                for (int yy = 0; yy < 6; yy++)
                {
                    for (int xx = 0; xx < 6; xx++)
                    {
                        {
                            float prob = float.Parse(grid[xx, yy].probability.text);
                            
                            grid[xx, yy].probability.text =
                                Math
                                    .Round( prob / sum, 4)
                                    .ToString();

                            // sum2 += float.Parse(grid[xx, yy].probability.text);
                        }
                    }
                }
                //  Debug.Log("sum2");
                // Debug.Log(sum2);

            }
            
            tile.SetIsCovered(false);
        }
    }

    public void PlaceGhost()
    {
        int x = UnityEngine.Random.Range(0, 6);
        int y = UnityEngine.Random.Range(0, 6);
        if (grid[x, y] == null)
        {
            Tile ghostTile =
                Instantiate(Resources.Load("Prefabs/red", typeof (Tile)),
                new Vector3(x, y, 0),
                Quaternion.identity) as
                Tile;
            grid[x, y] = ghostTile;
            grid[x, y].probability.text = "0.027";

            gx = x;
            gy = y;
            Debug.Log("(" + gx + ", " + gy + ")");
            PlaceColor (x, y);
        }
    }

    public void PlaceColor(int X, int Y)
    {
        int
            DistanceX = 0,
            DistanceY = 0,
            Distance = 0;
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                DistanceX = Math.Abs(x - X);

                DistanceY = Math.Abs(y - Y);

                Distance = DistanceX + DistanceY;

                String color = displayed_color(Distance);

                String path = string.Format("Prefabs/{0}", color);

                if (grid[x, y] == null)
                {
                    Tile colorTile =
                        Instantiate(Resources.Load(path, typeof (Tile)),
                        new Vector3(x, y, 0),
                        Quaternion.identity) as
                        Tile;
                    grid[x, y] = colorTile;

                    //float initprob = 0.027;
                    grid[x, y].probability.text = "0.027";
                    NumberOfTiles++;
                }
            }
        }
    }
}