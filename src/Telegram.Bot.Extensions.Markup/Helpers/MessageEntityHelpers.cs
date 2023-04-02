using System.Collections.Immutable;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Markup.Helpers;

/// <summary>
/// Extension methods to convert <see cref="Message.Text"/> and <see cref="Message.Caption"/> to string markup.
/// </summary>
public static class MessageEntityHelpers
{
    private static IReadOnlyCollection<MessageEntityType> AllMessageEntityTypes =>
        Enum.GetValues(typeof(MessageEntityType))
            .Cast<MessageEntityType>()
            .ToArray();

    private static ImmutableSortedDictionary<MessageEntity, string> EmptyDictionary =>
        new Dictionary<MessageEntity, string>()
            .ToImmutableSortedDictionary();

    /// <summary>
    /// It contains entities from this message filtered by their
    /// <see cref="MessageEntityType"/> attribute as the key, and the text that each entity
    /// belongs to as the value of the <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
    /// See <see cref="ParseEntity"/> for more info.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="types">
    /// List of <see cref="MessageEntity"/> types as strings. If the <see cref="MessageEntity.Type"/>
    /// attribute of an entity is contained in this list, it will be returned.
    /// Defaults to a list of all types. All types can be found as constants
    /// in <see cref="MessageEntityType"/>.
    /// </param>
    /// <returns>
    /// Returns a <see cref="IReadOnlyDictionary{TKey, TValue}"/> that maps  <see cref="MessageEntity"/> to <see cref="string"/>.
    /// A dictionary of entities mapped to the text that belongs to them, calculated based on UTF-16 codepoints.
    /// </returns>
    public static ImmutableSortedDictionary<MessageEntity, string> ParseEntities(
        this Message message,
        IReadOnlyCollection<MessageEntityType>? types = default)
    {
        types ??= AllMessageEntityTypes;

        return message.Entities?
            .Where(e => types.Contains(e.Type))
            .ToImmutableSortedDictionary(
                e => e,
                e => message.ParseEntity(e),
                MessageEntityComparer.Comparer)
            ?? EmptyDictionary;
    }

    /// <summary>
    /// Returns the text from a given <see cref="MessageEntityType"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="entity">
    /// The entity to extract the text from.It must be an entity that belongs to this message
    /// </param>
    /// <returns>The text of the given entity</returns>
    public static string ParseEntity(
        this Message message,
        MessageEntity entity)
    {
        ArgumentNullException.ThrowIfNull(message.Text);

        return message.Text[entity.Offset..(entity.Offset + entity.Length)];
    }

    /// <summary>
    /// It contains entities from this message's caption filtered by their
    /// <see cref="MessageEntityType"/> attribute as the key, and the text that each entity
    /// belongs to as the value of the <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
    /// See <see cref="ParseEntity"/> for more info.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="types">
    /// List of <see cref="MessageEntity"/> types as strings. If the <see cref="MessageEntity.Type"/>
    /// attribute of an entity is contained in this list, it will be returned.
    /// Defaults to a list of all types. All types can be found as constants
    /// in <see cref="MessageEntityType"/>.
    /// </param>
    /// <returns>
    /// Returns a <see cref="IReadOnlyDictionary{TKey, TValue}"/> that maps  <see cref="MessageEntity"/> to <see cref="string"/>.
    /// A dictionary of entities mapped to the text that belongs to them, calculated based on UTF-16 codepoints.
    /// </returns>
    public static ImmutableSortedDictionary<MessageEntity, string> ParseCaptionEntities(
        this Message message,
        IReadOnlyCollection<MessageEntityType>? types = default)
    {
        types ??= AllMessageEntityTypes;

        return message.CaptionEntities?
            .Where(e => types.Contains(e.Type))
            .ToImmutableSortedDictionary(
                e => e,
                e => message.ParseCaptionEntity(e),
                MessageEntityComparer.Comparer)
            ?? EmptyDictionary;
    }

    /// <summary>
    /// Returns the text from a given <see cref="MessageEntityType"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="entity">
    /// The entity to extract the text from. It must be an entity that belongs to this message.
    /// </param>
    /// <returns></returns>
    public static string ParseCaptionEntity(
        this Message message,
        MessageEntity entity)
    {
        ArgumentNullException.ThrowIfNull(message.Caption);

        return message.Caption[entity.Offset..(entity.Offset + entity.Length)];
    }
}
