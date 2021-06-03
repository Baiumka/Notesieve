using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Notesieve
{
    class Drawer
    {
        ElementStorage eStore;
        int windowOpacity;
        string userLogin = "UserLogin";
        

        public Drawer(MainForm form)
        {
            
            eStore = new ElementStorage(form);
            eStore.eventManager.onAddNewNoteButtonClicked += OpenAddWindow;
            eStore.eventManager.onAddNewNoteButtonMouseHovered += BackColorGray;
            eStore.eventManager.onAddNewNoteButtonMouseLeft += BackColorClean;
            eStore.eventManager.onNoteAdded += AddNewNote;
            eStore.eventManager.onNoteUpdated += UpdateNote;
            eStore.eventManager.onAddNoteTypeButtonMouseHovered += BackColorHover;
            eStore.eventManager.onAddNoteTypeButtonMouseLeft += BackColorClean;
            eStore.eventManager.onGoogleLoginButtonClicked += ShowLoginDialog;
            eStore.eventManager.onGoogleLoginDialogConfirmed += ChangeLoginState;
            eStore.eventManager.onNoteColorPanelClicked += SelectNewColorFromPanel;
            eStore.eventManager.onNoteColorPanelMouseHovered += ShowColorBorder;
            eStore.eventManager.onNoteColorPanelMouseLeft += HideColorBorder;
            eStore.eventManager.onAddNoteCloseButtonClicked += ShowAddNewNotePanel;
            eStore.eventManager.onAddNoteCloseButtonMouseHovered += BackColorHover;
            eStore.eventManager.onAddNoteCloseButtonMouseLeft += BackColorClean;
            eStore.eventManager.onNotePanelScrolled += ScrollNotePanel;
            eStore.eventManager.onApplicationStarted += FirstDisplayNotes;
            eStore.eventManager.onDeleteBoxButtonClicked += DeleteBox;
            eStore.eventManager.onTaskLabelClicked += SwitchComplete;
            eStore.eventManager.onTaskLabelMouseHovered += LightUpTask;
            eStore.eventManager.onTaskLabelMouseLeft += LightDownTask;
            eStore.eventManager.onNoteTypeChanged += ChengeType;
            eStore.eventManager.onAlarmTypeRadioCheckedChanged += AlarmChangeType;
            eStore.eventManager.onEditBoxButtonClicked += OpenEditWindow;
            eStore.eventManager.onTopFilterButtonClicked += ChengeFilter;
            eStore.eventManager.onTopMenuItemMouseHovered += FilterHover;
            eStore.eventManager.onTopFilterButtonMouseLeft += FilterLeave;
            eStore.eventManager.onAppHidden += ChengeFormVisible;
            eStore.eventManager.onAppExited += HideNotifyIcon;
            eStore.eventManager.onOpacityChanged += OpacityChange;
            eStore.eventManager.onNewScreenKeyHookedUp += ChangeScreenShotButton;
            eStore.eventManager.onNewHideKeyHookedUp += ChangeHideButton;
            eStore.eventManager.onSettingsButtonClicked += SettingsChangeVisible;
            eStore.eventManager.onAutoStartSettingChanged += ChangeAutoStartState;
            eStore.eventManager.onAutoLoginSettingChanged += ChangeAutoLoginState;
            eStore.eventManager.onHelperButtonClicked += ShowHelper;
            eStore.eventManager.onTopMenuItemMouseLeft += BackColorWhite;
            eStore.eventManager.onAppCloseButtonMouseLeft += BackColorDarkRed;
            eStore.eventManager.onBigAddButtonMouseLeft += BackColorTrans;
            eStore.eventManager.onGoogleSendError += OnGSyncRerturnError;
            eStore.eventManager.onGoogleLoginSuccsesfull += OnGSyncLoginSuccsesfull;
            eStore.eventManager.onGoogleLoginFailed += OnGSyncLoginFailed;
            eStore.eventManager.onGoogleLogout += OnGSyncLogout;
            eStore.eventManager.onGoogleLoginButtonMouseHovered += OnGoogleHover;
            eStore.eventManager.onGoogleLoginButtonMouseLeft += OnGoogleLeave;
            eStore.eventManager.onGoogleSyncButtonMouseHovered += OnGSyncHover;
            eStore.eventManager.onGoogleSyncButtonMouseLeft += OnGSyncLeave;
            eStore.eventManager.onSyncStateChanged += OnGSyncStateChanged;

            
        }

        void OnGSyncStateChanged(GSync.gSyncState state)
        {
            string toolTipText = "";
            switch (state)
            {
            
                case GSync.gSyncState.eInWork:
                    eStore.gSyncBackColor = Color.Orange;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    break;
                case GSync.gSyncState.eSuccessPut:
                    eStore.gSyncBackColor = Color.LightGreen;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    break;
                case GSync.gSyncState.eSuccessGet:
                    eStore.gSyncBackColor = Color.LightGreen;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    DisplayNotes(Note.NoteList);
                    break;
                case GSync.gSyncState.eFailed:
                    eStore.gSyncBackColor = Color.Red;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    break;
                case GSync.gSyncState.eNotFound:
                    eStore.gSyncBackColor = Color.LightGreen;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    break;
                case GSync.gSyncState.eChecking:
                    eStore.gSyncBackColor = Color.LightBlue;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    break;
                case GSync.gSyncState.eLostConnection:
                    eStore.gSyncBackColor = Color.Red;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.disconnect);
                    break;
                default:
                    eStore.gSyncBackColor = Color.White;
                    eStore.googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
                    break;
            }
            eStore.googleSyncImage.BackColor = eStore.gSyncBackColor;

            
        }
        void OnGSyncHover(object sender)
        {
            Control control = sender as Control;
            control.BackColor = Color.Gray;
        }
        void OnGSyncLeave(object sender)
        {
            Control control = sender as Control;
            control.BackColor = eStore.gSyncBackColor;
        }
        void OnGoogleHover(object sender, bool isLogin)
        {
            Control label = sender as Control;
            if(isLogin)
            {
                label.BackColor = Color.DarkRed;
                label.Text = "Выйти из учётной записи.";
            }
            else
            {
                label.BackColor = Color.Gray;
            }

        }
        void OnGoogleLeave(object sender, bool isLogin)
        {
            Control label = sender as Control;
            if (isLogin)
            {
                label.Text = userLogin;
                label.BackColor = Color.Black;
            }
            else
            {
                label.BackColor = Color.White;
            }

        }

        void OnGSyncRerturnError(string message)
        {
            NoteDialog noteDialog = new NoteDialog(message, NoteDialog.NoteDialogType.dError);
            noteDialog.ShowDialog();
        }

        void OnGSyncLoginFailed()
        {
            ChangeLoginState(2);
        }

        void OnGSyncLogout()
        {
            eStore.googleLoginImage.Location = new System.Drawing.Point(ElementStorage.WINDOW_PADDING + 64, 32);
            eStore.googleLoginImage.Size = new System.Drawing.Size(32, 32);
            eStore.googleSyncImage.Visible = false;

            eStore.googleLoginImage.Image = new Bitmap(Notesieve.Properties.Resources.google_small);
            ChangeLoginState(0);
        }

        void OnGSyncLoginSuccsesfull(string name, string url)
        {
            userLogin = name;
            eStore.googleLoginButton.Text = name;
            eStore.googleLoginImage.ImageLocation = url;
            eStore.googleLoginImage.Location = new System.Drawing.Point(ElementStorage.WINDOW_PADDING + 96, 0);
            eStore.googleLoginImage.Size = new System.Drawing.Size(64, 64);

            eStore.googleLoginButton.Location = new System.Drawing.Point(ElementStorage.WINDOW_PADDING, 64);
            eStore.googleLoginButton.Size = new System.Drawing.Size(ElementStorage.WINDOW_WIDHT - ElementStorage.WINDOW_PADDING * 2, 32);
            eStore.googleLoginButton.ForeColor = Color.White;
            eStore.googleLoginButton.BackColor = Color.Black;

            eStore.googleSyncImage.Visible = true;            
        }
        void ShowLoginDialog()
        {
            string dialogText = "Приложение Notesieve будет использовать ваше пространство на Google Drive для хранения ваших данных. " +
                "Это позвозлит вам получить доступ к вашим записям с любого устройства, где установлен Notesieve.\n\n" +
                "У вас будет создана таблица с именем 'NotesiveAppTable', пожалуйста не редактируйте её самостоятельно либо при помощи внешних приложений, " +
                "иначе ваши данные могут быть утеряны.\n\n" +
                "Вы согласны?";
            NoteDialog noteDialog = new NoteDialog(dialogText, NoteDialog.NoteDialogType.dConfirm);
            if (noteDialog.ShowDialog() == DialogResult.OK)
            {
                eStore.eventManager.GoogleLoginDialog_Confirm();
            }
        }
        void ChangeLoginState(int state)
        {
            eStore.googleLoginButton.ForeColor = Color.Black;
            eStore.googleLoginButton.BackColor = Color.White;

            eStore.googleLoginButton.Location = new System.Drawing.Point(ElementStorage.WINDOW_PADDING + 96, 32);
            eStore.googleLoginButton.Size = new System.Drawing.Size(64, 32);

            eStore.googleSyncImage.Visible = false;
            switch (state)
            {
                case 0:
                    eStore.googleLoginButton.Text = "Войти";
                    eStore.googleLoginButton.Size = new System.Drawing.Size(64, 32);
                    break;
                case 1:
                    eStore.googleLoginButton.Text = "Подключение";
                    eStore.googleLoginButton.Size = new System.Drawing.Size(128, 32);
                    break;
                case 2:
                    eStore.googleLoginButton.Text = "Ошибка";
                    eStore.googleLoginButton.Size = new System.Drawing.Size(64, 32);
                    break;               
            }

        }


        void ShowHelper()
        {
            HelperForm help = new HelperForm();
            help.Show();
        }

        void ChangeAutoStartState(bool isAutoStart)
        {
            if (isAutoStart)
            {
                eStore.autoStartButton.Image = new Bitmap(Notesieve.Properties.Resources.v);
            }
            else
            {
                eStore.autoStartButton.Image = new Bitmap(Notesieve.Properties.Resources.x);
            }

        }

        void ChangeAutoLoginState(bool isLoginStart)
        {
            if (isLoginStart)
            {
                eStore.autoLoginButton.Image = new Bitmap(Notesieve.Properties.Resources.v);
            }
            else
            {
                eStore.autoLoginButton.Image = new Bitmap(Notesieve.Properties.Resources.x);
            }

        }

        void SettingsChangeVisible()
        {
            eStore.settingPanel.Visible = !eStore.settingPanel.Visible;
            if(eStore.settingPanel.Visible)
            {
                eStore.settingsPictureBox.BackColor = Color.Gray;
            }
            else
            {
                eStore.settingsPictureBox.BackColor = Color.White;
            }
        }

        void ChangeHideButton()
        {
            eStore.hideButtonButton.Text = NotesieveSettings.keyHide.ToString();
            eStore.minLabel.Text = NotesieveSettings.keyHide.ToString();
        }

        void ChangeScreenShotButton()
        {
            eStore.screenButtonButton.Text = NotesieveSettings.keyScreenShot.ToString();
        }

        void OpacityChange(int op)
        {
            windowOpacity = op;
            eStore.drawingForm.Opacity = (float)windowOpacity / 100;            
        }

        void HideNotifyIcon()
        {
            eStore.drawingForm.HideIcon();
        }

        void ChengeFormVisible()
        {
            if (eStore.drawingForm.Visible)
            {
                eStore.drawingForm.Hide();
                eStore.drawingForm.notifyIcon.ContextMenuStrip.Items[1].Text = "Развернуть";
            }
            else
            {
                eStore.ShowForm();
            }

        }

        

        public void DrawStart()
        {
            //drawingForm.TransparencyKey = Color.Magenta;
            //drawingForm.BackColor = Color.Magenta;
            eStore.DrawRightPanel();
            eStore.DrawAddButton();
            eStore.eventManager.Application_Start();
        }
       

        private void OpTrackBar_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FirstDisplayNotes(List<Note> notesList)
        {
            if (!NotesieveSettings.isAdmin)
            {
                eStore.autoStartButton.Enabled = false;

                eStore.autoStartLabel.Text = "Запускать Notesieve вместе с Windows\nТребуються Права Администратора!";
                eStore.autoStartLabel.ForeColor = Color.DarkRed;
            }
            if (NotesieveSettings.autoStart)
            {
                eStore.autoStartButton.Image = new Bitmap(Notesieve.Properties.Resources.v);
            }
            else
            {
                eStore.autoStartButton.Image = new Bitmap(Notesieve.Properties.Resources.x);
            }

            if (NotesieveSettings.autoLogin)
            {
                eStore.autoLoginButton.Image = new Bitmap(Notesieve.Properties.Resources.v);
            }
            else
            {
                eStore.autoLoginButton.Image = new Bitmap(Notesieve.Properties.Resources.x);
            }

            windowOpacity = NotesieveSettings.opacity;
            eStore.drawingForm.Opacity = windowOpacity;
            eStore.opTrackBar.Value = windowOpacity;

            eStore.minLabel.Text = NotesieveSettings.keyHide.ToString();
            eStore.screenButtonButton.Text = NotesieveSettings.keyScreenShot.ToString();
            eStore.hideButtonButton.Text = NotesieveSettings.keyHide.ToString();

            DisplayNotes(notesList);
        }

        private void DisplayNotes(List<Note> notesList)
        {
            eStore.endScroll = 0;
            eStore.scrollPanel.Controls.Clear();

            notesList.Sort(delegate (Note note1, Note note2)
            { 
                return note1.RegDate.CompareTo(note2.RegDate); 
            });

            foreach (Note note in notesList)
            {
                if (note.IsDelete) continue;
                if (eStore.selectedFilterType.Contains("Common"))
                {
                    if (note.GetType() == typeof(Common)) eStore.DrawBox(note);
                }
                if (eStore.selectedFilterType.Contains("ToDoList"))
                {
                    if (note.GetType() == typeof(ToDoList)) eStore.DrawBox(note);
                }
                if (eStore.selectedFilterType.Contains("Alarm"))
                {
                    if (note.GetType() == typeof(Alarm)) eStore.DrawBox(note);
                }
                if (eStore.selectedFilterType.Contains("Screenshot"))
                {
                    if (note.GetType() == typeof(Screenshot)) eStore.DrawBox(note);
                }

            }
                                    

            
        }

        
        private void UpdateBox(Note newNote)
        {
            Panel panel = newNote.GetPanel();
            int oldHeight = panel.Size.Height;
            Label nameLabel = panel.Controls.Find("nameLabel", true)[0] as Label;
            Label mainTextLabel = panel.Controls.Find("mainTextLabel", true)[0] as Label;
            Label typeLabel = panel.Controls.Find("typeLabel", true)[0] as Label;
            Panel leftNotePanel = panel.Controls.Find("left_"+newNote.Id, true)[0] as Panel;
            Type newNoteType = newNote.GetType();
            leftNotePanel.BackColor = Color.FromArgb(newNote.ARGBColor);
            if (newNoteType != typeof(Screenshot))
            {               
                panel.AutoSize = true;
                leftNotePanel.Size = new System.Drawing.Size(150, panel.PreferredSize.Height);
            }
            nameLabel.Text = newNote.Name;
            mainTextLabel.Text = newNote.Text;
            
            if (newNoteType == typeof(ToDoList))
            {
                ToDoList toDoList = newNote as ToDoList;
                typeLabel.Text = Math.Round(toDoList.GetCompletePercent()).ToString() + "%";
                leftNotePanel.BackColor = Color.Gray;
                toDoList.AnyTaskCompleteChenge += eStore.UpdateListBox;
                foreach(Label l in toDoList.GetTaskLabels())
                {
                    l.Parent.Controls.Remove(l);
                }
                toDoList.GetTaskLabels().Clear();
                eStore.DrawTask(newNote, panel, mainTextLabel);
            }
            if (newNoteType != typeof(Screenshot))
            {
                panel.AutoSize = false;
                leftNotePanel.Size = new Size(leftNotePanel.Size.Width, panel.PreferredSize.Height);
                panel.Size = new Size(panel.Size.Width, panel.PreferredSize.Height-5); 
            }


            int addedSize = oldHeight - panel.Size.Height;
            MoveAllNotesDown(-addedSize, panel);
            eStore.endScroll += addedSize;            
        }

       


       

        private void MoveAllNotesDown(int distance, Control beginer)
        {
            foreach (Control control in eStore.scrollPanel.Controls)
            {
                if (control == beginer) continue;
                if (control.GetType() == typeof(Panel))
                {
                    if (control.Location.Y > beginer.Location.Y)
                    {
                        int Y = control.Location.Y;
                        control.Location = new Point(0, Y + distance);
                    }
                }

            }

        }

        void ScrollNotePanel(int delta)
        {
            if (eStore.scrollPanel.Controls.Count < 1) return;
            //Прокручиваем список записей
            int lastY = eStore.scrollPanel.Controls[0].Location.Y + eStore.scrollPanel.Controls[0].Height;

            int firstY = eStore.scrollPanel.Controls[eStore.scrollPanel.Controls.Count - 1].Location.Y;
            int newDelta = delta;

            if(firstY > 0)
            {
                newDelta = -firstY;
            }
            else if(lastY < eStore.scrollPanel.Height)
            {
                newDelta = eStore.scrollPanel.Height-lastY;
            }    
            foreach (Panel p in eStore.scrollPanel.Controls)
            {
                int y = p.Location.Y;
                p.Location = new Point(0, y + newDelta);
            }
            eStore.startScroll += newDelta;
        }
        void ShowAddNewNotePanel(bool show)
        {
            if (show == true)
            {
                eStore.addNewNoteButton.Visible = false;
                eStore.addNewNotePanel.Visible = true;
            }
            else
            {
                eStore.addNewNoteButton.Visible = true;
                eStore.addNewNotePanel.Visible = false;
            }            
        }

        void OpenAddWindow()
        {
            foreach (Control c in eStore.noteTypeButtonList)
            {
                c.Enabled = true;
            }
            eStore.alarmRadioButton.Enabled = true;
            eStore.timerRadioButton.Enabled = true;
            eStore.noteNameTextBox.Text = "";
            eStore.mainTextBox.Text = "";
            //addNoteButton.Image = new Bitmap(Notesieve.Properties.Resources.addNote);
            eStore.nameLabel.Text = "Новая запись!";
            eStore.addNoteButton.Text = "Создать";
            ShowAddNewNotePanel(true);
        }

        void OpenEditWindow(Note note)
        {
            if (note == null) return;

            eStore.noteNameTextBox.Text = note.Name;
            eStore.mainTextBox.Text = note.Text;
            Type noteType = note.GetType();
            
            if (noteType == typeof(Common))
            {
                eStore.eventManager.ChangeNoteType_Click(eStore.noteTypeButtonList[0], new EventArgs());
            }
            else if (noteType == typeof(Alarm))
            {
                Alarm alarm = note as Alarm;
                eStore.datePicker.Value = alarm.alarmDate;
                eStore.timePicker.Value = alarm.alarmDate;
                eStore.eventManager.ChangeNoteType_Click(eStore.noteTypeButtonList[1], new EventArgs());
            }
            else if (noteType == typeof(ToDoList))
            {
                ToDoList toDo = note as ToDoList;
                eStore.mainTextBox.Text += "\n";
                foreach (NoteTask t in toDo.Tasks)
                {
                    eStore.mainTextBox.Text += "*" + t.Name;
                }
                eStore.eventManager.ChangeNoteType_Click(eStore.noteTypeButtonList[2], new EventArgs());
            }
            else if (noteType == typeof(Screenshot))
            {
                eStore.eventManager.ChangeNoteType_Click(eStore.noteTypeButtonList[3], new EventArgs());
            }

            SelectNewColor(Color.FromArgb(note.ARGBColor));

            foreach (Control c in eStore.noteTypeButtonList)
            {
                c.Enabled = false;
            }
            eStore.alarmRadioButton.Enabled = false;
            eStore.timerRadioButton.Enabled = false;
            eStore.alarmRadioButton.Checked = true;
            //addNoteButton.Image = new Bitmap(Notesieve.Properties.Resources.editSave);
            eStore.nameLabel.Text = "Редактирование!";
            eStore.addNoteButton.Text = "Обновить";
            ShowAddNewNotePanel(true);
        }

        void BackColorDarkRed(object sender)
        {
            Control box = sender as Control;
            box.BackColor = Color.DarkRed;
        }
        void BackColorWhite(object sender)
        {
            Control box = sender as Control;
            box.BackColor = Color.White;
        }
        void BackColorTrans(object sender)
        {
            Control box = sender as Control;
            box.BackColor = Color.Transparent;
        }


        void BackColorGray(object sender)
        {
            Control box = sender as Control;
            box.BackColor = Color.DarkGray;
        }
        void BackColorHover(object sender)
        {
            Control box = sender as Control;
            box.BackColor = eStore.currentSelectColor;
        }
        void BackColorClean(object sender)
        {
            Control box = sender as Control;
            if(box != eStore.selectedNoteType) box.BackColor = Color.Gray; 
        }

        void SelectNewColor(Color color)
        {
            foreach (Panel c in eStore.noteColorPanels)
            {
                c.BorderStyle = BorderStyle.None;
            }
            SetNoteColor(color);
        }

        void SelectNewColorFromPanel(object sender)
        {
            foreach (Panel c in eStore.noteColorPanels)
            {
                c.BorderStyle = BorderStyle.None;
            }
            Panel box = sender as Panel;
            box.BorderStyle = BorderStyle.Fixed3D;
            SetNoteColor(box.BackColor);
        }

        void SetNoteColor(Color color)
        {
            eStore.currentNoteColor = color;
            eStore.currentSelectColor = eStore.GetLighterColor(eStore.currentNoteColor);

            eStore.selectedNoteType.BackColor = eStore.currentSelectColor;
            eStore.addNewNotePanel.BackColor = eStore.currentNoteColor;
        }
       
        
        void ShowColorBorder(object sender)
        {
            Panel box = sender as Panel;
            box.BorderStyle = BorderStyle.Fixed3D;
        }
        void HideColorBorder(object sender)
        {
            Panel box = sender as Panel;
            if (box.BackColor != eStore.currentNoteColor) box.BorderStyle = BorderStyle.None;
        }

       

        void ChengeFilter(object sender)
        {
            Control pb = sender as Control;
            string name = pb.Name.Split("_")[0];
            if (eStore.selectedFilterType.Contains(name))
            {
                pb.BackColor = Color.Gray;
                eStore.selectedFilterType.Remove(name);
            }
            else
            {
                pb.BackColor = Color.White;
                eStore.selectedFilterType.Add(name);
            }

            DisplayNotes(Note.NoteList);
            
        }

        void FilterHover(object sender)
        {
            Control pb = sender as Control;
            pb.BackColor = Color.LightGray;           
        }

        void FilterLeave(object sender)
        {
            Control pb = sender as Control;
            string name = pb.Name.Split("_")[0];
            
            if (eStore.selectedFilterType.Contains(name))
            {
                pb.BackColor = Color.White;
            }
            else
            {
                pb.BackColor = Color.Gray;
            }
        }

        void ChengeType(object sender)
        {
            foreach(Control c in eStore.noteTypeButtonList)
            {
                c.BackColor = Color.Gray;
            }
            Control box = sender as Control;
            box.BackColor = eStore.currentSelectColor;
            eStore.selectedNoteType = box;
            eStore.toDoListHelpLabel.Visible = false;
            eStore.alarmRadioButton.Visible = false;
            eStore.timerRadioButton.Visible = false;

            if (box.Name == "Alarm")
            {
                if (eStore.alarmRadioButton.Checked)
                {
                    eStore.datePicker.Enabled = true;
                    eStore.timePicker.Enabled = true;
                    eStore.timePicker.Value = DateTime.Now;
                }                
                else
                {
                    eStore.datePicker.Enabled = false;
                    eStore.timePicker.Enabled = true;
                    eStore.timePicker.Value = DateTime.Now.Date;
                }
                eStore.alarmRadioButton.Visible = true;
                eStore.timerRadioButton.Visible = true;

                eStore.datePicker.Value = DateTime.Now;
                
            }
            else if (box.Name == "ToDoList")
            {
                eStore.toDoListHelpLabel.Visible = true;
            }
            else
            {
                eStore.datePicker.Enabled = false;
                eStore.timePicker.Enabled = false;

                eStore.alarmRadioButton.Visible = false;
                eStore.timerRadioButton.Visible = false;
            }
        }

        void AlarmChangeType(object sender)
        {
            RadioButton rb = sender as RadioButton;
            if(rb == eStore.timerRadioButton)
            {
                eStore.datePicker.Enabled = false;

                eStore.timePicker.Enabled = true;
                eStore.timePicker.Value = DateTime.Now.Date;
            }
            else
            {
                eStore.datePicker.Enabled = true;

                eStore.timePicker.Enabled = true;
                eStore.timePicker.Value = DateTime.Now;
            }
            eStore.datePicker.Value = DateTime.Now;
        }

        void DeleteBox(object sender)
        {
            Control control = sender as Control;
            Control panel = control.Parent;
            int panelLocation = panel.Location.Y;
            int panelHeight = panel.Height;
            eStore.scrollPanel.Controls.Remove(panel);

            foreach (Control c in eStore.scrollPanel.Controls)
            {
                if(c.Location.Y > panelLocation)
                {
                    Point location = c.Location;
                    c.Location = new Point(c.Location.X, c.Location.Y - panelHeight - 10);
                }
            }
            eStore.endScroll -= (panelHeight + 10);
            
        }

        void SwitchComplete(Label task, bool isComplete)
        {
            if (isComplete)
            {
                task.Font = new Font(task.Font, FontStyle.Strikeout);
                task.Image = new Bitmap(Notesieve.Properties.Resources.complete);
            }
            else
            {
                task.Font = new Font(task.Font, FontStyle.Bold);
                task.Image = new Bitmap(Notesieve.Properties.Resources.incomplete);
            }

        }

        void LightUpTask(object sender)
        {
            Label task = sender as Label;
            task.ForeColor = Color.DarkRed;
        }

        void LightDownTask(object sender)
        {
            Label task = sender as Label;
            task.ForeColor = Color.Black;
        }


        void AddNewNote(Note newNote)
        {
            if (newNote == null) return;
            if (eStore.selectedFilterType.Contains(newNote.GetType().ToString().Split(".")[1]))
            {
                eStore.DrawBox(newNote);
                ScrollNotePanel(-eStore.startScroll);
            }
            eStore.noteNameTextBox.Text = "";
            eStore.mainTextBox.Text = "";
            ShowAddNewNotePanel(false);
        }

        void UpdateNote(Note oldNote)
        {
            if (oldNote == null)
            {
                return;
            } 
            UpdateBox(oldNote);
            eStore.noteNameTextBox.Text = "";
            eStore.mainTextBox.Text = "";
            ShowAddNewNotePanel(false);
        }

    }
}
