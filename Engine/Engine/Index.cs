using System.Collections.Generic;

namespace Engine {
    public class Index {
        public Dictionary<string, Token> tokens = new Dictionary<string, Token>();

        public void AddWord(string word, int documentId, int position) {
            Token wordItem;
            if (tokens.ContainsKey(word)) {
                wordItem = tokens[word];
            }
            else {
                wordItem = new Token(word);
                tokens[word] = wordItem;
            }
            
            wordItem.AddItem(documentId, position);
        }
    }
}