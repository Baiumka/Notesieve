using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notesieve
{
    class ElementStorage
    {
        public const int WINDOW_WIDHT = 500;
        public const int WINDOW_PADDING = 20;
        public const int NOTE_STANDART_HEIGHT = 200;


        public int startScroll = 0;
        public int endScroll = 0;

        public MainForm drawingForm;
        public EventManager eventManager;
        public Panel scrollPanel;
        public Panel addNewNotePanel;
        public PictureBox addNewNoteButton;
        public TextBox noteNameTextBox;
        public TextBox mainTextBox;
        public DateTimePicker datePicker;
        public DateTimePicker timePicker;
        public RadioButton timerRadioButton;
        public RadioButton alarmRadioButton;
        public Button addNoteButton;
        public Label nameLabel;
        public Button hideButtonButton;
        public Button screenButtonButton;
        public Label minLabel;
        public PictureBox settingsPictureBox;
        public Panel settingPanel;
        public PictureBox autoStartButton;
        public Label autoStartLabel;
        public PictureBox autoLoginButton;
        public Label autoLoginLabel;
        public TrackBar opTrackBar;
        public Label toDoListHelpLabel;
        public PictureBox turnOffPictureBox;
        public PictureBox helperPictureBox;
        public Label googleLoginButton;
        public PictureBox googleLoginImage;
        public PictureBox googleSyncImage;

        public List<string> selectedFilterType = new List<string>();
        public Color gSyncBackColor = Color.White;
        public List<Control> noteTypeButtonList = new List<Control>();
        public Control selectedNoteType;
        public Color currentSelectColor = Color.Green;
        public List<Control> noteColorPanels = new List<Control>();
        public Color currentNoteColor = Color.Green;

        public ElementStorage(MainForm form)
        {
            eventManager = new EventManager();
            drawingForm = form;

            drawingForm.notifyIcon.ContextMenuStrip.Items[1].Click += eventManager.HideButton_Click;
            drawingForm.notifyIcon.ContextMenuStrip.Items[2].Click += eventManager.AppCloseButton_Click;
            drawingForm.notifyIcon.MouseClick += eventManager.Tray_Click;
        }

        public void DrawRightPanel()
        {
            //Рисуем окно программы (боковое)
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            drawingForm.Size = new Size(WINDOW_WIDHT, screenSize.Height);
            drawingForm.Location = new Point(screenSize.Width - WINDOW_WIDHT, 0);//Правое расположение  
            //drawingForm.Location = new Point(0, 0);     //Левое расположение       
            drawingForm.TopMost = true;



            settingPanel = new Panel();
            settingPanel.AutoSize = false;
            settingPanel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), screenSize.Height - 300);
            settingPanel.BackColor = Color.Black;
            settingPanel.Location = new Point(WINDOW_PADDING, 112 + NOTE_STANDART_HEIGHT);
            settingPanel.AutoSize = true;
            settingPanel.Visible = false;
            drawingForm.Controls.Add(settingPanel);

            Label opLabel = new Label();
            opLabel.AutoSize = false;
            opLabel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 20, 25);
            opLabel.Location = new Point(10, 10);
            opLabel.ForeColor = Color.White;
            opLabel.Text = "Прозрачность:";
            settingPanel.Controls.Add(opLabel);

            opTrackBar = new TrackBar();
            opTrackBar.AutoSize = false;
            opTrackBar.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 20, 50);
            opTrackBar.Location = new Point(10, 35);
            opTrackBar.Minimum = 30;
            opTrackBar.Maximum = 100;
            opTrackBar.ValueChanged += eventManager.OpTrackBar_ValueChanged;
            settingPanel.Controls.Add(opTrackBar);

            hideButtonButton = new Button();
            hideButtonButton.AutoSize = false;
            hideButtonButton.Size = new Size(100, 50);
            hideButtonButton.Location = new Point(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 100, 130);
            hideButtonButton.ForeColor = Color.White;
            hideButtonButton.KeyUp += eventManager.HideKey_HookedUp;
            settingPanel.Controls.Add(hideButtonButton);

            Label hideButtonLabel = new Label();
            hideButtonLabel.AutoSize = false;
            hideButtonLabel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 20, 50);
            hideButtonLabel.Location = new Point(10, 130);
            hideButtonLabel.ForeColor = Color.White;
            hideButtonLabel.BorderStyle = BorderStyle.FixedSingle;
            hideButtonLabel.TextAlign = ContentAlignment.MiddleLeft;
            hideButtonLabel.Text = "Кнопка свернуть/развернуть: ";
            settingPanel.Controls.Add(hideButtonLabel);

            screenButtonButton = new Button();
            screenButtonButton.AutoSize = false;
            screenButtonButton.Size = new Size(100, 50);
            screenButtonButton.Location = new Point(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 100, 180);
            screenButtonButton.ForeColor = Color.White;
            screenButtonButton.KeyUp += eventManager.ScreenKey_HookedUp;
            settingPanel.Controls.Add(screenButtonButton);

            Label screenButtonLabel = new Label();
            screenButtonLabel.AutoSize = false;
            screenButtonLabel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 20, 50);
            screenButtonLabel.Location = new Point(10, 180);
            screenButtonLabel.ForeColor = Color.White;
            screenButtonLabel.BorderStyle = BorderStyle.FixedSingle;
            screenButtonLabel.TextAlign = ContentAlignment.MiddleLeft;
            screenButtonLabel.Text = "Кнопка создания скриншота: ";
            settingPanel.Controls.Add(screenButtonLabel);

            autoStartButton = new PictureBox();
            autoStartButton.AutoSize = false;
            autoStartButton.Size = new Size(90, 40);
            autoStartButton.Location = new Point(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 100, 255);
            autoStartButton.ForeColor = Color.White;
            autoStartButton.Image = new Bitmap(Notesieve.Properties.Resources.x);
            autoStartButton.SizeMode = PictureBoxSizeMode.Zoom;
            autoStartButton.BackColor = Color.Black;
            autoStartButton.Click += eventManager.AutoStartSetting_Click;
            settingPanel.Controls.Add(autoStartButton);

            autoStartLabel = new Label();
            autoStartLabel.AutoSize = false;
            autoStartLabel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 10, 50);
            autoStartLabel.Location = new Point(10, 250);
            autoStartLabel.ForeColor = Color.White;
            autoStartLabel.BorderStyle = BorderStyle.FixedSingle;
            autoStartLabel.TextAlign = ContentAlignment.MiddleLeft;
            autoStartLabel.Text = "Запускать Notesieve вместе с Windows";
            settingPanel.Controls.Add(autoStartLabel);

            autoLoginButton = new PictureBox();
            autoLoginButton.AutoSize = false;
            autoLoginButton.Size = new Size(90, 40);
            autoLoginButton.Location = new Point(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 100, 300);
            autoLoginButton.ForeColor = Color.White;
            autoLoginButton.Image = new Bitmap(Notesieve.Properties.Resources.x);
            autoLoginButton.SizeMode = PictureBoxSizeMode.Zoom;
            autoLoginButton.BackColor = Color.Black;
            autoLoginButton.Click += eventManager.AutoLoginSetting_Click;
            settingPanel.Controls.Add(autoLoginButton);

            autoLoginLabel = new Label();
            autoLoginLabel.AutoSize = false;
            autoLoginLabel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2) - 10, 50);
            autoLoginLabel.Location = new Point(10, 295);
            autoLoginLabel.ForeColor = Color.White;
            autoLoginLabel.BorderStyle = BorderStyle.FixedSingle;
            autoLoginLabel.TextAlign = ContentAlignment.MiddleLeft;
            autoLoginLabel.Text = "Выполнять авторизацию в Google, при запуске Notesieve";
            settingPanel.Controls.Add(autoLoginLabel);

            //Делаем прокручиваемую панель для записей
            scrollPanel = new Panel();
            scrollPanel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), screenSize.Height - 300);

            scrollPanel.Location = new Point(WINDOW_PADDING, 116 + NOTE_STANDART_HEIGHT);
            scrollPanel.AutoSize = false;
            scrollPanel.MouseWheel += eventManager.NotesPanel_Scroll;
            drawingForm.Controls.Add(scrollPanel);



            PictureBox filterPictureBox = new PictureBox();
            filterPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.screenshot);
            filterPictureBox.Name = "Screenshot_Filter";
            filterPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING - (48 * 4), 0);
            filterPictureBox.Size = new System.Drawing.Size(48, 64);
            filterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            filterPictureBox.TabIndex = 0;
            filterPictureBox.TabStop = false;
            filterPictureBox.Click += eventManager.TopFilterButton_Click;
            filterPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            filterPictureBox.MouseLeave += eventManager.TopFilterButton_Leave;
            filterPictureBox.Cursor = Cursors.Hand;
            filterPictureBox.BackColor = Color.White;
            filterPictureBox.BorderStyle = BorderStyle.FixedSingle;
            selectedFilterType.Add(filterPictureBox.Name.Split("_")[0]);
            OvalFilterBox(filterPictureBox);
            drawingForm.Controls.Add(filterPictureBox);

            filterPictureBox = new PictureBox();
            filterPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.common);
            filterPictureBox.Name = "Common_Filter";
            filterPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING - (48 * 3), 0);
            filterPictureBox.Size = new System.Drawing.Size(48, 64);
            filterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            filterPictureBox.TabIndex = 0;
            filterPictureBox.TabStop = false;
            filterPictureBox.Click += eventManager.TopFilterButton_Click;
            filterPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            filterPictureBox.MouseLeave += eventManager.TopFilterButton_Leave;
            filterPictureBox.Cursor = Cursors.Hand;
            filterPictureBox.BackColor = Color.FromArgb(255, 255, 255, 255);
            filterPictureBox.BorderStyle = BorderStyle.FixedSingle;
            selectedFilterType.Add(filterPictureBox.Name.Split("_")[0]);
            OvalFilterBox(filterPictureBox);
            drawingForm.Controls.Add(filterPictureBox);

            filterPictureBox = new PictureBox();
            filterPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.tasklist);
            filterPictureBox.Name = "ToDoList_Filter";
            filterPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING - (48 * 2), 0);
            filterPictureBox.Size = new System.Drawing.Size(48, 64);
            filterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            filterPictureBox.TabIndex = 0;
            filterPictureBox.TabStop = false;
            filterPictureBox.Cursor = Cursors.Hand;
            filterPictureBox.Click += eventManager.TopFilterButton_Click;
            filterPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            filterPictureBox.MouseLeave += eventManager.TopFilterButton_Leave;
            filterPictureBox.BackColor = Color.FromArgb(255, 255, 255, 255);
            filterPictureBox.BorderStyle = BorderStyle.FixedSingle;
            selectedFilterType.Add(filterPictureBox.Name.Split("_")[0]);
            OvalFilterBox(filterPictureBox);
            drawingForm.Controls.Add(filterPictureBox);

            filterPictureBox = new PictureBox();
            filterPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.alarm);
            filterPictureBox.Name = "Alarm_Filter";
            filterPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING - 48, 0);
            filterPictureBox.Size = new System.Drawing.Size(48, 64);
            filterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            filterPictureBox.TabIndex = 0;
            filterPictureBox.TabStop = false;
            filterPictureBox.Click += eventManager.TopFilterButton_Click;
            filterPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            filterPictureBox.MouseLeave += eventManager.TopFilterButton_Leave;
            filterPictureBox.Cursor = Cursors.Hand;
            filterPictureBox.BackColor = Color.FromArgb(255, 255, 255, 255);
            filterPictureBox.BorderStyle = BorderStyle.FixedSingle;
            selectedFilterType.Add(filterPictureBox.Name.Split("_")[0]);
            OvalFilterBox(filterPictureBox);
            drawingForm.Controls.Add(filterPictureBox);

            minLabel = new Label();
            minLabel.Image = new Bitmap(Notesieve.Properties.Resources.hide);
            minLabel.ImageAlign = ContentAlignment.MiddleRight;
            minLabel.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
            minLabel.TextAlign = ContentAlignment.MiddleLeft;
            minLabel.Location = new System.Drawing.Point(WINDOW_PADDING + 32, 0);
            minLabel.Size = new System.Drawing.Size(64, 32);
            minLabel.TabIndex = 0;
            minLabel.TabStop = false;
            minLabel.Click += eventManager.HideButton_Click;
            minLabel.MouseHover += eventManager.TopMenuItem_Hover;
            minLabel.MouseLeave += eventManager.TopMenuItem_Leave;
            minLabel.Cursor = Cursors.Hand;
            minLabel.BackColor = Color.FromArgb(255, 255, 255, 255);
            minLabel.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(minLabel);

            googleLoginImage = new PictureBox();
            googleLoginImage.Image = new Bitmap(Notesieve.Properties.Resources.google_small);
            googleLoginImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            googleLoginImage.Location = new System.Drawing.Point(WINDOW_PADDING + 64, 32);
            googleLoginImage.Size = new System.Drawing.Size(32, 32);
            googleLoginImage.TabIndex = 0;
            //googleLoginImage.Click += eventManager.OnGoogleClick;
            googleLoginImage.TabStop = false;
            googleLoginImage.Cursor = Cursors.Hand;
            googleLoginImage.BackColor = Color.FromArgb(255, 255, 255, 255);
            googleLoginImage.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(googleLoginImage);

            googleSyncImage = new PictureBox();
            googleSyncImage.Image = new Bitmap(Notesieve.Properties.Resources.sync);
            googleSyncImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            googleSyncImage.Location = new System.Drawing.Point(WINDOW_PADDING + 64, 32);
            googleSyncImage.Size = new System.Drawing.Size(32, 32);
            googleSyncImage.TabIndex = 0;
            googleSyncImage.Click += eventManager.GoogleSyncButton_Click;
            googleSyncImage.MouseHover += eventManager.GoogleSyncButton_Hover;
            googleSyncImage.MouseLeave += eventManager.GoogleSyncButton_Leave;
            googleSyncImage.Visible = false;
            googleSyncImage.TabStop = false;
            googleSyncImage.Cursor = Cursors.Hand;
            googleSyncImage.BackColor = gSyncBackColor;
            googleSyncImage.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(googleSyncImage);


            googleLoginButton = new Label();
            googleLoginButton.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            googleLoginButton.TextAlign = ContentAlignment.MiddleCenter;
            googleLoginButton.Text = "Войти";
            googleLoginButton.Location = new System.Drawing.Point(WINDOW_PADDING + 96, 32);
            googleLoginButton.Size = new System.Drawing.Size(64, 32);
            googleLoginButton.TabIndex = 0;
            googleLoginButton.TabStop = false;
            googleLoginButton.Click += eventManager.GoogleLoginButton_Click;
            googleLoginButton.MouseHover += eventManager.GoogleLoginButton_Hover;
            googleLoginButton.MouseLeave += eventManager.GoogleLoginButton_Leave;
            googleLoginButton.Cursor = Cursors.Hand;
            googleLoginButton.BackColor = Color.FromArgb(255, 255, 255, 255);
            googleLoginButton.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(googleLoginButton);


            Label versionLabel = new Label();
            versionLabel.Font = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Bold);
            versionLabel.ForeColor = Color.White;
            versionLabel.TextAlign = ContentAlignment.BottomLeft;
            versionLabel.Text = "v" + MainForm.currentVersion;
            versionLabel.Location = new System.Drawing.Point(WINDOW_PADDING + 160 , 32);
            versionLabel.Size = new System.Drawing.Size(200, 32);
            drawingForm.Controls.Add(versionLabel);



            settingsPictureBox = new PictureBox();
            settingsPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.settings);
            settingsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            settingsPictureBox.Location = new System.Drawing.Point(WINDOW_PADDING, 32);
            settingsPictureBox.Size = new System.Drawing.Size(32, 32);
            settingsPictureBox.TabIndex = 0;
            settingsPictureBox.TabStop = false;
            settingsPictureBox.Click += eventManager.SettingsButton_Click;
            settingsPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            settingsPictureBox.MouseLeave += eventManager.TopMenuItem_Leave;
            settingsPictureBox.Cursor = Cursors.Hand;
            settingsPictureBox.BackColor = Color.White;
            settingsPictureBox.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(settingsPictureBox);

            turnOffPictureBox = new PictureBox();
            turnOffPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.turnoff);
            turnOffPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            turnOffPictureBox.Location = new System.Drawing.Point(WINDOW_PADDING, 0);
            turnOffPictureBox.Size = new System.Drawing.Size(32, 32);
            turnOffPictureBox.TabIndex = 0;
            turnOffPictureBox.TabStop = false;
            turnOffPictureBox.Click += eventManager.AppCloseButton_Click;
            turnOffPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            turnOffPictureBox.MouseLeave += eventManager.AppCloseButton_Leave;
            turnOffPictureBox.Cursor = Cursors.Hand;
            turnOffPictureBox.BackColor = Color.DarkRed;
            turnOffPictureBox.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(turnOffPictureBox);

            helperPictureBox = new PictureBox();
            helperPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.help);
            helperPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            helperPictureBox.Location = new System.Drawing.Point(WINDOW_PADDING + 32, 32);
            helperPictureBox.Size = new System.Drawing.Size(32, 32);
            helperPictureBox.TabIndex = 0;
            helperPictureBox.TabStop = false;
            helperPictureBox.Click += eventManager.HelperButton_Click;
            helperPictureBox.MouseHover += eventManager.TopMenuItem_Hover;
            helperPictureBox.MouseLeave += eventManager.TopMenuItem_Leave;
            helperPictureBox.Cursor = Cursors.Hand;
            helperPictureBox.BackColor = Color.White;
            helperPictureBox.BorderStyle = BorderStyle.FixedSingle;
            drawingForm.Controls.Add(helperPictureBox);
        }

        public void DrawAddButton()
        {

            //===========================================================================================
            //Рисуем кнопку "Добавить Запись", которая будет сменяться формой добавления при нажатии
            addNewNoteButton = new PictureBox();
            addNewNoteButton.Image = new Bitmap(Notesieve.Properties.Resources.add);
            addNewNoteButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            addNewNoteButton.TabIndex = 0;
            addNewNoteButton.TabStop = false;
            addNewNoteButton.Click += eventManager.AddNewNoteButton_Click;
            addNewNoteButton.MouseLeave += eventManager.BigAddButton_Leave;
            addNewNoteButton.MouseHover += eventManager.AddNewNoteButton_MouseHover;
            addNewNoteButton.Visible = true;
            addNewNoteButton.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), NOTE_STANDART_HEIGHT);
            addNewNoteButton.Location = new Point(WINDOW_PADDING, 106);
            drawingForm.Controls.Add(addNewNoteButton);

            //========================================
            //Рисуем форму добавления новой записи            
            addNewNotePanel = new Panel();
            addNewNotePanel.Size = new Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), NOTE_STANDART_HEIGHT - 10);
            addNewNotePanel.Location = new Point(WINDOW_PADDING, 106);
            addNewNotePanel.BackColor = Color.Green;
            drawingForm.Controls.Add(addNewNotePanel);
            addNewNotePanel.Visible = false;

            for (int i = 0; i < 10; i++)
            {
                Panel colorPanel = new Panel();
                colorPanel.Size = new Size(15, 15);
                colorPanel.Location = new Point(i * 15, 30);
                switch (i)
                {
                    case 0:
                        colorPanel.BackColor = Color.Green;
                        //Цвет по умолчанию
                        colorPanel.BorderStyle = BorderStyle.Fixed3D;
                        currentNoteColor = Color.Green;
                        break;
                    case 1:
                        colorPanel.BackColor = Color.Red;
                        break;
                    case 8:
                        colorPanel.BackColor = Color.Orange;
                        break;
                    case 3:
                        colorPanel.BackColor = Color.Yellow;
                        break;
                    case 6:
                        colorPanel.BackColor = Color.LightBlue;
                        break;
                    case 5:
                        colorPanel.BackColor = Color.Blue;
                        break;
                    case 4:
                        colorPanel.BackColor = Color.Purple;
                        break;
                    case 2:
                        colorPanel.BackColor = Color.Brown;
                        break;
                    case 7:
                        colorPanel.BackColor = Color.Orchid;
                        break;
                    case 9:
                        colorPanel.BackColor = Color.Gold;
                        break;
                }
                colorPanel.Click += eventManager.NoteColor_Click;
                colorPanel.MouseHover += eventManager.NoteColor_MouseHover;
                colorPanel.MouseLeave += eventManager.NoteColor_MouseLeave;
                noteColorPanels.Add(colorPanel);
                addNewNotePanel.Controls.Add(colorPanel);

            }


            currentSelectColor = GetLighterColor(currentNoteColor);


            //Поле для ввода названия
            noteNameTextBox = new TextBox();
            noteNameTextBox.Size = new Size(500, 10);
            noteNameTextBox.Location = new Point(150, 0);
            noteNameTextBox.Font = new Font(FontFamily.GenericSerif, 10, FontStyle.Bold);
            noteNameTextBox.ForeColor = Color.Black;
            noteNameTextBox.Name = "AddNoteNameTextBox";
            addNewNotePanel.Controls.Add(noteNameTextBox);

            //Поле для ввода основного текста
            mainTextBox = new TextBox();
            mainTextBox.Size = new Size(310, 138);
            mainTextBox.WordWrap = true;
            mainTextBox.Multiline = true;
            mainTextBox.Location = new Point(150, 25);
            mainTextBox.Font = new Font(FontFamily.GenericSansSerif, 9);
            mainTextBox.ForeColor = Color.Black;
            mainTextBox.Name = "AddNoteTextTextBox";
            addNewNotePanel.Controls.Add(mainTextBox);

            datePicker = new DateTimePicker();
            datePicker.Size = new Size(155, 10);
            datePicker.Location = new Point(150, 163);
            datePicker.Enabled = false;
            datePicker.Name = "DatePicker";
            addNewNotePanel.Controls.Add(datePicker);

            timePicker = new DateTimePicker();
            timePicker.Size = new Size(155, 10);
            timePicker.Location = new Point(305, 163);
            timePicker.Enabled = false;
            timePicker.Format = DateTimePickerFormat.Time;
            timePicker.ShowUpDown = true;
            timePicker.Name = "TimePicker";
            addNewNotePanel.Controls.Add(timePicker);


            /*
            timeTextBox = new TextBox();
            timeTextBox.Size = new Size(155, 10);
            timeTextBox.TextAlign = HorizontalAlignment.Center;
            timeTextBox.Location = new Point(305, 163);
            timeTextBox.Enabled = false;
            timeTextBox.Text = SetTimeSpanText(DateTime.Now.TimeOfDay);
            addNewNotePanel.Controls.Add(timeTextBox);
            */

            //Кнопка для подтверждения добавления новой записи.
            PictureBox noteTypeBox = new PictureBox();
            noteTypeBox.Image = new Bitmap(Notesieve.Properties.Resources.common);
            noteTypeBox.Location = new System.Drawing.Point(0, 50);
            noteTypeBox.Size = new System.Drawing.Size(45, 53);
            noteTypeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            noteTypeBox.TabIndex = 0;
            noteTypeBox.TabStop = false;
            noteTypeBox.MouseHover += eventManager.AddNoteTypeButton_Hover;
            noteTypeBox.MouseLeave += eventManager.AddNoteTypeButton_Leave;
            noteTypeBox.Click += eventManager.ChangeNoteType_Click;
            noteTypeBox.Name = "Common";
            noteTypeBox.BackColor = currentSelectColor;
            selectedNoteType = noteTypeBox;
            noteTypeBox.Cursor = Cursors.Hand;
            OvalPictureBox(noteTypeBox);
            addNewNotePanel.Controls.Add(noteTypeBox);
            noteTypeButtonList.Add(noteTypeBox);

            //Кнопка для подтверждения добавления новой записи.
            noteTypeBox = new PictureBox();
            noteTypeBox.Image = new Bitmap(Notesieve.Properties.Resources.alarm);
            noteTypeBox.Location = new System.Drawing.Point(100, 50);
            noteTypeBox.Size = new System.Drawing.Size(45, 53);
            noteTypeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            noteTypeBox.TabIndex = 0;
            noteTypeBox.TabStop = false;
            noteTypeBox.MouseHover += eventManager.AddNoteTypeButton_Hover;
            noteTypeBox.MouseLeave += eventManager.AddNoteTypeButton_Leave;
            noteTypeBox.Click += eventManager.ChangeNoteType_Click;
            noteTypeBox.Name = "Alarm";
            noteTypeBox.BackColor = Color.Gray;
            noteTypeBox.Cursor = Cursors.Hand;
            OvalPictureBox(noteTypeBox);
            addNewNotePanel.Controls.Add(noteTypeBox);
            noteTypeButtonList.Add(noteTypeBox);

            //Кнопка для подтверждения добавления новой записи.
            noteTypeBox = new PictureBox();
            noteTypeBox.Image = new Bitmap(Notesieve.Properties.Resources.tasklist);
            noteTypeBox.Location = new System.Drawing.Point(50, 50);
            noteTypeBox.Size = new System.Drawing.Size(45, 53);
            noteTypeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            noteTypeBox.TabIndex = 0;
            noteTypeBox.TabStop = false;
            noteTypeBox.MouseHover += eventManager.AddNoteTypeButton_Hover;
            noteTypeBox.MouseLeave += eventManager.AddNoteTypeButton_Leave;
            noteTypeBox.Click += eventManager.ChangeNoteType_Click;
            noteTypeBox.Name = "ToDoList";
            noteTypeBox.BackColor = Color.Gray;
            noteTypeBox.Cursor = Cursors.Hand;
            OvalPictureBox(noteTypeBox);
            addNewNotePanel.Controls.Add(noteTypeBox);
            noteTypeButtonList.Add(noteTypeBox);

            noteTypeBox = new PictureBox();
            noteTypeBox.Image = new Bitmap(Notesieve.Properties.Resources.common);
            noteTypeBox.Location = new System.Drawing.Point(50, 50);
            noteTypeBox.Size = new System.Drawing.Size(50, 59);
            noteTypeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            noteTypeBox.TabIndex = 0;
            noteTypeBox.TabStop = false;
            noteTypeBox.MouseHover += eventManager.AddNoteTypeButton_Hover;
            noteTypeBox.MouseLeave += eventManager.AddNoteTypeButton_Leave;
            noteTypeBox.Click += eventManager.ChangeNoteType_Click;
            noteTypeBox.Name = "Screenshot";
            OvalPictureBox(noteTypeBox);
            noteTypeButtonList.Add(noteTypeBox);

            timerRadioButton = new RadioButton();
            timerRadioButton.Location = new System.Drawing.Point(10, 107);
            timerRadioButton.AutoSize = true;
            timerRadioButton.TabStop = false;
            timerRadioButton.Text = "Таймер";
            timerRadioButton.Checked = true;
            timerRadioButton.Visible = false;
            timerRadioButton.CheckedChanged += eventManager.AlarmTypeRadioButton_CheckedChange;
            addNewNotePanel.Controls.Add(timerRadioButton);

            alarmRadioButton = new RadioButton();
            alarmRadioButton.Location = new System.Drawing.Point(10, 126);
            alarmRadioButton.AutoSize = true;
            alarmRadioButton.TabStop = false;
            alarmRadioButton.Text = "Будильник";
            alarmRadioButton.Visible = false;
            alarmRadioButton.CheckedChanged += eventManager.AlarmTypeRadioButton_CheckedChange;
            addNewNotePanel.Controls.Add(alarmRadioButton);

            toDoListHelpLabel = new Label();
            toDoListHelpLabel.Location = new System.Drawing.Point(0, 110);
            toDoListHelpLabel.Size = new Size(150, 50);
            toDoListHelpLabel.Font = new Font(FontFamily.GenericMonospace, 7);
            toDoListHelpLabel.TextAlign = ContentAlignment.MiddleCenter;
            toDoListHelpLabel.TabStop = false;
            toDoListHelpLabel.Text = "Для создания нового пункта используйте: *";
            toDoListHelpLabel.Visible = false;
            addNewNotePanel.Controls.Add(toDoListHelpLabel);

            addNoteButton = new Button();
            addNoteButton.Location = new System.Drawing.Point(0, 160);
            addNoteButton.Size = new System.Drawing.Size(150, 30);
            addNoteButton.TabIndex = 0;
            addNoteButton.TabStop = false;
            addNoteButton.Click += eventManager.AddNote_Click;
            addNoteButton.Cursor = Cursors.Hand;
            addNoteButton.BackColor = Color.Gray;
            addNewNotePanel.Controls.Add(addNoteButton);

            //Кнопка отмены добавления
            PictureBox closePictureBox = new PictureBox();
            closePictureBox.Image = new Bitmap(Notesieve.Properties.Resources.close);
            closePictureBox.Location = new System.Drawing.Point(0, 0);
            closePictureBox.Size = new System.Drawing.Size(25, 25);
            closePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            closePictureBox.TabIndex = 0;
            closePictureBox.TabStop = false;
            closePictureBox.MouseHover += eventManager.AddNoteCloseButton_Hover;
            closePictureBox.MouseLeave += eventManager.AddNoteCloseButton_Leave;
            closePictureBox.Click += eventManager.AddNoteCloseButton_Click;
            closePictureBox.BackColor = Color.Gray;
            closePictureBox.Cursor = Cursors.Hand;
            addNewNotePanel.Controls.Add(closePictureBox);


            nameLabel = new Label();
            nameLabel.AutoSize = true;
            nameLabel.Location = new System.Drawing.Point(30, 2);
            nameLabel.Font = new Font(FontFamily.GenericSerif, 10, FontStyle.Bold);
            nameLabel.MaximumSize = new System.Drawing.Size(310, 1000);
            addNewNotePanel.Controls.Add(nameLabel);

        }

        public void DrawBox(Note newNote)
        {
            Type newNoteType = newNote.GetType();

            Panel notePanel;
            notePanel = new Panel();
            notePanel.AutoSize = true;
            notePanel.MinimumSize = new System.Drawing.Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), 200);
            notePanel.MaximumSize = new System.Drawing.Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), 1000);
            notePanel.Location = new System.Drawing.Point(0, startScroll);
            notePanel.BackColor = Color.White;
            notePanel.Name = "panel_" + newNote.Id;

            Label nameLabel = new Label();
            // nameLabel.AutoSize = true;
            nameLabel.AutoSize = true;
            nameLabel.Location = new System.Drawing.Point(155, 0);
            nameLabel.Font = new Font(FontFamily.GenericSerif, 10, FontStyle.Bold);
            nameLabel.Text = newNote.Name;
            nameLabel.MaximumSize = new System.Drawing.Size(260, 40);
            nameLabel.Name = "nameLabel";
            notePanel.Controls.Add(nameLabel);

            Label mainTextLabel = new Label();
            mainTextLabel.AutoSize = true;
            mainTextLabel.Location = new System.Drawing.Point(155, 20);
            mainTextLabel.Font = new Font(FontFamily.GenericSansSerif, 9);
            mainTextLabel.ForeColor = Color.Black;
            mainTextLabel.MaximumSize = new System.Drawing.Size(310, 1000);
            mainTextLabel.Text = newNote.Text;
            mainTextLabel.Name = "mainTextLabel";
            mainTextLabel.BackColor = Color.White;
            mainTextLabel.BorderStyle = BorderStyle.None;
            // mainTextLabel.MouseMove += eventManager.textLabel_MouseMove;
            mainTextLabel.Click += eventManager.CommonNoteTextLabel_Click;
            mainTextLabel.Cursor = new Cursor(new System.IO.MemoryStream(Notesieve.Properties.Resources.copy_cursor));
            notePanel.Controls.Add(mainTextLabel);
            if (newNoteType == typeof(ToDoList))
            {
                DrawTask(newNote, notePanel, mainTextLabel);
            }

            Panel leftNotePanel = new Panel();
            int screenShotHeight = (WINDOW_WIDHT - (WINDOW_PADDING * 2)) / 16 * 9;
            if (newNoteType == typeof(Screenshot))
            {
                leftNotePanel.Size = new System.Drawing.Size(WINDOW_WIDHT - (WINDOW_PADDING * 2), screenShotHeight);
                Screenshot screenshot = newNote as Screenshot;
                leftNotePanel.BackgroundImage = screenshot.GetImg();
                leftNotePanel.BackgroundImageLayout = ImageLayout.Stretch;
                nameLabel.Location = new System.Drawing.Point(5, screenShotHeight + 5);
                mainTextLabel.Location = new System.Drawing.Point(5, screenShotHeight + 25);
            }
            else
            {
                leftNotePanel.Size = new System.Drawing.Size(150, notePanel.PreferredSize.Height);
            }
            leftNotePanel.Location = new System.Drawing.Point(0, 0);
            leftNotePanel.BackColor = Color.FromArgb(newNote.ARGBColor);
            if (newNoteType == typeof(ToDoList)) leftNotePanel.BackColor = Color.Gray;
            if (newNoteType == typeof(Screenshot)) leftNotePanel.BackColor = Color.Transparent;
            leftNotePanel.Name = "left_" + newNote.Id;
            if (newNoteType == typeof(ToDoList)) leftNotePanel.Paint += LeftPanelPaint;
            notePanel.Controls.Add(leftNotePanel);

            //Изображение указывающие ТИП записи
            PictureBox typePictureBox = new PictureBox();
            if (newNoteType != typeof(Screenshot))
            {
                typePictureBox.Name = "typePictureBox";
                typePictureBox.Location = new System.Drawing.Point(51, (notePanel.PreferredSize.Height / 2) - 32);
                typePictureBox.Size = new System.Drawing.Size(48, 64);
                typePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

                if (newNoteType == typeof(Common)) typePictureBox.Image = new Bitmap(Notesieve.Properties.Resources.common);
                if (newNoteType == typeof(ToDoList)) typePictureBox.Image = new Bitmap(Notesieve.Properties.Resources.tasklist);
                if (newNoteType == typeof(Alarm)) typePictureBox.Image = new Bitmap(Notesieve.Properties.Resources.alarm);

                typePictureBox.TabIndex = 0;
                typePictureBox.TabStop = false;
                //typePictureBox.Click += eventManager.leftPanelBox_Click;
                //typePictureBox.Cursor = Cursors.Hand;
                typePictureBox.BackColor = Color.FromArgb(0, 0, 0, 0);
                leftNotePanel.Controls.Add(typePictureBox);
            }

            Label typeLabel = new Label();

            typeLabel.TextAlign = ContentAlignment.TopCenter;
            if (newNoteType == typeof(Common)) typeLabel.Text = newNote.RegDate.ToString().Split(" ")[0];
            if (newNoteType == typeof(ToDoList))
            {
                ToDoList toDoList = newNote as ToDoList;
                typeLabel.Text = Math.Round(toDoList.GetCompletePercent()).ToString() + "%";
                toDoList.AnyTaskCompleteChenge += UpdateListBox;
            }
            if (newNoteType == typeof(Alarm))
            {
                Alarm alarm = newNote as Alarm;
                alarm.TimerTick += UpdateAlarmBox;
                alarm.AlarmRing += MakeAlarm;
                typeLabel.TextAlign = ContentAlignment.MiddleCenter;
            }
            typeLabel.Font = new Font(FontFamily.GenericSerif, 10, FontStyle.Bold);
            typeLabel.AutoSize = false;
            typeLabel.Name = "typeLabel";
            typeLabel.Location = new System.Drawing.Point(25, (notePanel.PreferredSize.Height / 2) + 35);
            typeLabel.Size = new System.Drawing.Size(100, 40);
            typeLabel.BackColor = Color.FromArgb(0, 0, 0, 0);
            leftNotePanel.Controls.Add(typeLabel);


            PictureBox editPictureBox = new PictureBox();
            editPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.edit);
            editPictureBox.Name = "editPictureBox";
            editPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - 18 - WINDOW_PADDING * 2, 5);
            editPictureBox.Size = new System.Drawing.Size(16, 16);
            editPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            editPictureBox.TabIndex = 0;
            editPictureBox.TabStop = false;
            editPictureBox.BackColor = Color.FromArgb(0, 0, 0, 0);
            editPictureBox.Click += eventManager.EditBoxButton_Click;
            editPictureBox.Cursor = Cursors.Hand;
            notePanel.Controls.Add(editPictureBox);

            PictureBox deletePictureBox = new PictureBox();
            deletePictureBox.Image = new Bitmap(Notesieve.Properties.Resources.delete2);

            deletePictureBox.Name = "deletePictureBox";
            deletePictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - 36 - WINDOW_PADDING * 2, 5);
            deletePictureBox.Size = new System.Drawing.Size(16, 16);
            deletePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            deletePictureBox.TabIndex = 0;
            deletePictureBox.TabStop = false;
            deletePictureBox.BackColor = Color.FromArgb(0, 0, 0, 0);
            deletePictureBox.Click += eventManager.DeleteBoxButton_Click;
            deletePictureBox.Cursor = Cursors.Hand;
            notePanel.Controls.Add(deletePictureBox);



            if (newNoteType == typeof(Screenshot))
            {

                deletePictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING * 2 - 29, screenShotHeight + 5);
                editPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING * 2 - (29 * 2), screenShotHeight + 5);
                deletePictureBox.Visible = true;
                editPictureBox.Visible = true;
                deletePictureBox.BackColor = Color.LightCoral;
                editPictureBox.BackColor = Color.Orange;
                deletePictureBox.Size = new System.Drawing.Size(24, 24);
                editPictureBox.Size = new System.Drawing.Size(24, 24);


                PictureBox fullPictureBox = new PictureBox();
                fullPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.full);
                fullPictureBox.Name = "fullPictureBox";
                fullPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING * 2 - (29 * 3), screenShotHeight + 5);
                fullPictureBox.Size = new System.Drawing.Size(24, 24);
                fullPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                fullPictureBox.TabIndex = 0;
                fullPictureBox.TabStop = false;
                fullPictureBox.BackColor = Color.LightBlue;
                fullPictureBox.Click += eventManager.FullScreenImgButton_Click;
                fullPictureBox.Cursor = Cursors.Hand;
                notePanel.Controls.Add(fullPictureBox);

                PictureBox copyPictureBox = new PictureBox();
                copyPictureBox.Image = new Bitmap(Notesieve.Properties.Resources.copy);
                copyPictureBox.Name = "copyPictureBox";
                copyPictureBox.Location = new System.Drawing.Point(WINDOW_WIDHT - WINDOW_PADDING * 2 - (29 * 4), screenShotHeight + 5);
                copyPictureBox.Size = new System.Drawing.Size(24, 24);
                copyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                copyPictureBox.TabIndex = 0;
                copyPictureBox.TabStop = false;
                copyPictureBox.BackColor = Color.LightGreen;
                copyPictureBox.Click += eventManager.CopyImgButton_Click;
                copyPictureBox.Cursor = Cursors.Hand;
                notePanel.Controls.Add(copyPictureBox);

            }
            else
            {
                notePanel.AutoSize = false;
                notePanel.Height = leftNotePanel.Height;
            }

            int addedSize = notePanel.PreferredSize.Height + 10;



            MoveAllNotesDown(addedSize);
            scrollPanel.Controls.Add(notePanel);
            endScroll += addedSize;
            newNote.SetPanel(notePanel);
        }

        private void LeftPanelPaint(object sender, PaintEventArgs e)
        {
            Control control = sender as Control;
            int id = Convert.ToInt32(control.Name.Split("_")[1]);
            ToDoList note = Note.GetNoteById(id) as ToDoList;
            if (note == null) return;

            double firstHeight = control.Size.Height / 100f * (100f - note.GetCompletePercent());
            if (firstHeight == 0) firstHeight = -1;
            int secondHeight = control.Size.Height - (int)firstHeight;
            if (secondHeight == 0) secondHeight = -1;


            Rectangle rect = new Rectangle(75, 0, 75, (int)firstHeight);
            Rectangle rect2 = new Rectangle(75, (int)firstHeight, 75, secondHeight);


            Graphics g = e.Graphics;

            LinearGradientBrush lgb = new LinearGradientBrush(rect, Color.Gray, Color.Gray, LinearGradientMode.Vertical);
            g.FillRectangle(lgb, 0, 0, 150, (int)firstHeight);
            LinearGradientBrush lgb2 = new LinearGradientBrush(rect2, Color.FromArgb(note.ARGBColor), Color.FromArgb(note.ARGBColor), LinearGradientMode.Vertical);
            g.FillRectangle(lgb2, 0, (int)firstHeight, 150, secondHeight);
        }


        public void DrawTask(Note newNote, Panel notePanel, Label mainTextLabel)
        {
            int prevHeight = 0;
            ToDoList toDo = newNote as ToDoList;
            foreach (NoteTask t in toDo.Tasks)
            {
                Label taskLabel = new Label();
                taskLabel.AutoSize = true;
                taskLabel.Location = new System.Drawing.Point(155, mainTextLabel.Size.Height + 25 + prevHeight);
                taskLabel.ForeColor = Color.Black;
                taskLabel.MaximumSize = new Size(300, 1000);
                taskLabel.Text += "  " + t.Name;
                taskLabel.ImageAlign = ContentAlignment.TopLeft;
                taskLabel.Name = "task_" + t.Id;
                taskLabel.Click += eventManager.TaskLabel_Click;
                taskLabel.MouseHover += eventManager.TaskLabel_Hover;
                taskLabel.MouseLeave += eventManager.TaskLabel_Leave;

                if (t.IsComplete)
                {
                    taskLabel.Image = new Bitmap(Notesieve.Properties.Resources.complete);
                    taskLabel.Font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Strikeout);
                }
                else
                {
                    taskLabel.Image = new Bitmap(Notesieve.Properties.Resources.incomplete);
                    taskLabel.Font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);
                }
                notePanel.Controls.Add(taskLabel);
                prevHeight += taskLabel.Height + 5;
                toDo.GetTaskLabels().Add(taskLabel);
            }


        }


        private void MoveAllNotesDown(int distance)
        {
            foreach (Control control in scrollPanel.Controls)
            {
                if (control.GetType() == typeof(Panel))
                {
                    int Y = control.Location.Y;
                    control.Location = new Point(0, Y + distance);
                }

            }

        }


        private void OvalPictureBox(PictureBox pb)
        {
            var bounds = new Rectangle(0, 0, pb.Width, pb.Height);
            int radius = 11;
            GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddArc(bounds.X + 1, bounds.Y + 1, radius, radius, 180, 90);

            gp.AddArc(bounds.X + bounds.Width - radius - 1, bounds.Y + 1, radius, radius, 270, 90);
            gp.AddArc(bounds.X + bounds.Width - radius - 1, bounds.Y + bounds.Height - radius - 1, radius, radius, 0, 90);

            gp.AddArc(bounds.X + 2, bounds.Y + bounds.Height - radius, radius, radius, 90, 90);

            Region rg = new Region(gp);
            pb.Region = rg;
        }

        private void OvalFilterBox(PictureBox pb)
        {
            var bounds = new Rectangle(0, 0, pb.Width, pb.Height);
            int radius = 11;
            GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddArc(bounds.X + 3, bounds.Y + 3, radius, radius, 180, 90);

            gp.AddArc(bounds.X + bounds.Width - radius - 3, bounds.Y + 3, radius, radius, 270, 90);
            gp.AddArc(bounds.X + bounds.Width - radius - 3, bounds.Y + bounds.Height - radius - 4, radius, radius, 0, 90);

            gp.AddArc(bounds.X + 4, bounds.Y + bounds.Height - radius - 3, radius, radius, 90, 90);

            Region rg = new Region(gp);
            pb.Region = rg;
        }

        public Color GetLighterColor(Color oldColor)
        {
            int R = oldColor.R + 100;
            int G = oldColor.G + 100;
            int B = oldColor.B + 100;

            if (R > 255) R = 255;
            if (G > 255) G = 255;
            if (B > 255) B = 255;

            return Color.FromArgb(R, G, B);
        }

        public void UpdateListBox(ToDoList note)
        {
            if (note.GetPanel() == null) return;

            Control[] labelList = note.GetPanel().Controls.Find("typeLabel", true);
            if (labelList == null) return;
            if (labelList[0] == null) return;
            Label typeLabel = labelList[0] as Label;

            typeLabel.Text = Math.Round(note.GetCompletePercent()).ToString() + "%";
            typeLabel.Parent.Refresh();
        }

        public void ShowForm()
        {
            drawingForm.Show();
            drawingForm.notifyIcon.ContextMenuStrip.Items[1].Text = "Свернуть";
        }

        private void UpdateAlarmBox(Alarm note)
        {
            if (note.GetPanel() == null) return;
            Control[] labelList = note.GetPanel().Controls.Find("typeLabel", true);
            if (labelList == null) return;
            if (labelList[0] == null) return;
            Label typeTextLabel = labelList[0] as Label;
            typeTextLabel.Text = FormatTime(note.leftTime);
        }

        private void MakeAlarm(Alarm note)
        {
            Control[] pictureList = note.GetPanel().Controls.Find("typePictureBox", true);
            if (pictureList == null) return;
            if (pictureList[0] == null) return;
            PictureBox typePictureBox = pictureList[0] as PictureBox;

            typePictureBox.Size = new Size(64, 64);
            typePictureBox.Location = new Point(typePictureBox.Location.X - 8, typePictureBox.Location.Y);
            typePictureBox.Image = new Bitmap(Notesieve.Properties.Resources.alert);
            drawingForm.notifyIcon.ShowBalloonTip(3000, note.Name, "Напоминание:\n" + note.Text, drawingForm.notifyIcon.BalloonTipIcon);
            ShowForm();
        }

        string FormatTime(TimeSpan time)
        {
            int day = time.Days;
            int hour = time.Hours;
            int min = time.Minutes;
            int sec = time.Seconds;

            string sHour = "";
            if (hour <= 9) sHour = "0" + hour;
            else sHour = hour.ToString();

            string sMin = "";
            if (min <= 9) sMin = "0" + min;
            else sMin = min.ToString();

            string sSec = "";
            if (sec <= 9) sSec = "0" + sec;
            else sSec = sec.ToString();

            if (day > 0)
            {
                return day + " Дней\n" + sHour + ":" + sMin + ":" + sSec;
            }
            else
            {
                return sHour + ":" + sMin + ":" + sSec;
            }
        }

    }
}
