﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    public interface IIntelligenceManager
    {
        PipelinePack ComposePipeline(Commands command, string id);
    }
}
