﻿namespace Pet.Jira.Infrastructure.Jira.Dto
{
    public class IssueChangeLogItemDto
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public IssueChangeLogDto ChangeLog { get; set; }
        public string Author { get; set; }
    }
}
