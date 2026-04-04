using Lib.Tokenization.Model;
using Lib.Tokenization.Tokanizers;
using System.Text.Json;


namespace Lib.Tokenization.Tests
{
    [TestFixture]
    public class CharTokenizerTests
    {
        private CharTokenizer _tokenizer;
        private Vocabulary _vocab;

        [SetUp]
        public void Setup()
        {
            _vocab = new Vocabulary();
            _vocab.BuildFromText("Привіт світ"); 
            _tokenizer = new CharTokenizer(_vocab);
        }

        [Test]
        public void EncodeDecode_RoundTrip_ReturnsOriginalText()
        {
            string original = "Привіт"; 
            
            int[] encoded = _tokenizer.Encode(original);
            string decoded = _tokenizer.Decode(encoded);

            Assert.That(decoded, Is.EqualTo(original));
        }

        [Test]
        public void Encode_UnknownTokens_UsesUnkId()
        {
            int[] encoded = _tokenizer.Encode("X");

            Assert.That(encoded.Length, Is.EqualTo(1));
            Assert.That(encoded[0], Is.EqualTo(Vocabulary.UnkId));
        }

        [Test]
        public void EncodeDecode_EmptyText_HandlesGracefully()
        {
            int[] encoded = _tokenizer.Encode("");
            Assert.That(encoded.Length, Is.EqualTo(0)); 

            string decoded = _tokenizer.Decode(new int[0]);
            Assert.That(decoded, Is.EqualTo("")); 
        }

        [Test]
        public void Checkpoint_RoundTrip_Serialization()
        {
            var payload = _tokenizer.GetPayloadForCheckpoint();

            string jsonString = JsonSerializer.Serialize(payload);
            using JsonDocument doc = JsonDocument.Parse(jsonString);

            var factory = new CharTokenizerFactory();
            var restoredTokenizer = factory.FromPayload(doc.RootElement);

            string testText = "світ";
            int[] encoded = restoredTokenizer.Encode(testText);
            string decoded = restoredTokenizer.Decode(encoded);

            Assert.That(decoded, Is.EqualTo(testText));
        }
    }
}