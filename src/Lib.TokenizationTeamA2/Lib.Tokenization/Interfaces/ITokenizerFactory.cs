using System.Text.Json;
public interface ITokenizerFactory
{
    ITokenizer BuildFromText(string text);
    ITokenizer FromPayload(JsonElement payload);
}



