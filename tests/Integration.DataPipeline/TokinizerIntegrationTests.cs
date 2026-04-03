namespace Integration.DataPipeline.Tests;

public class Fake : IFileSystem
{
    public string? File { get; set; }
    public bool ExistsFile { get; set; }
    public string ReadAllText(string text) => File;
    public bool Exists(string text) => ExistsFile;
}

[TestFixture]
public class TokenizerIntegrationTests
{
    private CorpusLoader loader;
    private Fake fakeFikeSystem;

    [SetUp]
    public void Setup()
    {
        fakeFikeSystem = new Fake();
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

    [Test]
    public void CorpusLoader_LoadMissingFile_ReturnsFallback()
    {
        var loadOptions = new CorpusLoadOptions(true, 0.1, "1234567890");
        CorpusClass corpus = loader.Load("not existing file", loadOptions);

        ITokenizerFactory factory = new CharTokenizerFactory();
        ITokenizer tokenizerTrain = factory.BuildFromText(corpus.TrainText);
        ITokenizer tokenizerVal = factory.BuildFromText(corpus.ValText);

        int[] encodedTrain = tokenizerTrain.Encode(corpus.TrainText);
        string decodedTrain = tokenizerTrain.Decode(encodedTrain);

        int[] encodedVal = tokenizerVal.Encode(corpus.ValText);
        string decodedVal = tokenizerVal.Decode(encodedVal);

        Assert.That(decodedTrain, Is.EqualTo("123456789"));
        Assert.That(decodedVal, Is.EqualTo("0"));
    }

    [Test]
    public void CorpusAndTokenizer_EmptyText_HandledGracefully()
    {
        CorpusClass corpus = new CorpusClass("", "");

        ITokenizerFactory factory = new CharTokenizerFactory();
        ITokenizer tokenizerTrain = factory.BuildFromText(corpus.TrainText);
        ITokenizer tokenizerVal = factory.BuildFromText(corpus.ValText);

        int[] encodedTrain = tokenizerTrain.Encode(corpus.TrainText);
        string decodedTrain = tokenizerTrain.Decode(encodedTrain);

        int[] encodedVal = tokenizerVal.Encode(corpus.ValText);
        string decodedVal = tokenizerVal.Decode(encodedVal);

        Assert.That(decodedTrain, Is.EqualTo(""));
        Assert.That(decodedVal, Is.EqualTo(""));
    }

    [Test]
    public void ReadFromFileAndTokanize()
    {
        fakeFikeSystem.ExistsFile = true;
        fakeFikeSystem.File = "abcdefghijklvnopqrst";
        var options = new CorpusLoadOptions(true, 0.2, "fallback");

        CorpusClass corpus = loader.Load("existing.txt", options);

        ITokenizerFactory factory = new CharTokenizerFactory();
        ITokenizer tokenizerTrain = factory.BuildFromText(corpus.TrainText);
        ITokenizer tokenizerVal = factory.BuildFromText(corpus.ValText);

        int[] encodedTrain = tokenizerTrain.Encode(corpus.TrainText);
        string decodedTrain = tokenizerTrain.Decode(encodedTrain);

        int[] encodedVal = tokenizerVal.Encode(corpus.ValText);
        string decodedVal = tokenizerVal.Decode(encodedVal);

        Assert.That(decodedTrain, Is.EqualTo("abcdefghijklvnop"));
        Assert.That(decodedVal, Is.EqualTo("qrst"));
    }
}


