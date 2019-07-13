using System.Threading.Tasks;
using WeihanLi.Common.Event;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class RedisEventBusTest : BaseUnitTest
    {
        public RedisEventBusTest()
        {
        }

        [Fact]
        public void MainTest()
        {
        }

        private class CounterEvent : EventBase
        {
            public int Counter { get; set; }
        }

        private class CounterEventBaseHandler : IEventHandler<CounterEvent>
        {
            public Task Handle(CounterEvent @event)
            {
                throw new System.NotImplementedException();
            }
        }

        private class CounterEventBaseHandler2 : IEventHandler<CounterEvent>
        {
            public Task Handle(CounterEvent @event)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
