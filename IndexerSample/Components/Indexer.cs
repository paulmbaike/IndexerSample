using System;
using System.IO;
using System.Collections.Generic;
using TikaOnDotNet.TextExtraction;
using IndexerSample.Models;
using IndexerSample.Services.Helpers;
using Microsoft.Extensions.FileProviders;

namespace IndexerSample.Components
{
    public class Indexer 
    {
        private string _repoPath = "LocalRepository";
        private readonly IUtil _util;
        private readonly Tokenizer _tokenizer;
        private List<KeyValuePair<string, long>> _indexedFilesInfo;
        private HashSet<string> _indexedFiles = new HashSet<string>();

        public Indexer(IUtil util){
            _util = util;
            _tokenizer = new Tokenizer(util);
            _indexedFilesInfo = new List<KeyValuePair<string, long>>();
        }

        public TokenList<Token> Index(String path)
        {
            TextExtractor textExtractor = new TextExtractor();
            TextExtractionResult fileData = textExtractor.Extract(path);
            string[] termsRaw = _tokenizer.strip(fileData.Text);
            return _tokenizer.Tokenize(termsRaw, null);  
        }

        public void IndexRespository(){
            // Get all files in repository on initial load
            string[] allfiles = Directory.GetFiles(_repoPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allfiles){
                FileInfo info = new FileInfo(file);
                string filePath = info.FullName;

                // Check if file has been indexed before
                if(_indexedFiles.Contains(filePath)){
                    // Index file if it has changed or indexed version does not exist yet
                    int fileEntryIndex = _indexedFilesInfo.FindIndex((fileEntry) => fileEntry.Key == filePath);
                    KeyValuePair<string,long> fileEntry = _indexedFilesInfo[fileEntryIndex];
                    // Check if File has changed
                    if(fileEntry.Value != info.LastWriteTime.ToBinary()){
                        //TODO: Re-index file and update database entries
                        // Update indexed file info
                        _indexedFilesInfo[fileEntryIndex] = new KeyValuePair<string, long>(filePath, info.LastWriteTime.ToBinary());
                    }
                } else {
                    //TODO: Index file and update database entries
                    var tokenList = Index(filePath);


                    // Save Last Write Time per document
                    _indexedFilesInfo.Add(new KeyValuePair<string, long>(filePath, info.LastWriteTime.ToBinary()));
                }
            }
        }
    }
}