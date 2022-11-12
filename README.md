# TGbot_Bar-QR_Codes

## About

This is a small telegram bot that can generate barcodes and QR codes. It was made "on the knee" for personal use, but I decided to pour it here.

## Dependencies

* NuGet:
  + Telegram.Bot - 18.0.0
  + BarCode - 2022.10.9871

## Bugs

Since this bot was needed for personal use, it is not designed for high workload. Therefore, if he receives a lot of requests, he will not always return the correct answer. You can fix it yourself, just replace the *string* with the code in the *"codes"* class with a *Dictionary* in which you will remember the string and the chatId. I plan to do this, but I have a lot of other things to do.

### Deployments

Since this bot uses various libraries, don't forget to take care of all licenses when you deploy it outside VS.

## GIF how it works

[![BotGIF](https://s4.gifyu.com/images/ezgif.com-gif-makerb3f4ef92ed875622.gif)](https://gifyu.com/image/SEr5h)
