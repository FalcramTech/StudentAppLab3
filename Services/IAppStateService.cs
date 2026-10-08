using System;
using System.Collections.Generic;
using System.Text;

namespace StudentApp.Services
{
    public interface IAppStateService
    {
        string FullName { get; set; }

        string Group { get; set; }

        double AverageScore { get; set; }
    }
}