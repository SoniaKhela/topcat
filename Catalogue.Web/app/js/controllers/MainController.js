﻿(function() {

  angular.module('app.controllers').controller('MainController', function($scope, Account) {
    return Account.then(function(user) {
      return $scope.user = user;
    });
  });

}).call(this);