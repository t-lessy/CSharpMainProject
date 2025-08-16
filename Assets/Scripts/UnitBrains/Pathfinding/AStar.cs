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
    //    public class AStar : BaseUnitPath
    //    {
    //        //private IReadOnlyRuntimeModel _runtimeModel;
    //        private const int MaxLength = 100;

    //        private int[] dx = { -1, 0, 1, 0 };
    //        private int[] dy = { 0, 1, 0, -1 };

    //        private Vector2Int _currentPosition;

    //        public AStar(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    //        {
    //            _currentPosition = startPoint;
    //        }

    //        protected override void Calculate()
    //        {
    //            Node startNode = new Node(startPoint.x, startPoint.y);
    //            startNode.cost = 0; // Стоимость пути от старта


    //            Node targetNode = new Node(endPoint.x, endPoint.y);
    //            List<Node> neiboorsTargetList = new List<Node>();
    //            neiboorsTargetList.Add(targetNode);
    //            //  добавляем соседей по 4 напрвлениям от конечной точки в список соседних нодов
    //            for (int i = 0; i < dx.Length; i++)
    //            {

    //                int newX = targetNode.X + dx[i];// суммируем к координате i член массива, тем самым перебирая 4 направления
    //                int newY = targetNode.Y + dy[i];

    //                Vector2Int neiboorsTargetVector = new Vector2Int(newX, newY);
    //                Node neiboorsTargetNode = new Node(neiboorsTargetVector.x, neiboorsTargetVector.y);
    //                neiboorsTargetList.Add(neiboorsTargetNode);
    //            }

    //            List<Node> openList = new List<Node> { startNode };///////////////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //            HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>(runtimeModel.RoUnits.Select(u => u.Pos));
    //            List<Node> closedList = new List<Node>();


    //            while (openList.Count > 0 && openList.Count < MaxLength)// пока открытый список не пуст делаем
    //            {
    //                var currentNode = openList[0];// стартовый узел приравнивается переменной  currentNode
    //                foreach (var node in openList) // проходим по открытому списку и сравниваем Value его элементов, самый маленький Value приравнивается currentNode 
    //                {
    //                    if (node.Value < currentNode.Value)
    //                    {

    //                        currentNode = node;
    //                    }

    //                }

    //                openList.Remove(currentNode);//обнуляем список предыдущих возможных ячеек для ходьбы
    //                closedList.Add(currentNode);// добавляем ее в закртый список
    //                // проходим по списку neiboorsTargetList который включает в себя саму конечную точку и ее соседей
    //                foreach (var node in neiboorsTargetList)
    //                {
    //                    if (currentNode.X == node.X && currentNode.Y == node.Y)// сравниваем текущую ноду с конечной целью или ее соседом,при совпадении добавляем все ячейки пути в список path,пока текущая ячейка не будет пуста
    //                    {
    //                        List<Node> path = new List<Node>();
    //                        while (currentNode != null)
    //                        {
    //                            path.Add(currentNode);
    //                            currentNode = currentNode.Parent;
    //                        }
    //                        path.Reverse();//переворачиваем списко,т.к. мы добавляли ячейки с конца
    //                        this.path = path.Select(n => new Vector2Int(n.X, n.Y)).ToArray();
    //                        return;
    //                    }
    //                }

    //                for (int i = 0; i < dx.Length; i++)// если же текущая ячейка не конечная, мы ищем подходящие соседние
    //                {
    //                    int newX = currentNode.X + dx[i];// суммируем к координате i член массива, тем самым перебирая 4 направления
    //                    int newY = currentNode.Y + dy[i];
    //                    Vector2Int newPos = new Vector2Int(newX, newY);
    //                    if (runtimeModel.IsTileWalkable(newPos) || newPos == endPoint)
    //                    {
    //                        if (IsValid(runtimeModel, newPos))//получившуюяся ячеку провиряем на ограничения, если она "валидна" создаем Node сосед с этими координатами
    //                        {
    //                            Node neighboor = new Node(newPos.x, newPos.y);

    //                            if (closedList.Contains(neighboor)) continue;// проверка на то,что такая ячейка уже есть в закрытом списке ( пройдена )



    //                            //// Устанавливаем стоимость в зависимости от занятости
    //                            //bool isOccupied = occupiedPositions.Contains(newPos) && newPos != startPoint;
    //                            //neighboor.cost = isOccupied ? 30 : 10; // 30 если занята, 10 если свободна

    //                            neighboor.Parent = currentNode;// добавляем текущую ячейку в родительскую для того чтобы запомнить пердыдущую ячейку

    //                            neighboor.CalculateEstimate(targetNode.X, targetNode.Y);//высчитываем эвристику
    //                            neighboor.CalculateValue();// считаем итоговую оценку

    //                            openList.Add(neighboor);//добавляем валидного, лучшего по оценку соседа  в открытый список
    //                        }
    //                    }

    //                }

    //            }
    //            this.path = Array.Empty<Vector2Int>();
    //        }

    //        private bool IsUnitAtPos(Vector2Int pos) => runtimeModel.RoUnits.Any(u => u.Pos == pos);


    //        private bool IsValid(IReadOnlyRuntimeModel runtimeModel, Vector2Int pos)
    //        {
    //            if (pos == startPoint) return true;
    //            // Проверяем, что координаты находятся в пределах карты
    //            bool isInsideMap = pos.x >= 0 && pos.y >= 0 &&
    //                pos.x < runtimeModel.RoMap.Width &&
    //                pos.y < runtimeModel.RoMap.Height &&
    //                !IsUnitAtPos(pos) &&
    //                runtimeModel.IsTileWalkable(new Vector2Int(pos.x, pos.y));

    //            // Если координаты вне карты - сразу возвращаем false
    //            if (!isInsideMap)
    //                return false;
    //            else return true;
    //        }

    //        public override Vector2Int GetNextStepFrom(Vector2Int currentPos)
    //        {
    //            if (currentPos != _currentPosition)
    //            {
    //                _currentPosition = currentPos;
    //                path = null;
    //            }
    //            if (path == null || path.Length == 0)
    //            {
    //                Calculate();
    //            }

    //            if (path == null || path.Length == 0)
    //            {
    //                return _currentPosition;
    //            }

    //            int currentIndex = Array.IndexOf(path, _currentPosition);
    //            if (currentIndex < 0)
    //            {
    //                path = null;
    //                Calculate();
    //                currentIndex = Array.IndexOf(path, _currentPosition);
    //                if (currentIndex < 0)
    //                {
    //                    return _currentPosition;
    //                }
    //            }
    //            return currentIndex + 1 < path.Length ? path[currentIndex + 1] : _currentPosition;
    //        }
    //        public class Node
    //        {
    //            public int X;
    //            public int Y;

    //            public int cost = 10;
    //            public int Estimate;// оценка расстояния до цели
    //            public int Value;
    //            public Node Parent;
    //            public Node(int x, int y)
    //            {
    //                X = x;
    //                Y = y;
    //            }
    //            public void CalculateValue()
    //            {

    //                Value = cost + Estimate;

    //            }
    //            public void CalculateEstimate(int targetX, int targetY)
    //            {
    //                Estimate = Math.Abs(X - targetX) + Math.Abs(Y - targetY);

    //            }
    //            public override bool Equals(object obj)
    //            {
    //                return obj is Node node && X == node.X && Y == node.Y;
    //            }

    //            public override int GetHashCode()
    //            {
    //                return HashCode.Combine(X, Y);
    //            }
    //            //public override bool Equals(object? obj)
    //            //{
    //            //    if (obj is not Node node)
    //            //        return false;
    //            //    return X == node.X && Y == node.Y;
    //            //}
    //        }
    //    }
    public class AStar : BaseUnitPath
    {
        private class Node
        {
            public int X;
            public int Y;
            public int G; // Стоимость пути от старта
            public int H; // Эвристическая оценка до цели
            public int Value => G + H;
            public Node Parent;

            public Node(int x, int y)
            {
                X = x;
                Y = y;
                G = int.MaxValue;
            }
            //Старт: (1, 1), Цель: (4, 5) → H = |1-4| + |1-5| = 3 + 4 = 7.
            //Алгоритм A* использует F = G + H для выбора оптимального пути:
            //G — точная стоимость пройденного пути.
            //H — оценка оставшегося пути (чем меньше, тем приоритетнее узел).


            public void CalculateEstimate(int targetX, int targetY)
            {
                H = Math.Abs(X - targetX) + Math.Abs(Y - targetY);
            }

            public override bool Equals(object obj) => obj is Node node && X == node.X && Y == node.Y;
            public override int GetHashCode() => HashCode.Combine(X, Y);
            public Vector2Int ToVector() => new(X, Y);
        }

        private readonly int[] dx = { -1, 0, 1, 0 };
        private readonly int[] dy = { 0, 1, 0, -1 };

        private const int WaitFrames = 60;//колво фреймов ожидания
        private int waitCounter = 0;//счетчик ожидания

        private Vector2Int lastBlockedPosition;
        private Vector2Int _currentPosition;

        public AStar(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
            : base(runtimeModel, startPoint, endPoint)
        {
            _currentPosition = startPoint;
        }

        public override Vector2Int GetNextStepFrom(Vector2Int currentPos)
        {
            //Если юнит уже переместился в новую клетку (не совпадает с _currentPosition), обновляем его позицию и сбрасываем сохранённый путь
            if (currentPos != _currentPosition)
            {
                _currentPosition = currentPos;
                path = null;
            }
            // Вызываем алгоритм A* для поиска пути
            if (path == null || path.Length == 0)
            {
                Calculate();
            }
            //Если путь после расчёта остался пустым, возвращаем текущую позицию.Стоим на месте
            if (path == null || path.Length == 0)
            {
                return _currentPosition;
            }

            int currentIndex = Array.IndexOf(path, _currentPosition);//проверяем есть ли текущая клетка в массиве типа VectorInt2 path
            if (currentIndex < 0)// путь не актуален перерассчитываем
            {
                path = null;
                Calculate();
                currentIndex = Array.IndexOf(path, _currentPosition);
                if (currentIndex < 0) return _currentPosition;// Если снова не найден — стоим
            }

            return currentIndex + 1 < path.Length ? path[currentIndex + 1] : _currentPosition;// Если следующая точка в пути существует — возвращаем её,
                                                                                              // если юнит уже достиг конца пути — возвращаем текущую позицию 

        }

        protected override void Calculate()
        {
            if (waitCounter > 0)
            {
                waitCounter--;
                if (CheckNeighborsAvailable())
                {
                    waitCounter = 0;
                    CalculatePath();
                }
                return;
            }

            CalculatePath();
        }

        private void CalculatePath()
        {
            path = null;

            if (_currentPosition == endPoint)//сравниваем екущие координаты юнита  с целевой точкой 
            {
                path = new[] { _currentPosition };// создает массив пути из одного элемента 
                return;//перрываем дальнейшие вычисления
            }

            Node startNode = new Node(_currentPosition.x, _currentPosition.y) { G = 0 };
            Node targetNode = new Node(endPoint.x, endPoint.y);
            startNode.CalculateEstimate(targetNode.X, targetNode.Y);

            var openList = new List<Node> { startNode };
            var closedList = new HashSet<Node>();
            var allNodes = new Dictionary<Vector2Int, Node> { [_currentPosition] = startNode };

            Node bestNode = startNode;
            bool pathBlocked = true;

            while (openList.Count > 0)
            {
                Node currentNode = GetBestNode(openList);

                if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)//при совпадении текущей точки с конечной строим путь
                {
                    ReconstructPath(currentNode);
                    return;
                }

                if (currentNode.H < bestNode.H)// если эвристика текущей меньше чем у лучшей назначаем ее лучшей

                    bestNode = currentNode;

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                for (int i = 0; i < dx.Length; i++)
                {
                    Vector2Int neighborPos = new(currentNode.X + dx[i], currentNode.Y + dy[i]);

                    if (!IsValid(neighborPos))
                        continue;

                    if (!allNodes.TryGetValue(neighborPos, out Node neighbor))
                    {
                        neighbor = new Node(neighborPos.x, neighborPos.y);
                        neighbor.CalculateEstimate(targetNode.X, targetNode.Y);
                        allNodes[neighborPos] = neighbor;
                    }

                    if (closedList.Contains(neighbor))
                        continue;

                    int tentativeG = currentNode.G + GetMovementCost(neighborPos);

                    if (tentativeG < neighbor.G || !openList.Contains(neighbor))
                    {
                        neighbor.G = tentativeG;
                        neighbor.Parent = currentNode;
                        pathBlocked = false;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            if (pathBlocked)
            {
                lastBlockedPosition = _currentPosition;
                waitCounter = WaitFrames;
            }
            else
            {
                ReconstructPath(bestNode);
            }
        }

        private int GetMovementCost(Vector2Int pos)
        {
            // Можно добавить разные стоимости для разных типов terrain
            return runtimeModel.RoUnits.Any(u => u.Pos == pos) ? 30 : 10;
        }

        private bool CheckNeighborsAvailable()
        {
            for (int i = 0; i < dx.Length; i++)
            {
                Vector2Int checkPos = new(
                    lastBlockedPosition.x + dx[i],
                    lastBlockedPosition.y + dy[i]);

                if (IsValid(checkPos))
                    return true;
            }
            return false;
        }

        private Node GetBestNode(List<Node> nodes)
        {
            Node best = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].Value < best.Value ||
                   (nodes[i].Value == best.Value && nodes[i].H < best.H))
                {
                    best = nodes[i];
                }
            }
            return best;
        }

        private void ReconstructPath(Node endNode)
        {
            List<Vector2Int> pathList = new();
            Node current = endNode;

            while (current != null)
            {
                pathList.Add(current.ToVector());
                current = current.Parent;
            }

            pathList.Reverse();
            path = pathList.ToArray();
        }

        private bool IsValid(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= runtimeModel.RoMap.Width ||
                pos.y < 0 || pos.y >= runtimeModel.RoMap.Height)
                return false;

            if (runtimeModel.RoMap[pos])
                return false;

            if (pos == _currentPosition || pos == endPoint)
                return true;

            return !runtimeModel.RoUnits.Any(u => u.Pos == pos);
        }
    }
}
