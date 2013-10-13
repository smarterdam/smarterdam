using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Pipelines;

namespace Smarterdam.Client
{
    public class PipelinePack
    {
        public StreamPipeline OnlinePipeline;
        public StreamPipeline OfflinePipeline;
        public StreamPipeline DatabasePipeline;
    }
}
