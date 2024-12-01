using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace YA_Metro.Properties
{
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) YA_Metro.Properties.Resources.resourceMan, (object) null))
          YA_Metro.Properties.Resources.resourceMan = new ResourceManager("YA_Metro.Properties.Resources", typeof (YA_Metro.Properties.Resources).Assembly);
        return YA_Metro.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => YA_Metro.Properties.Resources.resourceCulture;
      set => YA_Metro.Properties.Resources.resourceCulture = value;
    }
  }
}
