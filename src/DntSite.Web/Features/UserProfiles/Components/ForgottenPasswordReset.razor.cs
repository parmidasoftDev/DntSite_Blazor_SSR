﻿using DntSite.Web.Features.AppConfigs.Components;
using DntSite.Web.Features.UserProfiles.RoutingConstants;
using DntSite.Web.Features.UserProfiles.Services.Contracts;

namespace DntSite.Web.Features.UserProfiles.Components;

public partial class ForgottenPasswordReset
{
    private string? _alertMessage;
    private AlertType _alertType = AlertType.Danger;

    [InjectComponentScoped] public IUserProfilesManagerService UsersService { set; get; } = null!;

    [InjectComponentScoped] internal IUsersManagerEmailsService UsersManagerEmailsService { set; get; } = null!;

    [Parameter] public string? Id { get; set; }

    [CascadingParameter] internal ApplicationState ApplicationState { set; get; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ApplicationState.DoNotLogPageReferrer = true;

        AddBreadCrumbs();

        if (string.IsNullOrWhiteSpace(Id))
        {
            _alertMessage = "اطلاعاتی یافت نشد!";

            return;
        }

        var operationResult = await UsersService.ResetPasswordAsync(Id);
        _alertMessage = operationResult.Message;

        if (operationResult.Stat == OperationStat.Succeeded)
        {
            _alertType = AlertType.Success;

            await UsersManagerEmailsService.SendResetPasswordEmailAsync(operationResult.Result.User!,
                operationResult.Result.Password);
        }
    }

    private void AddBreadCrumbs() => ApplicationState.BreadCrumbs.AddRange([UserProfilesBreadCrumbs.Login]);
}
