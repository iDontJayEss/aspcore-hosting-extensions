using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace ServiceExtensions.Discovery.Mef.MockImports
{
    [Export(typeof(IMySampleContract))]
    public class DefaultImplementation : IMySampleContract
    {

    }

    [Export(typeof(IAnotherSampleContract))]
    public class AnotherDefaultImplementation : IAnotherSampleContract { }
}
