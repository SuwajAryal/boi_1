using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Boi {
 sealed class CornerCounter : Form {
  readonly Pet pet;
  readonly ContextMenuStrip actions;
  readonly Font heading = new Font("Segoe UI",9,FontStyle.Bold);
  readonly Font digits = new Font("Segoe UI",18,FontStyle.Bold);
  string caption="", value="";
  float progress;

  public CornerCounter(Pet pet) {
   this.pet=pet;
   Text="boi_1 focus timer";
   FormBorderStyle=FormBorderStyle.None;
   ShowInTaskbar=false;
   TopMost=true;
   StartPosition=FormStartPosition.Manual;
   AutoScaleMode=AutoScaleMode.None;
   DoubleBuffered=true;
   Cursor=Cursors.Hand;
   using(var g=Graphics.FromHwnd(IntPtr.Zero))
    ClientSize=new Size((int)Math.Round(182*g.DpiX/96),(int)Math.Round(66*g.DpiY/96));
   using(var outline=SoftShape.Pill(ClientRectangle)) Region=new Region(outline);

   actions=new ContextMenuStrip();
   actions.Items.Add("Open focus timer",null,(s,e)=>pet.OpenTools(0));
   actions.Items.Add("Add one to tally",null,(s,e)=>ChangeCount(1));
   actions.Items.Add("Subtract one from tally",null,(s,e)=>ChangeCount(-1));
   actions.Items.Add("Reset tally",null,(s,e)=>{pet.Data.Count=0;pet.Save();});
   ContextMenuStrip=actions;
   MouseClick+=(s,e)=>{if(e.Button==MouseButtons.Left)pet.OpenTools(0);};
  }

  void ChangeCount(int delta) {
   pet.Data.Count=(int)Math.Max(0,Math.Min(int.MaxValue,(long)pet.Data.Count+delta));
   pet.Save();
  }

  protected override bool ShowWithoutActivation { get { return true; } }
  protected override CreateParams CreateParams {
   get { var p=base.CreateParams;p.ExStyle|=0x80|0x08000000;return p; }
  }
  protected override void WndProc(ref Message m) {
   if(m.Msg==0x21){m.Result=new IntPtr(3);return;}
   base.WndProc(ref m);
  }

  protected override void OnPaint(PaintEventArgs e) {
   var g=e.Graphics;
   g.SmoothingMode=SmoothingMode.AntiAlias;
   g.Clear(Color.FromArgb(255,248,233));
   using(var edge=new Pen(Color.FromArgb(198,213,235),2))
   using(var outline=SoftShape.Pill(new Rectangle(1,1,Width-2,Height-2)))
    g.DrawPath(edge,outline);
   var center=TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.NoPadding;
   TextRenderer.DrawText(g,caption,heading,new Rectangle(0,Height/10,Width,Height/4),Color.FromArgb(87,104,137),center);
   TextRenderer.DrawText(g,value,digits,new Rectangle(0,Height/3,Width,Height/2),Color.FromArgb(44,62,96),center);
   if(progress>0) {
    int left=Width/4, y=Height*9/10;
    using(var pen=new Pen(Color.FromArgb(134,165,214),2))
     g.DrawLine(pen,left,y,left+(int)(Width/2*progress),y);
   }
  }

  public void UpdateDisplay(Rectangle area,int eyeSeconds,bool visible) {
   if(!visible){if(Visible)Hide();return;}
   var clock=pet.Clock;
   string nextCaption=eyeSeconds>0?"LOOK FAR AWAY":clock.State==ClockState.Running?"FOCUS TIME":
    clock.State==ClockState.Paused?"ON A BREAK":clock.State==ClockState.Finished?"SESSION COMPLETE":"START FOCUS";
   string nextValue=eyeSeconds>0?eyeSeconds+"s":clock.Display;
   float nextProgress=eyeSeconds>0?eyeSeconds/20f:(float)Math.Max(0,Math.Min(1,clock.Remaining/(clock.Minutes*60)));
   if(caption!=nextCaption||value!=nextValue||Math.Abs(progress-nextProgress)>.005f) {
    caption=nextCaption;value=nextValue;progress=nextProgress;
    AccessibleName=caption+" "+value;
    Invalidate();
   }
   actions.Items[1].Text="Add one to tally ("+pet.Data.Count+")";
   Location=new Point(Math.Max(area.Left,area.Right-Width-8),Math.Max(area.Top,area.Bottom-Height-8));
   if(!Visible)Show();
  }

  protected override void Dispose(bool disposing) {
   if(disposing){actions.Dispose();heading.Dispose();digits.Dispose();}
   base.Dispose(disposing);
  }
 }
}
