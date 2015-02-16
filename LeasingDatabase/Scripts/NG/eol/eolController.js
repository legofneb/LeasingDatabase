angular.module('EOL')
.controller('EOLController', ['rootUrl', '$http', '$location', '$filter', '$window', '$timeout', function (rootUrl, $http, $location, $filter, $window, $timeout) {
  var self = this;

  function getEOL() {
    var dateAsString = $filter('date')(self.EOLDate, 'shortDate');

    $http.get(rootUrl + 'api/EOL?date=' + encodeURIComponent(dateAsString)).success(function (data) {
      self.models = data;
    });
  }

  self.editModel = function (model) {
    var ind = self.models.indexOf(model);
    self.editArray[ind] = true;
  }

  self.saveModel = function (model) {
    var ind = self.models.indexOf(model);
    self.editArray[ind] = false;
    $http.post(rootUrl + 'api/EOL', model).success(function (data) {
      getEOL();
    });
    
  }

  self.cancelModel = function (model) {
    var ind = self.models.indexOf(model);
    self.editArray[ind] = false;
    getEOL();
  }

  self.editingModel = function (model) {
    var ind = self.models.indexOf(model);
    return self.editArray[ind];
  }

  self.downloadExcel = function () {
    var dateAsString = $filter('date')(self.EOLDate, 'shortDate');
    $window.open(rootUrl + 'api/EOLReport/?date=' + encodeURIComponent(dateAsString));
  }

  self.incrementEOLDate = function () {
    var newDate = incrementDate(self.EOLDate);
    if (newDate) { self.EOLDate = newDate }

    if (self.timer != null) {
      $timeout.cancel(self.timer);
    }

    self.timer = $timeout(function () {
      getEOL();
    }, 500);

    getEOL();
  }

  self.decrementEOLDate = function () {
    var newDate = decrementDate(self.EOLDate);
    if (newDate) { self.EOLDate = newDate }
    if (self.timer != null) {
      $timeout.cancel(self.timer);
    }
    
    self.timer = $timeout(function () {
      getEOL()
    }, 500);
  }

  self.editArray = [];
  self.EOLDate = new Date();
  self.decrementEOLDate();

}]);

function decrementDate(date) {
  if (date.getMonth() > 0) {
    return new Date(date.getFullYear(), date.getMonth() - 1, 15);
  } else {
    return new Date(date.getFullYear() - 1, 11, 15);
  }
}

function incrementDate(date) {
  if (date.getMonth() < 11) {
    return new Date(date.getFullYear(), date.getMonth() + 1, 15);
  } else {
    return new Date(date.getFullYear() + 1, 0, 15);
  }
}