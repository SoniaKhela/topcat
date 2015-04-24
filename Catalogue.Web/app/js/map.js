﻿(function() {
  var baseLayer, calculateBestHeightForMap, getArea, getBestPadding, module, normal, select, tuples, updateTuples;

  module = angular.module('app.map');

  calculateBestHeightForMap = function($window, elem) {
    var elemTop, viewTop;
    viewTop = angular.element($window).innerHeight();
    elemTop = angular.element(elem).offset().top;
    return (viewTop - elemTop - 10) + 'px';
  };

  baseLayer = L.tileLayer('https://{s}.tiles.mapbox.com/v4/petmon.lp99j25j/{z}/{x}/{y}.png?access_token=pk.eyJ1IjoicGV0bW9uIiwiYSI6ImdjaXJLTEEifQ.cLlYNK1-bfT0Vv4xUHhDBA', {
    maxZoom: 18,
    attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, ' + '<a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' + 'Imagery © <a href="http://mapbox.com">Mapbox</a>',
    id: 'petmon.lp99j25j'
  });

  normal = {
    fillOpacity: 0.2,
    weight: 1,
    color: '#222'
  };

  select = {
    fillOpacity: 0.5,
    weight: 1,
    color: 'rgb(217,38,103)'
  };

  getBestPadding = function(tuples) {
    switch (tuples.length) {
      case 1:
        return {
          padding: [50, 50]
        };
      case 2:
        return {
          padding: [20, 20]
        };
      default:
        return {
          padding: [5, 5]
        };
    }
  };

  getArea = function(bounds) {
    var e, n, s, w, x, y, _ref, _ref1;
    (_ref = bounds[0], s = _ref[0], w = _ref[1]), (_ref1 = bounds[1], n = _ref1[0], e = _ref1[1]);
    x = e - w;
    y = n - s;
    return Math.abs(x * y);
  };

  tuples = {};

  updateTuples = function(results, scope) {
    var r;
    return tuples = (function() {
      var _i, _len, _results;
      _results = [];
      for (_i = 0, _len = results.length; _i < _len; _i++) {
        r = results[_i];
        if (r.box.north) {
          _results.push((function(r) {
            var bounds, rect;
            bounds = [[r.box.south, r.box.west], [r.box.north, r.box.east]];
            rect = L.rectangle(bounds, normal);
            rect.on('mouseover', function() {
              return rect.setStyle({
                fillOpacity: 0.4
              });
            });
            rect.on('mouseout', function() {
              return rect.setStyle({
                fillOpacity: 0.2
              });
            });
            rect.on('click', function() {
              return scope.$apply(function() {
                return scope.current.zoomed = r;
              });
            });
            return {
              r: r,
              bounds: bounds,
              rect: rect
            };
          })(r));
        }
      }
      return _results;
    })();
  };

  module.directive('tcSearchMap', function($window, $location, $anchorScroll) {
    return {
      link: function(scope, elem, attrs) {
        var group, map;
        map = L.map(elem[0]);
        map.addLayer(baseLayer);
        group = L.layerGroup().addTo(map);
        scope.$watch('result.results', function(results) {
          var ordered, x, _i, _len;
          updateTuples(results, scope);
          group.clearLayers();
          ordered = _(tuples).sortBy(function(x) {
            return getArea(x.bounds);
          }).reverse().value();
          for (_i = 0, _len = ordered.length; _i < _len; _i++) {
            x = ordered[_i];
            group.addLayer(x.rect);
          }
          elem.css('height', calculateBestHeightForMap($window, elem));
          scope.current.zoomed = null;
          if (tuples.length > 0) {
            scope.current.selected = tuples[0].r;
            return map.fitBounds((function() {
              var _j, _len1, _results;
              _results = [];
              for (_j = 0, _len1 = tuples.length; _j < _len1; _j++) {
                x = tuples[_j];
                _results.push(x.bounds);
              }
              return _results;
            })(), getBestPadding(tuples));
          }
        });
        scope.$watch('current.topmost', function(result) {
          if (!scope.current.zoomed) {
            return scope.current.selected = result;
          }
        });
        scope.$watch('current.selected', function(newer, older) {
          var _ref, _ref1;
          if (newer !== older) {
            if ((_ref = _(tuples).find(function(x) {
              return x.r === newer;
            })) != null) {
              _ref.rect.setStyle(select);
            }
            return (_ref1 = _(tuples).find(function(x) {
              return x.r === older;
            })) != null ? _ref1.rect.setStyle(normal) : void 0;
          }
        });
        return scope.$watch('current.zoomed', function(newer, older) {
          var tuple;
          if (newer && newer !== older) {
            tuple = _(tuples).find(function(x) {
              return x.r === newer;
            });
            map.fitBounds(tuple.bounds, {
              padding: [100, 100]
            });
            return scope.current.selected = newer;
          }
        });
      }
    };
  });

  module.directive('tcSearchResultScrollHighlighter', function($window) {
    return {
      link: function(scope, elem, attrs) {
        var win;
        win = angular.element($window);
        return win.bind('scroll', function() {
          var el, q, result, x;
          q = (function() {
            var _i, _len, _ref, _results;
            _ref = elem.children();
            _results = [];
            for (_i = 0, _len = _ref.length; _i < _len; _i++) {
              el = _ref[_i];
              if (angular.element(el).offset().top > win.scrollTop()) {
                _results.push(el);
              }
            }
            return _results;
          })();
          result = ((function() {
            var _i, _len, _ref, _results;
            _results = [];
            for (_i = 0, _len = tuples.length; _i < _len; _i++) {
              x = tuples[_i];
              if (x.r.id === ((_ref = q[0]) != null ? _ref.id : void 0)) {
                _results.push(x.r);
              }
            }
            return _results;
          })())[0];
          if (result) {
            return scope.$apply(function() {
              return scope.current.topmost = result;
            });
          }
        });
      }
    };
  });

  module.directive('tcStickToTop', function($window, $timeout) {
    return {
      link: function(scope, elem, attrs) {
        var f, getPositions, win;
        win = angular.element($window);
        getPositions = function() {
          return {
            v: win.scrollTop(),
            e: elem.offset().top,
            w: elem.width()
          };
        };
        f = function() {
          var initial;
          initial = getPositions();
          return win.bind('scroll', function() {
            var current;
            current = getPositions();
            if (current.v > current.e) {
              elem.addClass('stick-to-top');
              return elem.css('width', initial.w);
            } else if (current.v < initial.e) {
              elem.removeClass('stick-to-top');
              return elem.css('width', '');
            }
          });
        };
        return $timeout(f, 100);
      }
    };
  });

}).call(this);

//# sourceMappingURL=map.js.map