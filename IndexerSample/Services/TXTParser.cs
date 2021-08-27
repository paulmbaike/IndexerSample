using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IndexerSample.Models;
using IndexerSample.Services.Helpers;
using Microsoft.Extensions.Logging;

namespace IndexerSample.Services
{
    public class TXTParser : IFileParser
    {
        private readonly IUtil _util;
        private readonly ILogger<Util> _logger;

        public TXTParser(IUtil util, ILogger<Util> logger)
        {
            _util = util;
            _logger = logger;
        }

        public async Task<TokenList<Token>> GetTokenList(string fileName, Guid docGuid)
        {
            byte[] fileContent = await _util.GetFileContentAsync(fileName);

            string content = Encoding.UTF8.GetString(fileContent);

            
            TokenList<Token> tokenList = new();

            string[] termsRaw = content.Split(_util.TermDelimeters, StringSplitOptions.RemoveEmptyEntries);
            

            if (termsRaw != null && termsRaw.Length > 0)
            {
                tokenList = _util.Tokenizer(termsRaw, docGuid);
                _logger.LogInformation("extracted terms");
            }
            else
            {
                _logger.LogInformation("no terms found");
            }

            return tokenList;
        }
    }
}
