using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;//Убрать
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Notesieve
{
    class Worker
    {
        GSync gSync;
        SQLiteLoader dataLoader;
        string currentType = "Common";
        Color currentColor = Color.Green;
        bool cuurentTimerType = true;
        int currentEditMode = -1;      
        bool isLoign = false;
        bool isAlreadtDoingSync = false;
        string productName = "";
        string appPath = "";


        public delegate void SuccsesfullLogin(string name, string photoUrl);
        public delegate void FailedLogin();
        public delegate void LogoutHandler();
        public delegate void ErrorHandler(string error);

        public event SuccsesfullLogin onGoogleLoginSuccsesfull;
        public event FailedLogin onGoogleLoginFailed;
        public event LogoutHandler onGoogleLogout;
        public event ErrorHandler onGoogleSendError;


        public delegate void SyncHendler(GSync.gSyncState state);
        public event SyncHendler onSyncStateChanged;


        public Worker()
        {
            gSync = new GSync();
            
            gSync.OnError += OnGSyncRerturnError;
            gSync.OnLoginFailed += OnGSyncLoginFailed;
            gSync.OnLoginSuccsesfull += OnGSyncLoginSuccsesfull;
            gSync.OnLogout += OnGSynLogout;
            gSync.OnSyncStateChanged += OnGSyncStateChanged;

            Note.NoteList = new List<Note>();
            GetNoteList();

            productName = System.Windows.Forms.Application.ProductName;
            appPath = System.Windows.Forms.Application.StartupPath + "Notesieve.exe";
        }
        public List<Note> NoteList { get => Note.NoteList; }
        public string CurrentType { get => currentType; set => currentType = value; }
        public bool IsTimer { get => cuurentTimerType; set => cuurentTimerType = value; }
        public int EditMode { get => currentEditMode;}
        public bool IsLogin { get => isLoign; }


        //public Keys screenShotKey;
        //public Keys hideKey;

        //private bool isAdmin;
        //private bool isAutoStart;
        //private int opacity;
        //private bool isAutoLogin;

        //public bool IsAutoStart { get => isAutoStart; }
        //public bool IsAutoLogin { get => isAutoLogin; }
        //public bool IsAdmin { get => isAdmin; }
        //public int Opacity { get => opacity; set => opacity = value; }


        void OnGSynLogout()
        {
            isLoign = false;
            onGoogleLogout?.Invoke();
        }

        void OnGSyncRerturnError(string message)
        {
            onGoogleSendError?.Invoke(message);
        }

        void OnGSyncLoginFailed()
        {
            onGoogleLoginFailed?.Invoke();
        }

        void OnGSyncStateChanged(GSync.gSyncState state)
        {
            onSyncStateChanged?.Invoke(state);
        }

        void OnGSyncLoginSuccsesfull(string name, string url)
        {
            isLoign = true;
            onGoogleLoginSuccsesfull?.Invoke(name, url);
            DoSyncNow(true);

            TimerCallback tm = new TimerCallback(RepeatSync);
            int interval = 1000 * 60 *5;
            Timer timer = new Timer(tm, null, interval, interval);
        }

        void RepeatSync(object obj)
        {
            DoSyncNow(false);
        }

       

        public void DoSyncNow(bool doWithoutCheck)
        {
            if (isAlreadtDoingSync) return;

            isAlreadtDoingSync = true;
            if (doWithoutCheck)
            {
                gSync.DoSyncNow(this);
            }
            else
            {
                bool doesHaveUpdates = gSync.CheckForUpdates();
                if (doesHaveUpdates)
                {
                    gSync.DoSyncNow(this);
                }
                else
                {
                    onSyncStateChanged?.Invoke(GSync.gSyncState.eNotFound);
                }
            }
            isAlreadtDoingSync = false;
        }

        public void SetNoteColor(Color color)
        {
            currentColor = color;
        }

        public void ChengeEditMode(int mode)
        {
            currentEditMode = mode;
        }

        public void SwitchTaskStatus(NoteTask task)
        {
            task.IsComplete = !task.IsComplete;
            dataLoader.UpdateTask(task);
            Note updatedNote = ToDoList.GetToDoListByTask(task);
            if (updatedNote != null)
            {
                updatedNote.UpdateDate = DateTime.Now;
                gSync.UpdateNoteInTableAsync(updatedNote);
            }
        }

        public Note MakeScreenshot(int width, int height)
        {
            string prevType = CurrentType;
            CurrentType = "Screenshot";
            Bitmap BM = new Bitmap(width, height);
            Graphics GH = Graphics.FromImage(BM as Image);
            GH.CopyFromScreen(0, 0, 0, 0, new Size(width, height));

            DateTime now = DateTime.Now;
            string dateFormat = now.Day + "." + now.Month + "." + now.Year + "_" + now.Hour + "." + now.Minute + "." + now.Second + "." + now.Millisecond;
            string name = "ScreenShot (" + dateFormat + ")";

            Note newNote = AddNewNote(name, "", now, now.TimeOfDay, BM);
            CurrentType = prevType;

            return newNote;
        }

        
        public void UpdateSettings(string key, object value)
        {
            switch (key)
            {
                default:
                    onGoogleSendError?.Invoke("Не получается обновить настройки. Не существует ключа:" + key);
                    break;
                case "opacity":
                    NotesieveSettings.opacity = (int)value;
                    break;
                case "autoStart":
                    if (NotesieveSettings.isAdmin)
                    {
                        var regiserKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);

                        if (regiserKey.GetValue(productName) != null)
                        {
                            regiserKey.DeleteValue(productName);
                            NotesieveSettings.autoStart = false;
                        }
                        else
                        {                           
                            regiserKey.SetValue(productName, appPath);
                            NotesieveSettings.autoStart = true;
                        }
                    }
                    break;
                case "autoLogin":
                    NotesieveSettings.autoLogin = (bool)value;
                    break;
                case "keyHide":                    
                    NotesieveSettings.keyHide = value.ToString();
                    break;
                case "keyScreenShot":
                    NotesieveSettings.keyScreenShot = value.ToString();
                    break;
            }

            dataLoader.UpdateSettings();
        }

        

        public void LoadSettings()
        {
            dataLoader.LoadSettingsFromSQLite();

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                NotesieveSettings.isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (NotesieveSettings.isAdmin)
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);

                if (key.GetValue(productName) != null) NotesieveSettings.autoStart = true;
                else NotesieveSettings.autoStart = false;
            }

            
            if(NotesieveSettings.autoLogin)
            {
                MakeAutoLogin();
            }
            
        }

        public void ShowFullScreenScreenshot(Screenshot screen)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = screen.ImgPath;
                p.Start();
            }
            catch
            {

            }
        }

        public Note AddNewNote(string name, string text, DateTime dateTime, TimeSpan timeSpan, Image img = null)
        {
            string newText = text;            
            Note newNote;

            switch (currentType)
            {
                default:
                    newNote = dataLoader.SendDataToSQL(currentEditMode, name, newText, currentColor.ToArgb(), DateTime.Now, DateTime.Now);
                    break;
                case "Screenshot":
                    newNote = dataLoader.SendDataToSQL(currentEditMode, name, newText, currentColor.ToArgb(), DateTime.Now, DateTime.Now, img, true);
                    break;
                case "ToDoList":
                    string[] stringList = text.Split("*");
                    List<NoteTask> taskList = new List<NoteTask>();
                    newText = stringList[0].TrimEnd('\n');
                    for (int i = 1; i < stringList.Length; i++)
                    {
                         NoteTask newTask = new NoteTask(NoteTask.MaxID+1, stringList[i], false);
                         taskList.Add(newTask);
                    }
                    newNote = dataLoader.SendDataToSQL(currentEditMode, name, newText, currentColor.ToArgb(), DateTime.Now, DateTime.Now, taskList);
                    break;
                case "Alarm":
                    DateTime newDateTime;
                    if (cuurentTimerType)
                    {
                        newDateTime = DateTime.Now.Add(timeSpan);
                    }
                    else
                    {
                        newDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                    }

                    newNote = dataLoader.SendDataToSQL(currentEditMode, name, newText, currentColor.ToArgb(), DateTime.Now, DateTime.Now, newDateTime);
                    break;

            }

            if (currentEditMode < 0)
            {
                Note.NoteList.Add(newNote);
                gSync.AddNewNoteToTableAsync(newNote);
            }
            else
            {
                gSync.UpdateNoteInTableAsync(newNote);
            }
            return newNote;
        }

        void AddNoteFromGSync(int editMode, Note newNote)
        {
            string currentType = newNote.GetType().ToString().Split(".")[1];
            switch (currentType)
            {
                default:
                    newNote = dataLoader.SendDataToSQL(editMode, newNote.Name, newNote.Text, newNote.ARGBColor, newNote.RegDate, newNote.UpdateDate);
                    break;
                case "Screenshot":
                    Screenshot sc = newNote as Screenshot;
                    newNote = dataLoader.SendDataToSQL(editMode, sc.Name, sc.Text, sc.ARGBColor, sc.RegDate, sc.UpdateDate, sc.GetImg(), false);
                    break;
                case "ToDoList":
                    ToDoList td = newNote as ToDoList;
                    foreach (NoteTask t in td.Tasks)
                    {
                        t.GotNewID();
                    }
                    newNote = dataLoader.SendDataToSQL(editMode, td.Name, td.Text, td.ARGBColor, td.RegDate, td.UpdateDate, td.Tasks);
                    break;
                case "Alarm":
                    Alarm alarm = newNote as Alarm;
                    newNote = dataLoader.SendDataToSQL(editMode, alarm.Name, alarm.Text, alarm.ARGBColor, alarm.RegDate, alarm.UpdateDate, alarm.alarmDate);
                    break;
            }
            if (editMode < 0) Note.NoteList.Add(newNote);
        }

        public void CheckAndAdd(Note loadedNote)
        {
            bool found = false;
            if (loadedNote.IsDelete) return;
            foreach (Note n in Note.NoteList)
            {
                if (n.RegDate == loadedNote.RegDate)
                {
                    found = true;
                    if (loadedNote.UpdateDate > n.UpdateDate)
                    {
                        AddNoteFromGSync(n.Id, loadedNote);
                    }
                    break;
                }
            }
            if (found == false)
            {
                AddNoteFromGSync(-1, loadedNote);
            }
        }

        public async Task GoogleLogin()
        {
            if (isLoign == false)
            {
                await gSync.Login();
            }
            else
            {
                gSync.Logout();
            }
        }


        async void MakeAutoLogin()
        {
            await GoogleLogin();
        }



        public void DeleteNote(int noteId)
        {
            Note note = Note.GetNoteById(noteId);            
            string screenShotPath = dataLoader.DeleteFromSQL(note);
            gSync.UpdateNoteInTableAsync(note, screenShotPath);

        }



        private void GetNoteList()
        {
            dataLoader = new SQLiteLoader();
            Note.NoteList = dataLoader.LoadNotesFromSQLite();
        }

    }
}
