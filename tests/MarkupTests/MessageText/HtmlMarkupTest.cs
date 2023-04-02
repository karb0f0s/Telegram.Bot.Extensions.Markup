using MarkupTests.Fixture;
using Telegram.Bot.Types;

namespace MarkupTest.Markdown;

public class HtmlMarkupTest : IClassFixture<MarkupTestFixture>
{
    private readonly MarkupTestFixture _fixture;

    public HtmlMarkupTest(MarkupTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_text_html_simple()
    {
        string test_html_string = """
            <u>Test</u> for &lt;<b>bold</b>, <i>ita_lic</i>,
             <code>\`code</code>,
             <a href="http://github.com/abc\)def">links</a>,
             <a href="tg://user?id=123456789">text-mention</a> and
             <pre>`\pre</pre>. http://google.com
             and <i>bold <b>nested in <s>strk&gt;trgh</s> nested in</b> italic</i>.
             <pre><code class="python">Python pre</code></pre>.
             <span class="tg-spoiler">Spoiled</span>.
            """.ReplaceLineEndings("");

        string? text_html = _fixture.TestMessageV2.TextHtml();

        Assert.Equal(test_html_string, text_html);
    }

    [Fact]
    public void Test_text_html_empty()
    {
        Message message = new()
        {
            Text = null,
            Caption = "test",
        };

        Assert.Null(message.TextHtml());
    }

    [Fact]
    public void Test_text_html_urled()
    {
        string test_html_string = """
            <u>Test</u> for &lt;<b>bold</b>, <i>ita_lic</i>,
             <code>\`code</code>,
             <a href="http://github.com/abc\)def">links</a>,
             <a href="tg://user?id=123456789">text-mention</a> and
             <pre>`\pre</pre>. <a href="http://google.com">http://google.com</a>
             and <i>bold <b>nested in <s>strk&gt;trgh</s> nested in</b> italic</i>.
             <pre><code class="python">Python pre</code></pre>.
             <span class="tg-spoiler">Spoiled</span>.
            """.ReplaceLineEndings("");

        string? text_html = _fixture.TestMessageV2.TextHtmlUrled();

        Assert.Equal(test_html_string, text_html);
    }
}
