using System;
using System.Collections.Generic;
using System.Text;

namespace StudentApp.Services
{
    public class AppStateService : IAppStateService
    {
        public string FullName { get; set; } = "";

        public string Group { get; set; } = "";

        public double AverageScore { get; set; }
    }
}