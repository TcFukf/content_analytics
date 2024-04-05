using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;
using TelegramWrapper.Wrapper;
using TelegramWrapper.Wrapper.Bl.Handlers;

namespace social_analytics
{
    internal class UnexMessages : IUnexpectedMessagaHandler
    {
        public IClientWrapper tg { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<bool> Handle(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
