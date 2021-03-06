﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iQQ.Net.WebQQCore.Im.Core;
using iQQ.Net.WebQQCore.Im.Event;
using iQQ.Net.WebQQCore.Im.Http;

namespace iQQ.Net.WebQQCore.Im.Action
{
    public class CheckQRCodeAction : AbstractHttpAction
    {
        public CheckQRCodeAction(QQContext context, QQActionEventHandler listener) : base(context, listener) { }


        public override QQHttpRequest OnBuildRequest()
        {
            QQHttpRequest req = CreateHttpRequest("GET", QQConstants.URL_CHECK_QRCODE);
            req.AddGetValue("webqq_type", "10");
            req.AddGetValue("remember_uin", "1");
            req.AddGetValue("login2qq", "1");
            req.AddGetValue("aid", "501004106");
            req.AddGetValue("u1", "http://w.qq.com/proxy.html?login2qq=1&webqq_type=10");
            req.AddGetValue("ptredirect", "0");
            req.AddGetValue("ptlang", "2052");
            req.AddGetValue("daid", "164");
            req.AddGetValue("from_ui", "1");
            req.AddGetValue("pttype", "1");
            req.AddGetValue("dumy", "");
            req.AddGetValue("fp", "loginerroralert");
            req.AddGetValue("action", "0-0-4434");
            req.AddGetValue("mibao_css", "m_webqq");
            req.AddGetValue("t", "1");
            req.AddGetValue("g", "1");
            req.AddGetValue("js_type", "0");
            req.AddGetValue("js_ver", "10135");
            req.AddGetValue("login_sig", "");
            req.AddGetValue("pt_randsalt", "0");
            return req;
        }

        public override void OnHttpStatusOK(QQHttpResponse response)
        {
            var rex = new Regex(QQConstants.REGXP_LOGIN);
            var m = rex.Match(response.GetResponseString());
            if (m.Success)
            {
                var ret = m.Groups[1].Value;
                switch (ret)
                {
                    case "0": NotifyActionEvent(QQActionEventType.EVT_OK, m.Groups[3].Value); break;
                    case "66": throw new QQException(QQErrorCode.QRCODE_OK, m.Groups[5].Value);
                    case "67": throw new QQException(QQErrorCode.QRCODE_AUTH, m.Groups[5].Value);
                    default: throw new QQException(QQErrorCode.INVALID_USER, m.Groups[5].Value);
                }
            }
            else
            {
                throw new QQException(QQErrorCode.UNEXPECTED_RESPONSE);
            }
        }

    }

}
