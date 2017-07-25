﻿// Generated by IcedCoffeeScript 108.0.11
(function() {
  var __indexOf = [].indexOf || function(item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

  angular.module('app.controllers').controller('SearchController', function($scope, $rootScope, $location, $http, $timeout, $q, $modal) {
    var blankQuery, parseQuerystring, queryKeywords, queryRecords, updateUrl;
    $scope.app = {
      starting: true
    };
    $timeout((function() {
      return $scope.app.starting = false;
    }), 500);
    $scope.result = {
      results: {}
    };
    $scope.current = {};
    $scope.pageSize = 15;
    $scope.vocabulator = {};
    $scope.resultsView = 'list';
    updateUrl = function(query) {
      var blank;
      blank = blankQuery();
      $location.search('q', query.q || null);
      $location.search('k', query.k);
      $location.search('p', query.p || null);
      return $location.search('d', query.d || null);
    };
    queryRecords = function(query) {
      return $http.get('../api/search?' + $.param(query, true)).success(function(result) {
        if (angular.equals(result.query, query)) {
          return $scope.result = result;
        }
      }).error(function(e) {
        return $scope.notifications.add('Oops! ' + e.message);
      });
    };
    queryKeywords = function(query) {
      if (query.q) {
        return $http.get('../api/keywords?q=' + query.q).success(function(result) {
          return $scope.keywordSuggestions = result;
        }).error(function(e) {
          return $scope.notifications.add('Oops! ' + e.message);
        });
      } else {
        return $q["defer"]();
      }
    };
    $scope.doSearch = function(query) {
      var keywordsPromise, recordsPromise;
      updateUrl(query);
      if (query.q || query.k[0]) {
        $scope.busy.start();
        keywordsPromise = queryKeywords(query);
        recordsPromise = queryRecords(query);
        return $q.all([keywordsPromise, recordsPromise])["finally"](function() {
          $scope.busy.stop();
          if (!$scope.result.query.q) {
            return $scope.keywordSuggestions = {};
          }
        });
      } else {
        $scope.keywordSuggestions = {};
        return $scope.result = {};
      }
    };
    blankQuery = function() {
      return {
        q: '',
        k: [],
        p: 0,
        n: $scope.pageSize,
        d: null
      };
    };
    parseQuerystring = function() {
      var o;
      o = $location.search();
      if (o.k && !$.isArray(o.k)) {
        o.k = [o.k];
      }
      if (o.p) {
        o.p = o.p * 1;
      }
      return $.extend({}, blankQuery(), o);
    };
    $scope.query = parseQuerystring();
    $scope.$watch('query', $scope.doSearch, true);
    $scope.querystring = function() {
      return $.param($scope.query, true);
    };
    $scope.addKeywordsToQuery = function(keywords) {
      var k, keywordsAlreadyInQuery, keywordsToAddToQuery, _i, _j, _len, _len1, _ref;
      _ref = _(keywords).map($scope.keywordToString).partition(function(k) {
        return __indexOf.call($scope.query.k, k) >= 0;
      }).value(), keywordsAlreadyInQuery = _ref[0], keywordsToAddToQuery = _ref[1];
      for (_i = 0, _len = keywordsToAddToQuery.length; _i < _len; _i++) {
        k = keywordsToAddToQuery[_i];
        $scope.query.k.push(k);
      }
      for (_j = 0, _len1 = keywordsAlreadyInQuery.length; _j < _len1; _j++) {
        k = keywordsAlreadyInQuery[_j];
        $scope.notifications.add("Your query already contains the '" + ($scope.keywordFromString(k).value) + "' keyword");
      }
      if (keywordsToAddToQuery.length) {
        return $scope.query = angular.extend({}, blankQuery(), {
          'k': $scope.query.k
        });
      }
    };
    $scope.removeKeywordFromQuery = function(keyword) {
      var s;
      s = $scope.keywordToString(keyword);
      return $scope.query.k.splice($.inArray(s, $scope.query.k), 1);
    };
    $scope.keywordToString = function(k) {
      var s;
      s = k.vocab ? k.vocab + '/' + k.value : k.value;
      return s.replace('http://', '');
    };
    $scope.keywordFromString = function(s) {
      var slash;
      if ((s.indexOf('/')) === -1) {
        return {
          vocab: '',
          value: s
        };
      } else {
        slash = s.lastIndexOf('/');
        return {
          vocab: 'http://' + (s.substring(0, slash)),
          value: s.substring(slash + 1)
        };
      }
    };
    $scope.openVocabulator = function() {
      var modal;
      modal = $modal.open({
        controller: 'VocabulatorController',
        templateUrl: 'views/partials/vocabulator.html?' + new Date().getTime(),
        size: 'lg',
        scope: $scope
      });
      return modal.result.then(function(keywords) {
        return $scope.addKeywordsToQuery(keywords);
      });
    };
    $scope.setPage = function(n) {
      if (n > 0 && n <= ($scope.maxPages($scope.result.total, $scope.pageSize) + 1)) {
        return $scope.query.p = n - 1;
      }
    };
    $scope.range = function(min, max, step) {
      var i, input, _i, _results;
      step = step === void 0 ? 1 : step;
      input = [];
      _results = [];
      for (i = _i = 0; step > 0 ? _i <= max : _i >= max; i = _i += step) {
        _results.push(input.push(i));
      }
      return _results;
    };
    $scope.maxPages = function(total, pageLength) {
      return Math.ceil(total / pageLength) - 1;
    };
    $http.get('../api/collections').success(function(result) {
      return $scope.collections = _.chunk(result, 2);
    });
    return $http.get('../api/usage').success(function(result) {
      var r, _i, _len, _ref, _results;
      $scope.recentModifications = result.recentlyModifiedRecords;
      _ref = $scope.recentModifications;
      _results = [];
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        r = _ref[_i];
        if (r.event === 0) {
          _results.push(r.event = "created");
        } else {
          _results.push(r.event = "edited");
        }
      }
      return _results;
    });
  });

}).call(this);

//# sourceMappingURL=SearchController.js.map
