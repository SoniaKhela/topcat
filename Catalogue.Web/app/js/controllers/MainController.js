﻿(function() {

  angular.module('app.controllers').controller('MainController', function($scope, $timeout, Account, misc) {
    $scope.hashStringToColor = misc.hashStringToColor;
    return Account.then(function(user) {
      return $scope.user = user;
    });
  });

}).call(this);
