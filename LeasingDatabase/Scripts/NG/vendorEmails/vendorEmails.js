angular.module('vendorEmails')
.controller('vendorEmailsController', ['rootUrl', '$http', '$location', '$window', '$scope', function (rootUrl, $http, $location, $window, $scope) {
  var self = this;

  getEmails();

  self.delete = function (vendor) {

    $http.delete(rootUrl + 'api/VendorEmails/' + vendor.id).success(function (data) {
      self.vendors.splice(self.vendors.indexOf(vendor), 1);
    })
  }

  self.add = function () {
    $http.put(rootUrl + 'api/VendorEmails/?email=' + encodeURIComponent(self.email)).success(function (data) {
      getEmails();
      self.email = undefined;
    })
  }

  function getEmails() {
    $http.get(rootUrl + 'api/VendorEmails').success(function (data) {
      self.vendors = data;
    })
  }

}]);