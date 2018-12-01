function iotServices($http) {

    var URL_TAKE_SNAPSHOT = "api/devices/camera/snapshot";
    var URL_INITIALIZE_CAMERA = "api/devices/camera/initialize";
    ////////////////////////////////
    //Private
    var getPromise = function (pUrl) {
        var promise = $http({
            method: "GET",
            url: pUrl,
            cache: false
        });
        promise.success(
            function (data, status) {
                return data;
            }
        );

        promise.error(
            function (data, status) {
                console.log(data, status);
                return { "status": false };
            }
        );
        return promise;
    };


    ////////////////////////////////
    //Public
    this.takeSnapshot = function (successCallback, errorCallback) {
        getPromise(URL_TAKE_SNAPSHOT).then
            (
            function success(response) {
                successCallback(response.data);
            },
            function error(response) {
                errorCallback(response);
            }
            );
    };

    this.initializeCamera = function (successCallback, errorCallback) {
        getPromise(URL_INITIALIZE_CAMERA).then
            (
            function success(response) {
                successCallback(response.data);
            },
            function error(response) {
                errorCallback(response);
            }
            );
    };
    
}
