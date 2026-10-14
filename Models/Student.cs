using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StudentApp.Models
{
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Group { get; set; }
        public double AverageScore { get; set; }

        public string EncryptedStudentCardNumber { get; set; } = "";

        [Ignore]
        [JsonIgnore]
        public string StudentCardNumber { get; set; } = "";
    }
}
