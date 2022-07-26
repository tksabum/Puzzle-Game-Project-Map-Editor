using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomClass
{
    public class DisjointSet<T>
    {
        List<int> parent;
        List<T> values;
        List<int> rank;
        Dictionary<T, int> dict;

        public DisjointSet()
        {
            parent = new List<int>();
            values = new List<T>();
            rank = new List<int>();
            dict = new Dictionary<T, int>();
        }

        public DisjointSet(List<T> list)
        {
            parent = new List<int>();
            values = new List<T>();
            rank = new List<int>();
            dict = new Dictionary<T, int>();

            for (int i = 0; i < list.Count; i++)
            {
                parent.Add(i);
                values.Add(list[i]);
                rank.Add(1);
                dict.Add(list[i], i);
            }
        }

        public DisjointSet<T> Copy()
        {
            DisjointSet<T> clone = new DisjointSet<T>();
            for (int i = 0; i < parent.Count; i++)
            {
                clone.parent.Add(parent[i]);
                clone.values.Add(values[i]);
                clone.rank.Add(rank[i]);
            }
            foreach(KeyValuePair<T, int> kvp in dict)
            {
                clone.dict.Add(kvp.Key, kvp.Value);
            }

            return clone;
        }

        public T GetRoot(T key)
        {
            return values[Find(dict[key])];
        }

        public int Find(int u)
        {
            if (parent[u] == u)
            {
                return u;
            }

            return parent[u] = Find(parent[u]);
        }

        public void Union(int u, int v)
        {
            u = Find(u);
            v = Find(v);

            if (u == v) return;

            if (rank[u] < rank[v])
            {
                parent[u] = v;
                if (rank[u] == rank[v]) rank[v]++;
            }
            else
            {
                parent[v] = u;
                if (rank[u] == rank[v]) rank[u]++;
            }

        }

        public void Union(T u, T v)
        {
            if (!(dict.ContainsKey(u) && dict.ContainsKey(v)))
            {
                throw new System.Exception("Error: Union(T u, T v)");
            }
            Union(dict[u], dict[v]);
        }

        // u, v가 같은 집합에 속하면 true, 아니면 false
        public bool GroupCheck(T u, T v)
        {
            if (Find(dict[u]) == Find(dict[v]))
            {
                return true;
            }

            return false;
        }

        public bool ContainsKey(T key)
        {
            return dict.ContainsKey(key);
        }

        // elementOfSet과 같은 집합의 원소를 모두 List에 넣어 리턴
        public List<T> GetAllElementsList(T elementOfSet)
        {
            List<T> list = new List<T>();

            if (!dict.ContainsKey(elementOfSet))
            {
                return list;
            }

            int currentParent = Find(dict[elementOfSet]);

            for (int i = 0; i < parent.Count; i++)
            {
                if (currentParent == Find(i))
                {
                    list.Add(values[i]);
                }
            }

            return list;
        }

        // elementOfSet과 같은 집합의 원소를 모두 Hash에 넣어 리턴
        public HashSet<T> GetAllElementsHash(T elementOfSet)
        {
            HashSet<T> hash = new HashSet<T>();

            if (!dict.ContainsKey(elementOfSet))
            {
                return hash;
            }

            int currentParent = Find(dict[elementOfSet]);

            for (int i = 0; i < parent.Count; i++)
            {
                if (currentParent == Find(i))
                {
                    hash.Add(values[i]);
                }
            }

            return hash;
        }

        // 하나의 원소를 원래의 집합에서 제거하고 root노드가 되게 함
        public void SplitElement(T element)
        {
            int elementNum = dict[element];
            int root = Find(elementNum);

            for (int i = 0; i < parent.Count; i++)
            {
                Find(i);
                if (parent[i] != i)
                {
                    rank[parent[i]] = 2;
                }
            }

            if (root == elementNum)
            {
                int newRoot = -1;

                for (int i = 0; i < parent.Count; i++)
                {
                    if (i == elementNum) continue;

                    if (parent[i] == elementNum)
                    {
                        if (newRoot == -1)
                        {
                            parent[i] = i;
                            newRoot = i;
                            rank[i] = 1;
                        }
                        else
                        {
                            parent[i] = newRoot;
                            rank[newRoot] = 2;
                        }
                    }
                }

                parent[elementNum] = elementNum;
                rank[elementNum] = 1;
            }
            else
            {
                parent[elementNum] = elementNum;
                rank[elementNum] = 1;
            }

        }

        // 하나의 원소를 원래의 집합에서 제거하고 root의 집합에 포함시킨다.
        public void SplitElement(T element, T root)
        {
            SplitElement(element);
            Union(element, root);
        }

        // elements를 mainElement가 속한 집합에서 분리하여 root집합에 포함시킨다.
        // 단, elements에는 mainElement가 속할 수 있으며 mainElement는 분리하지 않음
        public void SplitElements(T mainElement, List<T> elements, T root)
        {
            // mainElement를 루트 노드로 rank2의 트리로 변환
            int mainElementNum = dict[mainElement];

            int preRoot = Find(mainElementNum);
            parent[mainElementNum] = mainElementNum;
            rank[mainElementNum] = 1;

            for (int i = 0; i < parent.Count; i++)
            {
                if (Find(i) == preRoot)
                {
                    parent[i] = mainElementNum;
                    rank[mainElementNum] = 2;
                }
            }

            // mainElement를 루트 노드로 하는 트리의 모드 노드를 분리하여 새로운 집합을 만듦
            int preElementNum = -1;
            for (int i = 0; i < elements.Count; i++)
            {
                int elementNum = dict[elements[i]];

                if (elementNum != mainElementNum && Find(elementNum) == mainElementNum)
                {
                    parent[elementNum] = elementNum;
                    rank[elementNum] = 1;
                    if (preElementNum == -1)
                    {
                        preElementNum = elementNum;
                    }
                    else
                    {
                        Union(preElementNum, elementNum);
                    }
                }
            }

            if (preElementNum != -1)
            {
                Union(preElementNum, dict[root]);
            }
        }

        // elements를 mainElement가 속한 집합에서 분리하여 새로운 집합으로 만듦
        // 단, elements에는 mainElement가 속할 수 있으며 mainElement는 분리하지 않음
        public void SplitElements(T mainElement, List<T> elements)
        {
            // mainElement를 루트 노드로 rank2의 트리로 변환
            int mainElementNum = dict[mainElement];

            int preRoot = Find(mainElementNum);
            parent[mainElementNum] = mainElementNum;
            rank[mainElementNum] = 1;

            for (int i = 0; i < parent.Count; i++)
            {
                if (Find(i) == preRoot)
                {
                    parent[i] = mainElementNum;
                    rank[mainElementNum] = 2;
                }
            }

            // mainElement를 루트 노드로 하는 트리의 모드 노드를 분리하여 새로운 집합을 만듦
            int preElementNum = -1;
            for (int i = 0; i < elements.Count; i++)
            {
                int elementNum = dict[elements[i]];

                if (elementNum != mainElementNum && Find(elementNum) == mainElementNum)
                {
                    parent[elementNum] = elementNum;
                    rank[elementNum] = 1;
                    if (preElementNum == -1)
                    {
                        preElementNum = elementNum;
                    }
                    else
                    {
                        Union(preElementNum, elementNum);
                    }
                }
            }
        }

        // root노드로 새로운 원소를 추가함
        public void AddElement(T element)
        {
            int elementNum = parent.Count;

            parent.Add(elementNum);
            values.Add(element);
            rank.Add(1);
            dict.Add(element, elementNum);
        }

        // element를 group과 같은 집합으로 새로 추가함
        public void AddElement(T element, T group)
        {
            int elementNum = parent.Count;

            int groupRoot = Find(dict[group]);
            parent.Add(groupRoot);
            values.Add(element);
            rank.Add(1);
            dict.Add(element, elementNum);

            if (rank[groupRoot] == 1) rank[groupRoot]++;
        }

        // 하나의 원소를 완전히 제거함
        public void RemoveElement(T element)
        {
            int elementNum = dict[element];
            int root = Find(elementNum);
            
            for (int i = 0; i < parent.Count; i++)
            {
                Find(i);
                if (parent[i] != i)
                {
                    rank[parent[i]] = 2;
                }
            }

            if (root == elementNum)
            {
                int newRoot = -1;

                for(int i = 0; i < parent.Count; i++)
                {
                    if (i == elementNum) continue;

                    if (parent[i] == elementNum)
                    {
                        if (newRoot == -1)
                        {
                            parent[i] = i;
                            newRoot = i;
                            rank[i] = 1;
                        }
                        else
                        {
                            parent[i] = newRoot;
                            rank[newRoot] = 2;
                        }
                    }
                }
                
            }

            parent.RemoveAt(elementNum);
            values.RemoveAt(elementNum);
            rank.RemoveAt(elementNum);
            dict.Remove(element);

            // RemoveAt을 하면 번호가 꼬이기 때문에 조정
            // parent 수정
            for (int i = 0; i < parent.Count; i++)
            {
                if (parent[i] > elementNum)
                {
                    parent[i]--;
                }
            }

            // dict 수정 foreach문에서 바로 수정할 경우 에러가 나서 hashset을 이용함
            // value 만 수정하기 때문에 foreach문에 문제가 생기진 않지만
            // dictionary의 특성상 dict[key] = value 가 데이터를 추가할 수 있기 때문인듯
            HashSet<T> set = new HashSet<T>();
            foreach (KeyValuePair<T, int> pair in dict)
            {
                if (pair.Value > elementNum)
                {
                    set.Add(pair.Key);
                }
            }

            foreach(T s in set)
            {
                dict[s]--;
            }

        }

        // 디버깅 용
        public override string ToString()
        {
            string str = "[count: " + values.Count + "] ";

            foreach (T value in values)
            {
                str += "[" + value + "] ";
            }

            return str;
        }
    }
}