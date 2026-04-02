

namespace Integration.DataPipeline.Tests;

[TestFixture]
public class TokenizerIntegrationTests
{
    private CorpusLoader loader;

    [SetUp]
    public void Setup()
    {
        var normalizer = new CorpusTextNormalizer();
        var splitter = new CorpusSplitter();
        var defaultFileSystem = new DefaultFileSystem();
        loader = new CorpusLoader(normalizer, splitter, defaultFileSystem);
    }


    [Test]
    public void CorpusAndCharTokenizer_EncodeDecodeRoundTrip()
    {
        var options = new CorpusLoadOptions(false, 0.1, "fallback");
        string rawText = "Привіт, світ! Тест команди В1.";
        var corpus = loader.LoadFromText(rawText, options);

        ITokenizerFactory factory = new CharTokenizerFactory();
        ITokenizer tokenizer = factory.BuildFromText(corpus.TrainText);

        int[] encoded = tokenizer.Encode(corpus.TrainText);
        string decoded = tokenizer.Decode(encoded);

        Assert.That(decoded, Is.EqualTo(corpus.TrainText));
    }


    [Test]
    public void CorpusAndWordTokenizer_EncodeDecodeRoundTrip()
    {
        var options = new CorpusLoadOptions(true, 0.1, "fallback");

        string rawText = "це просто текст для перевірки слів";
        var corpus = loader.LoadFromText(rawText, options);

        ITokenizerFactory factory = new WordTokenizerFactory();
        ITokenizer tokenizer = factory.BuildFromText(corpus.TrainText);

        int[] encoded = tokenizer.Encode(corpus.TrainText);
        string decoded = tokenizer.Decode(encoded);

        Assert.That(decoded, Is.EqualTo(corpus.TrainText));
    }


    [Test]
    public void CorpusLowercase_MatchesWordTokenizerVocabulary()
    {
        var options = new CorpusLoadOptions(true, 0.1, "fallback");
        string text = "ЦЕ ДОСИТЬ ДОВГИЙ ТЕКСТ ДЛЯ ПЕРЕВІРКИ";

        var corpus = loader.LoadFromText(text, options);
        ITokenizerFactory factory = new WordTokenizerFactory();
        ITokenizer tokenizer = factory.BuildFromText(corpus.TrainText);

        string upperCaseInput = corpus.TrainText.ToUpper();
        int[] encoded = tokenizer.Encode(upperCaseInput);
        string decoded = tokenizer.Decode(encoded);

        Assert.That(decoded, Is.EqualTo(corpus.TrainText));
    }
}


