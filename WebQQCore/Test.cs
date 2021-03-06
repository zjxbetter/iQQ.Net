﻿using System;
using System.Collections.Generic;
using iQQ.Net.WebQQCore.Im;
using iQQ.Net.WebQQCore.Im.Actor;
using iQQ.Net.WebQQCore.Im.Bean;
using iQQ.Net.WebQQCore.Im.Event;

namespace iQQ.Net.WebQQCore
{
    class Test
    {
        private readonly static QQNotifyHandler handler = (sender, Event) =>
        {
            var client = sender as IQQClient;
            if (client == null) return;

            switch (Event.Type)
            {
                case QQNotifyEventType.CHAT_MSG:
                {
                    var msg = (QQMsg)Event.Target;
                    try
                    {
                        Console.WriteLine("{0}-好友{1}消息：{2}", client.Account.QQ, msg.From.QQ, msg.GetText());
                    }
                    catch (QQException e)
                    {
                        Console.WriteLine(client.Account.QQ + e.StackTrace);
                    }
                    break;
                }
                case QQNotifyEventType.KICK_OFFLINE:
                {
                    Console.WriteLine(client.Account.QQ + "-被踢下线: " + (string)Event.Target);
                    break;
                }
                case QQNotifyEventType.CAPACHA_VERIFY:
                {
                    try
                    {
                        var verify = (QQNotifyEventArgs.ImageVerify)Event.Target;
                        verify.Image.Save("verify.png", System.Drawing.Imaging.ImageFormat.Png);
                        Console.WriteLine(verify.Reason);
                        Console.Write(client.Account.Username + "-请输入verify.png里面的验证码：");
                        var code = Console.ReadLine();
                        client.SubmitVerify(code, Event);
                    }
                    catch (Exception e)
                    {
                        // TODO Auto-generated catch block
                        Console.WriteLine(client.Account.QQ + e.StackTrace);
                    }
                    break;
                }

                case QQNotifyEventType.UNKNOWN_ERROR:
                case QQNotifyEventType.NET_ERROR:
                Console.WriteLine(client.Account.QQ + "-出错：" + Event.Target.ToString());
                break;

                default:
                Console.WriteLine(client.Account.QQ + "-" + Event.Type + ", " + Event.Target);
                break;

            }

        };



        public static void Main(string[] args)
        {
            var threadActorDispatcher = new ThreadActorDispatcher();
            IQQClient client = null;

            start:
            Console.Write("请输入QQ号：");
            var username = Console.ReadLine();
            Console.Write("请输入QQ密码：");
            var password = Console.ReadLine();
            client = new WebQQClient(username, password, handler, threadActorDispatcher);


            //测试同步模式登录
            var future = client.Login(QQStatus.ONLINE, null);
            Console.WriteLine(client.Account.Username + "-登录中......");

            var Event = future.WaitFinalEvent();
            if (Event.Type == QQActionEventType.EVT_OK)
            {
                Console.WriteLine(client.Account.Username + "-登录成功！！！！");

                var getUserInfoEvent = client.GetUserInfo(client.Account, null).WaitFinalEvent();

                if (getUserInfoEvent.Type == QQActionEventType.EVT_OK)
                {
                    Console.WriteLine(client.Account.QQ + "-用户信息:" + getUserInfoEvent.Target);


                    client.GetBuddyList(null).WaitFinalEvent();
                    Console.WriteLine(client.Account.QQ + "-Buddy count: " + client.GetBuddyList().Count);

                    foreach (var buddy in client.GetBuddyList())
                    {
                        var f = client.GetUserQQ(buddy, null);
                        var e = f.WaitFinalEvent();
                        var name = string.IsNullOrEmpty(buddy.MarkName) ? buddy.Nickname : buddy.MarkName;
                        Console.WriteLine("{0}, {1}", buddy.QQ, name);
                    }
                }
                //所有的逻辑完了后，启动消息轮询
                client.BeginPollMsg();
            }
            else if (Event.Type == QQActionEventType.EVT_ERROR)
            {
                var ex = (QQException)Event.Target;
                Console.WriteLine(ex.Message);
                client.Destroy();
                goto start;
            }
            else
            {
                Console.WriteLine(client.Account.Username + "-登录失败");
                client.Destroy();
                goto start;
            }

            Console.WriteLine("按任意键退出");
            Console.ReadLine();
        }
    }
}
