using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
namespace Boi {
 sealed class Tools : Form {
  readonly Pet pet; TabControl tabs;Label time,status,warning;Button primary;NumericUpDown minutes;float uiScale=1;
  FlowLayoutPanel list;TextBox input;Todo deleted;int deletedAt;Button undo;CheckBox done;
  readonly Color ink=Color.FromArgb(76,58,55), peach=Color.FromArgb(248,218,196);
  public Tools(Pet p){pet=p;Text="boi_1 - a little company";ClientSize=new Size(390,560);MinimumSize=new Size(400,600);MaximumSize=new Size(600,850);StartPosition=FormStartPosition.Manual;BackColor=Color.FromArgb(255,249,240);ForeColor=ink;Font=new Font("Segoe UI",10);ShowInTaskbar=true;KeyPreview=true;AutoScaleMode=AutoScaleMode.Dpi;
   var title=new Label{Text="boi_1",Font=new Font("Segoe UI",21,FontStyle.Bold),Dock=DockStyle.Top,Height=56,Padding=new Padding(18,8,0,0)};Controls.Add(title);
   tabs=new TabControl{Dock=DockStyle.Fill,Padding=new Point(18,9)};var timer=new TabPage("Timer");var tasks=new TabPage("Tasks");var settings=new TabPage("Cat & settings");tabs.TabPages.AddRange(new[]{timer,tasks,settings});foreach(TabPage page in tabs.TabPages){page.BackColor=BackColor;page.Padding=new Padding(16);}Controls.Add(tabs);tabs.BringToFront();
   warning=new Label{Dock=DockStyle.Bottom,Height=48,ForeColor=Color.Firebrick,Padding=new Padding(12,3,12,0),Font=new Font("Segoe UI",8)};Controls.Add(warning);warning.BringToFront();
   var column=Column(timer);AddLabel(column,"One small focus session",11,35);
   time=AddLabel(column,"25:00",46,100);time.Font=new Font("Segoe UI",46,FontStyle.Bold);status=AddLabel(column,"Ready when you are.",10,32);
   var presets=new FlowLayoutPanel{Width=330,Height=45};foreach(int n in new[]{5,15,25}){int v=n;var b=Btn(n+" min",94);b.Click+=(s,e)=>SetMinutes(v);presets.Controls.Add(b);}column.Controls.Add(presets);
   var duration=new FlowLayoutPanel{Width=330,Height=42};duration.Controls.Add(new Label{Text="Custom minutes",AutoSize=true,Padding=new Padding(0,8,8,0)});minutes=new NumericUpDown{Minimum=1,Maximum=180,Value=pet.Clock.Minutes,Width=90,AccessibleName="Custom timer minutes"};duration.Controls.Add(minutes);column.Controls.Add(duration);
   minutes.ValueChanged+=(s,e)=>{if(pet.Clock.State==ClockState.Idle){pet.Clock.Reset((int)minutes.Value);pet.Save();RefreshState();}};
   primary=Btn("Start timer",316);primary.Height=45;primary.Click+=(s,e)=>{if(pet.Clock.State==ClockState.Idle)pet.Clock.Start();else if(pet.Clock.State==ClockState.Running)pet.Clock.Pause();else if(pet.Clock.State==ClockState.Paused)pet.Clock.Resume();else{pet.Clock.Reset((int)minutes.Value);pet.React("Enjoying");}pet.Save();RefreshState();};column.Controls.Add(primary);
   var reset=Btn("Reset",316);reset.Click+=(s,e)=>{pet.Clock.Reset((int)minutes.Value);pet.Save();RefreshState();};column.Controls.Add(reset);
   var tc=Column(tasks);var taskHint=AddLabel(tc,"One small thing at a time.",12,36);var entry=new FlowLayoutPanel{Width=330,Height=43};input=new TextBox{Width=237,MaxLength=200,AccessibleName="New task",Margin=new Padding(0,6,5,0)};entry.Controls.Add(input);var add=Btn("Add",70);entry.Controls.Add(add);tc.Controls.Add(entry);add.Click+=(s,e)=>AddTask();input.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){AddTask();e.SuppressKeyPress=true;}};
   done=new CheckBox{Text="Show completed tasks",AutoSize=true,Padding=new Padding(0,4,0,6)};done.CheckedChanged+=(s,e)=>BuildTasks();tc.Controls.Add(done);
   list=new FlowLayoutPanel{Width=330,Height=220,AutoScroll=true,FlowDirection=FlowDirection.TopDown,WrapContents=false};tc.Controls.Add(list);
   undo=Btn("Undo last delete",316);undo.Enabled=false;undo.Click+=(s,e)=>{if(deleted==null)return;pet.Data.Tasks.Insert(Math.Min(deletedAt,pet.Data.Tasks.Count),deleted);deleted=null;undo.Enabled=false;pet.Save();BuildTasks();};tc.Controls.Add(undo);
   var sc=Column(settings);AddLabel(sc,"A little cat. A little company.",12,40);
   Setting(sc,"Gentle timer sound",pet.Data.Sound,b=>pet.Data.Sound=b);
   Setting(sc,"Reduced motion",pet.Data.ReducedMotion,b=>pet.Data.ReducedMotion=b);
   Setting(sc,"Pause roaming",pet.Data.PauseRoaming,b=>pet.Data.PauseRoaming=b);
   AddLabel(sc,"Try an expression",11,32);var reactions=new FlowLayoutPanel{Width=330,Height=260};foreach(string name in CatAnimation.States){string reaction=name;var b=Btn(name,100);b.Click+=(s,e)=>pet.React(reaction);reactions.Controls.Add(b);}sc.Controls.Add(reactions);
   var hide=Btn("Hide cat",150);hide.Click+=(s,e)=>pet.ToggleHidden();var quit=Btn("Quit boi_1",150);quit.Click+=(s,e)=>pet.Quit();var actions=new FlowLayoutPanel{Width=330,Height=44};actions.Controls.Add(hide);actions.Controls.Add(quit);sc.Controls.Add(actions);
   AddLabel(sc,"Ctrl + Alt + B opens tools.\nRight-click the cat for quick controls.\nYour tasks stay on this computer.",9,65);
   var credits=new LinkLabel{Text="Open-source inspirations & credits",AutoSize=true,LinkColor=ink};credits.LinkClicked+=(s,e)=>System.Diagnostics.Process.Start("https://github.com/SuwajAryal/boi_1/blob/main/CREDITS.md");sc.Controls.Add(credits);
   FormClosing+=(s,e)=>{if(e.CloseReason==CloseReason.UserClosing){e.Cancel=true;Hide();ClearUndo();}};KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Escape){Hide();ClearUndo();e.Handled=true;}};
   var root=new TableLayoutPanel{Dock=DockStyle.Fill,RowCount=3,ColumnCount=1,Margin=new Padding(0),Padding=new Padding(0)};root.RowStyles.Add(new RowStyle(SizeType.Absolute,56));root.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.RowStyles.Add(new RowStyle(SizeType.Absolute,40));title.Dock=DockStyle.Fill;tabs.Dock=DockStyle.Fill;warning.Dock=DockStyle.Fill;root.Controls.Add(title,0,0);root.Controls.Add(tabs,0,1);root.Controls.Add(warning,0,2);Controls.Add(root);
   BuildTasks();RefreshState();AutoScaleDimensions=new SizeF(96,96);PerformAutoScale();using(var g=Graphics.FromHwnd(IntPtr.Zero))uiScale=g.DpiX/96f;MinimumSize=new Size((int)(400*uiScale),(int)(600*uiScale));MaximumSize=new Size((int)(600*uiScale),(int)(850*uiScale));ClientSize=new Size((int)(390*uiScale),(int)(560*uiScale));
  }
  FlowLayoutPanel Column(Control host){var col=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true};host.Controls.Add(col);return col;}
  Label AddLabel(Control host,string text,float size,int height){var l=new Label{Text=text,Width=320,Height=height,Font=new Font("Segoe UI",size),ForeColor=ink,Margin=new Padding(0,2,0,2)};host.Controls.Add(l);return l;}
  Button Btn(string text,int width){return new Button{Text=text,Width=width,Height=34,FlatStyle=FlatStyle.Flat,BackColor=peach,ForeColor=ink,Cursor=Cursors.Hand,Margin=new Padding(0,3,6,3),AccessibleName=text};}
  void Setting(Control c,string title,bool value,Action<bool> update){var b=new CheckBox{Text=title,Checked=value,AutoSize=true,Margin=new Padding(0,5,0,5)};b.CheckedChanged+=(s,e)=>{update(b.Checked);pet.Save();};c.Controls.Add(b);}
  void SetMinutes(int value){if(pet.Clock.State!=ClockState.Idle){MessageBox.Show(this,"Reset your current timer to change its duration.","boi_1");return;}minutes.Value=value;}
  void AddTask(){string title=input.Text.Trim();if(title.Length==0){input.Focus();return;}pet.Data.Tasks.Add(new Todo{Title=title});input.Clear();pet.Save();BuildTasks();input.Focus();pet.React("Enjoying");}
  void ClearUndo(){deleted=null;undo.Enabled=false;}
  void BuildTasks(){list.SuspendLayout();while(list.Controls.Count>0)list.Controls[0].Dispose();
   foreach(var item in pet.Data.Tasks.Where(t=>!t.Done||done.Checked).OrderBy(t=>t.Done).ToArray()){
    Todo task=item;var row=new FlowLayoutPanel{Width=306,Height=64,WrapContents=false,Margin=new Padding(0,2,0,2)};
    var check=new CheckBox{Checked=task.Done,Width=22,Height=45,AccessibleName="Complete "+task.Title};check.CheckedChanged+=(s,e)=>{task.Done=check.Checked;pet.Save();if(task.Done)pet.React("Celebrating");BeginInvoke((Action)BuildTasks);};row.Controls.Add(check);
    var label=new Label{Text=task.Title,Width=164,Height=56,Padding=new Padding(0,8,0,0),AutoEllipsis=true,Font=new Font("Segoe UI",10,task.Done?FontStyle.Strikeout:FontStyle.Regular)};new ToolTip().SetToolTip(label,task.Title);row.Controls.Add(label);
    var edit=Btn("Edit",48);edit.Click+=(s,e)=>{string title=EditTask(task.Title);if(title!=null){task.Title=title;pet.Save();BuildTasks();}};row.Controls.Add(edit);
    var del=Btn("Del",44);del.AccessibleName="Delete "+task.Title;del.Click+=(s,e)=>{deleted=task;deletedAt=pet.Data.Tasks.IndexOf(task);pet.Data.Tasks.Remove(task);undo.Enabled=true;pet.Save();BuildTasks();};row.Controls.Add(del);if(uiScale!=1)row.Scale(new SizeF(uiScale,uiScale));list.Controls.Add(row);
   }
   if(list.Controls.Count==0)list.Controls.Add(new Label{Text="No tasks here yet. Add one small thing.",Width=300,Height=60});list.ResumeLayout();
  }
  string EditTask(string text){using(var f=new Form{Text="Edit task",ClientSize=new Size(340,105),StartPosition=FormStartPosition.CenterParent,FormBorderStyle=FormBorderStyle.FixedDialog,MinimizeBox=false,MaximizeBox=false,BackColor=BackColor}){var box=new TextBox{Text=text,MaxLength=200,Left=12,Top=14,Width=314,AccessibleName="Task title"};var ok=Btn("Save",90);ok.Location=new Point(236,56);ok.Click+=(s,e)=>{if(box.Text.Trim().Length>0)f.DialogResult=DialogResult.OK;};f.Controls.Add(box);f.Controls.Add(ok);f.AcceptButton=ok;return f.ShowDialog(this)==DialogResult.OK?box.Text.Trim():null;}}
  public void SelectPage(int index){tabs.SelectedIndex=index;}
  void Snapshot(string folder,string name){Refresh();Application.DoEvents();using(var b=new Bitmap(Width,Height)){DrawToBitmap(b,new Rectangle(0,0,Width,Height));b.Save(Path.Combine(folder,name));}}
  public void RunSmoke(string folder){
   pet.Clock.Reset(1);minutes.Value=1;RefreshState();primary.PerformClick();if(pet.Clock.State!=ClockState.Running)throw new Exception("Start failed");primary.PerformClick();if(pet.Clock.State!=ClockState.Paused)throw new Exception("Pause failed");primary.PerformClick();if(pet.Clock.State!=ClockState.Running)throw new Exception("Resume failed");Snapshot(folder,"timer.png");pet.Clock.Reset(25);minutes.Value=25;pet.Save();
   SelectPage(1);input.Text="Read a chapter";AddTask();input.Text="Water the plant";AddTask();var task=pet.Data.Tasks.Last();task.Done=true;pet.Save();done.Checked=true;BuildTasks();Snapshot(folder,"tasks.png");
   task.Title="Water the plant tomorrow";pet.Save();deleted=task;deletedAt=pet.Data.Tasks.IndexOf(task);pet.Data.Tasks.Remove(task);undo.Enabled=true;undo.PerformClick();if(!pet.Data.Tasks.Contains(task))throw new Exception("Undo failed");
   SelectPage(2);pet.React("Goofing");if(pet.Status!="Goofing")throw new Exception("Preview failed");pet.Data.ReducedMotion=true;pet.Save();Snapshot(folder,"settings.png");pet.Data.ReducedMotion=false;pet.Save();
  }
  public void RefreshState(){time.Text=pet.Clock.Display;primary.Text=pet.Clock.State==ClockState.Running?"Pause":pet.Clock.State==ClockState.Paused?"Resume":pet.Clock.State==ClockState.Finished?"Done - reset":"Start timer";status.Text=pet.Clock.State==ClockState.Finished?"All done. Take a little stretch.":pet.Clock.State==ClockState.Running?"I'm focusing with you.":pet.Clock.State==ClockState.Paused?"Paused. No rush.":"Ready when you are.";minutes.Enabled=pet.Clock.State==ClockState.Idle;warning.Text=pet.Storage.Warning.Length>0?pet.Storage.Warning:pet.Clock.State==ClockState.Running?"Timer running  "+pet.Clock.Display+"  - keeps going when tools close.":"Click the cat for tools. Escape hides this window.";}
 }
}
