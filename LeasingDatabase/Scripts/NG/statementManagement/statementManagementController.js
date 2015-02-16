angular.module('statementManagement')
.controller('statementManagementController', ['rootUrl', '$http', '$location', '$filter', '$window', function (rootUrl, $http, $location, $filter, $window) {
  var self = this;


  self.incrementBillingStatementDate = function () {
    var newDate = incrementDate(self.BillingStatementDate);
    if (newDate) { self.BillingStatementDate = newDate }
  }

  self.decrementBillingStatementDate = function () {
    var newDate = decrementDate(self.BillingStatementDate);
    if (newDate) { self.BillingStatementDate = newDate }
  }

  self.incrementEOLStatementDate = function () {
    var newDate = incrementDate(self.EOLStatementDate);
    if (newDate) { self.EOLStatementDate = newDate }
  }

  self.decrementEOLStatementDate = function () {
    var newDate = decrementDate(self.EOLStatementDate);
    if (newDate) { self.EOLStatementDate = newDate }
  }

  
  self.sendEOLStatements = function () {

  }

  self.sendStatements = function () {
    $http.post(rootUrl + 'api/StatementManagement', self.BillingStatementDate).success(function (data) {
      self.errors = data;
    })
  }

  self.EOLStatementDate = new Date(new Date().getFullYear(), new Date().getMonth(), 15);
  self.BillingStatementDate = new Date(new Date().getFullYear(), new Date().getMonth(), 15);

  self.incrementBillingStatementDate();

  $http.get(rootUrl + 'api/StatementManagement/').success(function (data) {
    self.LastBillingStatementDate = new Date(parseInt(data));
  });

  function decrementDate(date) {
    if (date.getMonth() > 0) {
      return new Date(date.getFullYear(), date.getMonth() - 1, 15);
    } else {
      return new Date(date.getFullYear() - 1, 11, 15);
    }
  }

  function incrementDate(date) {
    if (aboveDateLimit(date)) {
      return;
    }

    if (date.getMonth() < 11) {
      return new Date(date.getFullYear(), date.getMonth() + 1, 15);
    } else {
      return new Date(date.getFullYear() + 1, 0, 15);
    }
  }

  function aboveDateLimit(date) {
    return date.getMonth() > new Date().getMonth() && date.getYear() >= new Date().getYear();
  }

}]);