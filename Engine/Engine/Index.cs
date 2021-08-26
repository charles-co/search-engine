using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine {
    public class Index {
        public Dictionary<string, Token> tokens = new Dictionary<string, Token>();

        public void AddWord(string word, Document document, int position) {
            Token wordItem;
            if (tokens.ContainsKey(word)) {
                wordItem = tokens[word];
            }
            else {
                wordItem = new Token(word);
                tokens[word] = wordItem;
            }

            wordItem.AddItem(document, position);
        }

        public async Task SaveToDb() {
            List<Task> saveActions = new List<Task>();

            foreach (KeyValuePair<string,Token> token in tokens) {
                saveActions.Add(token.Value.SaveSelfToDb());
            }

            await Task.WhenAll(saveActions);
        }
    }
}