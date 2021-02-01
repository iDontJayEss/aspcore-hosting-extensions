using System.ComponentModel.Composition;

namespace ServiceExtensions.Discovery.Mef.MockImports
{
    [Export("first", typeof(IMySampleContract))]
    public class FirstNamedImpl : IMySampleContract { }
    [Export("second", typeof(IMySampleContract))]
    public class SecondNamedImpl : IMySampleContract { }
    [Export("multi", typeof(IMySampleContract))]
    public class FirstMultiImpl : IMySampleContract { }
    [Export("multi", typeof(IMySampleContract))]
    public class SecondMultiImpl : IMySampleContract { }

    [Export("first", typeof(IAnotherSampleContract))]
    public class AnotherFirstNamedImpl : IAnotherSampleContract { }
    [Export("second", typeof(IAnotherSampleContract))]
    public class AnotherSecondNamedImpl : IAnotherSampleContract { }
    [Export("multi", typeof(IAnotherSampleContract))]
    public class AnotherFirstMultiImpl : IAnotherSampleContract { }
    [Export("multi", typeof(IAnotherSampleContract))]
    public class AnotherSecondMultiImpl : IAnotherSampleContract { }

}
