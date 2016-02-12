
angular.module('app.controllers').controller 'PublicatorController',

    ($scope) -> 
        
        blankModel = ->
            q:        ''  # blah
            r:        []  # blah
         
            
        $scope.publishAsOpenData = ->
            $scope.form.publication = {} if !$scope.form.publication
            $scope.form.publication.openData = { lastAttempt: {}, lastSuccess: {}} if !$scope.form.publication.openData
            