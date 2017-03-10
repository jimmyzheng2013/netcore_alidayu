# netcore_alidayu
由于阿里大鱼SDK目前没有提供.net core版本，该项目实现在.net core 下面，阿里大鱼发送信息

# Demo Send

AlidayuMessageSender messageSender = new AlidayuMessageSender(url, appkey, appSecret); 
messageSender.SmsType = "normal"; 
messageSender.SmsFreeSignName = template.SmsFreeSignName; 
messageSender.SmsParam = string.Format(template.SmsParam,smsParam);
messageSender.RecNum = sendmobile; 
messageSender.SmsTemplateCode = template.SmsTemplateCode; 
string result = messageSender.SendMessage();


