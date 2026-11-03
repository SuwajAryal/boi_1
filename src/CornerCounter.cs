using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Boi {
 sealed class CornerCounter : Form {
  readonly Pet pet;
  readonly ContextMenuStrip actions;
  readonly Font heading = new Font("Segoe UI",9,FontStyle.Bold);
  readonly Font digits = new Font("Segoe UI",18,FontStyle.Bold);
  readonly Font quietDigits = new Font("Segoe UI",13,FontStyle.Bold);
  string caption="", value="";
  float progress;
  bool hovered, countUp;
  Bitmap surface;
  Point presentedAt;
  bool needsPaint=true;
  Rectangle CloseArea {get{return new Rectangle(Width-34,Height/2-12,24,24);}}

  public CornerCounter(Pet pet) {
   this.pet=pet;
   Text="boi_1 focus timer";
   FormBorderStyle=FormBorderStyle.None;
   ShowInTaskbar=false;
   TopMost=true;
   StartPosition=FormStartPosition.Manual;
   AutoScaleMode=AutoScaleMode.None;
   Cursor=Cursors.Hand;
   using(var g=Graphics.FromHwnd(IntPtr.Zero))
    ClientSize=new Size((int)Math.Round(182*g.DpiX/96),(int)Math.Round(66*g.DpiY/96));

   actions=new ContextMenuStrip();
   actions.Items.Add("Open focus timer",null,(s,e)=>pet.OpenTools(0));
   actions.Items.Add("Add one to tally",null,(s,e)=>ChangeCount(1));
   actions.Items.Add("Subtract one from tally",null,(s,e)=>ChangeCount(-1));
   actions.Items.Add("Hide corner bubble",null,(s,e)=>pet.HideCornerTimer());
   actions.Items.Add("Reset tally",null,(s,e)=>{pet.Data.Count=0;pet.Save();});
   ContextMenuStrip=actions;
   MouseEnter+=(s,e)=>{hovered=true;Redraw();};
   MouseLeave+=(s,e)=>{hovered=false;Redraw();};
   MouseClick+=(s,e)=>{
    if(e.Button!=MouseButtons.Left)return;
    if(CloseArea.Contains(e.Location))pet.HideCornerTimer();else pet.OpenTools(0);
   };
  }

  void ChangeCount(int delta) {
   pet.Data.Count=(int)Math.Max(0,Math.Min(int.MaxValue,(long)pet.Data.Count+delta));
   pet.Save();
  }

  protected override bool ShowWithoutActivation { get { return true; } }
  protected override CreateParams CreateParams {
   get { var p=base.CreateParams;p.ExStyle|=0x80000|0x80|0x08000000;return p; }
  }
  protected override void WndProc(ref Message m) {
   if(m.Msg==0x21){m.Result=new IntPtr(3);return;}
   base.WndProc(ref m);
  }

  void Redraw(){
   needsPaint=true;
   if(Visible)Present();
  }

  void Present(){
   if(needsPaint){
    if(surface!=null)surface.Dispose();
    surface=new Bitmap(Width,Height,PixelFormat.Format32bppPArgb);
    using(var g=Graphics.FromImage(surface))PaintGlass(g);
    needsPaint=false;
   }
   Native.Present(Handle,surface,Location);
   presentedAt=Location;
  }

  void PaintGlass(Graphics g) {
   g.SmoothingMode=SmoothingMode.AntiAlias;
   g.TextRenderingHint=TextRenderingHint.AntiAliasGridFit;
   var bounds=new Rectangle(2,2,Width-4,Height-4);
   using(var outline=SoftShape.Pill(bounds))
   using(var tint=new LinearGradientBrush(bounds,Color.FromArgb(92,245,252,255),Color.FromArgb(66,160,205,239),LinearGradientMode.Vertical))
   using(var reflection=new LinearGradientBrush(bounds,Color.White,Color.FromArgb(155,110,165,212),40f))
   using(var rim=new Pen(reflection,2.3f)){
    g.FillPath(tint,outline);
    g.DrawPath(rim,outline);
   }
   bounds.Inflate(-3,-3);
   using(var inner=SoftShape.Pill(bounds))
   using(var bevel=new LinearGradientBrush(bounds,Color.FromArgb(160,255,255,255),Color.FromArgb(85,92,153,204),LinearGradientMode.Vertical))
   using(var pen=new Pen(bevel,1.4f))g.DrawPath(pen,inner);
   using(var center=new StringFormat{Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center})
   using(var ink=new SolidBrush(Color.FromArgb(205,15,36,65))){
    g.DrawString(caption,heading,ink,new RectangleF(0,Height/10,Width,Height/4),center);
    g.DrawString(value,countUp?quietDigits:digits,ink,new RectangleF(0,Height/3,Width,Height/2),center);
    if(hovered){
     using(var fill=new SolidBrush(Color.FromArgb(195,230,244,255)))g.FillEllipse(fill,CloseArea);
     g.DrawString("×",heading,ink,CloseArea,center);
    }
   }
   if(progress>0) {
    int left=Width/4, y=Height*9/10;
    using(var pen=new Pen(Color.FromArgb(170,110,155,205),2))
     g.DrawLine(pen,left,y,left+(int)(Width/2*progress),y);
   }
  }

  public void UpdateDisplay(Rectangle area,int eyeSeconds,bool visible) {
   if(!visible){if(hovered)needsPaint=true;hovered=false;if(Visible)Hide();return;}
   var clock=pet.Clock;
   string nextCaption=eyeSeconds>0?"LOOK FAR AWAY":pet.CountUp?"SESSION COUNTER":clock.State==ClockState.Running?"FOCUS TIME":
    clock.State==ClockState.Paused?"ON A BREAK":clock.State==ClockState.Finished?"SESSION COMPLETE":"START FOCUS";
   bool nextCountUp=eyeSeconds==0&&pet.CountUp;
   string nextValue=eyeSeconds>0?eyeSeconds+"s":pet.CountUp?pet.SessionDisplay:clock.Display;
   float nextProgress=eyeSeconds>0?eyeSeconds/20f:pet.CountUp?0:(float)Math.Max(0,Math.Min(1,clock.Remaining/(clock.Minutes*60)));
   if(countUp!=nextCountUp||caption!=nextCaption||value!=nextValue||Math.Abs(progress-nextProgress)>.005f) {
    countUp=nextCountUp;caption=nextCaption;value=nextValue;progress=nextProgress;
    AccessibleName=caption+" "+value;
    needsPaint=true;
   }
   actions.Items[1].Text="Add one to tally ("+pet.Data.Count+")";
   Location=new Point(Math.Max(area.Left,area.Right-Width-8),Math.Max(area.Top,area.Bottom-Height-8));
   bool showing=!Visible;
   if(showing)Show();
   if(showing||needsPaint||presentedAt!=Location)Present();
  }

  protected override void Dispose(bool disposing) {
   if(disposing){actions.Dispose();heading.Dispose();digits.Dispose();quietDigits.Dispose();if(surface!=null)surface.Dispose();}
   base.Dispose(disposing);
  }
 }
}
