using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IndexerSample.Models;

namespace IndexerSample.Services
{
    public interface IFileParser
    {
        Task<TokenList<Token>> GetTokenList(string filePath, Guid docGuid);
    }
}
