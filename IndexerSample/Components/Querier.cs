using IndexerSample.Models;
using IndexerSample.Services.Helpers;

namespace IndexerSample.Components
{
    public class Querier
    {
        private Tokenizer _tokenizer;
        public string QueryString { get; private set; }
        public dynamic Results { get; }

        public Querier(IUtil util)
        {
            _tokenizer = new Tokenizer(util);
        }

        public TokenList<Token> Query(string query) {
            string[] rawQuery = _tokenizer.strip(query);
            TokenList<Token> tokenList = _tokenizer.Tokenize(rawQuery, null);
            return tokenList;
        }
    }
}