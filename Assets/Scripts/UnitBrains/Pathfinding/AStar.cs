using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitBrains.Pathfinding;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class AStar : BaseUnitPath
    {
        //private IReadOnlyRuntimeModel _runtimeModel;
        private const int MaxLength = 100;

        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };
        public AStar(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            Node startNode = new Node(startPoint.x, startPoint.y);
            startNode.cost = 0; // Стоимость пути от старта


            Node targetNode = new Node(endPoint.x, endPoint.y);
            List<Node> neiboorsTargetList = new List<Node>();
            neiboorsTargetList.Add(targetNode);
            //  добавляем соседей по 4 напрвлениям от конечной точки в список соседних нодов
            for (int i = 0; i < dx.Length; i++)
            {

                int newX = targetNode.X + dx[i];// суммируем к координате i член массива, тем самым перебирая 4 направления
                int newY = targetNode.Y + dy[i];

                Vector2Int neiboorsTargetVector = new Vector2Int(newX, newY);
                Node neiboorsTargetNode = new Node(neiboorsTargetVector.x, neiboorsTargetVector.y);
                neiboorsTargetList.Add(neiboorsTargetNode);
            }  

            List<Node> openList = new List<Node> { startNode };///////////////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>(runtimeModel.RoUnits.Select(u => u.Pos));
            List <Node> closedList = new List<Node>();


            while (openList.Count > 0 && openList.Count < MaxLength)// пока открытый список не пуст делаем
            {
                var currentNode = openList[0];// стартовый узел приравнивается переменной  currentNode
                foreach (var node in openList) // проходим по открытому списку и сравниваем Value его элементов, самый маленький Value приравнивается currentNode 
                {
                    if (node.Value < currentNode.Value)
                    {

                        currentNode = node;
                    }

                }

                openList.Remove(currentNode);//обнуляем список предыдущих возможных ячеек для ходьбы
                closedList.Add(currentNode);// добавляем ее в закртый список
                // проходим по списку neiboorsTargetList который включает в себя саму конечную точку и ее соседей
                foreach (var node in neiboorsTargetList)
                {
                    if (currentNode.X == node.X && currentNode.Y == node.Y)// сравниваем текущую ноду с конечной целью или ее соседом,при совпадении добавляем все ячейки пути в список path,пока текущая ячейка не будет пуста
                    {
                        List<Node> path = new List<Node>();
                        while (currentNode != null)
                        {
                            path.Add(currentNode);
                            currentNode = currentNode.Parent;
                        }
                        path.Reverse();//переворачиваем списко,т.к. мы добавляли ячейки с конца
                        this.path = path.Select(n => new Vector2Int(n.X, n.Y)).ToArray();
                        return;
                    }
                }
               
                for (int i = 0; i < dx.Length; i++)// если же текущая ячейка не конечная, мы ищем подходящие соседние
                {
                    int newX = currentNode.X + dx[i];// суммируем к координате i член массива, тем самым перебирая 4 направления
                    int newY = currentNode.Y + dy[i];
                    Vector2Int newPos = new Vector2Int(newX, newY);
                    if (runtimeModel.IsTileWalkable(newPos) || newPos == endPoint || IsUnitAtPos(newPos))
                    {
                        if (IsValid( runtimeModel, newPos))//получившуюяся ячеку провиряем на ограничения, если она "валидна" создаем Node сосед с этими координатами
                        {
                            Node neighboor = new Node(newPos.x, newPos.y);

                            if (closedList.Contains(neighboor)) continue;// проверка на то,что такая ячейка уже есть в закрытом списке ( пройдена )



                            // Устанавливаем стоимость в зависимости от занятости
                            bool isOccupied = occupiedPositions.Contains(newPos) && newPos != startPoint;
                            neighboor.cost = isOccupied ? 30 : 10; // 30 если занята, 10 если свободна

                            neighboor.Parent = currentNode;// добавляем текущую ячейку в родительскую для того чтобы запомнить пердыдущую ячейку

                            neighboor.CalculateEstimate(targetNode.X, targetNode.Y);//высчитываем эвристику
                            neighboor.CalculateValue();// считаем итоговую оценку

                            openList.Add(neighboor);//добавляем валидного, лучшего по оценку соседа  в открытый список
                        }
                    }
                    
                }
                
            }
            this.path = Array.Empty<Vector2Int>();
        }
      
        private bool IsUnitAtPos(Vector2Int pos)=>runtimeModel.RoUnits.Any(u => u.Pos == pos);
        public bool IsHorizontalMovement(int x)//определяем какая направленность клетки вертикальная или горизонатльная для того чтобы учесть разную стоимоть этиъ  ячеек
        {
            return x == -1 || x == 1;
        }
        
        private bool IsValid(IReadOnlyRuntimeModel runtimeModel, Vector2Int pos)
        {
            if (pos == startPoint) return true;
            // Проверяем, что координаты находятся в пределах карты
            bool isInsideMap = pos.x >= 0 && pos.y >= 0 && pos.x < runtimeModel.RoMap.Width && pos.y < runtimeModel.RoMap.Height &&
           runtimeModel.IsTileWalkable(new Vector2Int(pos.x, pos.y));

            // Если координаты вне карты - сразу возвращаем false
            if (!isInsideMap)
                return false;
            else return true;
        }
    }

    public class Node
    {
        public int X;
        public int Y;
        
        public int cost = 10;
        public int Estimate;// оценка расстояния до цели
        public int Value;
        public Node Parent;
        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void CalculateValue()
        {
            
                Value = cost + Estimate;

        }
        public void CalculateEstimate(int targetX, int targetY)
        {
            Estimate = Math.Abs(X - targetX) + Math.Abs(Y - targetY);

        }
        public override bool Equals(object obj)
        {
            return obj is Node node && X == node.X && Y == node.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        //public override bool Equals(object? obj)
        //{
        //    if (obj is not Node node)
        //        return false;
        //    return X == node.X && Y == node.Y;
        //}
    }
}
