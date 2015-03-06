﻿(function() {
  var __indexOf = [].indexOf || function(item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

  angular.module('app.controllers').controller('SearchController', function($scope, $rootScope, $location, $http, $timeout) {
    var blankQuery, ensureEndsWith, getKeywordFromPath, getPathFromKeyword, parseQuerystring, queryKeywords, queryRecords, updateUrl;
    $scope.app = {
      starting: true
    };
    $timeout((function() {
      return $scope.app.starting = false;
    }), 500);
    $scope.resultsView = 'list';
    updateUrl = function(query) {
      $location.search('q', query.q || null);
      return $location.search('k', query.k || null);
    };
    queryRecords = function(query) {
      $scope.busy.start();
      return $http.get('../api/search?' + $.param(query, true)).success(function(result) {
        if (angular.equals(result.query, query)) {
          return $scope.result = result;
        }
      }).error(function(e) {
        return $scope.notifications.add('Oops! ' + e.message);
      })["finally"](function() {
        return $scope.busy.stop();
      });
    };
    queryKeywords = function(query) {
      if (query.q) {
        $scope.busy.start();
        return $http.get('../api/keywords?q=' + query.q).success(function(result) {
          return $scope.keywordSuggestions = result;
        }).error(function(e) {
          return $scope.notifications.add('Oops! ' + e.message);
        })["finally"](function() {
          return $scope.busy.stop();
        });
      } else {
        return $scope.keywordSuggestions = {};
      }
    };
    $scope.doSearch = function(query) {
      updateUrl(query);
      if (query.q || query.k[0]) {
        queryKeywords(query);
        return queryRecords(query);
      } else {
        $scope.result = {};
        return $scope.keywordSuggestions = {};
      }
    };
    blankQuery = function() {
      return {
        q: '',
        k: [],
        p: 0,
        n: 25
      };
    };
    parseQuerystring = function() {
      var o;
      o = $location.search();
      if (o.k && !$.isArray(o.k)) {
        o.k = [o.k];
      }
      return $.extend({}, blankQuery(), o);
    };
    $scope.query = parseQuerystring();
    $scope.$watch('query', $scope.doSearch, true);
    $scope.addKeywordToQuery = function(keyword) {
      var k;
      k = getPathFromKeyword(keyword);
      if ($scope.query.k.length === 0) {
        return $scope.query = $.extend({}, blankQuery(), {
          'k': [k]
        });
      } else if (__indexOf.call($scope.query.k, k) >= 0) {
        return $scope.notifications.add('Your query already contains this keyword');
      } else {
        return $scope.query.k.push(k);
      }
    };
    $scope.removeKeywordFromQuery = function(keyword) {
      return $scope.query.k.splice($.inArray(keyword, $scope.query.k), 1);
    };
    $scope.querystring = function() {
      return $.param($scope.query, true);
    };
    getPathFromKeyword = function(keyword) {
      var path;
      path = ensureEndsWith(keyword.vocab, '/') + keyword.value;
      return path.replace("http://", "");
    };
    getKeywordFromPath = function(path) {
      var elements, i, value, vocab, _i, _ref;
      if (path.indexOf('/') === -1) {
        return {
          value: path,
          vocab: ''
        };
      } else {
        elements = path.split('/');
        value = elements[elements.length - 1];
        vocab = "http://";
        for (i = _i = 0, _ref = elements.length - 2; _i <= _ref; i = _i += 1) {
          vocab = vocab.concat(elements[i].concat('/'));
        }
        return {
          value: value,
          vocab: vocab
        };
      }
    };
    ensureEndsWith = function(str, suffix) {
      if (str !== '' && !(str.indexOf(suffix, str.length - suffix.length) !== -1)) {
        return str.concat(suffix);
      } else {
        return str;
      }
    };
    $scope.setPage = function(n) {
      return $scope.query.p = n - 1;
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
    return $scope.maxPages = function(total, pageLength) {
      return Math.ceil(total / pageLength) - 1;
    };
  });

}).call(this);

//# sourceMappingURL=SearchController.js.map
