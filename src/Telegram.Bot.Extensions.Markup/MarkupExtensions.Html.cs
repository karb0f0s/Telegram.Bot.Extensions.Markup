using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Web;
using Telegram.Bot.Extensions.Markup.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Markup;

public static partial class MarkupExtensions
{
    /// <summary>
    /// Creates an HTML-formatted string from the markup entities found in the message.
    /// Use this if you want to retrieve the message text with the entities formatted as HTML in
    /// the same way the original message was formatted.
    /// </summary>
    /// <returns>Message text with entities formatted as HTML.</returns>
    public static string? TextHtml(this Message message)
    {
        return ParseHtml(message.Text, message.ParseEntities(), urled: false);
    }

    /// <summary>
    /// Creates an HTML-formatted string from the markup entities found in the message.
    /// Use this if you want to retrieve the message text with the entities formatted as HTML.
    /// This also formats <see cref="MessageEntity.Url"/> as a hyperlink.
    /// </summary>
    /// <returns>Message text with entities formatted as HTML.</returns>
    public static string? TextHtmlUrled(this Message message)
    {
        return ParseHtml(message.Text, message.ParseEntities(), urled: true);
    }

    /// <summary>
    /// Creates an HTML-formatted string from the markup entities found in the message's
    /// caption.
    /// Use this if you want to retrieve the message caption with the caption entities formatted as
    /// HTML in the same way the original message was formatted.
    /// </summary>
    /// <returns>Message caption with caption entities formatted as HTML.</returns>
    public static string? CaptionHtml(this Message message)
    {
        return ParseHtml(message.Caption, message.ParseCaptionEntities(), urled: false);
    }

    /// <summary>
    /// Creates an HTML-formatted string from the markup entities found in the message's caption.
    /// Use this if you want to retrieve the message caption with the caption entities formatted as
    /// HTML.This also formats <see cref="MessageEntity.Url"/> as a hyperlink.
    /// </summary>
    /// <returns>Message caption with caption entities formatted as HTML.</returns>
    public static string? CaptionHtmlUrled(this Message message)
    {
        return ParseHtml(message.Caption, message.ParseCaptionEntities(), urled: true);
    }

    private static string? ParseHtml(
        string? messageText,
        ImmutableSortedDictionary<MessageEntity, string> entities,
        bool urled = false,
        int offset = 0)
    {
        if (messageText is null)
            return null;

        StringBuilder htmlText = new();
        var lastOffset = 0;

        List<MessageEntity> parsedEntities = new(entities.Count);

        foreach ((var entity, var text) in entities)
        {
            if (parsedEntities.Contains(entity))
                continue;

            var nestedEntities =
                entities
                    .Where(e => IsNestedEntity(e, entity))
                    .ToImmutableSortedDictionary(MessageEntityComparer.Comparer);

            parsedEntities.AddRange(nestedEntities.Keys);

            var escapedText = nestedEntities.Count > 0
                ? ParseHtml(
                    messageText: text,
                    entities: nestedEntities,
                    urled: urled,
                    offset: entity.Offset)
                : HttpUtility.HtmlEncode(text);

            var insert = entity.Type switch
            {
                MessageEntityType.TextLink =>
                    $"""<a href="{entity.Url}">{escapedText}</a>""",
                MessageEntityType.TextMention when entity.User is { } =>
                    $"""<a href="tg://user?id={entity.User.Id.ToString(CultureInfo.InvariantCulture)}">{escapedText}</a>""",
                MessageEntityType.Url when urled =>
                    $"""<a href="{escapedText}">{escapedText}</a>""",
                MessageEntityType.Bold =>
                        $"""<b>{escapedText}</b>""",
                MessageEntityType.Italic =>
                    $"""<i>{escapedText}</i>""",
                MessageEntityType.Code =>
                    $"""<code>{escapedText}</code>""",
                MessageEntityType.Pre when entity.Language is { } =>
                    $"""<pre><code class="{entity.Language}">{escapedText}</code></pre>""",
                MessageEntityType.Pre =>
                    $"""<pre>{escapedText}</pre>""",
                MessageEntityType.Underline =>
                        $"""<u>{escapedText}</u>""",
                MessageEntityType.Strikethrough =>
                        $"""<s>{escapedText}</s>""",
                MessageEntityType.Spoiler =>
                        $"""<span class="tg-spoiler">{escapedText}</span>""",
                _ => escapedText
            };

            if (offset == 0)
            {
                htmlText.Append(HttpUtility.HtmlEncode(messageText[lastOffset..(entity.Offset - offset)]));
            }
            else
            {
                htmlText.Append(messageText[lastOffset..(entity.Offset - offset)]);
            }
            htmlText.Append(insert);

            lastOffset = entity.Offset - offset + entity.Length;
        }

        if (offset == 0)
        {
            htmlText.Append(HttpUtility.HtmlEncode(messageText[lastOffset..]));
        }
        else
        {
            htmlText.Append(messageText[lastOffset..]);
        }

        return htmlText.ToString();

        static bool IsNestedEntity(KeyValuePair<MessageEntity, string> e, MessageEntity entity)
        {
            (var me, _) = e;

            return (me.Offset >= entity.Offset)
                && (me.Offset + me.Length <= entity.Offset + entity.Length)
                && (MessageEntityComparer.Comparer.Compare(me, entity) != 0);
        }
    }
}
