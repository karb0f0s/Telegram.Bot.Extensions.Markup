# Telegram Bot Markup Extensions

[![package](https://img.shields.io/nuget/vpre/Telegram.Bot.Extensions.Markup.svg?label=nuget&style=flat-square)](https://www.nuget.org/packages/Telegram.Bot.Extensions.Markup)
[![downloads](https://img.shields.io/nuget/dt/Telegram.Bot.Extensions.Markup.svg?style=flat-square&label=Package%20Downloads)](https://www.nuget.org/packages/Telegram.Bot.Extensions.Markup)
[![contributors](https://img.shields.io/github/contributors/karb0f0s/Telegram.Bot.Extensions.Markup.svg?style=flat-square&label=Contributors)](https://github.com/karb0f0s/Telegram.Bot.Extensions.Markup/graphs/contributors)
[![license](https://img.shields.io/github/license/karb0f0s/Telegram.Bot.Extensions.Markup.svg?style=flat-square&maxAge=2592000&label=License)](https://raw.githubusercontent.com/karb0f0s/Telegram.Bot.Extensions.Markup/master/LICENSE)
[![telegram chat](https://img.shields.io/badge/Support_Chat-Telegram-blue.svg?style=flat-square)](https://t.me/joinchat/B35YY0QbLfd034CFnvCtCA)

## Introduction

This library provides some convenience methods to work with Telegram Bot API markup.

Under `Telegram.Bot.Extensions.Markup` namespace following static methods are available:

- `Tools.EscapeMarkdown` - helper method to escape telegram markup symbols.
- `Tools.MentionHtml` - helper method to create a user mention as HTML tag.
- `Tools.MentionMarkdown` - helper method to create a user mention in Markdown syntax.
- `Tools.CreateDeepLinkedUrl` - helper method to create a deep-linked URL.

Under `Telegram.Bot.Extensions.Markup.Helpers` namespace following static methods are available:

- `MessageEntityHelpers.ParseEntities` - maps `Message.Entities` to `Message.Text`.
- `MessageEntityHelpers.ParseCaptionEntities` - maps `Message.CaptionEntities` to `Message.Caption`.

Under `Telegram.Bot.Extensions.Markup` namespace following `Message` extension methods are available:

- `TextHtml` - creates an HTML-formatted string from the markup entities found in the message.
- `TextHtmlUrled` - creates an HTML-formatted string from the markup entities found in the message.
- `CaptionHtml` - creates an HTML-formatted string from the markup entities found in the message's caption.
- `CaptionHtmlUrled` - creates an HTML-formatted string from the markup entities found in the message's caption.
- `TextMarkdown` - creates a Markdown-formatted string from the markup entities found in the message using `ParseMode.Markdown`.
- `TextMarkdownV2` - creates a Markdown-formatted string from the markup entities found in the message using `ParseMode.MarkdownV2`.
- `TextMarkdownUrled` - creates a Markdown-formatted string from the markup entities found in the message
using `ParseMode.Markdown`.
- `TextMarkdownV2Urled` - creates a Markdown-formatted string from the markup entities found in the message
using `ParseMode.MarkdownV2`.
- `CaptionMarkdown` - creates an Creates a Markdown-formatted string from the markup entities found in the message's
caption using `ParseMode.Markdown`.
- `CaptionMarkdownV2` - creates an Creates a Markdown-formatted string from the markup entities found in the message's
caption using `ParseMode.MarkdownV2`.
- `CaptionMarkdownUrled` - creates a Markdown-formatted string from the markup entities found in the message's caption using `ParseMode.Markdown`.
- `CaptionMarkdownV2Urled` - creates a Markdown-formatted string from the markup entities found in the message's caption using `ParseMode.MarkdownV2`.

## Installation

## Credits

This library is a .NET implementation of [python-telegram-bot](https://github.com/python-telegram-bot/python-telegram-bot).

Library use [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) for Telegram Bot API specific implementations.

## License

You may copy, distribute and modify the software provided that modifications are described and licensed for free under LGPL-3. Derivatives works (including modifications or anything statically linked to the library) can only be redistributed under LGPL-3, but applications that use the library don't have to be.
