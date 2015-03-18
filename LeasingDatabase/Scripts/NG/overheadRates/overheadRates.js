angular.module('overheadRates')
.controller('overheadRatesController', ['rootUrl', '$http', '$location', '$window', '$scope', function (rootUrl, $http, $location, $window, $scope) {
  var self = this;

  getRates();

  self.usePreviousOverheadRate = function () {
    self.overheadRate = "previous";
  }

  self.useCurrentOverheadRate = function () {
    self.overheadRate = "current";
  }

  self.overheadRate = "current";

  self.addNewRate = function () {
    self.modifyRates = angular.copy(self.overheadRates);

    angular.forEach(self.modifyRates, function (rate, key) {
      rate.PreviousRate = rate.CurrentRate;
      rate.CurrentRate.Rate = 0.00;
    });

    self.action = "New";

    self.editing = true;
  }

  self.editing = false;

  self.editCurrentRate = function () {
    self.modifyRates = angular.copy(self.overheadRates);
    self.action = "Modify";
    self.editing = true;
  }

  self.save = function () {
    if (self.action = "New") {
      $http.post(rootUrl + 'api/OverheadRates', self.modifyRates).success(function (data) {
        getRates();
      });
    } else if (self.action = "Modify") {
      $http.put(rootUrl + 'api/OverheadRates', self.modifyRates).success(function (data) {
        getRates();
      });
    }
  }

  self.cancelEditing = function () {
    self.editing = false;
  }

  function getRates() {
    $http.get(rootUrl + 'api/OverheadRates').success(function (data) {
      self.overheadRates = data;
    })
  }

}]);