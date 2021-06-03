using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace Notesieve
{
    class SQLiteLoader
    {

        private SQLiteConnection sqlConn;
        public static string screenShotFolderPath = "";

        

        public SQLiteLoader()
        {
            string databaseName = System.Windows.Forms.Application.StartupPath + "data.db";
            CreateDataBaseFile(databaseName);
        }

        void CreateDataBaseFile(string databaseName)
        {
            screenShotFolderPath = System.Windows.Forms.Application.StartupPath + @"\" + "ScreenShots";
            DirectoryInfo sceensDir = new DirectoryInfo(screenShotFolderPath);
            if (!sceensDir.Exists)
            {
                sceensDir.Create();
            }
            DirectoryInfo tempDir = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + @"\" + "temp");
            if (!tempDir.Exists)
            {
                tempDir.Create();
            }
            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);
                sqlConn = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
                sqlConn.Open();

                SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE `notes` (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT); ", sqlConn);
                //SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE `notes` (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,`name` TEXT,`text` TEXT, `type` TEXT,`img` TEXT, `color` INTEGER, `date` TEXT); ", sqlConn);
                cmd.ExecuteNonQuery();
                cmd = new SQLiteCommand("CREATE TABLE tasks (id  INTEGER NOT NULL UNIQUE, note_id INTEGER,FOREIGN KEY(note_id) REFERENCES Notes(id) ON DELETE CASCADE,PRIMARY KEY(id AUTOINCREMENT));", sqlConn);
                cmd.ExecuteNonQuery();
                cmd = new SQLiteCommand("CREATE TABLE settings(id INTEGER UNIQUE, hideKey TEXT, screenShotKey TEXT, opacity INTEGER, PRIMARY KEY(id AUTOINCREMENT)); ", sqlConn);
                cmd.ExecuteNonQuery();
                
            }
            else
            {
                sqlConn = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
                sqlConn.Open();
            }

            Dictionary<string, string> notesColums = new Dictionary<string, string>();
            notesColums.Add("id", "INTEGER");
            notesColums.Add("name", "TEXT");
            notesColums.Add("text", "TEXT");
            notesColums.Add("type", "TEXT");
            notesColums.Add("img", "TEXT");
            notesColums.Add("color", "INTEGER");
            notesColums.Add("date", "TEXT");
            notesColums.Add("updateDate", "TEXT");
            notesColums.Add("isDelete", "INTEGER");
            notesColums.Add("alarmDate", "TEXT");
            CheckColumnsInTable("notes", notesColums);

            Dictionary<string, string> tasksColums = new Dictionary<string, string>();
            tasksColums.Add("id", "INTEGER");
            tasksColums.Add("name", "TEXT");
            tasksColums.Add("complete", "INTEGER");
            tasksColums.Add("note_id", "INTEGER");
            CheckColumnsInTable("tasks", tasksColums);

            Dictionary<string, string> settingsColums = new Dictionary<string, string>();
            settingsColums.Add("id", "INTEGER");
            settingsColums.Add("hideKey", "TEXT");
            settingsColums.Add("screenShotKey", "TEXT");
            settingsColums.Add("opacity", "INTEGER");
            settingsColums.Add("autoLogin", "INTEGER");
            CheckColumnsInTable("settings", settingsColums);

            try
            {
                SQLiteCommand cmd = new SQLiteCommand("INSERT INTO settings (id, hideKey, screenShotKey, opacity, autoLogin) VALUES ('0', 'F6', 'PrintScreen', '100', '0');", sqlConn);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            SQLiteCommand pragma = new SQLiteCommand("PRAGMA foreign_keys = true;", sqlConn);
            pragma.ExecuteNonQuery();

        }


        void CheckColumnsInTable(string tableName, Dictionary<string, string> columnAndType)
        {
            SQLiteCommand cmd2 = new SQLiteCommand("PRAGMA table_info('"+ tableName + "');", sqlConn);
            SQLiteDataReader er = cmd2.ExecuteReader();
            while (er.Read())
            {
                string name = er.GetString(1);
                if (columnAndType.ContainsKey(name)) columnAndType.Remove(name);

            }
            er.Close();


            foreach (KeyValuePair<string, string> column in columnAndType)
            {
                cmd2 = new SQLiteCommand("ALTER TABLE " + tableName + " ADD COLUMN " + column.Key + " " + column.Value + ";", sqlConn);
                cmd2.ExecuteNonQuery();
            }
        }

       

        private void CleanToDoList(ToDoList toDo)
        {
            SQLiteCommand cmd = new SQLiteCommand("DELETE FROM `tasks` WHERE `note_id` = '" + toDo.Id + "'", sqlConn);
            cmd.ExecuteNonQuery();
        }

        private void UpdateNote(Note note)
        {
            SQLiteCommand cmd = new SQLiteCommand("UPDATE notes SET `name` = @name, `text` = @text, `color` = @color, `date` = @date, `updateDate` = @updateDate, `isDelete` = @isDelete, `alarmDate` = @alarmDate WHERE `id`= @id", sqlConn);

            SQLiteParameter param = new SQLiteParameter("@name", note.Name);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@text", note.Text);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@date", note.RegDate);
            cmd.Parameters.Add(param);            
            param = new SQLiteParameter("@updateDate", note.UpdateDate);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@isDelete", note.IsDelete);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@color", note.ARGBColor);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@id", note.Id);
            cmd.Parameters.Add(param);
            if (note.GetType() == typeof(Alarm))
            {
                Alarm alarm = note as Alarm;
                param = new SQLiteParameter("@alarmDate", alarm.alarmDate);
                
            }
            else
            {
                param = new SQLiteParameter("@alarmDate", "none");
            }
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();
            if(note.GetType() == typeof(ToDoList))
            {
                ToDoList toDo = note as ToDoList;
                CleanToDoList(toDo);
                foreach (NoteTask t in toDo.Tasks)
                {
                    cmd = new SQLiteCommand("INSERT INTO `tasks` (id,name,complete,note_id) VALUES (@id, @name, @comp, @todo);", sqlConn);

                    param = new SQLiteParameter("@name", t.Name);
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@id", t.Id);
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@comp", Convert.ToInt32(t.IsComplete));
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@todo", toDo.Id);
                    cmd.Parameters.Add(param);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateSettings()
        {
            SQLiteCommand cmd = new SQLiteCommand("UPDATE settings SET `opacity` = '"+ NotesieveSettings.opacity + "', `hideKey` = '" + NotesieveSettings.keyHide + "', `screenShotKey` = '" + NotesieveSettings.keyScreenShot + "', `autoLogin` = '" + Convert.ToInt32(NotesieveSettings.autoLogin) + "' WHERE `id`='0'", sqlConn);
            cmd.ExecuteNonQuery();            
        }

        public Note SendDataToSQL(int editMode, string name, string text, int argbColor, DateTime date, DateTime updateDate, List<NoteTask> taskList)
        {
            
            if (editMode < 0)
            {
                ToDoList newNote = AddNote(name, text, argbColor, date, updateDate, typeof(ToDoList)) as ToDoList;
                foreach (NoteTask t in taskList)
                {
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO `tasks` (id,name,complete,note_id) VALUES (@id, @name, @comp, @todo);", sqlConn);

                    SQLiteParameter param = new SQLiteParameter("@name", t.Name);
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@id", t.Id);
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@comp", Convert.ToInt32(t.IsComplete));
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@todo", newNote.Id);
                    cmd.Parameters.Add(param);
                    cmd.ExecuteNonQuery();
                }
                newNote.SetTaskList(taskList);
                return newNote;
            }
            else
            {               
                ToDoList note = Note.GetNoteById(editMode) as ToDoList;
                CleanToDoList(note);
                if (note == null) return null;
                note.Name = name;
                note.Text = text;
                note.ARGBColor = argbColor;
                note.UpdateDate = updateDate;
                note.SetTaskList(taskList);
                UpdateNote(note);
                return note;
            }
        }
        
        public Note SendDataToSQL(int editMode, string name, string text, int argbColor, DateTime date, DateTime updateDate, DateTime alarmDate)
        {
            if (editMode < 0)
            {
                Alarm newNote = AddNote(name, text, argbColor, date, updateDate, typeof(Alarm), null, alarmDate) as Alarm;
                return newNote;
            }
            else
            {
                Alarm note = Note.GetNoteById(editMode) as Alarm;
                if (note == null) return null;
                note.Name = name;
                note.Text = text;
                note.ARGBColor = argbColor;
                note.alarmDate = alarmDate;
                note.UpdateDate = updateDate;
                UpdateNote(note);
                return note;
            }
        }

        public Note SendDataToSQL(int editMode, string name, string text, int argbColor, DateTime date, DateTime updateDate, Image screen, bool isLocal)
        {
            if (editMode < 0)
            {
                if (isLocal)
                {
                    string screenShotPath = screenShotFolderPath + @"\" + name + ".png";
                    if (!File.Exists(screenShotPath))
                    {
                        using (var fileStream = File.Create(screenShotPath))
                        {
                            screen.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
                Screenshot newNote = AddNote(name, text, argbColor, date, updateDate, typeof(Screenshot), screen) as Screenshot;
                return newNote;
            }
            else
            {
                Screenshot note = Note.GetNoteById(editMode) as Screenshot;
                if (note == null) return null;
                note.Name = name;
                note.Text = text;
                note.ARGBColor = argbColor;
                note.UpdateDate = updateDate;
                UpdateNote(note);
                return note;
            }
        }

        public Note SendDataToSQL(int editMode, string name, string text, int argbColor, DateTime date, DateTime updateDate)
        {
            if (editMode < 0)
            {
                Common newNote = AddNote(name, text, argbColor, date, updateDate, typeof(Common)) as Common;
                return newNote;
            }
            else
            {
                Common note = Note.GetNoteById(editMode) as Common;
                if (note == null) return null;
                note.Name = name;
                note.Text = text;
                note.ARGBColor = argbColor;
                note.UpdateDate = updateDate;
                UpdateNote(note);
                return note;
            }
        }

        private Note AddNote(string name, string text, int argbColor, DateTime date,  DateTime updateDate, Type type, Image screen = null, DateTime alarmDate = new DateTime())
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT MAX(id)+1 as nextid FROM notes", sqlConn);
            SQLiteDataReader er = cmd.ExecuteReader();
            int nextid;
            if (er.HasRows)
            {
                er.Read();
                if (er.GetFieldType(0) == typeof(System.Int64))
                {
                    nextid = er.GetInt32(0);
                }
                else
                {
                    nextid = 1;
                }
                
                er.Close();
            }
            else
            {
                nextid = 1;
            }

            string screnShotPath = screenShotFolderPath + @"\" + name + ".png";
            string noteType = type.Name;
            cmd = new SQLiteCommand(" INSERT INTO `notes` (id, name, text, type, img, color, date, updateDate, isDelete, alarmDate) VALUES (@id, @name, @text, @type, @screnShotPath, @color, @date, @updateDate, @isDelete, @alarmDate);", sqlConn);

            SQLiteParameter param = new SQLiteParameter("@name", name);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@text", text);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@type", noteType);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@screnShotPath", screnShotPath);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@date", date);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@updateDate", updateDate);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@isDelete", false);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@color", argbColor);
            cmd.Parameters.Add(param);
            param = new SQLiteParameter("@id", nextid);
            cmd.Parameters.Add(param);
            if (noteType.Equals("Alarm"))
            {
                param = new SQLiteParameter("@alarmDate", alarmDate);
            }
            else
            {
                param = new SQLiteParameter("@alarmDate", "none");
            }
            cmd.Parameters.Add(param);



            cmd.ExecuteNonQuery();
           
            Note newNote = null;
            switch (noteType)
            {
                case "Common":
                    newNote = new Common();
                    break;
                case "Screenshot":
                    newNote = new Screenshot(screen, screnShotPath);
                    break;
                case "Alarm":
                    newNote = new Alarm(alarmDate);                    
                    break;
                case "ToDoList":
                    newNote = new ToDoList();
                    break;
                default:
                    newNote = new Common();
                    break;

            }
            newNote.Id = nextid;
            newNote.Name = name;
            newNote.Text = text;
            newNote.RegDate = Convert.ToDateTime(date);        
            newNote.UpdateDate = Convert.ToDateTime(updateDate);        
            newNote.ARGBColor = argbColor;
            //string img = er.GetString(4);
            //string timer = er.GetString(5);
            return newNote;
        }

        public string DeleteFromSQL(Note badNote)
        {
           string badPath = null;
           if (badNote.GetType() == typeof(Screenshot))
           {
                Screenshot badScreen = badNote as Screenshot;
                badPath = badScreen.ImgPath;
                if(badScreen.GetImg() != null)
                {
                    try
                    {
                        badScreen.Dispose();
                        badScreen = null;
                        GC.Collect();
                        File.Delete(badPath);
                    }
                    catch
                    {

                    }
                }
           }

            badNote.IsDelete = true;
            UpdateNote(badNote);
            return badPath;
        }

        public void UpdateTask(NoteTask task)
        {
            SQLiteCommand cmd = new SQLiteCommand("UPDATE tasks SET `complete` = '"+ Convert.ToInt32(task.IsComplete) +"' WHERE `id`=" + task.Id, sqlConn);
            cmd.ExecuteNonQuery();
        }

        public void LoadSettingsFromSQLite()
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT id,hideKey,screenShotKey,opacity,autoLogin FROM settings WHERE `id`= 0", sqlConn);
            SQLiteDataReader er = cmd.ExecuteReader();            
            if(er.HasRows)
            {
                er.Read();
                string hideKey = "F3";
                try { hideKey = er.GetString(1); } catch { }                   
                string screenShotKey = "PrintScreen";
                try { screenShotKey = er.GetString(2); } catch { }
                int opacity = 100;
                try { opacity = er.GetInt32(3); } catch { }
                bool autoLogin = false;
                try {                     
                    autoLogin = Convert.ToBoolean(er.GetInt32(4)); 
                } 
                catch 
                { 
                }
                NotesieveSettings.SetSettings(opacity, hideKey, screenShotKey, autoLogin);
            }
            else
            {
                NotesieveSettings.SetSettings(100, "F6", "PrintScreen", false);
            }            
        }
        public List<Note> LoadNotesFromSQLite()
        {
            List<Note> noteList = new List<Note>();
            SQLiteCommand cmd = new SQLiteCommand("SELECT id, name, text, type, img, color, date, updateDate, isDelete, alarmDate FROM Notes", sqlConn);
            SQLiteDataReader er = cmd.ExecuteReader();
            while (er.Read())
            {
                int id;
                string name = "";
                string text = "";
                string type = "";
                string imgPath = "";
                Image screenShot;
                int color = 0;
                string date = null;
                string updateDate = null;
                int isDelete = 0;
                string alarmDate = null;


                id = er.GetInt32(0);
                try { name = er.GetString(1); } catch { }
                try { text = er.GetString(2);} catch { }
                try { type = er.GetString(3); } catch { }
                try { imgPath = er.GetString(4); } catch { }
                try
                {
                   screenShot = Image.FromFile(imgPath);
                }
                catch
                {
                    screenShot = null;
                }
                try { color = er.GetInt32(5); } catch { }
                try { date = er.GetString(6); } catch { }
                try { updateDate = er.GetString(7); } catch { }
                try { isDelete = er.GetInt32(8); } catch { }
                try { 
                    alarmDate = er.GetString(9); 
                } 
                catch (Exception e)
                {
                   
                }

                Note newNote = null;
                switch (type)
                {
                    case "Screenshot":
                        newNote = new Screenshot(screenShot, imgPath);
                        break;
                    case "Common":
                        newNote = new Common();
                        break;
                    case "Alarm":
                        newNote = new Alarm(Convert.ToDateTime(alarmDate));
                        break;
                    case "ToDoList":
                        newNote = new ToDoList();
                        break;
                    default:
                        newNote = new Common();
                        break;
                }
                newNote.Id = id;
                newNote.Name = name;
                newNote.Text = text;
                //newNote.Img = img;
                newNote.ARGBColor = color;                
                newNote.RegDate = Convert.ToDateTime(date);
                newNote.UpdateDate = Convert.ToDateTime(updateDate);
                newNote.IsDelete = Convert.ToBoolean(isDelete);
                noteList.Add(newNote);
            }
            er.Close();

            foreach (Note n in noteList)
            {
                if (n.GetType() != typeof(ToDoList)) continue;
                ToDoList toDo = n as ToDoList;
                List<NoteTask> taskList = new List<NoteTask>();
                cmd = new SQLiteCommand("SELECT id,name,complete,note_id FROM tasks WHERE `note_id` = '"+ n.Id +"'", sqlConn);
                er = cmd.ExecuteReader();
                while (er.Read())
                {
                    int id = er.GetInt32(0);
                    string name = er.GetString(1);
                    bool isComplete = Convert.ToBoolean(er.GetInt32(2));
                    NoteTask newTask = new NoteTask(id, name, isComplete);
                    taskList.Add(newTask);
                }
                toDo.SetTaskList(taskList); 
                er.Close();

            }

            return noteList;
        }

    }
}
