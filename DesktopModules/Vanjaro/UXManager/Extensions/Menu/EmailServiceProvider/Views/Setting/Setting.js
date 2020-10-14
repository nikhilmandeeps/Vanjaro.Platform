﻿app.controller('setting_setting', function ($scope, $attrs, $routeParams, $http, CommonSvc, SweetAlert) {
    var common = CommonSvc.getData($scope);
    $scope.onInit = function () {
    };
    var Data = "";
    $scope.Click_TestSMTP = function () {

        if ($scope.ui.data.SMTPmode.Options) {
            Data = {
                Server: $scope.ui.data.Host_Server.Value,
                Username: $scope.ui.data.Host_Username.Value,
                Password: $scope.ui.data.Host_Password.Value,
                EnableSSL: $scope.ui.data.Host_EnableSSL.Options
            }
        }
        else {
            Data = {
                Server: $scope.ui.data.Portal_Server.Value,
                Username: $scope.ui.data.Portal_Username.Value,
                Password: $scope.ui.data.Portal_Password.Value,
                EnableSSL: $scope.ui.data.Portal_EnableSSL.Options
            }
        }
        common.webApi.post('Setting/SendTestEmail', '', Data).success(function (data) {
            if (data.IsSuccess) {
                window.parent.ShowNotification('[LS:EmailServiceProvider]', data.Data, 'success');
            }
            else if (data.HasErrors) {
                window.parent.swal(data.Message);
            }
        });
    };


    $scope.Click_Update = function () {
        
        if ($scope.ui.data.SMTPmode.Options) {

            Data = {
                SMTPmode: $scope.ui.data.SMTPmode.Options,
                Host_Server: $scope.ui.data.Host_Server.Value,
                Host_Username: $scope.ui.data.Host_Username.Value,
                Host_Password: $scope.ui.data.Host_Password.Value,
                Host_EnableSSL: $scope.ui.data.Host_EnableSSL.Options
            }    
        }
        else {
            Data = {
                SMTPmode: $scope.ui.data.SMTPmode.Options,
                Portal_Server: $scope.ui.data.Portal_Server.Value,
                Portal_Username: $scope.ui.data.Portal_Username.Value,
                Portal_Password: $scope.ui.data.Portal_Password.Value,
                Portal_EnableSSL: $scope.ui.data.Portal_EnableSSL.Options
            }
        }
        common.webApi.post('Setting/update', '', Data).success(function (data) {
            if (data.IsSuccess) {
                $scope.Click_Cancel();
            }
            else if (data.HasErrors) {
                window.parent.swal(data.Message);
            }
        });
    }

    $scope.Click_Cancel = function () {
        $(window.parent.document.body).find('[data-dismiss="modal"]').click();
    };
});