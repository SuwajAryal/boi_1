using System;
using System.Drawing;
using System.Windows.Forms;

namespace Boi {
 sealed class CornerCounter : Form {
  readonly Pet pet;
  readonly Label display;
  readonly Button minus, plus;
  readonly ContextMenuStrip actions;

  public CornerCounter(Pet pet) {
   this.pet=pet;
   Text="boi_1 corner counter";
   FormBorderStyle=FormBorderStyle.None;
   ShowInTaskbar=false;
   TopMost=true;
   StartPosition=FormStartPosition.Manual;
   ClientSize=new Size(224,38);
   BackColor=Color.FromArgb(227,242,255);
   Font=new Font("Segoe UI",9);
   display=new Label{Bounds=new Rectangle(34,0,156,38),TextAlign=ContentAlignment.MiddleCenter,ForeColor=Color.FromArgb(38,70,109)};
   minus=AddButton("−","Subtract one",3,-1);
   plus=AddButton("+","Add one",192,1);
   Controls.Add(display);
   actions=new ContextMenuStrip();
   actions.Items.Add("Reset count",null,(s,e)=>{pet.Data.Count=0;pet.Save();});
   actions.Items.Add("Open settings",null,(s,e)=>pet.OpenTools(2));
   ContextMenuStrip=actions;
  }

  Button AddButton(string text,string name,int left,int delta) {
   var button=new Button{Text=text,AccessibleName=name,Bounds=new Rectangle(left,4,29,30),FlatStyle=FlatStyle.Flat,BackColor=Color.White};
   button.FlatAppearance.BorderColor=Color.FromArgb(177,209,236);
   button.Click+=(s,e)=>{pet.Data.Count=(int)Math.Max(0,Math.Min(int.MaxValue,(long)pet.Data.Count+delta));pet.Save();};
   Controls.Add(button);
   return button;
  }

  protected override bool ShowWithoutActivation { get { return true; } }
  protected override CreateParams CreateParams {
   get { var p=base.CreateParams;p.ExStyle|=0x80|0x08000000;return p; }
  }
  protected override void WndProc(ref Message m) {
   if(m.Msg==0x21){m.Result=new IntPtr(3);return;}
   base.WndProc(ref m);
  }

  public void UpdateDisplay(Rectangle area,int eyeSeconds,bool visible) {
   if(!visible){if(Visible)Hide();return;}
   bool reminding=eyeSeconds>0;
   minus.Visible=plus.Visible=!reminding;
   display.Bounds=reminding?ClientRectangle:new Rectangle(34,0,156,38);
   string timer=pet.Clock.State==ClockState.Running||pet.Clock.State==ClockState.Paused?"   ·   "+pet.Clock.Display:"";
   display.Text=reminding?"Look far away  ·  "+eyeSeconds+"s":pet.Data.Count+timer;
   Location=new Point(Math.Max(area.Left,area.Right-Width-8),Math.Max(area.Top,area.Bottom-Height-8));
   if(!Visible)Show();
  }

  protected override void Dispose(bool disposing) {
   if(disposing)actions.Dispose();
   base.Dispose(disposing);
  }
 }
}
