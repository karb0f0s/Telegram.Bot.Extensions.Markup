using Telegram.Bot.Types.Enums;

namespace MarkupTest.Helpers;

public class HelperTests
{
    [Theory]
    [InlineData("*bold*", @"\*bold\*")]
    [InlineData("_italic_", @"\_italic\_")]
    [InlineData("`code`", @"\`code\`")]
    [InlineData("[text_link](https://github.com/)", @"\[text\_link](https://github.com/)")]
    public void Test_escape_markdown(string test_str, string expected)
    {
        Assert.Equal(expected, Tools.EscapeMarkdown(test_str));
    }

    [Theory]
    [InlineData(@"a_b*c[d]e", @"a\_b\*c\[d\]e")]
    [InlineData(@"(fg) ", @"\(fg\) ")]
    [InlineData(@"h~I`>JK#L+MN", @"h\~I\`\>JK\#L\+MN")]
    [InlineData(@"-O=|p{qr}s.t!\ ", @"\-O\=\|p\{qr\}s\.t\!\\ ")]
    [InlineData(@"\u", @"\\u")]
    public void Test_escape_markdown_v2(string test_str, string expected)
    {
        Assert.Equal(expected, Tools.EscapeMarkdown(test_str, parseMode: ParseMode.MarkdownV2));
    }

    [Theory]
    [InlineData(@"mono/pre:", "mono/pre:")]
    [InlineData(@"`abc`", @"\`abc\`")]
    [InlineData(@"\int", @"\\int")]
    [InlineData(@"(`\some \` stuff)", @"(\`\\some \\\` stuff)")]
    public void Test_escape_markdown_v2_monospaced(string test_str, string expected)
    {
        string escaped;

        escaped = Tools.EscapeMarkdown(
            test_str,
            parseMode: ParseMode.MarkdownV2,
            entityType: MessageEntityType.Pre);

        Assert.Equal(expected, escaped);

        escaped = Tools.EscapeMarkdown(
            test_str,
            parseMode: ParseMode.MarkdownV2,
            entityType: MessageEntityType.Code);

        Assert.Equal(expected, escaped);
    }

    [Fact]
    public void Test_escape_markdown_v2_text_link()
    {
        string test     = """https://url.containing/funny)cha)\ra\)cter\s""";
        string expected = """https://url.containing/funny\)cha\)\\ra\\\)cter\\s""";

        string escaped = Tools.EscapeMarkdown(
            test,
            parseMode: ParseMode.MarkdownV2,
            entityType: MessageEntityType.TextLink);

        Assert.Equal(expected, escaped);
    }

    [Fact]
    public void Test_markdown_invalid_version()
    {
        Assert.Throws<ArgumentException>(
            () => Tools.EscapeMarkdown("abc", parseMode: ParseMode.Html));

        Assert.Throws<ArgumentException>(
            () => Tools.MentionMarkdown(1, "abc", parseMode: ParseMode.Html));
    }

    [Fact]
    public void Test_create_deep_linked_url()
    {
        string username = "JamesTheMock";
        string payload = "hello";

        string expected, actual;

        expected = $"""https://t.me/{username}?start={payload}""";
        actual = Tools.CreateDeepLinkedUrl(username, payload);
        Assert.Equal(expected, actual);

        expected = $"""https://t.me/{username}?startgroup={payload}""";
        actual = Tools.CreateDeepLinkedUrl(username, payload, group: true);
        Assert.Equal(expected, actual);

        payload = "";
        expected = $"""https://t.me/{username}""";
        Assert.Equal(expected, Tools.CreateDeepLinkedUrl(username));
        Assert.Equal(expected, Tools.CreateDeepLinkedUrl(username, payload));

        payload = null!;
        expected = $"""https://t.me/{username}""";
        Assert.Equal(expected, Tools.CreateDeepLinkedUrl(username, payload));

        Assert.Throws<ArgumentException>(()
            => Tools.CreateDeepLinkedUrl(username, "text with spaces"));

        Assert.Throws<ArgumentException>(()
            => Tools.CreateDeepLinkedUrl(username, new string('0', 65)));

        Assert.Throws<ArgumentException>(()
            => Tools.CreateDeepLinkedUrl(null!, payload: null));

        Assert.Throws<ArgumentException>(()
            => Tools.CreateDeepLinkedUrl("abc", payload: null));
    }

    [Fact]
    public void Test_mention_html()
    {
        string expected = """<a href="tg://user?id=1">the name</a>""";

        string mention = Tools.MentionHtml(1, "the name");

        Assert.Equal(expected, mention);
    }

    [Theory]
    [InlineData(@"the name", @"[the name](tg://user?id=1)")]
    [InlineData(@"under_score", @"[under_score](tg://user?id=1)")]
    [InlineData(@"starred*text", @"[starred*text](tg://user?id=1)")]
    [InlineData(@"`backtick`", @"[`backtick`](tg://user?id=1)")]
    [InlineData(@"[square brackets", @"[[square brackets](tg://user?id=1)")]
    public void Test_mention_markdown(string test_str, string expected)
    {
        string mention = Tools.MentionMarkdown(1, test_str);

        Assert.Equal(expected, mention);
    }

    [Fact]
    public void Test_mention_markdown_2()
    {
        string expected = """[the\_name](tg://user?id=1)""";

        string mention = Tools.MentionMarkdown(1, @"the_name", parseMode: ParseMode.MarkdownV2);

        Assert.Equal(expected, mention);
    }
}
