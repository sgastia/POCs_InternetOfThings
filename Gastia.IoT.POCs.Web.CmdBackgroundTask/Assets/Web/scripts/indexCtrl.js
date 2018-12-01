///////////////////////////////////////////////////////////////////////////////////
//Controller

function indexCtrl($scope, serv) {
    $scope.model = new Object();

    $scope.model.Title = "EDGAR datasets status";

    ///////////////////////////////////////////
    $scope.initalize_camera = function () {
        $scope.model.message = "Initialize camera";
        serv.initializeCamera(
            function (data) { $scope.model.message = "Camera initialized: " + data; },
            function (error) { $scope.model.message = "Error initializing camera: " + error; }
        );
    }

    $scope.take_snapshot = function () {
        serv.takeSnapshot(
            function (response) 
            { 
                $scope.model.message = "Snapshot: " + response.data; 
                var objData = angular.fromJson(response.data);
                $scope.model.photoPath = objData.photoPath;
            },
            function (error) { $scope.model.message = "Error snapshot: " + error; }
        );
    }

    $scope.initVideoRecording = function () {
        $scope.model.message = "Start video recording";
    }

    $scope.stopVideoRecording = function () {
        $scope.model.message = "Stop video recording";
    }

    $scope.liveVideo = function () {
        $scope.model.message = "Live video";
    }
    ///////////////////////////////////////////
    //Paging
    /*
    var setPaging = function (currentPage) {
        $scope.pagingSettings = {
            currentPage: currentPage,
            offset: 0,
            pageLimit: MAX_ROWS,
            pageLimits: ['5', '10', '50', '100']
        };
    }
    setPaging(0);
    */
}
