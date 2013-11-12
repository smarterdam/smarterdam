using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ninject;
using Smarterdam.Client;
using Smarterdam.Tests.Mocks;

namespace Smarterdam.Tests
{
    [TestFixture]
    public class NeuralNetworkPipelineTest
    {
        [Test]
        public void Test1()
        {
            Assert.AreEqual(1, 1);

            SmarterdamFactory.Init(kernel =>
                {
                    kernel.Rebind<IMessageQueue>().To<MockMessageQueue>();
                    kernel.Rebind<ITestStartDateProvider>().To<MockTestStartDateProvider>();
                });

            var client = SmarterdamFactory.CreateClient();
            client.Start("", "1");
        }
    }
}
