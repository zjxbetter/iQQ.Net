﻿using iQQ.Net.WebQQCore.Im.Action;
using iQQ.Net.WebQQCore.Im.Bean;
using iQQ.Net.WebQQCore.Im.Event;

namespace iQQ.Net.WebQQCore.Im.Module
{
    /// <summary>
    /// <para>讨论组模块</para>
    /// <para>@author solosky</para>
    /// </summary>
    public class DiscuzModule : AbstractModule
    {
        public QQActionFuture GetDiscuzList(QQActionEventHandler listener)
        {
            return PushHttpAction(new GetDiscuzListAction(this.Context, listener));
        }

        public QQActionFuture GetDiscuzInfo(QQDiscuz discuz, QQActionEventHandler listener)
        {
            return PushHttpAction(new GetDiscuzInfoAction(this.Context, listener, discuz));
        }
    }

}
