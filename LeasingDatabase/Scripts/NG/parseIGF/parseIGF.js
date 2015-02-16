angular.module('parseIGF')
.controller('parseIGFController', ['rootUrl', '$http', '$location', '$window', '$scope', function (rootUrl, $http, $location, $window, $scope) {
  var self = this;
  $scope.submitting = false;

  $scope.setFile = function (files) {
    var fd = new FormData();
    fd.append('file', files);
    self.fd = fd;
  }

  $scope.uploadFile = function () {
    $scope.submitting = true;
    $scope.errors = undefined;
    $http.post(rootUrl + 'api/ParseIGF', self.fd, {
      transformRequest: angular.identity,
      headers: { 'Content-Type': undefined }
    }).success(function (data) {
      $scope.submitting = false;
      $scope.errors = data;
    });
  }
}]);