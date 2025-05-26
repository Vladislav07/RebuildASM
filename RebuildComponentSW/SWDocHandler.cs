using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStack.SwEx.AddIn.Base;
using CodeStack.SwEx.AddIn.Core;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace RebuildComponentSW
{
    internal class SWDocHandler : DocumentHandler
    {
        ModelDoc2 modelRoot;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(ISldWorks app, IModelDoc2 model)
        {
            modelRoot = (ModelDoc2)model;
        
            swDocumentTypes_e swDocType = (swDocumentTypes_e)modelRoot.GetType();

            if (swDocType != swDocumentTypes_e.swDocASSEMBLY)
            {
                //"This program only works with assemblies";
                return;
            }
        }

        public bool ReadTree()
        {

            return true;
        }
    }
}
