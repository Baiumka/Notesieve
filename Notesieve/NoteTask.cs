using System.Collections.Generic;

namespace Notesieve
{
    class NoteTask
    {
        private bool isComplete;
        private string name;
        private int id;

        public delegate void CompleteChangeHandler(NoteTask task);
        public event CompleteChangeHandler CompleteChenge;

        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }
        public bool IsComplete { get => isComplete; set => ChangeComplete(value); }
        
        public static int MaxID = 0;
        private static List<NoteTask> taskList = new List<NoteTask>();

        
        public NoteTask (int id, string name, bool isComplete)
        {
            this.name = name;
            this.isComplete = isComplete;
            this.id = id;
            if (id > MaxID) MaxID = id;

            taskList.Add(this);
        }

        public void GotNewID()
        {
            this.id = MaxID + 1;
            MaxID = this.id;
        }
        void ChangeComplete(bool isComplete)
        {
            this.isComplete = isComplete;
            CompleteChenge?.Invoke(this);
        }
        public static NoteTask GetTaskById(int id)
        {
            foreach (NoteTask t in taskList)
            {
                if (t.id == id) return t;
            }
            return null;
        }
    }
}
