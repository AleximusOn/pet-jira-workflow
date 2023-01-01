﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pet.Jira.Application.Worklogs.Dto;
using Pet.Jira.Web.Shared;
using System.Threading.Tasks;

namespace Pet.Jira.Web.Components.Worklogs
{
    public partial class WorklogItem : ComponentBase
    {
        [Parameter] public WorklogCollectionItem Entity { get; set; }        
        [Parameter] public EventCallback<WorklogCollectionItem> OnAddPressed { get; set; }

        [CascadingParameter] public ErrorHandler ErrorHandler { get; set; }

        private async Task AddAsync()
        {
            await OnAddPressed.InvokeAsync(Entity);
        }

        public Color Color => Entity.Source == WorklogCollectionItemSource.Assignee ? Color.Primary : Color.Info;
    }
}
