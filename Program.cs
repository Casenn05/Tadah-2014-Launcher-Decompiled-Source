// Decompiled with JetBrains decompiler
// Type: TadahLauncher.Program
// Assembly: TadahLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3111CEF3-12D2-4E80-9B4F-93665981ABEB
// Assembly location: C:\Users\Case\AppData\Local\Tadah\2014\TadahLauncher.exe

using System;
using System.Windows.Forms;

namespace TadahLauncher
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new Form1());
    }
  }
}
