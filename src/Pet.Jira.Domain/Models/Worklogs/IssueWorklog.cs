﻿using Pet.Jira.Domain.Models.Issues;
using System;

namespace Pet.Jira.Domain.Models.Worklogs
{
    public class IssueWorklog : IWorklog
    {
        public DateTime StartDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public IIssue Issue { get; set; }
        public string Author { get; set; }
        public WorklogSource Source { get; set; }
    }
}
