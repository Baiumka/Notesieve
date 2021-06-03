using System;
using System.Collections.Generic;
using System.Drawing;//Убрать
using System.Windows.Forms;

namespace Notesieve
{
    class EventManager
    {

        //Delegates
        public delegate void BoolEventHandler(bool arg);        
        public delegate void EventHandler();        
        public delegate void NoteEventHandler(Note note);        
        public delegate void EventWithSenderHendler(object sender);     
        public delegate void IntEventHandler(int arg);
        public delegate void applicationStartHandler(List<Note> noteList);
        public delegate void TaskHendler(Label task, bool isComplete);
        public delegate void SyncHendler(GSync.gSyncState state);

        public delegate void SuccsesfullLogin(string name, string photoUrl);
        public delegate void FailedLogin();
        public delegate void LogoutHandler();
        public delegate void ErrorHandler(string error);
        public delegate void GoogleHoverHandler(object sender, bool isLogin);

        //Events
        public event SyncHendler onSyncStateChanged;
        public event SuccsesfullLogin onGoogleLoginSuccsesfull;
        public event FailedLogin onGoogleLoginFailed;
        public event ErrorHandler onGoogleSendError;
        public event LogoutHandler onGoogleLogout;
        public event IntEventHandler onGoogleLoginDialogConfirmed;
        public event EventHandler onAddNewNoteButtonClicked;
        public event EventHandler onAppExited;
        public event EventHandler onGoogleLoginButtonClicked;
        public event BoolEventHandler onAutoStartSettingChanged;
        public event BoolEventHandler onAutoLoginSettingChanged;
        public event EventHandler onNewHideKeyHookedUp;
        public event EventHandler onNewScreenKeyHookedUp;
        public event EventHandler onSettingsButtonClicked;
        public event EventWithSenderHendler onAddNewNoteButtonMouseHovered;
        public event EventWithSenderHendler onAddNewNoteButtonMouseLeft;
        public event NoteEventHandler onNoteAdded;
        public event NoteEventHandler onNoteUpdated;
        public event EventWithSenderHendler onAddNoteTypeButtonMouseHovered;
        public event EventWithSenderHendler onAddNoteTypeButtonMouseLeft;
        public event BoolEventHandler onAddNoteCloseButtonClicked;
        public event EventWithSenderHendler onAddNoteCloseButtonMouseHovered;
        public event EventWithSenderHendler onAddNoteCloseButtonMouseLeft;
        public event IntEventHandler onNotePanelScrolled;
        public event IntEventHandler onOpacityChanged;
        public event applicationStartHandler onApplicationStarted;
        public event EventWithSenderHendler onDeleteBoxButtonClicked;
        public event NoteEventHandler onEditBoxButtonClicked;
        public event EventWithSenderHendler onNoteTypeChanged;
        public event EventWithSenderHendler onNoteColorPanelClicked;
        public event EventWithSenderHendler onNoteColorPanelMouseHovered;
        public event EventWithSenderHendler onNoteColorPanelMouseLeft;
        public event EventWithSenderHendler onAlarmTypeRadioCheckedChanged;
        public event TaskHendler onTaskLabelClicked;
        public event EventWithSenderHendler onTaskLabelMouseHovered;
        public event EventWithSenderHendler onTaskLabelMouseLeft;
        public event EventWithSenderHendler onGoogleSyncButtonMouseHovered;
        public event EventWithSenderHendler onGoogleSyncButtonMouseLeft;
        public event GoogleHoverHandler onGoogleLoginButtonMouseHovered;
        public event GoogleHoverHandler onGoogleLoginButtonMouseLeft;
        public event EventWithSenderHendler onTopFilterButtonClicked;
        public event EventWithSenderHendler onTopMenuItemMouseHovered;
        public event EventWithSenderHendler onTopFilterButtonMouseLeft;
        public event EventHandler onAppHidden;
        public event EventHandler onHelperButtonClicked;
        public event EventWithSenderHendler onAppCloseButtonMouseLeft;
        public event EventWithSenderHendler onBigAddButtonMouseLeft;
        public event EventWithSenderHendler onTopMenuItemMouseLeft;
        
