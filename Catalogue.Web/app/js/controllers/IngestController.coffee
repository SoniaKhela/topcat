angular.module('app.controllers').controller 'IngestController',

    ($scope, $http) -> 
        $scope.import = {id: 0, fileName: '', skipBadRecords: false}
        $scope.imports = [
            {id: 0, name: 'Topcat'},
            {id: 1, name: 'Activities'},
            {id: 2, name: 'Mesh'},
            {id: 3, name: 'Publications'}]
                
        errors = []
        $scope.errors =
            current: errors
            add: (message) ->
                n = message: message
                errors.push n
                remove = -> errors.splice ($.inArray n, errors)
                
        $scope.runImport = ->
            processResult = (response) ->
                if response.data.success
                    $scope.notifications.add 'Import run successfully'
                else
                    $scope.notifications.add 'Import failed'
                    $scope.errors.remove
                    $scope.errors.add response.data.exception
                                
                $scope.busy.stop()

            $scope.busy.start()
            $http.post('../api/ingest', $scope.import).then processResult
        
        
