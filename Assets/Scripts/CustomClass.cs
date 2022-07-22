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

        public bool ContainsKey(T key)
        {
            return dict.ContainsKey(key);
        }

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
            // not implement
        }

        // �������� ���Ҹ� ������ ���տ��� �����ϰ� �װ͵��� ���ο� ������ �̷�� ��
        // ��, elements�� �ݵ�� �ϳ��� ���տ� ���ؾ� ��
        public void SplitElements(List<T> elements)
        {
            // not implement
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
                    if (parent[i] != elementNum)
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
                parent.RemoveAt(elementNum);
                values.RemoveAt(elementNum);
                rank.RemoveAt(elementNum);
                dict.Remove(element);
            }
            else
            {
                parent.RemoveAt(elementNum);
                values.RemoveAt(elementNum);
                rank.RemoveAt(elementNum);
                dict.Remove(element);
            }
        }
    }
}