///////////////////////////////////////////////////////////////////////////////////
//Controller

function indexCtrl($scope, serv) {
    $scope.model = new Object();

    $scope.model.Title = "Windows 10 IoT POCs";

    $scope.model.btnTakeSnapshot_disabled = true;
    $scope.model.btnStartRecording_disabled = true;
    $scope.model.btnStopRecording_disabled = true;
    $scope.model.btnLiveVideo_disabled = true;
    $scope.model.video_show = false;
    $scope.model.photo_show = false;

    ///////////////////////////////////////////
    $scope.initalize_camera = function () {
        $scope.model.message = "Initialize camera";
        serv.initializeCamera(
            function (response)
            { 
                $scope.model.message = "Camera initialized: " + response.data; 
                //TODO: pending to validate right result
                $scope.model.btnTakeSnapshot_disabled = false;
                $scope.model.btnStartRecording_disabled = false;
                $scope.model.btnStopRecording_disabled = false;
                $scope.model.btnLiveVideo_disabled = false;
            },
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
                $scope.model.photo_show = true;
            },
            function (error) { $scope.model.message = "Error snapshot: " + error; }
        );
    }

    $scope.initVideoRecording = function () {
        $scope.model.message = "Start video recording";
        serv.startVideoRecording(
            function (response) {
                $scope.model.btnStopRecording_disabled = false;
                $scope.model.btnStartRecording_disabled = true;
                $scope.model.btnTakeSnapshot_disabled = true;
                $scope.model.btnLiveVideo_disabled = true;
                $scope.model.message = $scope.model.message + " - Message: " + response.data.message + " - File: " + response.data.videoPath;
                $scope.model.video_show = false;
                $scope.model.photo_show = false;
            },
            function (error) { $scope.model.message = "Error starting recording: " + error; }
        );
    }

    $scope.stopVideoRecording = function () {
        $scope.model.message = "Stop video recording";
        serv.stopVideoRecording(
            function (response) {
                $scope.model.btnStopRecording_disabled = true;
                $scope.model.btnStartRecording_disabled = false;
                $scope.model.btnTakeSnapshot_disabled = false;
                $scope.model.btnLiveVideo_disabled = false;
                $scope.model.message = $scope.model.message + " - Message: " + response.data.message + " - File: " + response.data.videoPath;
                $scope.model.videoPath = response.data.videoPath;
                $scope.model.video_show = true;
            },
            function (error) { $scope.model.message = "Error stoping recording: " + error; }
        );

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