        Worker worker;
        private globalKeyboardHook hideKeyHook = new globalKeyboardHook();
        private globalKeyboardHook screenShotKeyHook = new globalKeyboardHook();
        private Keys hideKey;
        private Keys screenShotKey;

        public EventManager()
        {
            worker = new Worker();

            worker.onGoogleSendError += GSyncRerturnError;
            worker.onGoogleLoginFailed += GSyncLoginFailed;
            worker.onGoogleLoginSuccsesfull += GSyncLoginSuccsesfull;
            worker.onGoogleLogout += GSyncLogout;
            worker.onSyncStateChanged += GSyncStateChanged;
        }

        void GSyncStateChanged(GSync.gSyncState state)
        {
            onSyncStateChanged?.Invoke(state);
        }

        void GSyncRerturnError(string message)
        {
            onGoogleSendError?.Invoke(message);
        }

        void GSyncLogout()
        {
            onGoogleLogout?.Invoke();
        }

        void GSyncLoginFailed()
        {
            onGoogleLoginFailed?.Invoke();
        }

        void GSyncLoginSuccsesfull(string name, string url)
        {
            onGoogleLoginSuccsesfull?.Invoke(name, url);
        }

        public void Application_Start()
        {
            worker.LoadSettings();

            hideKeyHook.KeyUp += HideKeyButton_Press;
            screenShotKeyHook.KeyUp += ScreenshotKey_Press;

            Keys keys;

            Enum.TryParse(NotesieveSettings.keyHide, out keys);
            hideKey = keys;
            hideKeyHook.HookedKeys.Add(hideKey);


            Enum.TryParse(NotesieveSettings.keyScreenShot, out keys);
            screenShotKey = keys;
            screenShotKeyHook.HookedKeys.Add(screenShotKey);

            onApplicationStarted?.Invoke(worker.NoteList);
        }

        public void CommonNoteTextLabel_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Clipboard.SetText(c.Text);
        }

        public void GoogleLoginButton_Click(object sender, EventArgs e)
        {
            onGoogleLoginButtonClicked?.Invoke();
        }

        public async void GoogleLoginDialog_Confirm()
        {
            onGoogleLoginDialogConfirmed?.Invoke(1);
            await worker.GoogleLogin();
        }

        public void AutoStartSetting_Click(object sender, EventArgs e)
        {
            worker.UpdateSettings("autoStart", !NotesieveSettings.autoStart);
            onAutoStartSettingChanged?.Invoke(NotesieveSettings.autoStart);      
        }

        public void AutoLoginSetting_Click(object sender, EventArgs e)
        {            
            worker.UpdateSettings("autoLogin", !NotesieveSettings.autoLogin);
            onAutoLoginSettingChanged?.Invoke(NotesieveSettings.autoLogin);
        }

        public void NotesPanel_Scroll(object sender, MouseEventArgs e)
        {            
            onNotePanelScrolled?.Invoke(e.Delta);
        }

        public void OpTrackBar_ValueChanged(object sender, EventArgs e)
        {            
            TrackBar tb = sender as TrackBar;            
            worker.UpdateSettings("opacity", tb.Value);
            onOpacityChanged?.Invoke(NotesieveSettings.opacity);
        }

        public void HideKey_HookedUp(object sender, KeyEventArgs e)
        {
            hideKeyHook.HookedKeys.Remove(hideKey);
            hideKey = e.KeyData;            
            hideKeyHook.HookedKeys.Add(hideKey);
            
            worker.UpdateSettings("keyHide", hideKey.ToString());
            onNewHideKeyHookedUp?.Invoke();            
        }

        public void ScreenKey_HookedUp(object sender, KeyEventArgs e)
        {
            screenShotKeyHook.HookedKeys.Remove(screenShotKey);
            screenShotKey = e.KeyCode;
            screenShotKeyHook.HookedKeys.Add(screenShotKey);
            
            worker.UpdateSettings("keyScreenShot", screenShotKey.ToString());
            onNewScreenKeyHookedUp?.Invoke();
        }


