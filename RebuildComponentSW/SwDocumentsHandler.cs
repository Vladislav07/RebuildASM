using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStack.SwEx.AddIn.Base;
using SolidWorks.Interop.sldworks;

namespace RebuildComponentSW
{
    internal class SwDocumentsHandler : IDocumentsHandler<SWDocHandler>
    {
        public SWDocHandler this[IModelDoc2 model] => throw new NotImplementedException();

        public event Action<SWDocHandler> HandlerCreated;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
