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

    self.action = "New";

    self.editing = true;
  }

  self.editing = false;

  self.editCurrentRate = function () {
    self.modifyRates = angular.copy(self.billingRates);
    self.action = "Modify";
    self.editing = true;
  }

  self.save = function () {
    if (self.action = "New") {
      $http.post(rootUrl + 'api/BillingRates', self.modifyRates).success(function (data) {
        getRates();
      });
    } else if (self.action = "Modify") {
      $http.put(rootUrl + 'api/BillingRates', self.modifyRates).success(function (data) {
        getRates();
      });
    }
  }

  self.cancelEditing = function () {
    self.editing = false;
  }

  function getRates() {
    $http.get(rootUrl + 'api/BillingRates').success(function (data) {
      self.billingRates = data;
    })
  }

}]);