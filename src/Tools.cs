using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Boi {
 // These paths style ordinary native controls; all cloud illustration is a raster asset.
 static class SoftShape {
  public static GraphicsPath Pill(Rectangle r) {var p=new GraphicsPath();int d=Math.Max(2,Math.Min(r.Height,r.Width)-1);p.AddArc(r.X,r.Y,d,d,90,180);p.AddArc(r.Right-d-1,r.Y,d,d,270,180);p.CloseFigure();return p;}
 }
 sealed class CloudButton : Button {
  public Color Fill=Color.FromArgb(234,244,255),Ink=Color.FromArgb(65,99,150),Edge=Color.FromArgb(214,231,252);
  public string Glyph="";public Font IconFont;bool over;
  public CloudButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;BackColor=Color.Transparent;Cursor=Cursors.Hand;TabStop=true;}
  protected override void OnMouseEnter(EventArgs e){over=true;Invalidate();base.OnMouseEnter(e);}
  protected override void OnMouseLeave(EventArgs e){over=false;Invalidate();base.OnMouseLeave(e);}
  protected override void OnPaintBackground(PaintEventArgs e){var host=FindForm() as Tools;if(host!=null)host.PaintBackdrop(e.Graphics,this);else e.Graphics.Clear(Color.FromArgb(255,252,246));}
  protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;var r=new Rectangle(1,1,Width-2,Height-2);using(var path=SoftShape.Pill(r))using(var brush=new SolidBrush(over?ControlPaint.Light(Fill,.15f):Fill))using(var pen=new Pen(Edge,1.2f)){e.Graphics.FillPath(brush,path);e.Graphics.DrawPath(pen,path);}
   Color ink=Enabled?Ink:Color.FromArgb(153,168,190);var bounds=ClientRectangle;
   if(Glyph.Length>0&&IconFont!=null){int iconWidth=Text.Length==0?Width:(int)(IconFont.SizeInPoints*2.3);var iconBounds=Text.Length==0?bounds:new Rectangle(Width/5,0,iconWidth,Height);TextRenderer.DrawText(e.Graphics,Glyph,IconFont,iconBounds,ink,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);if(Text.Length>0)bounds=new Rectangle(Width/5+iconWidth,0,Width*3/5-iconWidth,Height);}
   if(Text.Length>0)TextRenderer.DrawText(e.Graphics,Text,Font,bounds,ink,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
   if(Focused&&ShowFocusCues)using(var focus=new Pen(Ink,1.5f))using(var ring=SoftShape.Pill(new Rectangle(4,4,Width-8,Height-8)))e.Graphics.DrawPath(focus,ring);
  }
 }
 sealed class SoftField : Panel {
  public SoftField(){BackColor=Color.Transparent;SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
  protected override void OnPaintBackground(PaintEventArgs e){var host=FindForm() as Tools;if(host!=null)host.PaintBackdrop(e.Graphics,this);else e.Graphics.Clear(Color.FromArgb(255,252,246));}
  protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;using(var p=SoftShape.Pill(new Rectangle(1,1,Width-2,Height-2)))using(var pen=new Pen(Color.FromArgb(207,224,246),1.5f))using(var fill=new SolidBrush(Color.FromArgb(255,253,249))){e.Graphics.FillPath(fill,p);e.Graphics.DrawPath(pen,p);}base.OnPaint(e);}
 }
 sealed class Tools : Form {
  readonly Pet pet;readonly float scale;readonly Bitmap cloud,cloudScaled;readonly PrivateFontCollection icons=new PrivateFontCollection();
  readonly Color ink=Color.FromArgb(44,70,105),muted=Color.FromArgb(102,128,164),blue=Color.FromArgb(103,171,239),paper=Color.FromArgb(255,252,246);
  Font iconFont;Panel timerPage,taskPage,settingsPage;CloudButton timerTab,taskTab,gear,primary;Label time,status,warning;NumericUpDown minutes;
  FlowLayoutPanel list;TextBox input;Todo deleted;int deletedAt;CloudButton undo;CheckBox done;bool dragging;Point dragPoint;int selectedPage;
  int S(double value){return (int)Math.Round(value*scale);}
  void Place(Control c,int x,int y,int w,int h){c.SetBounds(S(x),S(y),S(w),S(h));}
  public Tools(Pet p){pet=p;using(var g=Graphics.FromHwnd(IntPtr.Zero))scale=g.DpiX/96f;Text="boi_1 - cloud tools";FormBorderStyle=FormBorderStyle.None;ShowInTaskbar=true;StartPosition=FormStartPosition.Manual;AutoScaleMode=AutoScaleMode.None;ClientSize=new Size(S(400),S(530));BackColor=paper;ForeColor=ink;Font=new Font("Comic Sans MS",10);KeyPreview=true;DoubleBuffered=true;
   string assets=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"assets");cloud=new Bitmap(Path.Combine(assets,"cloud-panel.png"));cloudScaled=new Bitmap(cloud,ClientSize);icons.AddFontFile(Path.Combine(assets,"fa-solid-900.ttf"));iconFont=new Font(icons.Families[0],12,FontStyle.Regular);
   using(var outline=new GraphicsPath()){for(int y=0;y<cloudScaled.Height;y++){int start=-1;for(int x=0;x<=cloudScaled.Width;x++){bool solid=x<cloudScaled.Width&&cloudScaled.GetPixel(x,y).A>200;if(solid&&start<0)start=x;if(!solid&&start>=0){outline.AddRectangle(new Rectangle(start,y,x-start,1));start=-1;}}}Region=new Region(outline);}
   var title=LabelAt(this,"boi_1",25,FontStyle.Bold,53,48,180,46,ContentAlignment.MiddleLeft);title.ForeColor=Color.FromArgb(73,105,157);
   gear=ButtonAt(this,"",290,58,33,33,()=>SelectPage(2));gear.Glyph="\uf013";gear.IconFont=iconFont;gear.AccessibleName="Cat and settings";
   var close=ButtonAt(this,"",330,58,33,33,()=>HideTools());close.Glyph="\uf00d";close.IconFont=iconFont;close.AccessibleName="Hide tools";
   timerTab=ButtonAt(this,"Timer",47,108,153,40,()=>SelectPage(0));timerTab.Glyph="\uf017";timerTab.IconFont=iconFont;
   taskTab=ButtonAt(this,"Tasks",200,108,153,40,()=>SelectPage(1));taskTab.Glyph="\uf03a";taskTab.IconFont=iconFont;
   timerPage=Page();taskPage=Page();settingsPage=Page();settingsPage.Width=S(320);
   LabelAt(timerPage,"A little time to focus",12,FontStyle.Regular,4,16,300,35,ContentAlignment.MiddleCenter);
   var timerCard=new SoftField();Place(timerCard,39,56,230,97);timerPage.Controls.Add(timerCard);
   time=LabelAt(timerCard,"25:00",39,FontStyle.Bold,3,0,224,95,ContentAlignment.MiddleCenter);time.Font=new Font("Segoe UI",39,FontStyle.Bold);
   status=LabelAt(timerPage,"Ready when you are.",8,FontStyle.Regular,0,155,308,22,ContentAlignment.MiddleCenter);
   int presetX=4;foreach(int n in new[]{5,15,25}){int v=n;ButtonAt(timerPage,n+" min",presetX,180,96,35,()=>SetMinutes(v));presetX+=102;}
   LabelAt(timerPage,"Custom minutes",9,FontStyle.Regular,15,227,165,31,ContentAlignment.MiddleLeft);
   var duration=new SoftField();Place(duration,175,225,122,33);timerPage.Controls.Add(duration);minutes=new NumericUpDown{Minimum=1,Maximum=180,Value=pet.Clock.Minutes,BorderStyle=BorderStyle.None,BackColor=Color.FromArgb(255,253,249),ForeColor=ink,AccessibleName="Custom timer minutes",Font=Font};Place(minutes,11,6,101,24);duration.Controls.Add(minutes);
   minutes.ValueChanged+=(s,e)=>{if(pet.Clock.State==ClockState.Idle){pet.Clock.Reset((int)minutes.Value);pet.Save();RefreshState();}};
   primary=ButtonAt(timerPage,"Start focus",1,269,306,40,ToggleTimer);primary.Fill=blue;primary.Edge=Color.FromArgb(86,157,230);primary.Ink=Color.White;primary.Font=new Font("Comic Sans MS",12,FontStyle.Bold);primary.Glyph="\uf04b";primary.IconFont=iconFont;
   var reset=ButtonAt(timerPage,"Reset",109,314,90,24,()=>{pet.Clock.Reset((int)minutes.Value);pet.Save();RefreshState();});reset.Fill=paper;reset.Edge=paper;reset.Font=new Font("Comic Sans MS",9);
   LabelAt(taskPage,"One small thing at a time.",12,FontStyle.Bold,1,8,306,40,ContentAlignment.MiddleLeft);
   var entry=new SoftField();Place(entry,0,58,238,34);taskPage.Controls.Add(entry);input=new TextBox{BorderStyle=BorderStyle.None,MaxLength=200,BackColor=Color.FromArgb(255,253,249),ForeColor=ink,Font=Font,AccessibleName="New task"};Place(input,12,7,213,24);entry.Controls.Add(input);ButtonAt(taskPage,"Add",245,58,63,34,AddTask);input.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){AddTask();e.SuppressKeyPress=true;}};
   done=new CheckBox{Text="Show completed tasks",ForeColor=muted,BackColor=Color.Transparent,Font=new Font("Comic Sans MS",9)};Place(done,3,101,300,28);taskPage.Controls.Add(done);done.CheckedChanged+=(s,e)=>BuildTasks();
   list=new FlowLayoutPanel{FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,BackColor=Color.Transparent};Place(list,0,139,311,164);taskPage.Controls.Add(list);
   undo=ButtonAt(taskPage,"Undo last delete",72,312,164,30,()=>{if(deleted==null)return;pet.Data.Tasks.Insert(Math.Min(deletedAt,pet.Data.Tasks.Count),deleted);deleted=null;undo.Enabled=false;pet.Save();BuildTasks();});undo.Enabled=false;
   settingsPage.AutoScroll=true;LabelAt(settingsPage,"A little cat. A little company.",11,FontStyle.Bold,0,4,300,32,ContentAlignment.MiddleLeft);
   Setting("Gentle timer sound",pet.Data.Sound,42,b=>pet.Data.Sound=b);Setting("Reduced motion",pet.Data.ReducedMotion,73,b=>pet.Data.ReducedMotion=b);Setting("Pause roaming",pet.Data.PauseRoaming,104,b=>pet.Data.PauseRoaming=b);
   Setting("Show corner focus timer",pet.Data.ShowCounter,135,b=>pet.Data.ShowCounter=b);
   Setting("Remind me to look away every 20 min",pet.Data.EyeBreaks,166,b=>pet.Data.EyeBreaks=b);
   Setting("Stay visible over fullscreen apps",pet.Data.PinOverFullscreen,197,b=>pet.Data.PinOverFullscreen=b);
   LabelAt(settingsPage,"Try an expression",10,FontStyle.Bold,0,238,300,28,ContentAlignment.MiddleLeft);int i=0;foreach(string name in CatAnimation.States){string reaction=name;var b=ButtonAt(settingsPage,name,(i%3)*101,273+(i/3)*36,95,29,()=>pet.React(reaction));b.Font=new Font("Comic Sans MS",8);b.Enabled=name!="Walking"||CatAnimation.WalkCycleVerified;i++;}
   ButtonAt(settingsPage,"Hide cat",0,504,144,33,()=>pet.ToggleHidden());ButtonAt(settingsPage,"Quit boi_1",153,504,144,33,()=>pet.Quit());
   LabelAt(settingsPage,"Ctrl + Alt + B opens tools.\nYour tasks stay on this computer.",8,FontStyle.Regular,0,547,300,46,ContentAlignment.MiddleLeft);
   var credits=new LinkLabel{Text="Open-source credits",LinkColor=muted,BackColor=Color.Transparent,AutoSize=false,Font=new Font("Comic Sans MS",8)};Place(credits,0,601,300,28);settingsPage.Controls.Add(credits);credits.LinkClicked+=(s,e)=>System.Diagnostics.Process.Start("https://github.com/SuwajAryal/boi_1/blob/main/CREDITS.md");
   warning=LabelAt(this,"",7,FontStyle.Regular,76,484,248,22,ContentAlignment.MiddleCenter);
   warning.BringToFront();
   MouseDown+=BeginDrag;MouseMove+=MoveDrag;MouseUp+=(s,e)=>dragging=false;title.MouseDown+=BeginDrag;title.MouseMove+=MoveDrag;title.MouseUp+=(s,e)=>dragging=false;
   FormClosing+=(s,e)=>{if(e.CloseReason==CloseReason.UserClosing){e.Cancel=true;HideTools();}};KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Escape){HideTools();e.Handled=true;}};
   BuildTasks();SelectPage(0);RefreshState();
  }
  void BeginDrag(object sender,MouseEventArgs e){if(e.Button==MouseButtons.Left){dragging=true;dragPoint=new Point(Cursor.Position.X-Left,Cursor.Position.Y-Top);}}
  void MoveDrag(object sender,MouseEventArgs e){if(dragging)Location=new Point(Cursor.Position.X-dragPoint.X,Cursor.Position.Y-dragPoint.Y);}
  void HideTools(){Hide();deleted=null;undo.Enabled=false;}
  protected override void OnPaintBackground(PaintEventArgs e){e.Graphics.Clear(paper);e.Graphics.DrawImage(cloud,ClientRectangle);}
  internal void PaintBackdrop(Graphics g,Control child){g.Clear(paper);Point origin=PointToClient(child.PointToScreen(Point.Empty));g.DrawImage(cloud,new Rectangle(-origin.X,-origin.Y,ClientSize.Width,ClientSize.Height));}
  Panel Page(){var p=new Panel{BackColor=Color.Transparent};Place(p,46,153,310,348);Controls.Add(p);return p;}
  Label LabelAt(Control host,string text,float size,FontStyle style,int x,int y,int w,int h,ContentAlignment align){var l=new Label{Text=text,ForeColor=ink,BackColor=Color.Transparent,Font=new Font("Comic Sans MS",size,style),TextAlign=align};Place(l,x,y,w,h);host.Controls.Add(l);return l;}
  CloudButton ButtonAt(Control host,string text,int x,int y,int w,int h,Action action){var b=new CloudButton{Text=text,Font=new Font("Comic Sans MS",10),AccessibleName=text};Place(b,x,y,w,h);b.Click+=(s,e)=>action();host.Controls.Add(b);return b;}
  void Setting(string text,bool value,int y,Action<bool> update){var c=new CheckBox{Text=text,Checked=value,ForeColor=ink,BackColor=Color.Transparent,Font=Font};Place(c,3,y,297,28);settingsPage.Controls.Add(c);c.CheckedChanged+=(s,e)=>{update(c.Checked);pet.Save();};}
  void ToggleTimer(){if(pet.Clock.State==ClockState.Idle)pet.Clock.Start();else if(pet.Clock.State==ClockState.Running)pet.Clock.Pause();else if(pet.Clock.State==ClockState.Paused)pet.Clock.Resume();else{pet.Clock.Reset((int)minutes.Value);pet.React("Enjoying");}pet.Save();RefreshState();}
  void SetMinutes(int value){if(pet.Clock.State!=ClockState.Idle){MessageBox.Show(this,"Reset your current timer to change its duration.","boi_1");return;}minutes.Value=value;}
  void AddTask(){string title=input.Text.Trim();if(title.Length==0){input.Focus();return;}pet.Data.Tasks.Add(new Todo{Title=title});input.Clear();pet.Save();BuildTasks();input.Focus();pet.React("Enjoying");}
  void BuildTasks(){list.SuspendLayout();while(list.Controls.Count>0)list.Controls[0].Dispose();foreach(var item in pet.Data.Tasks.Where(t=>!t.Done||done.Checked).OrderBy(t=>t.Done).ToArray()){Todo task=item;var row=new Panel{Width=S(288),Height=S(58),Margin=new Padding(0,0,0,S(4)),BackColor=Color.Transparent};var check=new CheckBox{Checked=task.Done,AccessibleName="Complete "+task.Title,BackColor=Color.Transparent};Place(check,0,10,23,28);row.Controls.Add(check);check.CheckedChanged+=(s,e)=>{task.Done=check.Checked;pet.Save();if(task.Done)pet.React("Celebrating");BeginInvoke((Action)BuildTasks);};var label=LabelAt(row,task.Title,9,task.Done?FontStyle.Strikeout:FontStyle.Regular,28,0,163,55,ContentAlignment.MiddleLeft);label.AutoEllipsis=true;ButtonAt(row,"Edit",197,12,43,29,()=>{string title=EditTask(task.Title);if(title!=null){task.Title=title;pet.Save();BuildTasks();}}).Font=new Font("Comic Sans MS",8);ButtonAt(row,"Del",245,12,40,29,()=>{deleted=task;deletedAt=pet.Data.Tasks.IndexOf(task);pet.Data.Tasks.Remove(task);undo.Enabled=true;pet.Save();BuildTasks();}).Font=new Font("Comic Sans MS",8);list.Controls.Add(row);}if(list.Controls.Count==0){var empty=new Label{Text="One small thing at a time.",Width=S(288),Height=S(60),ForeColor=muted,Font=Font};list.Controls.Add(empty);}list.ResumeLayout();}
  string EditTask(string text){using(var f=new Form{Text="Edit task",ClientSize=new Size(S(340),S(108)),StartPosition=FormStartPosition.CenterParent,FormBorderStyle=FormBorderStyle.FixedDialog,MinimizeBox=false,MaximizeBox=false,BackColor=paper,Font=Font}){var box=new TextBox{Text=text,MaxLength=200,AccessibleName="Task title"};Place(box,12,14,314,27);f.Controls.Add(box);var ok=ButtonAt(f,"Save",230,58,96,33,()=>{if(box.Text.Trim().Length>0)f.DialogResult=DialogResult.OK;});f.AcceptButton=ok;return f.ShowDialog(this)==DialogResult.OK?box.Text.Trim():null;}}
  public void SelectPage(int index){selectedPage=index;timerPage.Visible=index==0;taskPage.Visible=index==1;settingsPage.Visible=index==2;timerTab.Fill=index==0?Color.FromArgb(206,230,255):Color.FromArgb(250,251,254);taskTab.Fill=index==1?Color.FromArgb(206,230,255):Color.FromArgb(250,251,254);gear.Fill=index==2?Color.FromArgb(206,230,255):Color.FromArgb(234,244,255);timerTab.Invalidate();taskTab.Invalidate();gear.Invalidate();}
  void Snapshot(string folder,string name){Refresh();Application.DoEvents();using(var b=new Bitmap(Width,Height)){DrawToBitmap(b,new Rectangle(0,0,Width,Height));for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++)if(!Region.IsVisible(x,y))b.SetPixel(x,y,Color.Transparent);b.Save(Path.Combine(folder,name));}}
  public void RunSmoke(string folder){pet.Clock.Reset(1);minutes.Value=1;RefreshState();primary.PerformClick();if(pet.Clock.State!=ClockState.Running)throw new Exception("Start failed");primary.PerformClick();if(pet.Clock.State!=ClockState.Paused)throw new Exception("Pause failed");primary.PerformClick();if(pet.Clock.State!=ClockState.Running)throw new Exception("Resume failed");Snapshot(folder,"timer.png");pet.Clock.Reset(25);minutes.Value=25;pet.Save();SelectPage(1);input.Text="Read a chapter";AddTask();input.Text="Water the plant";AddTask();var task=pet.Data.Tasks.Last();task.Done=true;pet.Save();done.Checked=true;BuildTasks();Snapshot(folder,"tasks.png");task.Title="Water the plant tomorrow";pet.Save();deleted=task;deletedAt=pet.Data.Tasks.IndexOf(task);pet.Data.Tasks.Remove(task);undo.Enabled=true;undo.PerformClick();if(!pet.Data.Tasks.Contains(task))throw new Exception("Undo failed");SelectPage(2);pet.React("Goofing");if(pet.Status!="Goofing")throw new Exception("Preview failed");pet.Data.ReducedMotion=true;pet.Save();Snapshot(folder,"settings.png");pet.Data.ReducedMotion=false;pet.Save();}
  public void RefreshState(){time.Text=pet.Clock.Display;primary.Text=pet.Clock.State==ClockState.Running?"Pause":pet.Clock.State==ClockState.Paused?"Resume":pet.Clock.State==ClockState.Finished?"Done - reset":"Start focus";primary.Glyph=pet.Clock.State==ClockState.Running?"\uf04c":"\uf04b";status.Text=pet.Clock.State==ClockState.Finished?"All done. Take a little stretch.":pet.Clock.State==ClockState.Running?"I'm focusing with you.":pet.Clock.State==ClockState.Paused?"Paused. No rush.":"Ready when you are.";minutes.Enabled=pet.Clock.State==ClockState.Idle;warning.ForeColor=pet.Storage.Warning.Length>0?Color.Firebrick:muted;warning.Text=pet.Storage.Warning;warning.Visible=warning.Text.Length>0;}
  protected override void Dispose(bool disposing){if(disposing){cloud.Dispose();cloudScaled.Dispose();iconFont.Dispose();icons.Dispose();}base.Dispose(disposing);}
 }
}
