angular.module('billingCoordinator')
.config(function ($routeProvider, rootUrl) {

  $routeProvider.when("/FOP", {
    templateUrl: rootUrl + "NGBillingCoordinator/FOPView"
  });

  $routeProvider.when("/GID", {
    templateUrl: rootUrl + "NGBillingCoordinator/GIDView"
  });
})
.controller('billingCoordinatorController', ['rootUrl', '$http', '$location', '$filter', '$window', function (rootUrl, $http, $location, $filter, $window) {
  var self = this;

  self.setSearchByFOP = function () {
    self.searchType = "FOP";
  }

  self.setSearchByGID = function () {
    self.searchType = "GID";
  }

  self.startSearch = function () {
    if (self.searchType == "FOP") {
      $http.get(rootUrl + 'api/BillingCoordinatorByFOP/?FOP=' + encodeURIComponent(self.searchTerm)).success(function (data) {
        $location.path('/FOP');
        self.FOP = data;
      });
    } else if (self.searchType == "GID") {
      $http.get(rootUrl + 'api/BillingCoordinatorByGID/?GID=' + encodeURIComponent(self.searchTerm)).success(function (data) {
        $location.path('/GID');
        self.GID = data;
      });
    } else {
      alert("You must select a search type");
    }
  }

  self.editMode = function () {
    self.edit = true;
  }

  self.cancelMode = function () {
    self.edit = false;
    self.startSearch();
  }

  self.saveMode = function () {
    if ($location.path() == "/FOP") {
      $http.post(rootUrl + 'api/BillingCoordinatorByFOP', self.FOP);
    } else if ($location.path() == "/GID") {
      $http.post(rootUrl + 'api/BillingCoordinatorByGID', self.GID);
    }

    self.edit = false;
    self.startSearch();
  }

  self.addToFOP = function () {
    self.FOP.Coordinators.push({});
  }

  self.removeFromFOP = function (dept) {
    var ind = self.FOP.Coordinators.indexOf(dept);
    self.FOP.Coordinators.splice(ind, 1);
  }

  self.addToGID = function () {
    self.GID.FOPs.push("");
  }

  self.removeFromGID = function (GID) {
    var index = self.GID.FOPs.indexOf(GID);
    self.GID.FOPs.splice(index, 1);
  }


  self.edit = false;
  $location.path("/");

}]);