        public void GoogleLoginButton_Hover(object sender, EventArgs e)
        {            
            onGoogleLoginButtonMouseHovered?.Invoke(sender, worker.IsLogin);
        }


        public void GoogleLoginButton_Leave(object sender, EventArgs e)
        {
            onGoogleLoginButtonMouseLeft?.Invoke(sender, worker.IsLogin);
        }


        public void SettingsButton_Click(object sender, EventArgs e)
        {
            onSettingsButtonClicked?.Invoke();
        }
        
        public void TopMenuItem_Hover(object sender, EventArgs e)
        {
            onTopMenuItemMouseHovered?.Invoke(sender);
        }
        public void TopMenuItem_Leave(object sender, EventArgs e)
        {
            onTopMenuItemMouseLeft?.Invoke(sender);
        }

        public void GoogleSyncButton_Hover(object sender, EventArgs e)
        {
            onGoogleSyncButtonMouseHovered?.Invoke(sender);
        }
        public void GoogleSyncButton_Leave(object sender, EventArgs e)
        {
            onGoogleSyncButtonMouseLeft?.Invoke(sender);
        }

        public void AppCloseButton_Leave(object sender, EventArgs e)
        {
            onAppCloseButtonMouseLeft?.Invoke(sender);
        }
        public void BigAddButton_Leave(object sender, EventArgs e)
        {
            onBigAddButtonMouseLeft?.Invoke(sender);
        }

        public void TopFilterButton_Leave(object sender, EventArgs e)
        {
            onTopFilterButtonMouseLeft?.Invoke(sender);
        }
        public void TopFilterButton_Click(object sender, EventArgs e)
        {
            onTopFilterButtonClicked?.Invoke(sender);
        }


        
        public void HelperButton_Click(object sender, EventArgs e)
        {
            onHelperButtonClicked?.Invoke();
        }

        public void HideButton_Click(object sender, EventArgs e)
        {
            onAppHidden?.Invoke();
        }

