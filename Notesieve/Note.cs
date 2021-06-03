using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Notesieve
{
    class Note
    {
        protected int id;
        protected string name;
        protected string text;      
        protected int argbColor;
        protected DateTime regDate;
        protected DateTime updateDate;
        protected bool isDelete;
        protected Panel panel;
        protected static List<Note> noteList = new List<Note>();

        public string Name { get => name; set => name = value; }
        public string Text { get => text; set => text = value; }
        public int Id { get => id; set => id = value; }
        public DateTime RegDate { get => regDate; set => regDate = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public DateTime UpdateDate { get => updateDate; set => updateDate = value; }
        public int ARGBColor { get => argbColor; set => argbColor = value; }        
        public static List<Note> NoteList { get => noteList; set => noteList = value; }

        public Note()
        {
            
        }
        public static Note GetNoteById(int noteId)
        {
            foreach (Note n in noteList)
            {
                if (n.id == noteId) return n;
            }
            return null;
        }
        public void SetPanel(Panel panel) { this.panel = panel; }
        public Panel GetPanel() { return this.panel; }

    }
    class Alarm : Note
    {
        public delegate void AlarmHandler(Alarm alarm);
        public event AlarmHandler TimerTick;
        public event AlarmHandler AlarmRing;

        public bool isAlert = false;
        public DateTime alarmDate;

        public TimeSpan leftTime;


        public Alarm(DateTime alarmDate)
        {
            this.alarmDate = alarmDate;

            Timer myTimer = new Timer();
            myTimer.Tick += TimerEventProcessor;
            myTimer.Interval = 1000;
            myTimer.Start();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            if (leftTime >= TimeSpan.Zero)
            {
                TimerTick?.Invoke(this);
                leftTime = this.alarmDate - DateTime.Now;
                if (leftTime < TimeSpan.Zero)
                {
                    leftTime = TimeSpan.Zero;
                    if (!this.isAlert)
                    {
                        this.isAlert = true;
                        AlarmRing?.Invoke(this);
                    }
                }
            }
        }
    }
    class ToDoList : Note
    {
        private List<Label> taskLabels = new List<Label>();
        private List<NoteTask> tasks = new List<NoteTask>();

        public delegate void AnyTaskCompleteChangeHandler(ToDoList toDoList);
        public event AnyTaskCompleteChangeHandler AnyTaskCompleteChenge;
        public List<NoteTask> Tasks { get => tasks; }
       
        public ToDoList()
        {
            this.tasks = new List<NoteTask>();
        }

        public static ToDoList GetToDoListByTask(NoteTask task)
        {
            foreach(Note n in Note.NoteList)
            {
                if(n.GetType() == typeof(ToDoList))
                {
                    ToDoList tdl = n as ToDoList;
                    if (tdl.Tasks.Contains(task)) return tdl;
                }
            }
            return null;
        }

        public List<Label> GetTaskLabels()
        {
            return taskLabels;
        }
        
        public void SetTaskList(List<NoteTask> newTaskList)
        {
            if (tasks == null)
            {
                tasks = newTaskList;
                foreach (NoteTask t in tasks)
                {
                    t.CompleteChenge += TaskCompleteChenge;
                }
            }
            else
            {
                List<NoteTask> newAddedTasksList = new List<NoteTask>(); 
                foreach (NoteTask newTask in newTaskList)//Поиск новых записей, в новом наборе.
                {
                    bool found = false;
                    foreach (NoteTask oldTask in tasks)
                    {
                        if(newTask.Name == oldTask.Name)
                        {
                            found = true;
                            oldTask.Id = newTask.Id;
                            break;
                        }                        
                    }
                    if (!found)
                    {
                        newAddedTasksList.Add(newTask);
                    }
                }

                List<NoteTask> removedTasksList = new List<NoteTask>();
                foreach (NoteTask oldTask in tasks)//Поиск отсутствующих записей в новом наборе (для удаления)
                {
                    bool found = false;
                    foreach (NoteTask newTask in newTaskList)
                    {
                        if (newTask.Name == oldTask.Name)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        removedTasksList.Add(oldTask);
                    }
                }

                foreach (NoteTask removedTask in removedTasksList)
                {
                    removedTask.CompleteChenge -= TaskCompleteChenge;
                    tasks.Remove(removedTask);
                    
                }

                foreach (NoteTask addedTask in newAddedTasksList)
                {
                    tasks.Add(addedTask);
                    addedTask.CompleteChenge += TaskCompleteChenge;
                }

                tasks.Sort(delegate (NoteTask note1, NoteTask note2)
                {
                    return note1.Id.CompareTo(note2.Id);
                });
            }
        }

        private void TaskCompleteChenge(NoteTask t)
        {
            AnyTaskCompleteChenge?.Invoke(this);
        }

        public double GetCompletePercent()
        {
            double all = 0;
            double complete = 0;
            foreach(NoteTask t in tasks)
            {
                all++;
                if (t.IsComplete) complete++; 
            }
            if(all == 0)
            {
                return 100;
            }
            return (complete/all)*100;
        }
    }

    class Screenshot : Note
    {
        Image img = new Bitmap(Notesieve.Properties.Resources.notFound);
        string imgPath = "";
        public string ImgPath { get => imgPath; set => imgPath = value; }

        public Screenshot(Image screen, string path)
        {
            if (screen == null)
            {
                this.img = new Bitmap(Notesieve.Properties.Resources.notFound);
                this.imgPath = null;
            }
            else
            {
                this.img = screen;
                this.imgPath = path;
            }
        }

        public Image GetImg()
        {
            return img;
        }

        public void ChangeImageByPath(string path)
        {
            try
            {
                Image newImg = Image.FromFile(path);
                this.img = newImg;
                this.imgPath = path;
            }
            catch
            {
                this.img = new Bitmap(Notesieve.Properties.Resources.notFound);
                this.imgPath = null;
            }
        }
        
        public void Dispose()
        {
            img.Dispose();            
        }

        
    }

    class Common : Note
    {
        public Common()
        {
        
        }
    }
}
