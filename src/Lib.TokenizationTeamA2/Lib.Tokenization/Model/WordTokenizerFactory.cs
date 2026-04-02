using System.Text.Json;
namespace Lib.Tokenization;

public class WordTokenizerFactory : ITokenizerFactory
{
    public ITokenizer BuildFromText(string text)
    {
        var vocab = new WordVocabulary();
        if (string.IsNullOrEmpty(text) == false)
        {
            vocab.BuildFromText(text);
        }
        return new WordTokenizer(vocab);
    }

    public ITokenizer FromPayload(JsonElement payload)
    {
        var vocab = TokenizerPayloadSerializer.DeserializeWordVocab(payload);
        return new WordTokenizer(vocab);
    }
}

