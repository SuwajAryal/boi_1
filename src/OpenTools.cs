using System;
using System.Diagnostics;
using System.IO;
class OpenTools { [STAThread] static void Main(){Process.Start(new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"boi_1.exe"),"--tools"){UseShellExecute=true});} }
