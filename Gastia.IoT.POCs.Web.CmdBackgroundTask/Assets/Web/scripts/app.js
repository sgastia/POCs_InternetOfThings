'use strict';

const MAX_ROWS = 10;

var iotApp = angular.module('iotApp', ['angularSimplePagination']);

iotApp.service('iotServices', ['$http', iotServices]);

iotApp.controller('indexCtrl', indexCtrl);
indexCtrl.$inject = ['$scope', 'iotServices'];



