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

        // u, v�� ���� ���տ� ���ϸ� true, �ƴϸ� false
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

        // elementOfSet�� ���� ������ ���Ҹ� ��� List�� �־� ����
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

        // elementOfSet�� ���� ������ ���Ҹ� ��� Hash�� �־� ����
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

        // �ϳ��� ���Ҹ� ������ ���տ��� �����ϰ� root��尡 �ǰ� ��
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

        // �ϳ��� ���Ҹ� ������ ���տ��� �����ϰ� root�� ���տ� ���Խ�Ų��.
        public void SplitElement(T element, T root)
        {
            SplitElement(element);
            Union(element, root);
        }

        // elements�� mainElement�� ���� ���տ��� �и��Ͽ� root���տ� ���Խ�Ų��.
        // ��, elements���� mainElement�� ���� �� ������ mainElement�� �и����� ����
        public void SplitElements(T mainElement, List<T> elements, T root)
        {
            // mainElement�� ��Ʈ ���� rank2�� Ʈ���� ��ȯ
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

            // mainElement�� ��Ʈ ���� �ϴ� Ʈ���� ��� ��带 �и��Ͽ� ���ο� ������ ����
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

        // elements�� mainElement�� ���� ���տ��� �и��Ͽ� ���ο� �������� ����
        // ��, elements���� mainElement�� ���� �� ������ mainElement�� �и����� ����
        public void SplitElements(T mainElement, List<T> elements)
        {
            // mainElement�� ��Ʈ ���� rank2�� Ʈ���� ��ȯ
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

            // mainElement�� ��Ʈ ���� �ϴ� Ʈ���� ��� ��带 �и��Ͽ� ���ο� ������ ����
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

        // root���� ���ο� ���Ҹ� �߰���
        public void AddElement(T element)
        {
            int elementNum = parent.Count;

            parent.Add(elementNum);
            values.Add(element);
            rank.Add(1);
            dict.Add(element, elementNum);
        }

        // element�� group�� ���� �������� ���� �߰���
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

        // �ϳ��� ���Ҹ� ������ ������
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

            // RemoveAt�� �ϸ� ��ȣ�� ���̱� ������ ����
            // parent ����
            for (int i = 0; i < parent.Count; i++)
            {
                if (parent[i] > elementNum)
                {
                    parent[i]--;
                }
            }

            // dict ���� foreach������ �ٷ� ������ ��� ������ ���� hashset�� �̿���
            // value �� �����ϱ� ������ foreach���� ������ ������ ������
            // dictionary�� Ư���� dict[key] = value �� �����͸� �߰��� �� �ֱ� �����ε�
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

        // ����� ��
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