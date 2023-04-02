using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MarkupTests.Fixture;

public class MarkupTestFixture
{
    public static readonly MessageEntity[] TestEntities = new MessageEntity[]
    {
        new () { Length = 4, Offset = 10, Type = MessageEntityType.Bold },
        new () { Length = 3, Offset = 16, Type = MessageEntityType.Italic },
        new () { Length = 3, Offset = 20, Type = MessageEntityType.Italic },
        new () { Length = 4, Offset = 25, Type = MessageEntityType.Code },
        new () { Length = 5, Offset = 31, Type = MessageEntityType.TextLink, Url = """http://github.com/ab_""" },
        new () {
            Length = 12,
            Offset = 38,
            Type = MessageEntityType.TextMention,
            User = new() { Id = 123456789, Username = "mentioned user", IsBot = false },
        },
        new () { Length = 3, Offset = 55, Type = MessageEntityType.Pre, Language = "python" },
        new () { Length = 21, Offset = 60, Type = MessageEntityType.Url },
    };

    public static readonly string TestText = """
        Test for <bold, ita_lic, code, links, text-mention and pre. http://google.com/ab_
        """.ReplaceLineEndings("");

    public static readonly MessageEntity[] TestEntitiesV2 = new MessageEntity[]
    {
        new () { Length = 4, Offset = 0, Type = MessageEntityType.Underline },
        new () { Length = 4, Offset = 10, Type = MessageEntityType.Bold },
        new () { Length = 7, Offset = 16, Type = MessageEntityType.Italic },
        new () { Length = 6, Offset = 25, Type = MessageEntityType.Code },
        new () { Length = 5, Offset = 33, Type = MessageEntityType.TextLink, Url = """http://github.com/abc\)def""" },
        new () {
            Length = 12,
            Offset = 40,
            Type = MessageEntityType.TextMention,
            User = new User() { Id = 123456789, Username = "mentioned user", IsBot = false },
        },
        new () { Length = 5, Offset = 57, Type = MessageEntityType.Pre },
        new () { Length = 17, Offset = 64, Type = MessageEntityType.Url },
        new () { Length = 41, Offset = 86, Type = MessageEntityType.Italic },
        new () { Length = 29, Offset = 91, Type = MessageEntityType.Bold },
        new () { Length = 9, Offset = 101, Type = MessageEntityType.Strikethrough },
        new () { Length = 10, Offset = 129, Type = MessageEntityType.Pre, Language = "python" },
        new () { Length = 7, Offset = 141, Type = MessageEntityType.Spoiler },
    };

    public static readonly string TestTextV2 = """
        Test for <bold, ita_lic, \`code, links, text-mention and `\pre.
         http://google.com and bold nested in strk>trgh nested in italic. Python pre. Spoiled.
        """.ReplaceLineEndings("");

    public Message TestMessage => new()
    {
        MessageId = 1,
        From = default,
        Date = default,
        Chat = default!,
        Text = TestText,
        Entities = TestEntities,
        Caption = TestText,
        CaptionEntities = TestEntities,
    };

    public Message TestMessageV2 => new()
    {
        MessageId = 1,
        From = default,
        Date = default,
        Chat = default!,
        Text = TestTextV2,
        Entities = TestEntitiesV2,
        Caption = TestTextV2,
        CaptionEntities = TestEntitiesV2,
    };
}
