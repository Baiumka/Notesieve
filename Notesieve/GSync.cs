using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Notesieve
{
    class GSync
    {
        public delegate void SuccsesfullLogin(string name, string photoUrl);
        public delegate void FailedLogin();
        public delegate void LogoutHandler();
        public delegate void SyncHendler(gSyncState state);
        public delegate void ErrorHandler(string error);

        public event SuccsesfullLogin OnLoginSuccsesfull;
        public event FailedLogin OnLoginFailed;
        public event LogoutHandler OnLogout;
        public event ErrorHandler OnError;
        public event SyncHendler OnSyncStateChanged;

        public enum gSyncState
        {
            eInWork,            
            eNotFound,
            eSuccessPut,
            eSuccessGet,
            eFailed,
            eChecking,
            eLostConnection
        }


        string[] scopes = { DriveService.Scope.DriveFile, PeopleServiceService.Scope.UserinfoProfile, DriveService.Scope.DriveAppdata };
        string applicationName = "Notesieve";
        string tableName = "NotesiveAppTable";

        string jsonFileName = "SyncSettings_v003.json";
        string jsonFileLocalPath = System.Windows.Forms.Application.StartupPath + "temp" + @"\" + "gSyncSettings_Temp.json";
        string jsonDownloadFileLocalPath = System.Windows.Forms.Application.StartupPath +  "temp" + @"\" + "files_temp.json";       
        UserCredential credential;
        CancellationTokenSource source;
        CancellationToken token;
        bool isAlreadyInLoginProcess;
        bool isLogin;
        SheetsService spreadSheetsService;
        DriveService driveService;        
        JsonSerializerSettings jsonSettings;

        GSyncSettings gSyncSettings;
        string gSyncSettingFileId = "none";

        public GSync()
        {
            jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All

            };            
        }

        bool IsSyncAvaible()
        {
            if (credential == null)
                return false;
            if (isLogin == false)
                return false;
            if (isAlreadyInLoginProcess == true)
                return false;


            return true;
        }

        private void UpdateDataVersion()
        {
            if (gSyncSettings == null) return;

            if(!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return;
            }
            gSyncSettings.dataVersion++;

            try
            {
                string tempLocalPath = jsonFileLocalPath + DateTime.Now.TimeOfDay;
                tempLocalPath = tempLocalPath.Replace(":", "_");

                using (StreamWriter sw = new StreamWriter(tempLocalPath, false, System.Text.Encoding.Default))
                {
                    string json = JsonConvert.SerializeObject(gSyncSettings);
                    sw.WriteLine(json);
                }

            
                Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File();

                FilesResource.UpdateMediaUpload newRequest;
                using (var stream = new System.IO.FileStream(tempLocalPath, System.IO.FileMode.Open))
                {
                       newRequest = driveService.Files.Update(file, gSyncSettingFileId, stream, "application/json");
                    newRequest.Fields = "id";              
                    newRequest.Upload();
                    
                }
            
                File.Delete(tempLocalPath);
            }
            catch (System.Net.Http.HttpRequestException e)
            {                
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);                
            }
            catch
            {

            }
        }

        private void UploadScreenshot(Screenshot screenshot)
        {
            if (screenshot.IsDelete || screenshot == null || screenshot.ImgPath == null) return;
            if (!File.Exists(screenshot.ImgPath)) return;

            if (!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return;
            }

            if (driveService == null)
            {
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }
            try
            {
                string screenshotName = Path.GetFileName(screenshot.ImgPath);
                var request = driveService.Files.List();
                request.Spaces = "appDataFolder";
                request.Fields = "nextPageToken, files(id, name)";
                request.Q = "name = '" + screenshotName + "'";
                var result = request.Execute();
                bool found = false;
                foreach (var file in result.Files)
                {
                    if (file.Name.Equals(screenshotName))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = screenshotName,
                        Parents = new List<string>()
                    {
                    "appDataFolder"
                    }
                    };
                    FilesResource.CreateMediaUpload newRequest;
                    using (var stream = new System.IO.FileStream(screenshot.ImgPath, System.IO.FileMode.Open))
                    {
                        newRequest = driveService.Files.Create(fileMetadata, stream, "image/png");
                        newRequest.Fields = "id";
                        newRequest.Upload();
                    }
                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
        }

        private void DeleteScreenshot(Screenshot screenshot, string screenShotPath)
        {
            if (screenshot == null) return;
            if (screenShotPath == null) return;
           // if (!File.Exists(screenShotPath)) return;

            if (!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return;
            }

            if (driveService == null)
            {
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }
            try
            {
                string screenshotName = Path.GetFileName(screenShotPath);
                var request = driveService.Files.List();
                request.Spaces = "appDataFolder";
                request.Fields = "nextPageToken, files(id, name)";
                request.Q = "name = '" + screenshotName + "'";
                var result = request.Execute();
                foreach (var file in result.Files)
                {
                    if (file.Name.Equals(screenshotName))
                    {
                        try
                        {
                            driveService.Files.Delete(file.Id).Execute();
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
        }

        
        public bool CheckForUpdates()
        {
            OnSyncStateChanged?.Invoke(gSyncState.eChecking);
            if (gSyncSettingFileId.Equals("none")) return false;

            if (!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return false;
            }

            if (driveService == null)
            {
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }

            try
            {                
                using (var stream = new System.IO.FileStream(jsonDownloadFileLocalPath, System.IO.FileMode.Create))
                {
                    driveService.Files.Get(gSyncSettingFileId).DownloadWithStatus(stream);
                }
                bool result;
                using (StreamReader sr = new StreamReader(jsonDownloadFileLocalPath))
                {
                    string json = sr.ReadToEnd();
                    GSyncSettings loadedGSyncSettings = JsonConvert.DeserializeObject<GSyncSettings>(json);
                    if (loadedGSyncSettings.dataVersion > gSyncSettings.dataVersion)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                File.Delete(jsonDownloadFileLocalPath);
                return result;
                              
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
                return false;
            }
            catch (Exception e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                OnError?.Invoke(e.Message);
                return false;
            }            
        }

        private bool CheckEthernetConnection()
        {
            string strServer = "http://www.google.com";
            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create(strServer);

                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();
                if (HttpStatusCode.OK == rspFP.StatusCode)
                {
                    // HTTP = 200 - Интернет безусловно есть!
                    rspFP.Close();
                    return true;
                }
                else
                {
                    // сервер вернул отрицательный ответ, возможно что инета нет
                    rspFP.Close();
                    return false;
                }
            }
            catch (WebException e)
            {
                // Ошибка, значит интернета у нас нет. Плачем :'(
                return false;
            }
        }

        public void FilesList()
        {
            var request = driveService.Files.List();
            request.Spaces = "appDataFolder";
            request.Fields = "nextPageToken, files(id, name)";
            request.Q = "mimeType='image/png'";
            var result = request.Execute();
            string list = "";
            int i = 1;
            foreach (var file in result.Files)
            {
                list += i + ". " + file.Name + "\n";
                //driveService.Files.Delete(file.Id).Execute();
                i++;
            }
            OnError?.Invoke(list);

        }


        public void DoSyncNow(Worker worker)
        {
            if (!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return;
            }
            bool isConnectionAvaible = CheckEthernetConnection();
            if (isConnectionAvaible)
            {
                try
                {
                    OnSyncStateChanged?.Invoke(gSyncState.eInWork);

                    List<Note> loadedNotes = GetAllNotes();
                    if (loadedNotes != null)
                    {
                        foreach (Note s in loadedNotes)
                        {
                            if (s != null)
                            {
                                worker.CheckAndAdd(s);
                            }
                        }
                        UpdateTable();
                        //FilesList();
                    }
                    OnSyncStateChanged?.Invoke(gSyncState.eSuccessGet);
                }
                catch (System.Net.Http.HttpRequestException e)
                {
                    OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
                }
            }
            else
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
        }


        public void CancelLogin()
        {
            if (source != null)
            {
                source.Cancel();
                source.Dispose();
            }
            OnSyncStateChanged?.Invoke(gSyncState.eFailed);
        }

        private List<Note> GetAllNotes()
        {
            List<Note> answer = null;
            if (!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return answer;
            }

            if (driveService == null)
            {
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }
            try
            {
                var request = driveService.Files.List();
                request.Spaces = "appDataFolder";
                request.Fields = "nextPageToken, files(id, name)";
                request.Q = "name = '" + jsonFileName + "'";
                var result = request.Execute();

                bool found = false;
                foreach (var file in result.Files)
                {
                    if (file.Name.Equals(jsonFileName))
                    {
                        found = true;
                        gSyncSettingFileId = file.Id;

                        using (var stream = new System.IO.FileStream(jsonDownloadFileLocalPath, System.IO.FileMode.Create))
                        {
                            driveService.Files.Get(gSyncSettingFileId).DownloadWithStatus(stream);
                        }
                        using (StreamReader sr = new StreamReader(jsonDownloadFileLocalPath))
                        {
                            string json = sr.ReadToEnd();
                            gSyncSettings = JsonConvert.DeserializeObject<GSyncSettings>(json);
                            answer = ReadNotesFromTable();
                        }
                        File.Delete(jsonDownloadFileLocalPath);

                        break;
                    }

                }
                if (!found)
                {
                    CreateTable(tableName);
                }
                return answer;
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
                return null;
            }

        }

        List<Note> ReadNotesFromTable()
        {
            List<Note> answer = new List<Note>();
            if (spreadSheetsService == null)
            {
                spreadSheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }

            try
            {
                SpreadsheetsResource.ValuesResource.GetRequest request = spreadSheetsService.Spreadsheets.Values.Get(gSyncSettings.spreadshetrID, "Notesieve!A1:B");
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        try
                        {
                            string json = row[0].ToString();
                            Note note = JsonConvert.DeserializeObject<Note>(json, jsonSettings);
                            answer.Add(note);

                            if(note.GetType() == typeof(Screenshot) && note.IsDelete == false)
                            {
                                Screenshot screenshot = note as Screenshot;
                                string screenshotName = Path.GetFileName(screenshot.ImgPath);
                                var requestFiles = driveService.Files.List();
                                requestFiles.Spaces = "appDataFolder";
                                requestFiles.Fields = "nextPageToken, files(id, name)";
                                requestFiles.Q = "name = '" + screenshotName + "'";
                                var result = requestFiles.Execute();
                                foreach (var file in result.Files)
                                {
                                    if (file.Name.Equals(screenshotName))
                                    {
                                        try
                                        {                                         
                                            string screenShotPath = SQLiteLoader.screenShotFolderPath + @"\" + screenshotName;
                                            if (!File.Exists(screenShotPath))
                                            {
                                                using (var stream = new System.IO.FileStream(screenShotPath, System.IO.FileMode.Create))
                                                {
                                                    driveService.Files.Get(file.Id).DownloadWithStatus(stream);
                                                }
                                            }
                                            screenshot.ChangeImageByPath(screenShotPath);
                                        }
                                        catch
                                        {
                                            
                                        }
                                        break;
                                    }
                                }
                            }

                        }
                        catch (JsonReaderException je)
                        {
                            OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                            OnError?.Invoke("Ваши данные на Google Drive были кем-то затронуты и повреждены.");
                        }
                        

                    }
                }
                else
                {
                    return new List<Note>();//Данные не найдены, возвращаем пустой список
                }
            }
            catch (Google.GoogleApiException gExc)
            {
                if (gExc.Error != null)
                {
                    if (gExc.Error.Code == 404)
                    {
                        OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                        OnError?.Invoke("Таблица с записями не найдена на вашем Google Drive");
                        CreateTable(tableName);
                    }
                    else
                    {
                        OnError?.Invoke(gExc.Message);
                    }
                }
            }
           
            return answer;
        }

        void ChangeTextInCell(string cell, Note note, string screenShotPath)
        {          
            try
            {
                IList<IList<object>> insertValues = new List<IList<object>>();

                IList<object> ROW = new List<object>();
                string json = JsonConvert.SerializeObject(note, jsonSettings);
                ROW.Add(json);
                insertValues.Add(ROW);

                ValueRange body = new ValueRange();
                body.Values = insertValues;
                SpreadsheetsResource.ValuesResource.UpdateRequest result = spreadSheetsService.Spreadsheets.Values.Update(body, gSyncSettings.spreadshetrID, cell);
                result.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                result.Execute();

                if (note.GetType() == typeof(Screenshot) && note.IsDelete == true)
                {
                    Screenshot screen = note as Screenshot;
                    DeleteScreenshot(screen, screenShotPath);
                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
            catch
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
            }
        }

        public async void AddNewNoteToTableAsync(Note newNote)
        {
            await Task.Run(() => AddNewNoteToTable(newNote));
        }

        private void AddNewNoteToTable(Note newNote)
        {
            try
            {
                if (gSyncSettings == null)
                {
                    return;
                }
                if (!IsSyncAvaible())
                {
                    OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                    return;
                }

                OnSyncStateChanged?.Invoke(gSyncState.eInWork);

                if (spreadSheetsService == null)
                {

                    spreadSheetsService = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName,
                    });
                }

                IList<IList<object>> insertValues = new List<IList<object>>();

                IList<object> ROW = new List<object>();
                ROW.Add(JsonConvert.SerializeObject(newNote, jsonSettings));
                insertValues.Add(ROW);
                if (newNote.GetType() == typeof(Screenshot))
                {
                    Screenshot screen = newNote as Screenshot;
                    UploadScreenshot(screen);
                }

                ValueRange body = new ValueRange();
                body.Values = insertValues;
                SpreadsheetsResource.ValuesResource.AppendRequest result = spreadSheetsService.Spreadsheets.Values.Append(body, gSyncSettings.spreadshetrID, "Notesieve!A1:B");
                result.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                result.Execute();
                UpdateDataVersion();

                OnSyncStateChanged?.Invoke(gSyncState.eSuccessPut);
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
            catch
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
            }
        }

        public async void UpdateNoteInTableAsync(Note updateNote, string screenShotPath = null)
        {
            await Task.Run(() => UpdateNoteInTable(updateNote, screenShotPath));
        }
        private void UpdateNoteInTable(Note updateNote, string screenShotPath)
        {

            if (!IsSyncAvaible())
            {
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                return;
            }

            if (spreadSheetsService == null)
            {

                spreadSheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }

            OnSyncStateChanged?.Invoke(gSyncState.eInWork);

            try
            {
                SpreadsheetsResource.ValuesResource.GetRequest request = spreadSheetsService.Spreadsheets.Values.Get(gSyncSettings.spreadshetrID, "Notesieve!A1:B");
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    int counter = 1;
                    foreach (var row in values)
                    {
                        try
                        {                            
                            string json = row[0].ToString();
                            Note readNote = JsonConvert.DeserializeObject<Note>(json, jsonSettings);
                            if (readNote.Id == updateNote.Id && readNote.RegDate == updateNote.RegDate)
                            {
                                ChangeTextInCell("Notesieve!A" + counter, updateNote, screenShotPath);
                                break;
                            }
                            counter++;
                            
                        }
                        catch (JsonReaderException je)
                        {
                            OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                            OnError?.Invoke("Ваши данные на Google Drive были кем-то затронуты и повреждены.");
                        }

                    }
                }
                else
                {

                }

                UpdateDataVersion();
                OnSyncStateChanged?.Invoke(gSyncState.eSuccessPut);
            }
            catch (Google.GoogleApiException gExc)
            {
                if (gExc.Error != null)
                {
                    if (gExc.Error.Code == 404)
                    {
                        OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                        OnError?.Invoke("Таблица с записями не найдена на вашем Google Drive");
                        CreateTable(tableName);
                    }
                    else
                    {
                        OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                        OnError?.Invoke(gExc.Message);
                    }
                }
                OnSyncStateChanged?.Invoke(gSyncState.eFailed);
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
        }

        private void UpdateTable()
        {
            try
            {
                if (spreadSheetsService == null)
                {

                    spreadSheetsService = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName,
                    });
                }

                IList<IList<object>> insertValues = new List<IList<object>>();

                foreach (Note note in Note.NoteList)
                {
                    IList<object> ROW = new List<object>();
                    ROW.Add(JsonConvert.SerializeObject(note, jsonSettings));
                    insertValues.Add(ROW);
                    if (note.GetType() == typeof(Screenshot))
                    {
                        Screenshot screen = note as Screenshot;
                        UploadScreenshot(screen);
                    }
                }

                ClearValuesRequest clearValuesRequest = new ClearValuesRequest();
                ValueRange body = new ValueRange();
                body.Values = insertValues;

                SpreadsheetsResource.ValuesResource.ClearRequest clearResult = spreadSheetsService.Spreadsheets.Values.Clear(clearValuesRequest, gSyncSettings.spreadshetrID, "Notesieve!A1:B");
                SpreadsheetsResource.ValuesResource.UpdateRequest result = spreadSheetsService.Spreadsheets.Values.Update(body, gSyncSettings.spreadshetrID, "Notesieve!A1:B");
                clearResult.Execute();
                result.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                result.Execute();
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }
   

        }

        void CreateTable(string tableName)
        {

            if (spreadSheetsService == null)
            {
                spreadSheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }
            try
            {

                // TODO: Assign values to desired properties of `requestBody`:
                Google.Apis.Sheets.v4.Data.Spreadsheet requestBody = new Google.Apis.Sheets.v4.Data.Spreadsheet();
                requestBody.Properties = new SpreadsheetProperties();
                requestBody.Properties.Title = tableName;
                List<Sheet> sheets = new List<Sheet>();
                Sheet mainSheet = new Sheet();
                mainSheet.Properties = new SheetProperties();
                mainSheet.Properties.Title = "Notesieve";
                sheets.Add(mainSheet);
                requestBody.Sheets = sheets;

                SpreadsheetsResource.CreateRequest request = spreadSheetsService.Spreadsheets.Create(requestBody);
                // To execute asynchronously in an async method, replace `request.Execute()` as shown:
                Google.Apis.Sheets.v4.Data.Spreadsheet response = request.Execute();
                // Data.Spreadsheet response = await request.ExecuteAsync();

                // TODO: Change code below to process the `response` object:
                // Console.WriteLine();


                if (driveService == null)
                {
                    driveService = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName,
                    });
                }

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = jsonFileName,
                    Parents = new List<string>()
                {
                    "appDataFolder"
                }
                };

                using (StreamWriter sw = new StreamWriter(jsonFileLocalPath, false, System.Text.Encoding.Default))
                {
                    if (gSyncSettings == null) gSyncSettings = new GSyncSettings();
                    gSyncSettings.spreadshetrID = response.SpreadsheetId;
                    gSyncSettings.dataVersion = 0;

                    sw.WriteLine(JsonConvert.SerializeObject(gSyncSettings));
                }

                FilesResource.CreateMediaUpload newRequest;
                using (var stream = new System.IO.FileStream(jsonFileLocalPath, System.IO.FileMode.Open))
                {
                    newRequest = driveService.Files.Create(fileMetadata, stream, "application/json");
                    newRequest.Fields = "id";
                    newRequest.Upload();

                    gSyncSettingFileId = newRequest.ResponseBody.Id;
                }


                File.Delete(jsonFileLocalPath);
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
            }


        }

        public void Logout()
        {
            if (!isLogin)
            {
                OnError?.Invoke("Вы еще не авторизировались.");
            }
            else
            {
                string path = "token.json";
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                isLogin = false;  
                credential = null;
                isAlreadyInLoginProcess = false;
                OnLogout?.Invoke();
            }
        }

        public async Task Login()
        {
            if (isAlreadyInLoginProcess || credential != null)
            {
                CancelLogin();
            }
            else
            {
                isAlreadyInLoginProcess = true;
                source = new CancellationTokenSource(new TimeSpan(0, 1, 0));
                token = source.Token;

                FileStream stream = new FileStream(System.Windows.Forms.Application.StartupPath  + "credentials.json", FileMode.Open, FileAccess.Read);
                string credPath = "token.json";
                await Task.Run(() =>
                {
                    try
                    {
                        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        "NotesieveDesktopUser",
                        token,
                        new FileDataStore(credPath, true)).Result;
                    }
                    catch (System.AggregateException e)
                    {
                        if (token.IsCancellationRequested)
                        {
                            //Операция прервана

                        }
                        else
                        {
                            OnError?.Invoke(e.InnerException + " " + e.Message);
                            
                        }
                        isAlreadyInLoginProcess = false;
                    }
                    catch (System.Net.Http.HttpRequestException e)
                    {
                        isAlreadyInLoginProcess = false;
                        OnSyncStateChanged?.Invoke(gSyncState.eFailed);
                        OnError?.Invoke("Проблема с интрнєт-соединением.\nВы можете продолжать использовать Notesieve но временно, не сможете его синхронизировать.");
                        
                    }

                });
                if (credential == null)
                {
                    isAlreadyInLoginProcess = false;
                    OnLoginFailed?.Invoke();

                }
                else
                {
                    try
                    {
                        isAlreadyInLoginProcess = false;
                        PeopleServiceService peopleService = new PeopleServiceService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = applicationName,
                        });
                        isLogin = true;
                        PeopleResource.GetRequest peopleRequest = peopleService.People.Get("people/me");
                        peopleRequest.PersonFields = "names,photos";
                        Person profile = peopleRequest.Execute();
                        string name = profile.Names[0].DisplayName;
                        string photo = profile.Photos[0].Url;
                        OnLoginSuccsesfull?.Invoke(name, photo);
                    }
                    catch (System.Net.Http.HttpRequestException e)
                    {
                        isAlreadyInLoginProcess = false;
                        OnSyncStateChanged?.Invoke(gSyncState.eLostConnection);
                        OnError?.Invoke("Проблема с интрнєт-соединением.\nВы можете продолжать использовать Notesieve но временно, не сможете его синхронизировать.");

                    }
                }
            }
        }
    }

    class GSyncSettings 
    {
        public string spreadshetrID = "";
        public int dataVersion = 0;
        public GSyncSettings()
        {

        }
    }

    
}
