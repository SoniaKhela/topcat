﻿// Generated by IcedCoffeeScript 108.0.11
(function() {
  angular.module('app.controllers').controller('OpenDataPublishingController', function($scope, $http, $location, $timeout) {
    var load1, load2, load3, load4, load5, loadList, loadSummary, m;
    m = {
      tab: 5,
      openData: {
        summary: {},
        list: []
      }
    };
    $scope.m = m;
    $scope.signOffStatus = {};
    $scope.recordTimeoutMap = {};
    loadSummary = function() {
      return $http.get('../api/publishing/opendata/summary').success(function(result) {
        return m.openData.summary = result;
      });
    };
    load1 = function() {
      return $http.get('../api/publishing/opendata/publishedsincelastupdated').success(function(result) {
        return m.openData.list = result;
      });
    };
    load2 = function() {
      return $http.get('../api/publishing/opendata/notpublishedsincelastupdated').success(function(result) {
        return m.openData.list = result;
      });
    };
    load3 = function() {
      return $http.get('../api/publishing/opendata/publicationneverattempted').success(function(result) {
        return m.openData.list = result;
      });
    };
    load4 = function() {
      return $http.get('../api/publishing/opendata/lastpublicationattemptwasunsuccessful').success(function(result) {
        return m.openData.list = result;
      });
    };
    load5 = function() {
      return $http.get('../api/publishing/opendata/pendingsignoff').success(function(result) {
        var r, _i, _len, _results;
        m.openData.list = result;
        _results = [];
        for (_i = 0, _len = result.length; _i < _len; _i++) {
          r = result[_i];
          _results.push($scope.signOffStatus[r.id] = "Sign Off");
        }
        return _results;
      });
    };
    loadList = function() {
      switch (m.tab) {
        case 1:
          return load1();
        case 2:
          return load2();
        case 3:
          return load3();
        case 4:
          return load4();
        case 5:
          return load5();
      }
    };
    $scope.$watch('m.tab', loadList);
    loadSummary();
    load5();
    $scope.submitSignOff = function(recordId) {
      $scope.signOffRequest = {};
      $scope.signOffRequest.id = recordId;
      $scope.signOffRequest.comment = "";
      return $http.put('../api/publishing/opendata/signoff', $scope.signOffRequest).success(function(result) {
        $scope.signOffStatus[recordId] = "Signed Off";
        loadSummary();
        $scope.loadRecordsPendingSignOff();
        return $scope.notifications.add("Successfully signed off");
      })["catch"](function(error) {
        if (error.status === 401) {
          $scope.notifications.add("Unauthorised - not in valid sign off group");
        } else {
          $scope.notifications.add(error.data.exceptionMessage);
        }
        $scope.signOffStatus[recordId] = "Retry?";
        return delete $scope.recordTimeoutMap[recordId];
      });
    };
    $scope.allowGraceTime = function(recordId) {
      if ($scope.recordTimeoutMap[recordId] > 0) {
        $scope.signOffStatus[recordId] = "Cancel " + ("0" + $scope.recordTimeoutMap[recordId]--).slice(-2);
        return $timeout($scope.allowGraceTime.bind(null, recordId), 1000);
      } else if (recordId in $scope.recordTimeoutMap) {
        return $scope.submitSignOff(recordId);
      }
    };
    $scope.cancelSignOff = function(recordId) {
      delete $scope.recordTimeoutMap[recordId];
      return $scope.signOffStatus[recordId] = "Sign Off";
    };
    return $scope.signOffButtonClick = function(recordId) {
      if (!(recordId in $scope.recordTimeoutMap)) {
        $scope.recordTimeoutMap[recordId] = 10;
        return $scope.allowGraceTime(recordId);
      } else {
        return $scope.cancelSignOff(recordId);
      }
    };
  });

}).call(this);

//# sourceMappingURL=OpenDataPublishingController.js.map
