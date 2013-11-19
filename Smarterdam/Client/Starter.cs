using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;

namespace Smarterdam.Client
{
    public class Starter : ISmarterdamClient
    {
        private IQueryParser queryParser;
        private IModelsStarter _modelsStarter;

        [Inject]
		public Starter(IQueryParser queryParser, IModelsStarter _modelsStarter)
        {
            this.queryParser = queryParser;
			this._modelsStarter = _modelsStarter;
        }

        public void Start(string query, string measurementId)
        {
            StartThread(query, measurementId);
        }

        private void StartThread(string query, string measurementId)
        {
            var command = queryParser.Parse(query);

            _modelsStarter.StartModels(measurementId);
        }
    }
}
