using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Telegram.Bot.Extensions.Markup.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Markup;

public static partial class MarkupExtensions
{
    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message
    /// using <see cref="ParseMode.Markdown"/>.
    /// Use this if you want to retrieve the message text with the entities formatted as Markdown
    /// in the same way the original message was formatted.
    /// </summary>
    /// <remarks>
    /// <see cref="ParseMode.Markdown"/> is a legacy mode, retained by
    /// Telegram for backward compatibility.You should use <see cref="TextMarkdownV2"/> instead.
    /// </remarks>
    /// <param name="message"></param>
    /// <returns>Message text with entities formatted as Markdown.</returns>
    public static string? TextMarkdown(this Message message)
    {
        return ParseMarkdown(message.Text, message.ParseEntities(), urled: false);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message
    /// using <see cref="ParseMode.MarkdownV2"/>.
    /// Use this if you want to retrieve the message text with the entities formatted as Markdown
    /// in the same way the original message was formatted.
    /// </summary>
    /// <param name="message">Message text with entities formatted as Markdown.</param>
    /// <returns></returns>
    public static string? TextMarkdownV2(this Message message)
    {
        return ParseMarkdown(message.Text, message.ParseEntities(), urled: false, ParseMode.MarkdownV2);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message
    /// using <see cref="ParseMode.Markdown"/>.
    /// Use this if you want to retrieve the message text with the entities formatted as Markdown.
    /// This also formats <see cref="MessageEntity.Url"/> as a hyperlink.
    /// </summary>
    /// <remarks>
    /// <see cref="ParseMode.Markdown"/> is a legacy mode, retained by
    /// Telegram for backward compatibility. You should use <see cref="TextMarkdownV2Urled"/>
    /// instead.
    /// </remarks>
    /// <param name="message"></param>
    /// <returns>Message text with entities formatted as Markdown.</returns>
    public static string? TextMarkdownUrled(this Message message)
    {
        return ParseMarkdown(message.Text, message.ParseEntities(), urled: true);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message
    /// using <see cref="ParseMode.MarkdownV2"/>.
    /// Use this if you want to retrieve the message text with the entities formatted as Markdown.
    /// This also formats <see cref="MessageEntity.Url"/> as a hyperlink.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Message text with entities formatted as Markdown.</returns>
    public static string? TextMarkdownV2Urled(this Message message)
    {
        return ParseMarkdown(message.Text, message.ParseEntities(), urled: true, ParseMode.MarkdownV2);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message's
    /// caption using <see cref="ParseMode.Markdown"/>.
    /// Use this if you want to retrieve the message caption with the caption entities formatted as
    /// Markdown in the same way the original message was formatted.
    /// </summary>
    /// <remarks>
    /// <see cref="ParseMode.Markdown"/> is a legacy mode, retained by
    /// Telegram for backward compatibility. You should use <see cref="CaptionMarkdownV2(Message)"/>
    /// instead.
    /// </remarks>
    /// <param name="message"></param>
    /// <returns>Message caption with caption entities formatted as Markdown.</returns>
    public static string? CaptionMarkdown(this Message message)
    {
        return ParseMarkdown(message.Caption, message.ParseCaptionEntities(), urled: false);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message's
    /// caption using <see cref="ParseMode.MarkdownV2"/>.
    /// Use this if you want to retrieve the message caption with the caption entities formatted as
    /// Markdown in the same way the original message was formatted.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Message caption with caption entities formatted as Markdown.</returns>
    public static string? CaptionMarkdownV2(this Message message)
    {
        return ParseMarkdown(message.Caption, message.ParseCaptionEntities(), urled: false, ParseMode.MarkdownV2);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message's
    /// caption using <see cref="ParseMode.Markdown"/>.
    /// Use this if you want to retrieve the message caption with the caption entities formatted as
    /// Markdown. This also formats <see cref="MessageEntity.Url"/> as a hyperlink.
    /// </summary>
    /// <remarks>
    /// <see cref="ParseMode.Markdown"/> is a legacy mode, retained by
    /// Telegram for backward compatibility. You should use <see cref="CaptionMarkdownV2Urled(Message)"/>
    /// instead.
    /// </remarks>
    /// <param name="message"></param>
    /// <returns>Message caption with caption entities formatted as Markdown.</returns>
    public static string? CaptionMarkdownUrled(this Message message)
    {
        return ParseMarkdown(message.Caption, message.ParseCaptionEntities(), urled: true);
    }

    /// <summary>
    /// Creates a Markdown-formatted string from the markup entities found in the message's
    /// caption using <see cref="ParseMode.MarkdownV2"/>.
    /// Use this if you want to retrieve the message caption with the caption entities formatted as
    /// Markdown. This also formats <see cref="MessageEntity.Url"/> as a hyperlink.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Message caption with caption entities formatted as Markdown.</returns>
    public static string? CaptionMarkdownV2Urled(this Message message)
    {
        return ParseMarkdown(message.Caption, message.ParseCaptionEntities(), urled: true, ParseMode.MarkdownV2);
    }

    private static string? ParseMarkdown(
        string? messageText,
        ImmutableSortedDictionary<MessageEntity, string> entities,
        bool urled = false,
        ParseMode parseMode = ParseMode.Markdown,
        int offset = 0)
    {
        if (messageText is null)
            return null;

        StringBuilder markdownText = new();
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

            if (nestedEntities.Count > 0 && parseMode != ParseMode.MarkdownV2)
                throw new ArgumentException("Nested entities are not supported for Markdown version 1", nameof(parseMode));

            var escapedText = nestedEntities.Count > 0
                ? ParseMarkdown(
                    messageText: text,
                    entities: nestedEntities,
                    urled: urled,
                    parseMode: parseMode,
                    offset: entity.Offset)
                : Tools.EscapeMarkdown(text, parseMode);

            var insert = entity.Type switch
            {
                MessageEntityType.TextLink when parseMode is ParseMode.Markdown =>
                    $"""[{escapedText}]({entity.Url})""",
                MessageEntityType.TextLink =>
                    // Links need special escaping. Also can't have entities nested within
                    $"""[{escapedText}]({(Tools.EscapeMarkdown(
                                    entity.Url!, parseMode, entityType: MessageEntityType.TextLink))})""",
                MessageEntityType.TextMention when entity.User is { } =>
                    $"""[{escapedText}](tg://user?id={entity.User.Id.ToString(CultureInfo.InvariantCulture)})""",
                MessageEntityType.Url when urled && parseMode is ParseMode.Markdown =>
                    $"""[{text}]({text})""",
                MessageEntityType.Url when urled =>
                    $"""[{escapedText}]({text})""",
                MessageEntityType.Bold =>
                    $"""*{escapedText}*""",
                MessageEntityType.Italic =>
                    $"""_{escapedText}_""",
                MessageEntityType.Code =>
                    // Monospace needs special escaping. Also can't have entities nested within
                    $"""`{Tools.EscapeMarkdown(text, parseMode, MessageEntityType.Code)}`""",
                MessageEntityType.Pre =>
                    ToPreMarkup(text, parseMode, entity),
                MessageEntityType.Underline when parseMode is ParseMode.Markdown =>
                    throw new ArgumentException("Spoiler entities are not supported for Markdown version 1", nameof(parseMode)),
                MessageEntityType.Underline =>
                    $"""__{escapedText}__""",
                MessageEntityType.Strikethrough when parseMode is ParseMode.Markdown =>
                    throw new ArgumentException("Spoiler entities are not supported for Markdown version 1", nameof(parseMode)),
                MessageEntityType.Strikethrough =>
                    $"""~{escapedText}~""",
                MessageEntityType.Spoiler when parseMode is ParseMode.Markdown =>
                    throw new ArgumentException("Spoiler entities are not supported for Markdown version 1", nameof(parseMode)),
                MessageEntityType.Spoiler =>
                    $"""||{escapedText}||""",
                _ => escapedText
            };

            if (offset == 0)
            {
                markdownText.Append(Tools.EscapeMarkdown(messageText[lastOffset..(entity.Offset - offset)], parseMode));
            }
            else
            {
                markdownText.Append(messageText[lastOffset..(entity.Offset - offset)]);
            }
            markdownText.Append(insert);

            lastOffset = entity.Offset - offset + entity.Length;
        }

        if (offset == 0)
        {
            markdownText.Append(Tools.EscapeMarkdown(messageText[lastOffset..], parseMode));
        }
        else
        {
            markdownText.Append(messageText[lastOffset..]);
        }

        return markdownText.ToString();

        static bool IsNestedEntity(KeyValuePair<MessageEntity, string> e, MessageEntity entity)
        {
            (var me, _) = e;

            return (me.Offset >= entity.Offset)
                && (me.Offset + me.Length <= entity.Offset + entity.Length)
                && (MessageEntityComparer.Comparer.Compare(me, entity) != 0);
        }

        static string ToPreMarkup(string originalText, ParseMode parseMode, MessageEntity entity)
        {
            // Monospace needs special escaping. Also can't have entities nested within
            var code = Tools.EscapeMarkdown(
                text: originalText,
                parseMode: parseMode,
                entityType: MessageEntityType.Pre);

            var prefix = entity switch
            {
                _ when entity.Language is { } => $"```{entity.Language}\n",
                _ when code.StartsWith('\\') => "```",
                _ => "```\n",
            };

            return $"""{prefix}{code}```""";
        }
    }
}
