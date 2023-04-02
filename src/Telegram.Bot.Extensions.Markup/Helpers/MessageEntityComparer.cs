using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Markup.Helpers;

internal sealed class MessageEntityComparer : Comparer<MessageEntity>
{
    public static MessageEntityComparer Comparer => new();

    public override int Compare(MessageEntity? x, MessageEntity? y)
    {
        int comparison;

        comparison = x!.Offset.CompareTo(y!.Offset);
        if (comparison != 0)
            return comparison;

        comparison = x!.Length.CompareTo(y!.Length);
        if (comparison != 0)
            return comparison;

        comparison = x!.Type.CompareTo(y!.Type);
        return comparison;
        /*
           (me.Offset >= entity.Offset)
        && (me.Offset + me.Length <= entity.Offset + entity.Length)
        && (MessageEntityComparer.Comparer.Compare(me, entity) != 0);
        */
    }
}
