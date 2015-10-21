﻿(function() {
  angular.module('app.controllers', ['ui.bootstrap']).controller('MainController', function($scope, $rootScope, $timeout, $cookies, Account) {
    var busyCount, notifications;
    busyCount = 0;
    $scope.busy = {
      start: function() {
        return busyCount = busyCount + 1;
      },
      stop: function() {
        return busyCount = busyCount - 1;
      },
      value: function() {
        return busyCount > 0;
      }
    };
    notifications = [];
    $scope.notifications = {
      current: notifications,
      add: function(message) {
        var n, remove;
        n = {
          message: message
        };
        notifications.push(n);
        remove = function() {
          return notifications.splice($.inArray(n, notifications));
        };
        return $timeout(remove, 4000);
      }
    };
    Account.then(function(user) {
      return $scope.user = user;
    });
    return $rootScope.$on('$locationChangeStart', function() {
      return $('.qtip').qtip('hide');
    });
  });

}).call(this);

//# sourceMappingURL=MainController.js.map
