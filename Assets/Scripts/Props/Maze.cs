using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Maze : MonoBehaviour
{
    //https://stackoverflow.com/questions/38502/whats-a-good-algorithm-to-generate-a-maze
    //http://weblog.jamisbuck.org/under-the-hood/
    //http://weblog.jamisbuck.org/2011/2/7/maze-generation-algorithm-recap.html

    public GameObject wall = null;
    public float wall_y_offset = 0f;
    public Vector2Int size = Vector2Int.zero;

    public Maze_Type_Enum maze_type = Maze_Type_Enum.Recursive_backtracker;
    public Growing_Tree_Select_Cells growing_tree_cell_select = Growing_Tree_Select_Cells.newest_backtracker;
    public enum Maze_Type_Enum {Recursive_backtracker, Eller, Kruskal, Prim, Growing_Tree};
    public enum Growing_Tree_Select_Cells { newest_backtracker, random_prim, oldest_stright, middle }

    public FadeIn_Anim_Enum maze_create_anim = FadeIn_Anim_Enum.from_top_random;
    public enum FadeIn_Anim_Enum {none, from_top, from_top_random};

    public wall_hole_struct Enter = new wall_hole_struct();
    public wall_hole_struct Exit = new wall_hole_struct();
    [System.Serializable]
    public struct wall_hole_struct {
        public direction_enum direction;
        public int index;
    }
    public enum direction_enum {none, left, right, top, bottom}

    cell_info[,] maze = null;
    class cell_info {
        public bool topWall    = true;
        public bool bottomWall = true;
        public bool leftWall   = true;
        public bool rightWall  = true;
        public bool isVisited  = false;
    }

    List<Transform> TList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Engine.check_key(Engine.Key.Maze_Regenerate)) {
            Generate();
        }
    }

    void Generate() {
        foreach (var t in TList) {
            Destroy(t.gameObject);
        }
        TList.Clear();

        if (maze_type == Maze_Type_Enum.Recursive_backtracker) Generate_Recursive_backtracker();
        else if (maze_type == Maze_Type_Enum.Eller) Generate_Eller();
        else if (maze_type == Maze_Type_Enum.Kruskal) Generate_Kruskal();
        else if (maze_type == Maze_Type_Enum.Prim) Generate_prim();
        else if (maze_type == Maze_Type_Enum.Growing_Tree) Generate_GrowingTree();

        if (Enter.direction != direction_enum.none) {
            if (Enter.direction == direction_enum.top)    maze[Enter.index, size.y-1].bottomWall = false;
            if (Enter.direction == direction_enum.bottom) maze[Enter.index, 0].topWall = false;
            if (Enter.direction == direction_enum.left)   maze[0, Enter.index].leftWall = false;
            if (Enter.direction == direction_enum.right)  maze[size.x-1, Enter.index].rightWall = false;
        }
        if (Exit.direction != direction_enum.none) {
            if (Exit.direction == direction_enum.top)    maze[Enter.index, size.y-1].bottomWall = false;
            if (Exit.direction == direction_enum.bottom) maze[Enter.index, 0].topWall = false;
            if (Exit.direction == direction_enum.left)   maze[0, Enter.index].leftWall = false;
            if (Exit.direction == direction_enum.right)  maze[size.x-1, Enter.index].rightWall = false;
        }

        Create();
    }

    void Generate_Recursive_backtracker()
    {
        //https://habr.com/ru/post/262345/

        //Init
        maze = new cell_info[size.x, size.y];
        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                maze[x,y] = new cell_info();
            }
        }

        int visited_count = 1;
        int visited_max = size.x * size.y;
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int cur_cell = Vector2Int.zero;
        maze[cur_cell.x, cur_cell.y].isVisited = true;

        while (visited_count < visited_max) {
            List<Vector2Int> unvisited = new List<Vector2Int>();
            if (cur_cell.x > 0 && maze[cur_cell.x-1, cur_cell.y].isVisited == false)
                unvisited.Add (new Vector2Int(cur_cell.x-1, cur_cell.y));
            if (cur_cell.x < size.x - 1 && maze[cur_cell.x+1, cur_cell.y].isVisited == false) 
                unvisited.Add (new Vector2Int(cur_cell.x+1, cur_cell.y));
            if (cur_cell.y > 0 && maze[cur_cell.x, cur_cell.y-1].isVisited == false)
                unvisited.Add (new Vector2Int(cur_cell.x, cur_cell.y-1));
            if (cur_cell.y < size.y - 1 && maze[cur_cell.x, cur_cell.y+1].isVisited == false)
                unvisited.Add (new Vector2Int(cur_cell.x, cur_cell.y+1));

            if (unvisited.Count > 0) {
                stack.Push(cur_cell);
                int r = Random.Range(0, unvisited.Count);
                var neighbour = unvisited[r];
                if (neighbour.x < cur_cell.x) {
                    maze[cur_cell.x, cur_cell.y].leftWall = false;
                    maze[neighbour.x, neighbour.y].rightWall = false;
                } else if (neighbour.x > cur_cell.x) {
                    maze[cur_cell.x, cur_cell.y].rightWall = false;
                    maze[neighbour.x, neighbour.y].leftWall = false;
                } else if (neighbour.y > cur_cell.y) {
                    maze[cur_cell.x, cur_cell.y].bottomWall = false;
                    maze[neighbour.x, neighbour.y].topWall = false;
                } else if (neighbour.y < cur_cell.y) {
                    maze[cur_cell.x, cur_cell.y].topWall = false;
                    maze[neighbour.x, neighbour.y].bottomWall = false;
                }

                maze[neighbour.x, neighbour.y].isVisited = true;
                cur_cell = neighbour;
                visited_count++;
            } else if (stack.Count > 0) {
                cur_cell = stack.Pop();
            } else {
                Debug.Log("Stack empty! This should never happens!");
                visited_count++;
            }
        }
    }

    void Generate_Eller() {
        //https://habr.com/ru/post/176671/

        //Init
        maze = new cell_info[size.x, size.y];
        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                cell_info ci = new cell_info();
                if (x != 0) ci.leftWall = false;
                if (x != maze.GetUpperBound(0)) ci.rightWall = false;
                if (y != 0) ci.topWall = false;
                if (y != maze.GetUpperBound(1)) ci.bottomWall = false;
                maze[x,y] = ci;
            }
        }

        //First row - each cell in separate group
        int unique = 1;
        List<int> row_groups = new List<int>();
        row_groups.AddRange(Enumerable.Repeat(-1, size.x));

        for (int cur_y = 0; cur_y < size.y; cur_y++) {
            //Присвойте ячейкам, не входящим в множество, свое уникальное множество.
            for (int c = 0; c < size.x; c++ ) {
                if (row_groups[c] == -1) { row_groups[c] = unique; unique++; }
            }

            //Creating right borders
            //Loop 0-8 for 10 sized maze, because we don't need last cell
            for (int c = 0; c < size.x - 1; c++ ) {
                if (row_groups[c] == row_groups[c+1]) {
                    //Если текущая ячейка и ячейка справа принадлежат одному множеству, то создайте границу (для предотвращения зацикливаний)
                    maze[c, cur_y].rightWall = true;
                } else {
                    if (Random.Range(0, 2) == 0) {
                        //Create right border
                        maze[c, cur_y].rightWall = true;
                    } else {
                        //Don't create right border
                        //Если вы решили не добавлять границу, то объедините два множества в которых находится текущая ячейка и ячейка справа.
                        row_groups[c+1] = row_groups[c];
                    }
                }
            }

            //Creating random bottom borders
            for (int c = 0; c < size.x; c++ ) {
                //Если ячейка в своем множестве одна, то не создавайте границу снизу
                if (row_groups.Where((x)=> x == row_groups[c]).Count() == 1) continue;

                if (Random.Range(0, 2) == 0) maze[c, cur_y].bottomWall = true;
            }

            //Убедитесь что каждое множество имеет хотя бы одну ячейку без нижней границы (для предотвращения изолирования областей)
            var distinct_groups = row_groups.Select((item, index) => new {index, item}).GroupBy((x)=>x.item);
            foreach (var grp in distinct_groups) {
                bool has_bottom_hole = false;
                foreach (var item in grp) {
                    if (maze[item.index, cur_y].bottomWall == false) { has_bottom_hole = true; break; }
                }
                if (!has_bottom_hole) {
                    int r = Random.Range(0, grp.Count());
                    maze[grp.ElementAt(r).index, cur_y].bottomWall = false;
                }
            }

            //Решите, будете ли вы дальше добавлять строки или хотите закончить лабиринт
            if (cur_y == size.y-1) break;

            //Если вы хотите добавить еще одну строку, то:
            for (int c = 0; c < size.x; c++ ) {
                //Удалите ячейки с нижней границей из их множества
                if (maze[c, cur_y].bottomWall) row_groups[c] = -1;
            }
        }

        //Если вы решите закончить лабиринт, то:
        //Если текущая ячейка и ячейка справа члены разных множеств, то: 
        //  Удалите правую границу
        //  Объедините множества текущей ячейки и ячейки справа
        int last = size.y - 1;
        for (int c = 0; c < size.x - 1; c++ ) {
            maze[c, last].bottomWall = true;

            //Get right border from previous row
            maze[c, last].rightWall = maze[c, last-1].rightWall;

            if (row_groups[c] != row_groups[c+1]) {
                maze[c, last].rightWall = false;
                row_groups[c+1] = row_groups[c];
            }
        }

        //Last cell bottom wall
        maze[size.x-1, last].bottomWall = true;
    }

    void Generate_Kruskal() {
        //http://weblog.jamisbuck.org/2011/1/3/maze-generation-kruskal-s-algorithm

        //Init
        maze = new cell_info[size.x, size.y];

        int set = 1;
        Dictionary<cell_info, int> cell_set = new Dictionary<cell_info, int>();

        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                maze[x,y] = new cell_info();
                cell_set.Add(maze[x,y], set); set++;
            }
        }

        List <int[]> edges = new List <int[]>();
        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                if (x < maze.GetUpperBound(0)) edges.Add( new int[]{x, y, 0} ); //horizontal
                if (y < maze.GetUpperBound(1)) edges.Add( new int[]{x, y, 1} ); //vertical
            }
        }

        while (edges.Count > 0) {
            int r = Random.Range(0, edges.Count);

            cell_info ci1 = maze[ edges[r][0], edges[r][1] ];
            cell_info ci2 = null;
            if (edges[r][2] == 0) ci2 = maze[ edges[r][0] + 1, edges[r][1] ];
            else                  ci2 = maze[ edges[r][0], edges[r][1] + 1 ];

            int s1 = cell_set[ci1];
            int s2 = cell_set[ci2];
            if (s1 != s2) {
                //Remove edge from maze
                if (edges[r][2] == 0) {
                    //horizontal
                    ci1.rightWall = false; ci2.leftWall = false;
                } else {
                    //vertical
                    ci1.bottomWall = false; ci2.topWall = false;
                }

                //Merge sets
                var ci_with_set_s2 = cell_set.Where(kv => kv.Value == s2).ToArray();
                foreach (var kv in ci_with_set_s2) {
                    cell_set[kv.Key] = s1;
                }
            }
            
            //Remove edge from the bug
            edges.RemoveAt(r);
        }
    }

    void Generate_prim() {
        //http://weblog.jamisbuck.org/2011/1/10/maze-generation-prim-s-algorithm

        //Init
        maze = new cell_info[size.x, size.y];
        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                maze[x,y] = new cell_info();
            }
        }

        List<Vector2Int> processed = new List<Vector2Int>();
        List<Vector2Int> adjacent = new List<Vector2Int>();

        //Pick random point, and add it to processed list
        int x1 = Random.Range(0, size.x);
        int y1 = Random.Range(0, size.y);
        processed.Add( new Vector2Int(x1, y1) );

        //Get adjacent cells of that point
        if (x1 >= 1) adjacent.Add( new Vector2Int(x1-1, y1) );
        if (y1 >= 1) adjacent.Add( new Vector2Int(x1, y1-1) );
        if (x1 < size.x-1) adjacent.Add( new Vector2Int(x1+1, y1) );
        if (y1 < size.y-1) adjacent.Add( new Vector2Int(x1, y1+1) );

        while (adjacent.Count > 0) {
            //Randomly select one of adjacent cells
            int r1 = Random.Range(0, adjacent.Count);
            x1 = adjacent[r1].x; y1 = adjacent[r1].y;

            //Get processed cells, which are adjacent to selected
            List<int[]> connected = new List<int[]>();
            if (x1 >= 1 && processed.Contains(new Vector2Int(x1-1, y1))) connected.Add( new int[]{x1-1, y1} );
            if (y1 >= 1 && processed.Contains(new Vector2Int(x1, y1-1))) connected.Add( new int[]{x1, y1-1} );
            if (x1 < size.x-1 && processed.Contains(new Vector2Int(x1+1, y1))) connected.Add( new int[]{x1+1, y1} );
            if (y1 < size.y-1 && processed.Contains(new Vector2Int(x1, y1+1))) connected.Add( new int[]{x1, y1+1} );

            //Randomly select one of processed cells, which are adjacent to selected
            int r2 = Random.Range(0, connected.Count);
            int x2 = connected[r2][0]; int y2 = connected[r2][1];

            //Make a passage between cells r and r2
            if (x1 < x2) {
                maze[x1, y1].rightWall = false; maze[x2, y2].leftWall = false;
            }
            else if (x1 > x2) {
                maze[x1, y1].leftWall = false; maze[x2, y2].rightWall = false;
            }
            else if (y1 < y2) {
                maze[x1, y1].bottomWall = false; maze[x2, y2].topWall = false;
            }
            else if (y1 > y2) {
                maze[x1, y1].topWall = false; maze[x2, y2].bottomWall = false;
            }

            //Remove new processed cell from adjacent and add to processed
            adjacent.RemoveAt(r1);
            processed.Add( new Vector2Int(x1, y1) );

            //Add new adjacent
            List<Vector2Int> adj_tmp = new List<Vector2Int>();
            if (x1 >= 1) adj_tmp.Add( new Vector2Int(x1-1, y1) );
            if (y1 >= 1) adj_tmp.Add( new Vector2Int(x1, y1-1) );
            if (x1 < size.x-1) adj_tmp.Add( new Vector2Int(x1+1, y1) );
            if (y1 < size.y-1) adj_tmp.Add( new Vector2Int(x1, y1+1) );
            foreach (var a in adj_tmp) {
                if (!adjacent.Contains(a) && !processed.Contains(a)) adjacent.Add(a);
            }
        }
    }

    void Generate_GrowingTree()
    {
        //http://weblog.jamisbuck.org/2011/1/27/maze-generation-growing-tree-algorithm

        //Init
        maze = new cell_info[size.x, size.y];
        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                maze[x,y] = new cell_info();
            }
        }

        List<Vector2Int> cells = new List<Vector2Int>();

        //Pick random point, and add it to cell list
        cells.Add( new Vector2Int( Random.Range(0, size.x), Random.Range(0, size.y)) );

        //prevent eternal loop while debug
        int iterations = 0;

        while (cells.Count > 0) {
            int ind = Generate_GrowingTree_SelectCell(cells);
            int x = cells[ind].x, y = cells[ind].y;

            //Mark as visited
            maze[x, y].isVisited = true;

            //Get neighbors
            List<Vector2Int> neighbors = new List<Vector2Int>();
            if (x >= 1 && !maze[x-1, y].isVisited) neighbors.Add( new Vector2Int(x-1, y) );
            if (y >= 1 && !maze[x, y-1].isVisited) neighbors.Add( new Vector2Int(x, y-1) );
            if (x < size.x-1 && !maze[x+1, y].isVisited) neighbors.Add( new Vector2Int(x+1, y) );
            if (y < size.y-1 && !maze[x, y+1].isVisited) neighbors.Add( new Vector2Int(x, y+1) );

            if (neighbors.Count > 0) {
                var cell_to = neighbors[Random.Range(0, neighbors.Count)];

                //Make a passage between cells r and r2
                if (x < cell_to.x) {
                    maze[x, y].rightWall = false; maze[cell_to.x, cell_to.y].leftWall = false;
                }
                else if (x > cell_to.x) {
                    maze[x, y].leftWall = false; maze[cell_to.x, cell_to.y].rightWall = false;
                }
                else if (y < cell_to.y) {
                    maze[x, y].bottomWall = false; maze[cell_to.x, cell_to.y].topWall = false;
                }
                else if (y > cell_to.y) {
                    maze[x, y].topWall = false; maze[cell_to.x, cell_to.y].bottomWall = false;
                }

                cells.Add( new Vector2Int(cell_to.x, cell_to.y) );
                maze[cell_to.x, cell_to.y].isVisited = true;
            } else {
                //If no neighbors - remove cell from list and continue loop
                cells.RemoveAt(ind); continue;
            }

            //prevent eternal loop while debug
            iterations++;
            if (iterations > 300) break;
        }
    }

    int Generate_GrowingTree_SelectCell(List<Vector2Int> cells)
    {
        //newest (Recursive backtracker)
        if (growing_tree_cell_select == Growing_Tree_Select_Cells.newest_backtracker) return cells.Count - 1;
        
        //random (Prim)
        if (growing_tree_cell_select == Growing_Tree_Select_Cells.random_prim) return Random.Range(0, cells.Count);

        //oldest
        if (growing_tree_cell_select == Growing_Tree_Select_Cells.oldest_stright) return 0;

        //middle
        if (growing_tree_cell_select == Growing_Tree_Select_Cells.middle) return Mathf.RoundToInt(cells.Count / 2);

        //fallback to default - newest (Recursive backtracker)
        return cells.Count - 1;
    }

    void Create() {
        Vector2 cell_size = new Vector2(1f, 1f);
        
        //Vector2 full_maze_size = new Vector2( (float)(maze.GetUpperBound(0) + 1) * cell_size.x, (float)(maze.GetUpperBound(1) + 1) * cell_size.y );
        for ( int x = 0; x <= maze.GetUpperBound(0); x++ ) {
            for ( int y = 0; y <= maze.GetUpperBound(1); y++ ) {
                var cell = maze[x,y];
                Vector2 cell_coord = new Vector2((float)x * cell_size.x, (float)y * cell_size.y);
                if (cell.leftWall) {
                    Transform w = Instantiate(wall, transform).transform;
                    TList.Add(w);
                    w.localPosition = new Vector3(cell_coord.x - (cell_size.x / 2), wall_y_offset, cell_coord.y);
                    w.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    if (x > 0) maze[x-1, y].rightWall = false;
                }

                if (cell.rightWall) {
                    Transform w = Instantiate(wall, transform).transform;
                    TList.Add(w);
                    w.localPosition = new Vector3(cell_coord.x + (cell_size.x / 2), wall_y_offset, cell_coord.y);
                    w.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    if (x < size.x - 1) maze[x+1, y].leftWall = false;
                }

                if (cell.topWall) {
                    Transform w = Instantiate(wall, transform).transform;
                    TList.Add(w);
                    w.localPosition = new Vector3(cell_coord.x, wall_y_offset, cell_coord.y - (cell_size.y / 2));
                    w.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    if (y > 0) maze[x, y-1].bottomWall = false;
                }

                if (cell.bottomWall) {
                    Transform w = Instantiate(wall, transform).transform;
                    TList.Add(w);
                    w.localPosition = new Vector3(cell_coord.x, wall_y_offset, cell_coord.y + (cell_size.y / 2));
                    w.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    if (y < size.y - 1) maze[x, y+1].topWall = false;
                }
            }
        }

        if (maze_create_anim == FadeIn_Anim_Enum.from_top) {
            foreach (var t in TList) {
                t.DOLocalMoveY(t.localPosition.y + 5f, 0.7f).From();
            }
        }
        if (maze_create_anim == FadeIn_Anim_Enum.from_top_random) {
            foreach (var t in TList) {
                float r = Random.Range(0.25f, 0.75f);
                float o = Random.Range(0f, 2f);
                t.DOLocalMoveY(t.localPosition.y + 3f + o, r).From();
            }
        }

    }
}
