using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using IronBarCode;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using TGbot_BarQr_Codes;


var botClient = new TelegramBotClient("YOUR_TOKEN");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    if ((message.Text != "/Bar") && (message.Text != "/QR"))
    {
        codes.code = message.Text;
        Console.WriteLine($"Now codes is {codes.code}");
    }

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "/Bar", "/QR" },
    })
    {
        ResizeKeyboard = true
    };

    InlineKeyboardMarkup inlineKeyboard = new(new[]
    {
        InlineKeyboardButton.WithUrl(
            text: "Link to the Repository",
            url: "https://github.com/zabulonov/TGbot_Bar-QR_Codes"
        )
    }
);
    if (message.Text == "/start")
    {
        Message messageStart = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Hi! I am a bot written in c#. I can generate barcodes and QR codes from your text. You can also view the source code :):" + codes.code,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
    }

    if (message.Text == "/Bar")
    {
        Message messageBarGen = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Generating a barcode from: " + codes.code,
            cancellationToken: cancellationToken);
        try
        {
            var MyBarCode = IronBarCode.BarcodeWriter.CreateBarcode(codes.code, BarcodeEncoding.Code128);
            MyBarCode.SaveAsImage(chatId.ToString());

            await using Stream stream = System.IO.File.OpenRead(chatId.ToString());
            Message messageSendBar = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: stream,
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        catch
        {
            Console.WriteLine($"Error in chat {chatId}");
            Message messageBarError = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Oops... Something went wrong and I couldn't generate it, try again :) Write in English and without spaces",
                cancellationToken: cancellationToken);
        }
        return;
    }
    if (message.Text == "/QR")
    {
        Message messageQrGen = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Generating a QR code from: " + codes.code,
            cancellationToken: cancellationToken);
        try
        {
            QRCodeWriter.CreateQrCode(codes.code, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsImage(chatId.ToString());

            await using Stream stream = System.IO.File.OpenRead(chatId.ToString());
            Message messageQrSend = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: stream,
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);

            FileInfo fileDel = new FileInfo(chatId.ToString());
            fileDel.Delete();
        }
        catch
        {
            Console.WriteLine($"Error in chat {chatId}");
            Message messageBarError = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Oops... Something went wrong and I couldn't generate it, try again :)",
                cancellationToken: cancellationToken);
        }
        return;
    }
    Message sentMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Enter the string from which you want to generate the code, then select the type of the desired code on the keyboard:",
    replyMarkup: replyKeyboardMarkup,
    cancellationToken: cancellationToken);

}



Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

