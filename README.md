# Документацiя “How to integrate”

Цей гайд описує процес підключення підсистем Lib.Corpus та Lib.Tokenization для роботи з текстовими даними, створення токенізатора та підготовки до тренування моделі.

## 1. Опцiї CorpusLoadOptions
Для налаштування процесу завантаження та обробки датасету використовується клас CorpusLoadOptions. Він має такі ключові параметри:

LowerCase (bool): Якщо true, весь текст корпусу буде приведено до нижнього регістру перед токенізацією.
ValidateFraction (double): Відсоток тексту, який буде відрізано для валідації (за замовчуванням 0.1, тобто 10%).
FallBack (string): Текст, який буде використано у випадку, якщо файл корпусу не знайдено за вказаним шляхом.

## 2. Як вибрати word vs char
Підсистема токенізації підтримує дві стратегії, за кожну з яких відповідає своя фабрика, що реалізує інтерфейс ITokenizerFactory:

Word (послівна): Використовуйте WordTokenizerFactory. Розбиває текст на окремі слова, ігноруючи пунктуацію та пробіли.
Char (посимвольна): Використовуйте CharTokenizerFactory. Кожен унікальний символ стає окремим токеном. Словник виходить значно меншим, підходить для базових моделей.

## 3. Як отримати ITokenizer з корпусу
Оскільки корпус і токенізатор розділені, процес виглядає так: спочатку завантажити CorpusClass за допомогою CorpusLoader, а потім передаєте тренувальний текст (TrainText) у відповідну фабрику токенізатора.

## 4. Приклад коду для ініціалізації, Trainer та Chat

Ось приклад того, як підключати наші модулі та використовувати їх у своїх компонентах Trainer та Chat:

```csharp
using System;

// 1. Налаштовуємо опції завантаження
var options = new CorpusLoadOptions(lowerCase: true, validateFraction: 0.15, fallBack: "Default text");

// 2. Ініціалізуємо завантажувач корпусу з необхідними залежностями
var normalizer = new CorpusTextNormalizer();
var splitter = new CorpusSplitter();
var fileSystem = new DefaultFileSystem();
var corpusLoader = new CorpusLoader(normalizer, splitter, fileSystem);

// Завантажуємо текст (розбитий на Train та Validation)
CorpusClass corpus = corpusLoader.Load("path/to/your/dataset.txt", options);

// 3. Обираємо стратегію токенізації (Word або Char)
ITokenizerFactory tokenizerFactory = new WordTokenizerFactory();

// Отримуємо готовий ITokenizer, побудований на основі тренувального тексту
ITokenizer tokenizer = tokenizerFactory.BuildFromText(corpus.TrainText);

// 4. Приклад інтеграції з підсистемами Trainer та Chat
var trainer = new Trainer(tokenizer);
var model = trainer.Train(corpus.TrainText);

var chat = new Chat(model, tokenizer);
string response = chat.GenerateResponse("Привіт, як справи?");
Console.WriteLine(response);
