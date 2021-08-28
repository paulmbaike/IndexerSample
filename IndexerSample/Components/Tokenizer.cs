using System;
using System.Collections.Generic;
using Porter2Stemmer;
using IndexerSample.Models;
using IndexerSample.Services.Helpers;

namespace IndexerSample.Components
{
     public interface ITokenizer {
        char[] TermDelimeters { get; set; }
        List<string> StopWords { get; set; }
        int TermMinimumLength { get; set; }
        string AlphaOnlyString(string dirty);
        TokenList<Token> Tokenize(string[] termsRaw, Guid? docGuid);
    }
    public class Tokenizer
    {
        private readonly EnglishPorter2Stemmer _stemmer;
        private readonly IUtil _util;

        public Tokenizer(IUtil util){
            _stemmer = new EnglishPorter2Stemmer();
            _util = util;
        }

         private static char[] _TokenDelimeters = new char[]
        {
                '!',
                '\"',
                '#',
                '$',
                '%',
                '&',
                '\'',
                '(',
                ')',
                '*',
                '+',
                ',',
                '-',
                '.',
                '/',
                ':',
                ';',
                '<',
                '=',
                '>',
                '?',
                '@',
                '[',
                '\\',
                ']',
                '^',
                '_',
                '`',
                '{',
                '|',
                '}',
                '~',
                ' ',
                '\'',
                '\"',
                '\u001a',
                '\r',
                '\n',
                '\t'
           };

        private static List<string> _StopWords = new ()
        {
            "a",
            "about",
            "above",
            "after",
            "again",
            "against",
            "aint",
            "ain't",
            "all",
            "also",
            "am",
            "an",
            "and",
            "any",
            "are",
            "arent",
            "aren't",
            "as",
            "at",
            "be",
            "because",
            "been",
            "before",
            "being",
            "below",
            "between",
            "both",
            "but",
            "by",
            "cant",
            "can't",
            "cannot",
            "could",
            "couldnt",
            "couldn't",
            "did",
            "didnt",
            "didn't",
            "do",
            "does",
            "doesnt",
            "doesn't",
            "doing",
            "dont",
            "don't",
            "down",
            "during",
            "each",
            "few",
            "for",
            "from",
            "further",
            "had",
            "hadnt",
            "hadn't",
            "has",
            "hasnt",
            "hasn't",
            "have",
            "havent",
            "haven't",
            "having",
            "he",
            "hed",
            "he'd",
            "he'll",
            "hes",
            "he's",
            "her",
            "here",
            "heres",
            "here's",
            "hers",
            "herself",
            "him",
            "himself",
            "his",
            "how",
            "hows",
            "how's",
            "i",
            "id",
            "i'd",
            "i'll",
            "im",
            "i'm",
            "ive",
            "i've",
            "if",
            "in",
            "into",
            "is",
            "isnt",
            "isn't",
            "it",
            "its",
            "it's",
            "its",
            "itself",
            "lets",
            "let's",
            "me",
            "more",
            "most",
            "mustnt",
            "mustn't",
            "my",
            "myself",
            "no",
            "nor",
            "not",
            "of",
            "off",
            "on",
            "once",
            "only",
            "or",
            "other",
            "ought",
            "our",
            "ours",
            "ourselves",
            "out",
            "over",
            "own",
            "same",
            "shall",
            "shant",
            "shan't",
            "she",
            "she'd",
            "she'll",
            "shes",
            "she's",
            "should",
            "shouldnt",
            "shouldn't",
            "so",
            "some",
            "such",
            "than",
            "that",
            "thats",
            "that's",
            "the",
            "their",
            "theirs",
            "them",
            "themselves",
            "then",
            "there",
            "theres",
            "there's",
            "these",
            "they",
            "theyd",
            "they'd",
            "theyll",
            "they'll",
            "theyre",
            "they're",
            "theyve",
            "they've",
            "this",
            "those",
            "thou",
            "though",
            "through",
            "to",
            "too",
            "under",
            "until",
            "unto",
            "up",
            "very",
            "was",
            "wasnt",
            "wasn't",
            "we",
            "we'd",
            "we'll",
            "were",
            "we're",
            "weve",
            "we've",
            "werent",
            "weren't",
            "what",
            "whats",
            "what's",
            "when",
            "whens",
            "when's",
            "where",
            "wheres",
            "where's",
            "which",
            "while",
            "who",
            "whos",
            "who's",
            "whose",
            "whom",
            "why",
            "whys",
            "why's",
            "with",
            "wont",
            "won't",
            "would",
            "wouldnt",
            "wouldn't",
            "you",
            "youd",
            "you'd",
            "youll",
            "you'll",
            "youre",
            "you're",
            "youve",
            "you've",
            "your",
            "yours",
            "yourself",
            "yourselves"
        };

        private static int _TermMinimumLength = 3;

        public char[] TermDelimeters {
            get => _TokenDelimeters;
            set {
                if (value == null) throw new ArgumentNullException(nameof(TermDelimeters));
                _TokenDelimeters = value;
            }
        }
        public List<string> StopWords {
            get => _StopWords;
            set => _StopWords = value;
        }
        public int TermMinimumLength {
            get => _TermMinimumLength;
            set => _TermMinimumLength = value; }


        public string AlphaOnlyString(string dirty)
        {
            if (String.IsNullOrEmpty(dirty)) return null;
            string clean = null;
            for (int i = 0; i < dirty.Length; i++)
            {
                int val = (int)(dirty[i]);
                if (
                    ((val > 64) && (val < 91))          // A...Z
                    || ((val > 96) && (val < 123))      // a...z
                    )
                {
                    clean += dirty[i];
                }
            }
            return clean;
        }

        public string[] strip(string text)
        {
            //Use Delimeters to include only a selected portion of the data
            return text.Split(TermDelimeters, StringSplitOptions.RemoveEmptyEntries);
        }

        public TokenList<Token> Tokenize(string[] termsRaw, string docId)
        {
            List<string> termsAlphaOnly = new List<string>();
            TokenList<Token> tokenList = new();
            HashSet<string> terms = new();

            foreach (string curr in termsRaw)
            {
                if (String.IsNullOrEmpty(curr)) continue;
                if (curr.Length < TermMinimumLength) continue;
                if (StopWords.Contains(curr.ToLower())) continue;

                string currAlphaOnly = AlphaOnlyString(curr);
                if (String.IsNullOrEmpty(currAlphaOnly)) continue;
                var stemmedWord = _stemmer.Stem(currAlphaOnly).Value;
                termsAlphaOnly.Add(stemmedWord);
            }

            for (var i = 0; i < termsAlphaOnly.Count; i++)
            {
                var term = termsAlphaOnly[i];
                if (!terms.Contains(term))
                {
                    terms.Add(term);
                    tokenList.Add(new Token(term, i, docId));
                }
                else
                {
                    tokenList[term].Positions.Add(i);
                }
            }
            return tokenList;
        }
    }
}