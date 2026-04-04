using Lib.Tokenization.Interfaces;
using Lib.Tokenization.Serialization;
using Lib.Tokenization.Tokanizers;
using System.Text.Json;

namespace Lib.Tokenization.Model
{
    public class CharTokenizerFactory : ITokenizerFactory
    {
        public ITokenizer BuildFromText(string text)
        {
            var vocab = new Vocabulary();
            if (string.IsNullOrEmpty(text) == false)
            {
                vocab.BuildFromText(text);
            }
            return new CharTokenizer(vocab);
        }

        public ITokenizer FromPayload(JsonElement payload)
        {
            var vocab = TokenizerPayloadSerializer.DeserializeCharVocab(payload);
            return new CharTokenizer(vocab);
        }
    }
}