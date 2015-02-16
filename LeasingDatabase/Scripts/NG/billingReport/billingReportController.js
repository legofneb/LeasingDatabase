angular.module('billingReport')
.controller('billingReportController', ['rootUrl', '$http', '$location', '$filter', '$window', function (rootUrl, $http, $location, $filter, $window) {
  var self = this;


  self.incrementBillDate = function () {
    if (self.BillDate.getMonth() > new Date().getMonth() && self.BillDate.getYear() >= new Date().getYear()) {
      return;
    }

    if (self.BillDate.getMonth() < 11) {
      self.BillDate = new Date(self.BillDate.getFullYear(), self.BillDate.getMonth() + 1, 15);
    } else {
      self.BillDate = new Date(self.BillDate.getFullYear() + 1, 0, 15);
    }
  }

  self.decrementBillDate = function () {
    if (self.BillDate.getMonth() > 0) {
      self.BillDate = new Date(self.BillDate.getFullYear(), self.BillDate.getMonth() - 1, 15);
    } else {
      self.BillDate = new Date(self.BillDate.getFullYear() - 1, 11, 15);
    }
  }

  self.generateSimpleBilling = function () {
    var dateAsString = $filter('date')(self.BillDate, 'shortDate');

    $window.open(rootUrl + 'api/BillingReport/?type=Simple&statementDate=' + encodeURIComponent(dateAsString) + '&FOP=all');
  }

  self.generateDetailedBilling = function () {
    var dateAsString = $filter('date')(self.BillDate, 'shortDate');

    $window.open(rootUrl + 'api/BillingReport/?type=Detailed&statementDate=' + encodeURIComponent(dateAsString) + '&FOP=' + encodeURIComponent(self.FOP));
  }

  self.FOP = "101002 155301 7000";
  self.BillDate = new Date(new Date().getFullYear(), new Date().getMonth(), 15)
  self.incrementBillDate();

}]);