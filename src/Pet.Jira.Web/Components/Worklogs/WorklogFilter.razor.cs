﻿using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pet.Jira.Application.Authentication;
using Pet.Jira.Application.Issues.Queries;
using Pet.Jira.Application.Storage;
using Pet.Jira.Application.Worklogs.Dto;
using Pet.Jira.Application.Worklogs.Queries;
using Pet.Jira.Domain.Models.Issues;
using Pet.Jira.Infrastructure.Jira;
using Pet.Jira.Web.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pet.Jira.Web.Components.Worklogs
{
    public partial class WorklogFilter : ComponentBase
    {
        private readonly ComponentModel _model = ComponentModel.Create();

        [Parameter] public EventCallback<GetWorklogCollection.Query> OnSearchPressed { get; set; }
        [Inject] private IMediator Mediator { get; set; }
        [CascadingParameter] public ErrorHandler ErrorHandler { get; set; }
        [Inject] private IStorage<string, UserWorklogFilter> _filterStorage { get; set; }
        [Inject] private IIdentityService _identityService { get; set; }

        protected async Task Search()
        {
            try
            {
                await SaveFilterAsync();
                await OnSearchPressed.InvokeAsync(new GetWorklogCollection.Query()
                {
                    StartDate = _model.Filter.StartDate.Value,
                    EndDate = _model.Filter.EndDate.Value.AddDays(1).AddMinutes(-1),
                    DailyWorkingStartTime = _model.Filter.DailyWorkingStartTime.Value,
                    DailyWorkingEndTime = _model.Filter.DailyWorkingEndTime.Value,
                    IssueStatusId = _model.Filter.IssueStatus.Id,
                    CommentWorklogTime = _model.Filter.CommentWorklogTime.Value,
                    LunchTime = _model.Filter.LunchTime.Value,
                });
            }
            catch (Exception e)
            {
                ErrorHandler.ProcessError(e);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var user = await _identityService.GetCurrentUserAsync();
            if (user != null)
            {
                var filter = await _filterStorage.GetValueAsync(user.Key);
                _model.Filter.Initialize(filter);
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await RenderFilterAsync();
                StateHasChanged();
            }
            else
            {
                await SaveFilterAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task RenderFilterAsync()
        {
            if (_model.Filter.IsInitialized)
            {
                return;
            }
            var user = await _identityService.GetCurrentUserAsync();
            var filter = await _filterStorage.GetValueAsync(user?.Key);
            _model.Filter.Initialize(filter);
            await _filterStorage.UpdateAsync(user?.Key, filter);
        }

        private async Task SaveFilterAsync()
        {
            var user = await _identityService.GetCurrentUserAsync();
            var filter = _model.Filter.Convert();
            filter.Username = user?.Key;
            await _filterStorage.UpdateAsync(user?.Key, filter);
        }

        private async Task<IEnumerable<IssueStatus>> SearchIssueStatuses(string value)
        {
            try
            {
                var result = await Mediator.Send(new GetIssueStatuses.Query());

                if (string.IsNullOrEmpty(value)
                    || value.Equals(_model.Filter.IssueStatus?.Name, StringComparison.InvariantCultureIgnoreCase))
                    return result.IssueStatuses;
                return result.IssueStatuses
                    .Where(status => status.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                ErrorHandler.ProcessError(e);
                return await Task.FromResult(_model.Filter.IssueStatus != null
                    ? new List<IssueStatus> { _model.Filter.IssueStatus }
                    : Enumerable.Empty<IssueStatus>());
            }
        }

        public class ComponentModel
        {
            public static ComponentModel Create()
            {
                return new ComponentModel();
            }

            public FilterModel Filter { get; set; } = new FilterModel();
        }

        public class FilterModel
        {
            [Required]
            [JsonIgnore]
            public DateRange DateRange { get; set; } = new DateRange(DateTime.Now.AddDays(-7).Date, DateTime.Now.Date);

            [Required] 
            public DateTime? StartDate => DateRange.Start;

            [Required] 
            public DateTime? EndDate => DateRange.End;

            [Required] 
            public TimeSpan? DailyWorkingStartTime { get; set; } = TimeSpan.FromHours(10);

            [Required] 
            public TimeSpan? DailyWorkingEndTime { get; set; } = TimeSpan.FromHours(19);

            [Required]
            public IssueStatus IssueStatus { get; set; } = JiraConstants.Status.Default;

            [Required]
            public TimeSpan? CommentWorklogTime { get; set; } = TimeSpan.Zero;

            [Required]
            public TimeSpan? LunchTime { get; set; } = TimeSpan.FromHours(1);

            public bool IsInitialized { get; set; }

            public void Initialize(UserWorklogFilter filter)
            {
                if (filter != null)
                {
                    DailyWorkingStartTime = filter.DailyWorkingStartTime;
                    DailyWorkingEndTime = filter.DailyWorkingEndTime;
                    IssueStatus = filter.IssueStatus;
                    CommentWorklogTime = filter.CommentWorklogTime;
                    LunchTime = filter.LunchTime;
                }
            }

            public UserWorklogFilter Convert()
            {
                return new UserWorklogFilter
                {
                    DailyWorkingStartTime = DailyWorkingStartTime,
                    DailyWorkingEndTime = DailyWorkingEndTime,
                    IssueStatus = IssueStatus,
                    CommentWorklogTime = CommentWorklogTime,
                    LunchTime = LunchTime
                };
            }
        }
    }
}
