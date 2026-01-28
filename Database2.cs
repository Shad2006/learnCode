using System.Data;
using System.IO;
using System.Xml;
using System;

public class Database2
{
    DataSet dataSet = new DataSet("LearningApp");
    string dataFile = "learning.xml";

    public Database2()
    {
        if (File.Exists(dataFile))
        {
            try
            {
                dataSet.ReadXml(dataFile);
            }
            catch
            {
                // Если файл поврежден, не перезаписываем его
                // Просто создаем пустой DataSet
                CreateEmptyTables();
            }
        }
        else
        {
            // Файла нет - создаем пустые таблицы
            CreateEmptyTables();
            Save();
        }
    }

    private void CreateEmptyTables()
    {
        // Создаем таблицу Users
        if (!dataSet.Tables.Contains("Users"))
        {
            DataTable users = new DataTable("Users");
            users.Columns.Add("id", typeof(int));
            users.Columns.Add("fio", typeof(string));
            users.Columns.Add("level", typeof(string));
            users.Columns.Add("course", typeof(string));
            users.Columns.Add("experience", typeof(int));
            users.PrimaryKey = new DataColumn[] { users.Columns["id"] };
            dataSet.Tables.Add(users);
        }

        // Создаем таблицу Lessons
        if (!dataSet.Tables.Contains("Lessons"))
        {
            DataTable lessons = new DataTable("Lessons");
            lessons.Columns.Add("lesson_id", typeof(int));
            lessons.Columns.Add("lesson_number", typeof(int));
            lessons.Columns.Add("lesson_name", typeof(string));
            lessons.Columns.Add("course", typeof(string));
            lessons.Columns.Add("theory_text", typeof(string));
            lessons.Columns.Add("is_locked", typeof(bool));
            lessons.Columns.Add("experience_required", typeof(int));
            lessons.PrimaryKey = new DataColumn[] { lessons.Columns["lesson_id"] };
            dataSet.Tables.Add(lessons);
        }

        // Создаем таблицу Questions
        if (!dataSet.Tables.Contains("Questions"))
        {
            DataTable questions = new DataTable("Questions");
            questions.Columns.Add("question_id", typeof(int));
            questions.Columns.Add("lesson_number", typeof(int));
            questions.Columns.Add("course", typeof(string));
            questions.Columns.Add("question_text", typeof(string));
            questions.Columns.Add("question_type", typeof(string));
            questions.Columns.Add("correct_answer", typeof(string));
            questions.Columns.Add("options", typeof(string));
            questions.PrimaryKey = new DataColumn[] { questions.Columns["question_id"] };
            dataSet.Tables.Add(questions);
        }
    }

    public void Save()
    {
        try
        {
            dataSet.WriteXml(dataFile, XmlWriteMode.WriteSchema);
        }
        catch
        {
            // Игнорируем ошибки записи
        }
    }

    public DataTable GetTable(string tableName)
    {
        return dataSet.Tables.Contains(tableName) ? dataSet.Tables[tableName] : null;
    }

    public void AddUser(string fio, string level, string course)
    {
        DataTable users = GetTable("Users");
        if (users == null) return;

        int newId = 1;
        if (users.Rows.Count > 0)
        {
            // Находим максимальный ID
            foreach (DataRow userRow in users.Rows)
            {
                int currentId = (int)userRow["id"];
                if (currentId >= newId)
                {
                    newId = currentId + 1;
                }
            }
        }

        DataRow newRow = users.NewRow();
        newRow["id"] = newId;
        newRow["fio"] = fio;
        newRow["level"] = level;
        newRow["course"] = course;
        newRow["experience"] = 0;
        users.Rows.Add(newRow);
        Save();
    }

    public DataRow[] GetLessons(string course)
    {
        DataTable lessons = GetTable("Lessons");
        if (lessons == null) return new DataRow[0];

        return lessons.Select($"course = '{course.Replace("'", "''")}'", "lesson_number");
    }

    public DataRow[] GetQuestions(int lessonNumber, string course)
    {
        DataTable questions = GetTable("Questions");
        if (questions == null) return new DataRow[0];

        return questions.Select($"lesson_number = {lessonNumber} AND course = '{course.Replace("'", "''")}'");
    }

    public void UpdateUserExperience(int experience)
    {
        DataTable users = GetTable("Users");
        if (users == null || users.Rows.Count == 0) return;

        users.Rows[0]["experience"] = (int)users.Rows[0]["experience"] + experience;
        Save();
    }

    public void UnlockLesson(int lessonNumber, string course)
    {
        DataTable lessons = GetTable("Lessons");
        if (lessons == null) return;

        foreach (DataRow lesson in lessons.Rows)
        {
            if ((int)lesson["lesson_number"] == lessonNumber && lesson["course"].ToString() == course)
            {
                lesson["is_locked"] = false;
                Save();
                break;
            }
        }
    }
}