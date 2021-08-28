using System;
using System.Collections;
using System.Collections.Generic;

namespace IndexerSample.Models
{

    public interface IToken
    {
        public string Term { get; set; }
        public HashSet<long> Positions {get; set;}
        public string DocId { get; set; }
    }

    public class Token : IToken
    {
        public string Term { get; set; }
        public HashSet<long> Positions { get; set; }
        public string DocId { get; set; }

        public Token(string term, long position, string docId)
        {
            this.Term = term;
            this.Positions = new HashSet<long> { position };
            this.DocId = docId;
        }
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

        public List<TokenDocument> GetTokenDocuments(){
            List<TokenDocument> result = new List<TokenDocument>();
            foreach(var token in tokenList){
                TokenDocument tDoc = new TokenDocument(token.Term);
                tDoc.addDocument(token.DocId, token.Positions); 
                result.Add(tDoc);  
            }  
            return result;
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
