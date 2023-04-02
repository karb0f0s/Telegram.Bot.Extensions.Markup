using MarkupTests.Fixture;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MarkupTest.Markdown;

public class MarkdownMarkupTests : IClassFixture<MarkupTestFixture>
{
    private readonly MarkupTestFixture _fixture;

    public MarkdownMarkupTests(MarkupTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_text_markdown_simple()
    {
        string test_md_string = """
            Test for <*bold*, _ita_\__lic_, `code`,
             [links](http://github.com/ab_),
             [text-mention](tg://user?id=123456789) and ```python\\npre```.
             http://google.com/ab\_
            """
            .ReplaceLineEndings("")
            .Replace(@"\\n", "\n", StringComparison.Ordinal);

        string? text_markdown = _fixture.TestMessage.TextMarkdown();

        Assert.Equal(test_md_string, text_markdown);
    }

    [Fact]
    public void Test_text_markdown_v2_simple()
    {
        string test_md_string = """
            __Test__ for <*bold*, _ita\_lic_, `\\\`code`,
             [links](http://github.com/abc\\\)def),
             [text\-mention](tg://user?id=123456789) and ```\`\\pre```\.
             http://google\.com and _bold *nested in ~strk\>trgh~ nested in* italic_\.
             ```python\\nPython pre```\. ||Spoiled||\.
            """
            .ReplaceLineEndings("")
            .Replace(@"\\n", "\n", StringComparison.Ordinal);

        string? text_markdown = _fixture.TestMessageV2.TextMarkdownV2();

        Assert.Equal(test_md_string, text_markdown);
    }

    [Fact]
    public void Test_text_markdown_new_in_v2()
    {
#pragma warning disable IDE0017 // Simplify object initialization
        Message message = new()
        {
            Text = "test",
        };
#pragma warning restore IDE0017 // Simplify object initialization

        message.Entities = new MessageEntity[]
        {
            new() { Type = MessageEntityType.Bold, Offset = 0, Length = 4 },
            new() { Type = MessageEntityType.Italic, Offset = 0, Length = 4 },
        };
        Assert.Throws<ArgumentException>(() => message.TextMarkdown());

        message.Entities = new MessageEntity[]
        {
            new() { Type = MessageEntityType.Underline, Offset = 0, Length = 4 },
        };
        Assert.Throws<ArgumentException>(() => message.TextMarkdown());

        message.Entities = new MessageEntity[]
        {
            new() { Type = MessageEntityType.Strikethrough, Offset = 0, Length = 4 },
        };
        Assert.Throws<ArgumentException>(() => message.TextMarkdown());

        message.Entities = new MessageEntity[]
        {
            new() { Type = MessageEntityType.Spoiler, Offset = 0, Length = 4 },
        };
        Assert.Throws<ArgumentException>(() => message.TextMarkdown());
    }

    [Fact]
    public void Test_text_markdown_empty()
    {
        Message message = new()
        {
            Text = null,
            Caption = "test",
        };

        Assert.Null(message.TextMarkdown());
        Assert.Null(message.TextMarkdownV2());
    }

    [Fact]
    public void Test_text_markdown_urled()
    {
        string test_md_string = """
            Test for <*bold*, _ita_\__lic_, `code`,
             [links](http://github.com/ab_),
             [text-mention](tg://user?id=123456789) and ```python\\npre```.
             [http://google.com/ab_](http://google.com/ab_)
            """
            .ReplaceLineEndings("")
            .Replace(@"\\n", "\n", StringComparison.Ordinal);

        string? text_markdown = _fixture.TestMessage.TextMarkdownUrled();

        Assert.Equal(test_md_string, text_markdown);
    }

    [Fact]
    public void Test_text_markdown_v2_urled()
    {
        string test_md_string = """
            __Test__ for <*bold*, _ita\_lic_, `\\\`code`,
             [links](http://github.com/abc\\\)def),
             [text\-mention](tg://user?id=123456789) and ```\`\\pre```\.
             [http://google\.com](http://google.com) and _bold *nested in ~strk\>trgh~
             nested in* italic_\. ```python\\nPython pre```\. ||Spoiled||\.
            """
            .ReplaceLineEndings("")
            .Replace(@"\\n", "\n", StringComparison.Ordinal);

        string? text_markdown = _fixture.TestMessageV2.TextMarkdownV2Urled();

        Assert.Equal(test_md_string, text_markdown);
    }
}
