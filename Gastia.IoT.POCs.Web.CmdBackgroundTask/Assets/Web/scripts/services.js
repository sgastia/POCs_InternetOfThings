function iotServices($http) {

    var URL_TAKE_SNAPSHOT = "api/devices/camera/snapshot";
    var URL_INITIALIZE_CAMERA = "api/devices/camera/initialize";
    var URL_START_VIDEO_RECORDING = "api/devices/camera/startvideorecording";
    var URL_STOP_VIDEO_RECORDING = "api/devices/camera/stopvideorecording";
    var URL_LIVE_VIDEO = "api/devices/camera/livevideo";
    
    ////////////////////////////////
    //Public
    this.takeSnapshot = function (successCallback, errorCallback) {
        $http({
            method: "GET",
            url: URL_TAKE_SNAPSHOT,
            cache: false
        }).then(successCallback, errorCallback);
    };

    this.initializeCamera = function (successCallback, errorCallback) {
        $http({
            method: "GET",
            url: URL_INITIALIZE_CAMERA,
            cache: false
        }).then(successCallback,errorCallback);
    };

}
