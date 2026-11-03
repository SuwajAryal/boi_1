using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
namespace Boi {
 static class Native {
  [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X,Y; public POINT(int x,int y){X=x;Y=y;} }
  [StructLayout(LayoutKind.Sequential)] public struct SIZE { public int X,Y; public SIZE(int x,int y){X=x;Y=y;} }
  [StructLayout(LayoutKind.Sequential,Pack=1)] public struct BLEND { public byte Op,Flags,Alpha,Format; }
  [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left,Top,Right,Bottom; }
  [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
  [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr hwnd);
  [DllImport("user32.dll")] public static extern int ReleaseDC(IntPtr hwnd,IntPtr dc);
  [DllImport("gdi32.dll")] public static extern IntPtr CreateCompatibleDC(IntPtr dc);
  [DllImport("gdi32.dll")] public static extern bool DeleteDC(IntPtr dc);
  [DllImport("gdi32.dll")] public static extern IntPtr SelectObject(IntPtr dc,IntPtr obj);
  [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr obj);
  [DllImport("user32.dll")] public static extern bool UpdateLayeredWindow(IntPtr hwnd,IntPtr dst,ref POINT pos,ref SIZE size,IntPtr src,ref POINT origin,int key,ref BLEND blend,int flags);
  [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern int GetClassName(IntPtr hwnd,StringBuilder name,int max);
  [DllImport("user32.dll")] public static extern bool DestroyIcon(IntPtr icon);
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd,out RECT rect);
  [DllImport("user32.dll")] public static extern bool RegisterHotKey(IntPtr hwnd,int id,int mod,int key);
  [DllImport("user32.dll")] public static extern bool UnregisterHotKey(IntPtr hwnd,int id);
  [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern uint RegisterWindowMessage(string name);
  [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern bool PostMessage(IntPtr hwnd,uint msg,IntPtr w,IntPtr l);
  [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hwnd,IntPtr after,int x,int y,int cx,int cy,uint flags);
  public static void KeepAbove(IntPtr hwnd) { SetWindowPos(hwnd,new IntPtr(-1),0,0,0,0,0x0013); }
  [DllImport("user32.dll")] static extern IntPtr CreatePopupMenu();
  [DllImport("user32.dll")] static extern bool DestroyMenu(IntPtr menu);
  [DllImport("user32.dll",CharSet=CharSet.Unicode)] static extern bool AppendMenu(IntPtr menu,uint flags,UIntPtr id,string text);
  [DllImport("user32.dll")] static extern int TrackPopupMenuEx(IntPtr menu,uint flags,int x,int y,IntPtr owner,IntPtr options);
  [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hwnd);

  public static int ShowTrayMenu(IntPtr owner,Point position,string[] labels){
   const uint separator=0x800, rightButton=0x2, bottomAlign=0x20, returnCommand=0x100;
   var popup=CreatePopupMenu();
   if(popup==IntPtr.Zero)return 0;
   try {
    for(int i=0;i<labels.Length;i++)
     if(!AppendMenu(popup,labels[i]==null?separator:0,new UIntPtr((uint)(i+1)),labels[i]))return 0;
    SetForegroundWindow(owner);
    try { return TrackPopupMenuEx(popup,rightButton|bottomAlign|returnCommand,position.X,position.Y,owner,IntPtr.Zero); }
    finally { PostMessage(owner,0,IntPtr.Zero,IntPtr.Zero); } // WM_NULL lets the next tray menu open normally.
   }finally{DestroyMenu(popup);}
  }
  public static void Present(IntPtr hwnd,Bitmap bitmap,Point location) {
   IntPtr screen=GetDC(IntPtr.Zero),dc=CreateCompatibleDC(screen),hb=IntPtr.Zero,old=IntPtr.Zero;
   try { hb=bitmap.GetHbitmap(Color.FromArgb(0)); old=SelectObject(dc,hb);
    var p=new POINT(location.X,location.Y);var s=new SIZE(bitmap.Width,bitmap.Height); var z=new POINT(0,0);var b=new BLEND{Alpha=255,Format=1};
    UpdateLayeredWindow(hwnd,screen,ref p,ref s,dc,ref z,0,ref b,2);
   } finally { if(old!=IntPtr.Zero)SelectObject(dc,old); if(hb!=IntPtr.Zero)DeleteObject(hb);DeleteDC(dc);ReleaseDC(IntPtr.Zero,screen); }
  }
 }
}
