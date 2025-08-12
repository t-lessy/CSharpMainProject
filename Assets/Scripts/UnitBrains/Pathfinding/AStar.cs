using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitBrains.Pathfinding;
using UnityEngine;
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
            Node targetNode = new Node(endPoint.x, endPoint.y);

            List<Node> openList = new List<Node> { startNode };///////////////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            List<Node> closedList = new List<Node>();

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
                openList.Remove(currentNode);// удаляем  из открытого списка старую ячейку
                closedList.Add(currentNode);// добавляем ее в закртый список
                if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)// сравниваем текущую ноду с конечной целью,при совпадении добавляем все ячейки пути в список path,пока текущая ячейка не будет пуста
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
                for (int i = 0; i < dx.Length; i++)// если же текущая ячейка не конечная, мы ищем подходящие соседние
                {
                    int newX = currentNode.X + dx[i];// суммируем к координате i член массива, тем самым перебирая 4 направления
                    int newY = currentNode.Y + dy[i];

                    if (IsValid(newX, newY, runtimeModel))//получившуюяся ячеку провиряем на ограничения, если она "валидна" создаем Node сосед с этими координатами
                    {
                        Node neighboor = new Node(newX, newY);
                        if (closedList.Contains(neighboor)) continue;// проверка на то,что такая ячейка уже есть в закрытом списке ( пройдена )

                        neighboor.Parent = currentNode;// добавляем текцщую ячейку в родительскую для того чтобы запомнить пердыдущую ячейку
                        neighboor.CalculateEstimate(targetNode.X, targetNode.Y);//высчитываем эвристику
                        neighboor.CalculateValue(IsHorizontalMovement(newX));// считаем итоговую оценку

                        openList.Add(neighboor);//добавляем валидного, лучшего по оценку соседа  в открытый список
                    }
                }

            }
            this.path = Array.Empty<Vector2Int>();
        }


        public bool IsHorizontalMovement(int x)//определяем какая направленность клетки вертикальная или горизонатльная для того чтобы учесть разную стоимоть этиъ  ячеек
        {
            return x == -1 || x == 1;
        }
        
        private bool IsValid(int x, int y, IReadOnlyRuntimeModel runtimeModel)
        {
            // Проверяем, что координаты находятся в пределах карты
            bool isInsideMap = x >= 0 && y >= 0 && x < runtimeModel.RoMap.Width && y < runtimeModel.RoMap.Height &&
           runtimeModel.IsTileWalkable(new Vector2Int(x, y));

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
        public int Horizontalcost = 10;
        public int Verticalcost = 15;
        public int Estimate;// оценка расстояния до цели
        public int Value;
        public Node Parent;
        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void CalculateValue(bool movement)
        {
            if (movement)
            {
                Value = Horizontalcost + Estimate;
            }
            else
            {
                Value = Verticalcost + Estimate;
            }

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
