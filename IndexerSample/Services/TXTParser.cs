using System;
using System.Text;
using System.Threading.Tasks;
using IndexerSample.Models;
using IndexerSample.Components;
using IndexerSample.Services.Helpers;
using Microsoft.Extensions.Logging;

namespace IndexerSample.Services
{
    public class TXTParser : IFileParser
    {
        private readonly IUtil _util;
        private readonly ILogger<Util> _logger;
        private readonly ITokenizer _tokenizer;

        public TXTParser(IUtil util, ILogger<Util> logger, ITokenizer tokenizer)
        {
            _util = util;
            _logger = logger;
            _tokenizer = tokenizer;
        }

        public void GetTokenList(string fileName, Guid docGuid)
        {
            // byte[] fileContent = await _util.GetFileContentAsync(fileName);

            // string content = Encoding.UTF8.GetString(fileContent);
            
            // TokenList<Token> tokenList = new();

            // string[] termsRaw = content.Split(_tokenizer.TermDelimeters, StringSplitOptions.RemoveEmptyEntries);
            

            // if (termsRaw != null && termsRaw.Length > 0)
            // {
            //     tokenList = _tokenizer.Tokenize(termsRaw, docGuid);
            //     _logger.LogInformation("extracted terms");
            // }
            // else
            // {
            //     _logger.LogInformation("no terms found");
            // }

            // return tokenList;
        }
    }
}
