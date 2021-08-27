using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace IndexerSample.Models
{

    public interface IToken
    {
        public string Term { get; set; }
    }

    public class Token : IToken
    {

        public Token(string name, long position, Guid? docGuid)
        {
            this.Term = name;
            this.Positions = new List<long> { position };
            this.DocGuid = docGuid;
        }

        public string Term { get; set; }
        public List<long> Positions { get; set; }
        public Guid? DocGuid { get; set; }
    }

    public class TokenList<T> : IEnumerable where T : IToken
    {

        private List<T> tokenList = new();

        public T this[string key] => FindTokenByIndex(key);

        public T FindTokenByIndex(string key)
        {
            foreach (var item in tokenList)
            {
                if (item.Term == key) return item;
            }

            throw new ArgumentOutOfRangeException("index is out of bound");
        }

        public void Add(T token)
        {
            tokenList.Add(token);
        }

        public IEnumerator GetEnumerator()
        {
            return new TokenListEnumerator(tokenList);
        }

        private class TokenListEnumerator : IEnumerator
        {
            public List<T> tokenList;
            int index = -1;

            //constructor
            public TokenListEnumerator(List<T> list)
            {
                tokenList = list;
            }

            private IEnumerator getEnumerator()
            {
                return (IEnumerator)this;
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return tokenList[index];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public bool MoveNext()
            {
                index++;
                return (index < tokenList.Count);
            }

            public void Reset()
            {
                index = -1;
            }
        }

    }
}
