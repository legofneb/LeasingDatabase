angular.module('billingRates')
.controller('billingRatesController', ['rootUrl', '$http', '$location', '$window', '$scope', function (rootUrl, $http, $location, $window, $scope) {
  var self = this;

  getRates();

  self.usePreviousBillingRate = function () {
    self.billingRate = "previous";
  }

  self.useCurrentBillingRate = function () {
    self.billingRate = "current";
  }

  self.billingRate = "current";

  self.addNewRate = function () {
    self.modifyRates = angular.copy(self.billingRates);

    angular.forEach(self.modifyRates, function (rate, key) {
      rate.PreviousRate = rate.CurrentRate;
      rate.CurrentRate.Rate = 0.00;
    });

    self.editing = true;
  }

  self.editing = false;

  self.editCurrentRate = function () {

  }

  function getRates() {
    $http.get(rootUrl + 'api/BillingRates').success(function (data) {
      self.billingRates = data;
    })
  }

}]);