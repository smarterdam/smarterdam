using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation.Strategies;
using Smarterdam.Api;
using Smarterdam.Client;

namespace Smarterdam.PipelineModels
{
	public interface IPipelineModel
	{
        string Name { get; }
		void Start(string id, IMessageQueue messageQueue);
	}
}
