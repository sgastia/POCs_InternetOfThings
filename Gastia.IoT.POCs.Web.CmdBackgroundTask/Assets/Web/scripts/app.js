'use strict';

var MAX_ROWS = 10;

//var iotApp = angular.module('iotApp', ['angularSimplePagination']);
var iotApp = angular.module('iotApp',[]);

iotApp.service('iotServices', ['$http', iotServices]);

iotApp.controller('indexCtrl', indexCtrl);
indexCtrl.$inject = ['$scope', 'iotServices'];



