using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IndexerSample.Models;

namespace IndexerSample.Services
{
    public class XMLParser : IFileParser
    {
        public Task<TokenList<Token>> GetTokenList(string filePath, Guid docGuid)
        {
            throw new NotImplementedException();
        }
    }
}
