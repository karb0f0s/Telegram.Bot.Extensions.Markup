using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Web;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Markup;

/// <summary>
/// Helpful markup functions
/// </summary>
public static class Tools
{
    /// <summary>
    /// Helper function to escape telegram markup symbols.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="parseMode">
    /// Use to specify the version of telegrams Markdown.
    /// Either <see cref="ParseMode.Markdown"/> or <see cref="ParseMode.MarkdownV2"/>.
    /// Defaults to <see cref="ParseMode.Markdown"/>
    /// </param>
    /// <param name="entityType">
    /// For the entity types <see cref="MessageEntityType.Pre"/>, <see cref="MessageEntityType.Code"/> and
    /// the link part of <see cref="MessageEntityType.TextLink"/>, only certain characters
    /// need to be escaped in <see cref="ParseMode.MarkdownV2"/>.
    /// See the official API documentation for details. Only valid in combination with
    /// <paramref name="parseMode"/>: <see cref="ParseMode.MarkdownV2"/>, will be ignored else.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EscapeMarkdown(
        string text,
        ParseMode parseMode = ParseMode.Markdown,
        MessageEntityType? entityType = default)
    {
        var escaped = (parseMode, entityType) switch
        {
            (ParseMode.Markdown, _)                            => Escaped[(ParseMode.Markdown, null)],
            (ParseMode.MarkdownV2, MessageEntityType.Pre)      => Escaped[(ParseMode.MarkdownV2, MessageEntityType.Pre)],
            (ParseMode.MarkdownV2, MessageEntityType.Code)     => Escaped[(ParseMode.MarkdownV2, MessageEntityType.Code)],
            (ParseMode.MarkdownV2, MessageEntityType.TextLink) => Escaped[(ParseMode.MarkdownV2, MessageEntityType.TextLink)],
            (ParseMode.MarkdownV2, _)                          => Escaped[(ParseMode.MarkdownV2, null)],
            _ => throw new ArgumentException("Only ParseMode.Markdown and ParseMode.MarkdownV2 allowed.", nameof(parseMode)),
        };

        return Regex.Replace(
            input: text,
            pattern: $"""([{escaped}])""",
            replacement: """\$1""",
            RegexOptions.CultureInvariant,
            matchTimeout: TimeSpan.FromSeconds(1));
    }

    private static ImmutableDictionary<(ParseMode parseMode, MessageEntityType? entityType), string> Escaped => new Dictionary<(ParseMode parseMode, MessageEntityType? entityType), string>()
    {
        { (ParseMode.Markdown, null), Regex.Escape("""_*`[""") },
        { (ParseMode.MarkdownV2, MessageEntityType.Pre), Regex.Escape("""\`""") },
        { (ParseMode.MarkdownV2, MessageEntityType.Code), Regex.Escape("""\`""") },
        { (ParseMode.MarkdownV2, MessageEntityType.TextLink), Regex.Escape("""\)""") },
        {
            (ParseMode.MarkdownV2, null),
            Regex.Escape(str: """\_*()~`>#+-=|{}.![]""")
                .Replace("]", "\\]", StringComparison.Ordinal)
                .Replace("-", "\\-", StringComparison.Ordinal)
        },
    }.ToImmutableDictionary();

    /// <summary>
    /// Helper function to create a user mention as HTML tag.
    /// </summary>
    /// <param name="userId">The user's id which you want to mention.</param>
    /// <param name="name">The name the mention is showing.</param>
    /// <returns>The inline mention for the user as HTML.</returns>
    public static string MentionHtml(ChatId userId, string name)
    {
        return $"""<a href="tg://user?id={userId}">{HttpUtility.HtmlEncode(name)}</a>""";
    }

    /// <summary>
    /// Helper function to create a user mention in Markdown syntax.
    /// </summary>
    /// <param name="userId">The user's id which you want to mention.</param>
    /// <param name="name">The name the mention is showing.</param>
    /// <param name="parseMode">Use to specify the version of Telegram's Markdown.</param>
    /// <returns>The inline mention for the user as Markdown.</returns>
    public static string MentionMarkdown(ChatId userId, string name, ParseMode parseMode = ParseMode.Markdown)
    {
        var tgLink = $"""tg://user?id={userId}""";
        if (parseMode == ParseMode.Markdown)
        {
            return $"""[{name}]({tgLink})""";
        }

        return $"""[{EscapeMarkdown(name, parseMode)}]({tgLink})""";
    }

    /// <summary>
    /// Creates a deep-linked URL for this <paramref name="botUsername"/> with the
    /// specified <paramref name="payload"/>. See
    /// <see href="https://core.telegram.org/bots/features#deep-linking"/> to learn more.
    /// The <paramref name="payload"/> may consist of the following characters:
    /// <c>`A-Z, a-z, 0-9, _, -`</c>
    /// </summary>
    /// <param name="botUsername">The username to link to.</param>
    /// <param name="payload">Parameters to encode in the created URL.</param>
    /// <param name="group">
    /// If <see langword="true"/> the user is prompted to select a group to
    /// add the bot to. If <see langword="false"/>, opens a one-on-one conversation with the bot.
    /// Defaults to <see langword="false"/>.
    /// </param>
    /// <returns>An URL to start the bot with specific parameters.</returns>
    public static string CreateDeepLinkedUrl(
        string botUsername,
        string? payload = default,
        bool group = false)
    {
        if (botUsername is not { Length: > 3 })
            throw new ArgumentException("You must provide a valid botUsername.", nameof(botUsername));

        var baseUrl = $"""https://t.me/{botUsername}""";
        if (payload is not { Length: > 0 })
            return baseUrl;

        if (payload is { Length: > 64 })
            throw new ArgumentException("The deep-linking payload must not exceed 64 characters.", nameof(payload));

        if (!ValidPayload(payload))
            throw new ArgumentException(
                "Only the following characters are allowed for deep-linked URLs: A-Z, a-z, 0-9, _ and -",
                nameof(payload));

        var key = group ? "startgroup" : "start";

        return $"""{baseUrl}?{key}={payload}""";

        static bool ValidPayload(string payload)
        {
            return Regex.IsMatch(
                input: payload,
                pattern: """^[A-Za-z0-9_-]+$""",
                options: RegexOptions.CultureInvariant,
                matchTimeout: TimeSpan.FromSeconds(1));
        }
    }
}
