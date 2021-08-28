using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using IndexerSample.Helpers;
using IndexerSample.Models;
using IndexerSample.Services;
using IndexerSample.Components;
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
        private Indexer _indexer;
        private Querier _querier;

        public IndexerController(IMapper mapper, IFileParser txtParser, DatabaseContext context, IUtil util)
        {
            _mapper = mapper;
            _txtParser = txtParser;
            _context = context;
            _util = util;
            _indexer = new Indexer(_util);
            _querier = new Querier(_util);
        }

        // GET: api/values
        [HttpGet]
        public ActionResult Get()
        {   
            return Ok();
            // var fileName = "GreatExpectations.txt";

            // //get documentId
            // var doc = _context.Documents.SingleOrDefault(d => d.DocPath == "fileName");
            // var docGuid = doc?.DocGuid ?? Guid.NewGuid();

            // var tokenList = _txtParser.GetTokenList(fileName, docGuid).GetAwaiter().GetResult();

            // if (doc == null)
            // {
            //     doc = new Document { DocPath = fileName, CreatedAt = DateTime.UtcNow };
            //     _context.Documents.Add(doc);
            //     _context.SaveChanges();
            // }

            // var indices = _mapper.Map<List<InvertedIndex>>(tokenList);
            // _context.InvertedIndices.AddRange(indices);
            // _context.SaveChanges();

            // return tokenList;
        }

        [HttpGet("search")]
        public async Task<ActionResult> Querier([FromQuery] string query)
        {
            var tokenList = _querier.Query(query);
            var tokenizedQuery = new List<string>();
            foreach (Token token in tokenList) tokenizedQuery.Add(token.Term);
            List<ITokenDocument> tokenDocuments = await _util.GetTokenDocumentsByTerms(tokenizedQuery);
            var documentIDList = new List<string>();
            foreach (TokenDocument tDoc in tokenDocuments) {
                foreach(var doc in tDoc.Docs){
                    documentIDList.Add(doc.Key);
                }
            }
            HashSet<IDocument> documents = await _util.GetDocumentsByID(documentIDList);
            Console.WriteLine($"Token Documents: {tokenDocuments}");
            Console.WriteLine($"Documents: {documents}");
            //var result = _context.InvertedIndices.Where(i => tokenizedQuery.Contains(i.Token));
            return Ok();
        }
    }
}
