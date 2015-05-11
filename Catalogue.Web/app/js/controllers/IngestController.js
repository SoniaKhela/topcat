(function() {
  angular.module('app.controllers').controller('IngestController', function($scope, $http) {
    var errors;
    $scope["import"] = {
      id: 0,
      fileName: '',
      skipBadRecords: false
    };
    $scope.imports = [
      {
        id: 0,
        name: 'Topcat'
      }, {
        id: 1,
        name: 'Activities'
      }, {
        id: 2,
        name: 'Mesh'
      }, {
        id: 3,
        name: 'Publications'
      }
    ];
    errors = [];
    $scope.errors = {
      current: errors,
      add: function(message) {
        var n, remove;
        n = {
          message: message
        };
        errors.push(n);
        return remove = function() {
          return errors.splice($.inArray(n, errors));
        };
      }
    };
    return $scope.runImport = function() {
      var processResult;
      processResult = function(response) {
        if (response.data.success) {
          $scope.notifications.add('Import run successfully');
        } else {
          $scope.notifications.add('Import failed');
          $scope.errors.remove;
          $scope.errors.add(response.data.exception);
        }
        return $scope.busy.stop();
      };
      $scope.busy.start();
      return $http.post('../api/ingest', $scope["import"]).then(processResult);
    };
  });

}).call(this);

//# sourceMappingURL=IngestController.js.map