        public void Tray_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                onAppHidden?.Invoke();
            }
        }

        public void AppCloseButton_Click(object sender, EventArgs e)
        {
            onAppExited?.Invoke();
            Application.Exit();
        }

        public void HideKeyButton_Press(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == hideKey)
            {
                onAppHidden?.Invoke();
            }

        }

        public void ScreenshotKey_Press(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == screenShotKey)
            {
                Note newNote = worker.MakeScreenshot(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                
                if (worker.EditMode < 0) onNoteAdded?.Invoke(newNote);
                else onNoteUpdated?.Invoke(newNote);
                
            }

        }
        public void TaskLabel_Click(object sender, EventArgs e)
        {
            Label taskLabel = sender as Label;
            int id = Convert.ToInt32(taskLabel.Name.Split("_")[1]);
            NoteTask t = NoteTask.GetTaskById(id);
            if (t == null) return;
            worker.SwitchTaskStatus(t);
            onTaskLabelClicked?.Invoke(taskLabel, t.IsComplete);            
        }
        public void TaskLabel_Hover(object sender, EventArgs e)
        {
            onTaskLabelMouseHovered?.Invoke(sender);
        }
        public void TaskLabel_Leave(object sender, EventArgs e)
        {
            onTaskLabelMouseLeft?.Invoke(sender);
        }

        public void ChangeNoteType_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            worker.CurrentType = control.Name;
            onNoteTypeChanged?.Invoke(sender);
        }

        public void AlarmTypeRadioButton_CheckedChange(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                worker.IsTimer = !worker.IsTimer;              
                onAlarmTypeRadioCheckedChanged?.Invoke(sender);
            }

        }

        public void DeleteBoxButton_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            int noteId = Convert.ToInt32(control.Parent.Name.Split('_')[1]);
            worker.DeleteNote(noteId);
            onDeleteBoxButtonClicked?.Invoke(sender);
        }

        public void EditBoxButton_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            int noteId = Convert.ToInt32(control.Parent.Name.Split('_')[1]);
            worker.ChengeEditMode(noteId);
            Note editedNote = Note.GetNoteById(noteId);
            worker.SetNoteColor(Color.FromArgb(editedNote.ARGBColor));
            onEditBoxButtonClicked?.Invoke(editedNote);
        }

        public void CopyImgButton_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            int noteId = Convert.ToInt32(control.Parent.Name.Split('_')[1]);
            Screenshot screen = Note.GetNoteById(noteId) as Screenshot;
            Clipboard.SetImage(screen.GetImg());
        }

        public void FullScreenImgButton_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            int noteId = Convert.ToInt32(control.Parent.Name.Split('_')[1]);
            Screenshot screen = Note.GetNoteById(noteId) as Screenshot;
            worker.ShowFullScreenScreenshot(screen);     
        }
        public void AddNewNoteButton_Click(object sender, EventArgs e)
        {
            worker.ChengeEditMode(-2);
            onAddNewNoteButtonClicked?.Invoke();
        }
        public void AddNewNoteButton_MouseLeave(object sender, EventArgs e)
        {
            onAddNewNoteButtonMouseLeft?.Invoke(sender);
        }

        public void AddNewNoteButton_MouseHover(object sender, EventArgs e)
        { 
            onAddNewNoteButtonMouseHovered?.Invoke(sender);
        }


        public void NoteColor_Click(object sender, EventArgs e)
        {
            Panel box = sender as Panel;
            Color color = box.BackColor;
            worker.SetNoteColor(color);
            onNoteColorPanelClicked?.Invoke(sender);
        }

        public void NoteColor_MouseLeave(object sender, EventArgs e)
        {
            onNoteColorPanelMouseLeft?.Invoke(sender);
        }
        public void NoteColor_MouseHover(object sender, EventArgs e)
        {
            onNoteColorPanelMouseHovered?.Invoke(sender);
        }

        public void AddNote_Click(object sender, EventArgs e)
        {
            Button pb = sender as Button;
            string name = "";
            string text = "";
            DateTime dateTime = new DateTime();
            TimeSpan timeSpan = new TimeSpan();

            foreach (object contol in pb.Parent.Controls)
            {
                if (contol.GetType() == typeof(TextBox))
                {
                    TextBox tb = contol as TextBox;
                    switch (tb.Name)
                    {
                        case "AddNoteNameTextBox":
                            name = tb.Text;
                            break;
                        case "AddNoteTextTextBox":
                            text = tb.Text;
                            break;
                    }
                }

               
                if (contol.GetType() == typeof(DateTimePicker))
                {
                    DateTimePicker dtp = contol as DateTimePicker;
                    switch (dtp.Name)
                    {
                        case "DatePicker":
                            dateTime = dtp.Value;
                            break;
                        case "TimePicker":
                            dtp.Enabled = false;
                            dtp.Enabled = true;

                            TimeSpan time = dtp.Value.TimeOfDay;
                            timeSpan = time;                            
                            break;
                    }
                }
            }
            Note newNote = worker.AddNewNote(name, text, dateTime, timeSpan);
            if (worker.EditMode < 0) onNoteAdded?.Invoke(newNote);
            else onNoteUpdated?.Invoke(newNote);
        }
        public void AddNoteTypeButton_Leave(object sender, EventArgs e)
        {
            onAddNoteTypeButtonMouseLeft?.Invoke(sender);
        }
        public void AddNoteTypeButton_Hover(object sender, EventArgs e)
        {
            onAddNoteTypeButtonMouseHovered?.Invoke(sender);
        }
        public void GoogleSyncButton_Click(object sender, EventArgs e)
        {
            worker.DoSyncNow(true);            
        }
        public void AddNoteCloseButton_Click(object sender, EventArgs e)
        {
            worker.ChengeEditMode(-1);
            onAddNoteCloseButtonClicked?.Invoke(false);
        }
        public void AddNoteCloseButton_Leave(object sender, EventArgs e)
        {
            onAddNoteCloseButtonMouseLeft?.Invoke(sender);
        }
        public void AddNoteCloseButton_Hover(object sender, EventArgs e)
        {
            onAddNoteCloseButtonMouseHovered?.Invoke(sender);
        }
    }
}
