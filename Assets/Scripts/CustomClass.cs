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

        public bool ContainsKey(T key)
        {
            return dict.ContainsKey(key);
        }

        public List<T> GetAllElements(T elementOfSet)
        {
            List<T> list = new List<T>();

            if (!dict.ContainsKey(elementOfSet))
            {
                return list;
            }

            int currentParent = parent[dict[elementOfSet]];

            for (int i = 0; i < parent.Count; i++)
            {
                if (currentParent == parent[i])
                {
                    list.Add(values[i]);
                }
            }

            return list;
        }
    }
}