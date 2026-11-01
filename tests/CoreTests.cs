using System;
using System.IO;
using System.Threading;
using Boi;
class CoreTests {
 static int count;
 static void Check(bool value,string name){if(!value)throw new Exception(name);count++;Console.WriteLine("PASS "+name);}
 static void Main(){
  var d=new SaveData();var c=new FocusClock(d);Check(c.Display=="25:00","initial duration");
  int min,max;RoamingLane.Bounds(0,1920,96,out min,out max);Check(min==1632&&max==1824,"rightmost 15 percent includes pet width");
  Check(FrameTiming.Index(.5,12,12,false)==6,"animation follows elapsed time");Check(FrameTiming.Index(1,12,12,false)==0,"walk loop wraps");Check(FrameTiming.Index(4,8,4,true)==0,"reduced motion freezes frames");
  RoamingLane.Bounds(-1920,1920,144,out min,out max);Check(min==-288&&max==-144,"offset monitor roaming bounds");
  RoamingLane.Bounds(0,400,144,out min,out max);Check(min==256&&max==256,"small screen lane never inverts");
  c.Reset(1);c.Start();Thread.Sleep(100);c.Pause();double remaining=c.Remaining;Thread.Sleep(50);c.Tick();Check(c.State==ClockState.Paused&&c.Remaining==remaining,"pause preserves remaining");
  c.Resume();Check(c.State==ClockState.Running,"resume");c.Reset(15);Check(c.State==ClockState.Idle&&c.Remaining==900,"reset");
  var expired=new FocusClock(new SaveData{TimerState=ClockState.Running,DeadlineUtc=DateTime.UtcNow.AddSeconds(-5),Remaining=5});Check(expired.State==ClockState.Finished&&!expired.Tick(),"expired restart is finished once");
  var live=new FocusClock(new SaveData{TimerState=ClockState.Running,DeadlineUtc=DateTime.UtcNow.AddSeconds(.04),Remaining=1});Thread.Sleep(80);Check(live.Tick()&&!live.Tick(),"single completion transition");
  var folder=Path.Combine(Path.GetTempPath(),"boi-tests-"+Guid.NewGuid().ToString("N"));var store=new Store(folder);
  try{d.Tasks.Add(new Todo{Title="Test task"});Check(store.Save(d),"first save: "+store.Warning);d.Tasks[0].Done=true;Check(store.Save(d),"atomic second save");Check(store.Load().Tasks[0].Done,"task round trip");
   File.WriteAllText(Path.Combine(folder,"state.xml"),"broken");var backup=store.Load();Check(backup.Tasks.Count==1&&!backup.Tasks[0].Done&&store.Recovered,"backup recovery");backup.Tasks[0].Title="Recovered edit";Check(store.Save(backup),"recovery save");Check(new Store(folder).Load().Tasks[0].Title=="Recovered edit","recovered data survives restart");
   var blocked=new Store(Path.Combine(folder,"state.xml","child"));Check(!blocked.Save(d)&&blocked.Warning.Length>0,"save error visible");
  }finally{Directory.Delete(folder,true);}
  Console.WriteLine(count+" checks passed.");
 }
}
