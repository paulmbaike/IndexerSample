using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using IndexerSample.Entities;
using IndexerSample.Helpers;
using IndexerSample.Models;
using IndexerSample.Services;
using IndexerSample.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IndexerSample.Controllers
{
    [Route("api/[controller]")]
    public class IndexerController : Controller
    {
        private readonly IFileParser _txtParser;
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IUtil _util;

        public IndexerController(IMapper mapper, IFileParser txtParser, DatabaseContext context, IUtil util)
        {
            _mapper = mapper;
            _txtParser = txtParser;
            _context = context;
            _util = util;
        }

        // GET: api/values
        [HttpGet]
        public ActionResult<TokenList<Token>> Get()
        {   
            var fileName = "GreatExpectations.txt";

            //get documentId
            var doc = _context.Documents.SingleOrDefault(d => d.DocPath == "fileName");
            var docGuid = doc?.DocGuid ?? Guid.NewGuid();

            var tokenList = _txtParser.GetTokenList(fileName, docGuid).GetAwaiter().GetResult();

            if (doc == null)
            {
                doc = new Document { DocPath = fileName, CreatedAt = DateTime.UtcNow };
                _context.Documents.Add(doc);
                _context.SaveChanges();
            }

            var indices = _mapper.Map<List<InvertedIndex>>(tokenList);
            _context.InvertedIndices.AddRange(indices);
            _context.SaveChanges();

            return tokenList;
        }

        [HttpGet("search")]
        public ActionResult Querier([FromQuery] string query)
        {
            var rawQuery = query.Split(_util.TermDelimeters, StringSplitOptions.RemoveEmptyEntries);
            var tokenList = _util.Tokenizer(rawQuery, null);
            //var tokenizedQuery = _mapper.Map<List<string>>(tokenList);
            var tokenizedQuery = new List<string>();

            foreach (Token token in tokenList) tokenizedQuery.Add(token.Term);

             var result = _context.InvertedIndices.Where(i => tokenizedQuery.Contains(i.Token));
            return Ok(result);
        }


    }
}
