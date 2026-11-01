using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Boi {
 public enum ClockState { Idle, Running, Paused, Finished }
 public class Todo { public string Id = Guid.NewGuid().ToString(); public string Title; public bool Done; }
 public class SaveData {
  public int Version = 1;
  public List<Todo> Tasks = new List<Todo>();
  public ClockState TimerState; public int Minutes = 25;
  public double Remaining; public DateTime DeadlineUtc;
  public bool Sound = true, ReducedMotion, PauseRoaming;
  public int HomeX = -1;
 }
 public sealed class FocusClock {
  public ClockState State; public int Minutes = 25;
  public double Remaining; public DateTime DeadlineUtc;
  readonly Stopwatch elapsed = new Stopwatch(); double startRemaining;
  public FocusClock(SaveData d) {
   Minutes = Math.Max(1, Math.Min(180, d.Minutes)); State = d.TimerState;
   DeadlineUtc = d.DeadlineUtc; Remaining = d.Remaining;
   if (State == ClockState.Running) {
    Remaining = Math.Max(0, (DeadlineUtc - DateTime.UtcNow).TotalSeconds);
    if(Remaining <= 0) State = ClockState.Finished; else BeginElapsed();
   }
   if(State == ClockState.Idle) Remaining = Minutes*60;
  }
  void BeginElapsed() { startRemaining=Remaining; elapsed.Restart(); }
  public void Start() { if(State!=ClockState.Idle) return; Remaining=Minutes*60; State=ClockState.Running; DeadlineUtc=DateTime.UtcNow.AddSeconds(Remaining); BeginElapsed(); }
  public bool Tick() { if(State!=ClockState.Running) return false; Remaining=Math.Max(0,startRemaining-elapsed.Elapsed.TotalSeconds); if(Remaining>0) return false; State=ClockState.Finished; elapsed.Stop(); return true; }
  public void Pause() { if(State!=ClockState.Running) return; Tick(); if(State==ClockState.Finished) return; State=ClockState.Paused; elapsed.Stop(); }
  public void Resume() { if(State!=ClockState.Paused) return; State=ClockState.Running; DeadlineUtc=DateTime.UtcNow.AddSeconds(Remaining); BeginElapsed(); }
  public void Reset(int minutes) { elapsed.Reset(); Minutes=Math.Max(1,Math.Min(180,minutes)); Remaining=Minutes*60; State=ClockState.Idle; }
  public void Export(SaveData d) { d.TimerState=State; d.Minutes=Minutes; d.Remaining=Remaining; d.DeadlineUtc=State==ClockState.Running?DateTime.UtcNow.AddSeconds(Remaining):DeadlineUtc; }
  public string Display { get { int s=(int)Math.Ceiling(Remaining); return (s/60).ToString("00")+":"+(s%60).ToString("00"); } }
 }
 public sealed class Store {
  public readonly string Folder; public string Warning=""; public bool Recovered;
  public Store(string folder) { Folder=folder; }
  public SaveData Load() {
   string recoveredPath=Path.Combine(Folder,"recovered-state.xml");
   if(File.Exists(recoveredPath)){try{var recovered=Read(recoveredPath);Recovered=true;Warning="Using recovered data; your original damaged file is preserved.";return recovered;}catch{}}
   string path=Path.Combine(Folder,"state.xml"); if(!File.Exists(path)) return new SaveData();
   try { return Read(path); } catch { }
   try { var data=Read(path+".bak"); Warning="Recovered your last saved backup. The damaged file is preserved."; Recovered=true; return data; }
   catch { Warning="Saved data could not be read. It has been preserved; new changes will be saved separately."; Recovered=true; return new SaveData(); }
  }
  static SaveData Read(string p) {
   using(var f=File.OpenRead(p)) {
    var d=(SaveData)new XmlSerializer(typeof(SaveData)).Deserialize(f);
    if(d.Version!=1 || d.Tasks==null || !Enum.IsDefined(typeof(ClockState),d.TimerState) || double.IsNaN(d.Remaining) || double.IsInfinity(d.Remaining) || d.Remaining<0 || d.Remaining>10800) throw new InvalidDataException();
    foreach(var t in d.Tasks) if(t==null || string.IsNullOrWhiteSpace(t.Title) || t.Title.Length>200) throw new InvalidDataException();
    return d;
   }
  }
  public bool Save(SaveData data) {
   try {
    Directory.CreateDirectory(Folder);
    string path=Path.Combine(Folder,Recovered?"recovered-state.xml":"state.xml");
    string temp=path+".tmp";
    using(var f=File.Create(temp)) { new XmlSerializer(typeof(SaveData)).Serialize(f,data); f.Flush(true); }
    if(File.Exists(path)) File.Replace(temp,path,path+".bak"); else File.Move(temp,path);
    if(!Recovered) Warning=""; return true;
   } catch(Exception ex) { Warning="Changes are in memory only. Could not save to local storage. "+ex.Message; return false; }
  }
 }
}
