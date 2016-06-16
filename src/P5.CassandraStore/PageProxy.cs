using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P5.Store.Core.Models;

namespace P5.CassandraStore
{
    public static class PageProxyHandleExtensions
    {
        public static PageProxyHandle<T> ToPageProxyHandle<T>(this IPage<T> page)
        {
            return new PageProxyHandle<T>(page);
        }
    }

    // this is for giving out to the UI pages
    public class PageProxyHandle<T>
    {
      
        public PageProxyHandle(IPage<T> page)
        {
            Count = page.Count;
            IsReadOnly = page.IsReadOnly;
            CurrentPagingState = page.CurrentPagingState == null?"":Convert.ToBase64String(page.CurrentPagingState);
            PagingState = page.PagingState == null?"":Convert.ToBase64String(page.PagingState);
            Data = page;
        }
        public int Count { get; set; }
        public bool IsReadOnly { get; set; }
        public string CurrentPagingState { get; set; }
        public string PagingState { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
    //  good old hardcoded mapping vs the Castle intercepter with reflection.
    public class PageProxy<T> : IPage<T>
    {
        private Cassandra.Mapping.IPage<T> _cassandraPage;
        public PageProxy(Cassandra.Mapping.IPage<T> cassandraPage)
        {
            _cassandraPage = cassandraPage;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _cassandraPage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cassandraPage.GetEnumerator();
        }

        public void Add(T item)
        {
            _cassandraPage.Add(item);
        }

        public void Clear()
        {
            _cassandraPage.Clear();
        }

        public bool Contains(T item)
        {
            return _cassandraPage.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _cassandraPage.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _cassandraPage.Remove(item);
        }

        public int Count { get { return _cassandraPage.Count; } }
        public bool IsReadOnly { get { return _cassandraPage.IsReadOnly; } }
        public byte[] CurrentPagingState { get { return _cassandraPage.CurrentPagingState; } }
        public byte[] PagingState { get { return _cassandraPage.PagingState; } }

    }
}
