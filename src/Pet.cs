using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Boi {
 static class Program {
  internal static bool Diagnostics;
  internal static bool SmokeTest;
  internal static bool PreviewOnly;
  internal static readonly EventWaitHandle ShowRequest=new EventWaitHandle(false,EventResetMode.AutoReset,"Local\\boi_1.open-tools.v1");
  internal static void Trace(string text){if(Diagnostics)File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"startup.log"),text+Environment.NewLine);}
  internal static readonly uint Wake=Native.RegisterWindowMessage("boi_1.show-tools.v1");
  [STAThread] static void Main(string[] args) {
   bool diagnostic=Array.IndexOf(args,"--diagnostics")>=0;
   Diagnostics=diagnostic;
   SmokeTest=Array.IndexOf(args,"--smoke-test")>=0;
   PreviewOnly=Array.IndexOf(args,"--render-preview")>=0;
   if(diagnostic)File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"startup.log"),"Start "+DateTime.UtcNow+Environment.NewLine);
   Native.SetProcessDPIAware(); bool first;
   using(var mutex=new Mutex(true,(SmokeTest||PreviewOnly)?"Local\\boi_1.test."+Guid.NewGuid():"Local\\boi_1.desktop.v1",out first)) {
    if(diagnostic)File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"startup.log"),"First instance: "+first+Environment.NewLine);
    if(!first){ShowRequest.Set();return;}
    Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
    try { Application.Run(new Pet(Array.IndexOf(args,"--tools")>=0||SmokeTest||PreviewOnly)); }
    catch(Exception ex) {MessageBox.Show("boi_1 could not start.\n"+ex.Message,"boi_1",MessageBoxButtons.OK,MessageBoxIcon.Error);}
   }
  }
 }
 sealed class Atlas : IDisposable {
  public Bitmap[] Frames;
  public Atlas(string path,int cols,int rows) {
   using(var image=new Bitmap(path)) {
    Frames=new Bitmap[cols*rows];int maxWidth=1,maxHeight=1;
    for(int i=0;i<Frames.Length;i++) {
     int x0=(int)Math.Round((i%cols)*image.Width/(double)cols),x1=(int)Math.Round(((i%cols)+1)*image.Width/(double)cols);
     int y0=(int)Math.Round((i/cols)*image.Height/(double)rows),y1=(int)Math.Round(((i/cols)+1)*image.Height/(double)rows);int w=x1-x0,h=y1-y0;
     using(var cell=image.Clone(new Rectangle(x0,y0,w,h),PixelFormat.Format32bppArgb)) {
      int left=w,top=h,right=0,bottom=0;
      for(int y=0;y<h;y++)for(int x=0;x<w;x++){var c=cell.GetPixel(x,y);if(c.A<=8)cell.SetPixel(x,y,Color.Transparent);else if(c.A>24){left=Math.Min(left,x);right=Math.Max(right,x);top=Math.Min(top,y);bottom=Math.Max(bottom,y);}}
      if(left>=right)throw new InvalidDataException("Empty cat frame in "+path);
      // Runtime alpha bounds remove only empty atlas padding, preserving original raster art.
      Frames[i]=cell.Clone(Rectangle.FromLTRB(left,top,right+1,bottom+1),PixelFormat.Format32bppArgb);
      maxWidth=Math.Max(maxWidth,Frames[i].Width);maxHeight=Math.Max(maxHeight,Frames[i].Height);
     }
    }
    for(int i=0;i<Frames.Length;i++){var source=Frames[i];var normalized=new Bitmap(maxWidth,maxHeight,PixelFormat.Format32bppArgb);using(var g=Graphics.FromImage(normalized))g.DrawImageUnscaled(source,(maxWidth-source.Width)/2,maxHeight-source.Height);source.Dispose();Frames[i]=normalized;}
   }
  }
  public void Dispose(){foreach(var f in Frames)f.Dispose();}
 }
 sealed class Pet : Form {
  public SaveData Data; public FocusClock Clock; public Store Storage;
  public Tools Tools;
  public readonly Stopwatch SessionCounter=new Stopwatch();
  public bool CountUp;
  public string SessionSummary {get{
   if(SessionCounter.ElapsedTicks==0&&!SessionCounter.IsRunning)return "Ready";
   long minutes=(long)SessionCounter.Elapsed.TotalMinutes;
   if(minutes==0)return "< 1 min";
   if(minutes<60)return minutes+" min";
   return (minutes/60)+"h "+(minutes%60)+"m";
  }}
  public string SessionDisplay {get{
   if(SessionCounter.ElapsedTicks==0&&!SessionCounter.IsRunning)return "Ready";
   var elapsed=SessionCounter.Elapsed;
   if(elapsed.TotalSeconds<1)return "< 1 sec";
   if(elapsed.TotalHours<1)return elapsed.Minutes+":"+elapsed.Seconds.ToString("00");
   return ((long)elapsed.TotalHours)+":"+elapsed.Minutes.ToString("00")+":"+elapsed.Seconds.ToString("00");
  }}
  public void HideCornerTimer(){eyeBreakEnds=0;Data.ShowCounter=false;Save();UpdateCounter(uptime.Elapsed.TotalSeconds);}
  CornerCounter counter;
  double nextEyeBreak, eyeBreakEnds;
  bool foregroundFullscreen;
  IntPtr pinnedFullscreenWindow;
  NotifyIcon tray;ContextMenuStrip menu;
  System.Windows.Forms.Timer tick;Atlas expressions,walk,idle,classic,extra;
  readonly Stopwatch uptime=Stopwatch.StartNew();readonly Random random=new Random();
  double previous,x,decisionAt,playUntil,nextCheck,animationStart;int direction=-1;string activity="Idle",preview="",animationState="";
  bool dragging,down,moved,hidden,full,quitting;Point downCursor,downLocation;
  double lastInteraction; int baseSize;Icon catIcon;
  Bitmap cachedFrame;string cachedKey="";Point renderedLocation=new Point(int.MinValue,int.MinValue);
  public string Status {get {return preview!=""?preview:activity;}}
  protected override bool ShowWithoutActivation { get {return true;} }
  protected override CreateParams CreateParams {get {var p=base.CreateParams;p.ExStyle|=0x80000|0x80|0x08000000;return p;}}
  public Pet(bool openTools) {
   Text="boi_1 - desktop cat";FormBorderStyle=FormBorderStyle.None;ShowInTaskbar=false;TopMost=true;StartPosition=FormStartPosition.Manual;
   Storage=new Store((Program.SmokeTest||Program.PreviewOnly)?Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"smoke-data"):Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"boi_1"));Data=(Program.SmokeTest||Program.PreviewOnly)?new SaveData():Storage.Load();Clock=new FocusClock(Data);
   string folder=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"assets");
   expressions=new Atlas(Path.Combine(folder,"cat-reaction-loops.png"),4,4);walk=new Atlas(Path.Combine(folder,"cat-walk-loop.png"),4,3);idle=new Atlas(Path.Combine(folder,"cat-idle-loop.png"),4,2);classic=new Atlas(Path.Combine(folder,"cat-expressions.png"),3,3);extra=new Atlas(Path.Combine(folder,"cat-extra-expressions.png"),4,2);
   Program.Trace("Art loaded");
   using(var g=Graphics.FromHwnd(IntPtr.Zero))baseSize=(int)(96*g.DpiX/96);
   Size=new Size(baseSize,baseSize);var area=Screen.PrimaryScreen.WorkingArea;
   x=Data.HomeX<0?area.Right-Width-70:Data.HomeX;Location=new Point((int)x,area.Bottom-Height-3);Clamp();
   menu=new ContextMenuStrip(); menu.Items.Add("Timer",null,(s,e)=>OpenTools(0));menu.Items.Add("To-do list",null,(s,e)=>OpenTools(1));
   menu.Items.Add("Expressions & settings",null,(s,e)=>OpenTools(2));menu.Items.Add(new ToolStripSeparator());
   menu.Items.Add("Pause / resume roaming",null,(s,e)=>{Data.PauseRoaming=!Data.PauseRoaming;Save();});
   menu.Items.Add("Show / hide cat",null,(s,e)=>ToggleHidden());menu.Items.Add("Quit boi_1",null,(s,e)=>Quit());
   tray=new NotifyIcon{Icon=SystemIcons.Information,Text="boi_1 - click for timer & tasks",Visible=true,ContextMenuStrip=menu};
   using(var tiny=new Bitmap(idle.Frames[0],32,32)){IntPtr handle=tiny.GetHicon();using(var icon=Icon.FromHandle(handle))catIcon=(Icon)icon.Clone();Native.DestroyIcon(handle);}tray.Icon=catIcon;
   tray.MouseClick+=(s,e)=>{if(e.Button==MouseButtons.Left)OpenTools(0);};
   counter=new CornerCounter(this);
   MouseDown+=OnDown;MouseMove+=OnMove;MouseUp+=OnUp;
   tick=new System.Windows.Forms.Timer{Interval=16};tick.Tick+=(s,e)=>Step();
   Shown+=(s,e)=>{
    Program.Trace("Pet shown");
    if(!Native.RegisterHotKey(Handle,1,0x4000|2|1,(int)Keys.B))Storage.Warning="Ctrl+Alt+B is already in use. Open tools from the cat or tray.";
    tick.Start(); Render();
    if(openTools)BeginInvoke((Action)(()=>{if(Program.PreviewOnly){RunPreview();return;}OpenTools(0);if(Program.SmokeTest)RunSmoke();}));
    if(Clock.State==ClockState.Finished)Complete(false);
   };
   Microsoft.Win32.SystemEvents.DisplaySettingsChanged+=DisplayChanged;
   FormClosing+=(s,e)=>{if(!quitting&&e.CloseReason==CloseReason.UserClosing){e.Cancel=true;ToggleHidden();}else if(!Program.PreviewOnly)Save();};
   Program.Trace("Constructed");
  }
  void RunPreview(){string root=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"preview");Directory.CreateDirectory(root);foreach(string state in CatAnimation.States){string folder=Path.Combine(root,state);Directory.CreateDirectory(folder);for(int i=0;i<36;i++)using(var b=RenderFrame(state,i/18.0))b.Save(Path.Combine(folder,i.ToString("000")+".png"),ImageFormat.Png);}quitting=true;Close();}
  void RunSmoke(){
   string folder=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","evidence");Directory.CreateDirectory(folder);
   try {Tools.RunSmoke(folder);using(var b=RenderFrame("Idle"))b.Save(Path.Combine(folder,"pet-idle.png"),ImageFormat.Png);
    using(var b=RenderFrame("Walking"))b.Save(Path.Combine(folder,"pet-walk.png"),ImageFormat.Png);
    File.WriteAllText(Path.Combine(folder,"smoke-result.txt"),"PASS: native tool form rendered; timer start/pause/resume/reset; session counter start/pause; task add/complete/edit/delete/undo; expression preview; reduced motion.\nDesktop compositor interactions are not verified by this harness.");
   }catch(Exception ex){File.WriteAllText(Path.Combine(folder,"smoke-result.txt"),"FAIL: "+ex);}
   quitting=true;Close();
  }
  void DisplayChanged(object s,EventArgs e){if(IsHandleCreated&&!IsDisposed)BeginInvoke((Action)(()=>{Clamp();Render();}));}
  protected override void WndProc(ref Message m){if(m.Msg==0x21){m.Result=new IntPtr(3);return;}if(m.Msg==0x312||m.Msg==(int)Program.Wake){OpenTools(0);return;}base.WndProc(ref m);}
  void OnDown(object s,MouseEventArgs e){if(e.Button!=MouseButtons.Left)return; down=true;moved=false;downCursor=Cursor.Position;downLocation=Location;Capture=true;lastInteraction=uptime.Elapsed.TotalSeconds;}
  void OnMove(object s,MouseEventArgs e){if(!down)return;var d=new Point(Cursor.Position.X-downCursor.X,Cursor.Position.Y-downCursor.Y);if(Math.Abs(d.X)+Math.Abs(d.Y)>6){dragging=true;moved=true;Location=new Point(downLocation.X+d.X,downLocation.Y+d.Y);x=Left;Render();}}
  void OnUp(object s,MouseEventArgs e){if(e.Button==MouseButtons.Right){menu.Show(Cursor.Position);return;}if(e.Button!=MouseButtons.Left)return; down=false;dragging=false;Capture=false;lastInteraction=uptime.Elapsed.TotalSeconds;preview="";
   if(moved){Clamp();Data.HomeX=Left;Save();}else OpenTools(0);
  }
  void Clamp(){var a=Screen.PrimaryScreen.WorkingArea;int min,max;RoamingLane.Bounds(a.Left,a.Width,Width,out min,out max);x=Math.Max(min,Math.Min(max,x));Location=new Point((int)x,a.Bottom-Height-3);}
  bool IsFullscreen(IntPtr fg){if(fg==Handle||(Tools!=null&&fg==Tools.Handle))return false;var name=new System.Text.StringBuilder(256);Native.GetClassName(fg,name,256);if(name.ToString()=="Progman"||name.ToString()=="WorkerW"||name.ToString()=="Shell_TrayWnd")return false;Native.RECT r;if(!Native.GetWindowRect(fg,out r))return false;var b=Screen.PrimaryScreen.Bounds;return r.Left<=b.Left&&r.Top<=b.Top&&r.Right>=b.Right&&r.Bottom>=b.Bottom;}
  void Step(){double now=uptime.Elapsed.TotalSeconds,dt=Math.Min(.1,now-previous);previous=now;
   if(Clock.Tick()){Complete(true);Save();}
   if(now>=nextCheck){
    nextCheck=now+.25;
    if(Program.ShowRequest.WaitOne(0))OpenTools(0);
    var foreground=Native.GetForegroundWindow();
    foregroundFullscreen=IsFullscreen(foreground);
    full=foregroundFullscreen&&!Data.PinOverFullscreen;
    bool showPet=!hidden&&!full;
    if(Visible!=showPet){if(showPet)Show();else Hide();}
    UpdateCounter(now);
    if(!Data.PinOverFullscreen)pinnedFullscreenWindow=IntPtr.Zero;
    // Reordering the overlay repeatedly can cover taskbar and app context menus.
    if(foregroundFullscreen&&Data.PinOverFullscreen&&foreground!=pinnedFullscreenWindow&&!menu.Visible&&!counter.ContextMenuStrip.Visible){
     pinnedFullscreenWindow=foreground;
     if(Visible)Native.KeepAbove(Handle);
     if(counter.Visible)Native.KeepAbove(counter.Handle);
     if(Tools!=null&&Tools.Visible)Native.KeepAbove(Tools.Handle);
    }
    if(Tools!=null&&!Tools.IsDisposed){Tools.TopMost=Data.PinOverFullscreen;Tools.RefreshState();}
   }
   if(hidden||full)return;
   bool hovering=Bounds.Contains(Cursor.Position),tools=Tools!=null&&Tools.Visible;
   if(hovering)lastInteraction=now;
   string next=activity;
   if(preview!=""&&now>=playUntil)preview="";
   if(dragging) next="Dragged";
   else if(preview!="")next=preview;
   else if(hovering) next="Curious";
   else if(Clock.State==ClockState.Running)next="Serious";
   else if(tools||Data.PauseRoaming||Data.ReducedMotion)next="Idle";
   else if(now-lastInteraction>90)next="Sleepy";
   else if(now>=decisionAt){int choice=random.Next(20);next=choice<8?(CatAnimation.WalkCycleVerified?"Walking":"Idle"):choice<13?"Idle":choice<15?"Goofing":choice<17?"Enjoying":choice==17?"Stretching":choice==18?"Yawning":"Confused";decisionAt=now+(next=="Walking"?5+random.Next(7):3+random.Next(6));}
   activity=next;
   if(next=="Walking"&&!hovering&&!tools&&!Data.PauseRoaming&&!Data.ReducedMotion){x+=direction*26*dt;var a=Screen.PrimaryScreen.WorkingArea;int min,max;RoamingLane.Bounds(a.Left,a.Width,Width,out min,out max);if(x<min||x>max){direction=-direction;x=Math.Max(min,Math.Min(max,x));}Location=new Point((int)x,a.Bottom-Height-3);}
   if(!dragging&&Math.Abs(Top-(Screen.PrimaryScreen.WorkingArea.Bottom-Height-3))>1){int target=Screen.PrimaryScreen.WorkingArea.Bottom-Height-3;Top+=(int)((target-Top)*.2);if(Math.Abs(target-Top)<6)Top=target;}
   Render();
  }
  void UpdateCounter(double now){
   if(!Data.EyeBreaks){nextEyeBreak=eyeBreakEnds=0;}
   else if(nextEyeBreak==0){nextEyeBreak=now+20*60;}
   else if(now>=nextEyeBreak){
    eyeBreakEnds=now+20;
    nextEyeBreak=now+20*60;
    if(Data.Sound)System.Media.SystemSounds.Asterisk.Play();
    if(full)tray.ShowBalloonTip(5000,"Eye break","Look about 20 feet (6 metres) away for 20 seconds.",ToolTipIcon.Info);
   }
   int eyeSeconds=(int)Math.Ceiling(Math.Max(0,eyeBreakEnds-now));
   var screen=Screen.PrimaryScreen;
   var area=Data.PinOverFullscreen&&foregroundFullscreen?screen.Bounds:screen.WorkingArea;
   int bottom=hidden?area.Bottom:Math.Min(area.Bottom,Top);
   counter.UpdateDisplay(Rectangle.FromLTRB(area.Left,area.Top,area.Right,bottom),eyeSeconds,!full&&(Data.ShowCounter||eyeSeconds>0));
  }
  public void React(string state){preview=state;playUntil=uptime.Elapsed.TotalSeconds+(state=="Sleepy"?8:4);lastInteraction=uptime.Elapsed.TotalSeconds;Render();}
  public void Complete(bool sound){React("Celebrating");if(sound&&Data.Sound)System.Media.SystemSounds.Asterisk.Play();tray.Text="boi_1 - timer finished";}
  public void Save(){Clock.Export(Data);Storage.Save(Data);if(Tools!=null&&!Tools.IsDisposed){Tools.TopMost=Data.PinOverFullscreen;Tools.RefreshState();}}
  public void OpenTools(int page){Program.Trace("Open tools");preview="";lastInteraction=uptime.Elapsed.TotalSeconds;if(Tools==null||Tools.IsDisposed)Tools=new Tools(this);UpdateCounter(uptime.Elapsed.TotalSeconds);Tools.SelectPage(page);Tools.TopMost=Data.PinOverFullscreen;var a=Screen.PrimaryScreen.WorkingArea;int right=counter.Visible&&counter.Left-12-Tools.Width>=a.Left?counter.Left-12:Left+Width;Tools.Location=new Point(Math.Max(a.Left,Math.Min(a.Right-Tools.Width,right-Tools.Width)),Math.Max(a.Top,a.Bottom-Height-Tools.Height-10));Tools.Show();Tools.Activate();Program.Trace("Tools visible: "+Tools.Visible);}
  public void ToggleHidden(){hidden=!hidden;if(hidden){Hide();if(Tools!=null)Tools.Hide();}else {Clamp();if(!full)Show();Render();} }
  public void Quit(){if(Clock.State==ClockState.Running&&MessageBox.Show("Your timer will be saved, but cannot alert while boi_1 is closed. Quit?","boi_1",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;Save();quitting=true;Close();}
  void Render(){if(!IsHandleCreated||hidden||full)return;string state=dragging?"Dragged":preview!=""?preview:activity;if(animationState!=state){animationState=state;animationStart=uptime.Elapsed.TotalSeconds;}double seconds=uptime.Elapsed.TotalSeconds-animationStart;Clip clip=CatAnimation.Get(state);string key=state+":"+FrameTiming.Index(seconds,clip.Count,clip.Fps,Data.ReducedMotion)+":"+direction+":"+Width+":"+Height;bool changed=key!=cachedKey;
   if(changed){if(cachedFrame!=null)cachedFrame.Dispose();cachedFrame=RenderFrame(state,seconds);cachedKey=key;}
   if(changed||Location!=renderedLocation){Native.Present(Handle,cachedFrame,Location);renderedLocation=Location;}
  }
  Bitmap RenderFrame(string state){if(animationState!=state){animationState=state;animationStart=uptime.Elapsed.TotalSeconds;}return RenderFrame(state,uptime.Elapsed.TotalSeconds-animationStart);}
  Bitmap RenderFrame(string state,double seconds){
   if(state=="Walking"&&!CatAnimation.WalkCycleVerified)state="Idle";
   Clip clip=CatAnimation.Get(state);Atlas atlas=clip.Atlas=="walk"?walk:clip.Atlas=="reactions"?expressions:clip.Atlas=="classic"?classic:clip.Atlas=="extra"?extra:idle;
   Bitmap src=atlas.Frames[clip.Start+FrameTiming.Index(seconds,clip.Count,clip.Fps,Data.ReducedMotion)];
   var b=new Bitmap(Width,Height,PixelFormat.Format32bppArgb);using(var g=Graphics.FromImage(b)){
    g.Clear(Color.Transparent);g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.SmoothingMode=SmoothingMode.AntiAlias;
    float w=Width*.88f,h=Height*.88f;float scale=Math.Min(w/src.Width,h/src.Height);w=src.Width*scale;h=src.Height*scale;
    g.TranslateTransform(Width/2f,Height-8-h/2);if(state=="Walking"&&direction>0)g.ScaleTransform(-1,1);g.DrawImage(src,-w/2,-h/2,w,h);
   }
   return b;
  }
  protected override void Dispose(bool disposing){if(disposing){tick.Stop();tick.Dispose();Native.UnregisterHotKey(Handle,1);Microsoft.Win32.SystemEvents.DisplaySettingsChanged-=DisplayChanged;tray.Visible=false;tray.Dispose();menu.Dispose();counter.Dispose();catIcon.Dispose();expressions.Dispose();walk.Dispose();idle.Dispose();classic.Dispose();extra.Dispose();if(cachedFrame!=null)cachedFrame.Dispose();if(Tools!=null)Tools.Dispose();}base.Dispose(disposing);}
 }
}
