﻿// Generated by IcedCoffeeScript 108.0.11
(function() {
  angular.module('app.controllers').controller('OpenDataManagementController', function($scope, $http, $location, signOffGroup) {
    var loadList, loadSummary, loadTab1Data, loadTab2Data, loadTab3Data, loadTab4Data, loadTab5Data, m;
    m = {
      tab: 2,
      openData: {
        summary: {},
        list: []
      }
    };
    $scope.m = m;
    loadSummary = function() {
      return $http.get('../api/publishing/opendata/summary').success(function(result) {
        return m.openData.summary = result;
      });
    };
    loadTab1Data = function() {
      return $http.get('../api/publishing/opendata/publishedsincelastupdated').success(function(result) {
        return m.openData.list = result;
      });
    };
    loadTab2Data = function() {
      return $http.get('../api/publishing/opendata/notpublishedsincelastupdated').success(function(result) {
        return m.openData.list = result;
      });
    };
    loadTab3Data = function() {
      return $http.get('../api/publishing/opendata/publicationneverattempted').success(function(result) {
        return m.openData.list = result;
      });
    };
    loadTab4Data = function() {
      return $http.get('../api/publishing/opendata/lastpublicationattemptwasunsuccessful').success(function(result) {
        return m.openData.list = result;
      });
    };
    loadTab5Data = function() {
      return $http.get('../api/publishing/opendata/pendingsignoff').success(function(result) {
        return m.openData.list = result;
      });
    };
    loadList = function() {
      switch (m.tab) {
        case 1:
          return loadTab1Data();
        case 2:
          return loadTab2Data();
        case 3:
          return loadTab3Data();
        case 4:
          return loadTab4Data();
        case 5:
          return loadTab5Data();
      }
    };
    $scope.$watch('m.tab', loadList);
    loadSummary();
    return loadTab2Data();
  });

}).call(this);

//# sourceMappingURL=OpenDataManagementController.js.map
