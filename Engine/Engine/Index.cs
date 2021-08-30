using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine {
    public class Index {
        public readonly Dictionary<string, Token> Tokens = new Dictionary<string, Token>();

        public void AddWord(string word, DbDocument dbDocument, int position) {
            Token wordItem;
            if (Tokens.ContainsKey(word)) {
                wordItem = Tokens[word];
            }
            else {
                wordItem = new Token(word);
                Tokens[word] = wordItem;
            }

            wordItem.AddItem(dbDocument, position);
        }

        public async Task SaveToDb() {
            var saveActions = Tokens.Select(token => token.Value.SaveSelfToDb()).ToList();

            await Task.WhenAll(saveActions);
        }
    }
}