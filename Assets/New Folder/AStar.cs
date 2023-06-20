using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class AStar : MonoBehaviour
{
    public int mapWidth = 8;
    public int mapHeight = 6;
    public Point[,] map;
    void Start()
    {
        InitMap();
        Point start = map[2, 3];
        Point end = map[6, 3];
        FindPath(start, end);
        ShowPath(start, end);
    }

    /// <summary>
    /// �������������� �����
    /// </summary>
    private void InitMap()
    {
        map = new Point[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y] = new Point(x, y);
            }
        }
        map[4, 2].IsWall = true;
        map[4, 3].IsWall = true;
        map[4, 4].IsWall = true;
    }

    /// <summary>
    /// �������� ���� ��� �����
    /// </summary>
    private void CreateCube(int x, int y, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(x, y, 0);
        go.GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// �������� ���������� ����
    /// </summary>
    private void ShowPath(Point start, Point end)
    {
        Point temp = end;
        while (true)
        {
            //Debug.Log(temp.X + "-" + temp.Y);
            Color color = Color.gray;
            if (temp == start)
            {
                color = Color.green;
            }
            if (temp == end)
            {
                color = Color.red;
            }
            CreateCube(temp.X, temp.Y, color);
            if (temp.Parent == null)
                break;
            temp = temp.Parent;
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map[x, y].IsWall)
                {
                    CreateCube(x, y, Color.blue);
                }
            }
        }
    }

    /// <summary>
    /// ����� ����
    /// </summary>
    /// <param name = "start"> ��������� ����� </param>
    /// <param name = "end"> ������� ����� </param>
    private void FindPath(Point start, Point end)
    {
        List<Point> openList = new List<Point>();
        List<Point> closeList = new List<Point>();
        openList.Add(start);
        while (openList.Count > 0)
        {
            Point point = FindMinFOfPoint(openList);
            openList.Remove(point);
            closeList.Add(point);
            List<Point> surroundPointsList = GetSurroundPoints(point);
            PointsFilter(surroundPointsList, closeList);
            foreach (Point surroundPoint in surroundPointsList)
            {
                if (openList.IndexOf(surroundPoint) > -1)
                {
                    float nowG = CalculateG(surroundPoint, point);
                    if (nowG < surroundPoint.G)
                    {
                        surroundPoint.UpdateParent(point, nowG);
                    }
                }
                else
                {
                    surroundPoint.Parent = point;
                    CalculateF(surroundPoint, end);
                    openList.Add(surroundPoint);
                }
            }
            // �������, �������� �� �� ������� �����
            if (openList.IndexOf(end) > -1)
            {
                break;
            }
        }
    }

    /// <summary>
    /// ������������� ����� � �������� ������
    /// </summary>
    private void PointsFilter(List<Point> src, List<Point> closePoint)
    {
        foreach (Point p in closePoint)
        {
            if (src.IndexOf(p) > -1)
            {
                src.Remove(p);
            }
        }
    }

    private List<Point> GetSurroundPoints(Point point)
    {
        Point up = null, down = null, left = null, right = null;
        Point lu = null, ld = null, ru = null, rd = null;
        // ��������� ����� �����, ����, ����� � ������
        if (point.Y < mapHeight - 1)
        {
            up = map[point.X, point.Y + 1];
        }
        if (point.Y > 0)
        {
            down = map[point.X, point.Y - 1];
        }
        if (point.X > 0)
        {
            left = map[point.X - 1, point.Y];
        }
        if (point.X < mapWidth - 1)
        {
            right = map[point.X + 1, point.Y];
        }
        // �������� ������� �����, ������ �����, ������� ������ � ������ ������ �����
        if (left != null && up != null)
        {
            lu = map[point.X - 1, point.Y + 1];
        }
        if (left != null && down != null)
        {
            ld = map[point.X - 1, point.Y - 1];
        }
        if (right != null && up != null)
        {
            ru = map[point.X + 1, point.Y + 1];
        }
        if (right != null && down != null)
        {
            rd = map[point.X + 1, point.Y - 1];
        }

        List<Point> pointList = new List<Point>();
        // ���� ���������� ����� �� �������� ������, �� ������ ������
        if (up != null && up.IsWall == false)
        {
            pointList.Add(up);
        }
        if (down != null && down.IsWall == false)
        {
            pointList.Add(down);
        }
        if (left != null && left.IsWall == false)
        {
            pointList.Add(left);
        }
        if (right != null && right.IsWall == false)
        {
            pointList.Add(right);
        }
        // ����� ������ � ��� ����
        if (lu != null && lu.IsWall == false && left.IsWall == false && up.IsWall == false)
        {
            pointList.Add(lu);
        }
        if (ld != null && ld.IsWall == false && left.IsWall == false && down.IsWall == false)
        {
            pointList.Add(ld);
        }
        if (ru != null && ru.IsWall == false && right.IsWall == false && up.IsWall == false)
        {
            pointList.Add(ru);
        }
        if (rd != null && rd.IsWall == false && right.IsWall == false && down.IsWall == false)
        {
            pointList.Add(rd);
        }
        return pointList;
    }

    /// <summary>
    /// ������� ���������� �������� F � �������� ������
    /// </summary>
    /// <param name="openList"></param>
    /// <returns></returns>
    private Point FindMinFOfPoint(List<Point> openList)
    {
        float f = float.MaxValue;
        Point temp = null;
        foreach (Point p in openList)
        {
            if (p.F < f)
            {
                temp = p;
                f = p.F;
            }
        }
        return temp;
    }

    /// <summary>
    /// ������������ �������� F
    /// </summary>
    /// <param name = "now"> ������� ������� </param>
    /// <param name = "end"> ������� �������������� </param>
    private void CalculateF(Point now, Point end)
    {
        //F = G + H
        float h = Mathf.Abs(end.X - now.X) + Mathf.Abs(end.Y - now.Y);
        float g = 0;
        if (now.Parent == null)
        {
            g = 0;
        }
        else
        {
            g = Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(now.Parent.X, now.Parent.Y)) + now.Parent.G;
        }
        float f = g + h;
        now.F = f;
        now.G = g;
        now.H = h;
    }

    /// <summary>
    /// ������������ �������� F
    /// </summary>
    private float CalculateG(Point now, Point parent)
    {
        return Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(parent.X, parent.Y)) + parent.G;
    }
}

