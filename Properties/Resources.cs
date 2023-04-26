// Decompiled with JetBrains decompiler
// Type: TadahLauncher.Properties.Resources
// Assembly: TadahLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3111CEF3-12D2-4E80-9B4F-93665981ABEB
// Assembly location: C:\Users\Case\AppData\Local\Tadah\2014\TadahLauncher.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TadahLauncher.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
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
        if (TadahLauncher.Properties.Resources.resourceMan == null)
          TadahLauncher.Properties.Resources.resourceMan = new ResourceManager("TadahLauncher.Properties.Resources", typeof (TadahLauncher.Properties.Resources).Assembly);
        return TadahLauncher.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => TadahLauncher.Properties.Resources.resourceCulture;
      set => TadahLauncher.Properties.Resources.resourceCulture = value;
    }
  }
}
