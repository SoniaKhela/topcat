
angular.module('app.controllers').controller 'PublicatorController',

    ($scope, record) -> 
        
        blankModel = ->
            r:        record
            
        # we extend (don't overwrite) the object reference from the parent page
        #$scope.vocabulator = {} if !$scope.vocabulator
        #angular.extend $scope.vocabulator, blankModel() if angular.equals {}, $scope.vocabulator
        #m = $scope.vocabulator
        
        $scope.x = record
        console.log 'hi'
        console.log record

        $scope.publishAsOpenData = ->
            $scope.form.publication = {} if !$scope.form.publication
            $scope.form.publication.openData = { lastAttempt: {}, lastSuccess: {}} if !$scope.form.publication.openData
            