using System.Text.Json;

namespace Lib.Tokenization.Interfaces
{
    public interface ITokenizerFactory
    {
        ITokenizer BuildFromText(string text);
        ITokenizer FromPayload(JsonElement payload);
    }
}



