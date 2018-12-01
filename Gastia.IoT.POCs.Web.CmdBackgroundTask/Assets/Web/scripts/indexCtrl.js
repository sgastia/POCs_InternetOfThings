///////////////////////////////////////////////////////////////////////////////////
//Controller

function indexCtrl($scope, serv) {
    $scope.model = new Object();

    $scope.model.Title = "EDGAR datasets status";

    ///////////////////////////////////////////
    $scope.initalize_camera = function () {
        $scope.model.message = "Initialize camera";
    }

    $scope.take_snapshot = function () {
        $scope.model.message = "Take snapshot";
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
    var setPaging = function (currentPage) {
        $scope.pagingSettings = {
            currentPage: currentPage,
            offset: 0,
            pageLimit: MAX_ROWS,
            pageLimits: ['5', '10', '50', '100']
        };
    }
    setPaging(0);

}
