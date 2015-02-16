angular.module('departments')
.controller('departmentsController', ['rootUrl', '$http', '$location', '$filter', '$window', function (rootUrl, $http, $location, $filter, $window) {
  var self = this;

  function getDepartments() {
    $http.get(rootUrl + 'api/Departments').success(function (data) {
      self.Departments = data;

      for (var i = 0; i < self.Departments.length; i++) {
        self.editArray[i] = false;
      }
    });
  }

  getDepartments();

  self.editDept = function (dept) {
    var ind = self.Departments.indexOf(dept);
    self.editArray[ind] = true;
  }

  self.saveDept = function (dept) {
    $http.post(rootUrl + 'api/Departments', dept).success(function (data) {
      getDepartments();
    });
  }

  self.cancelDept = function (dept) {
    var ind = self.Departments.indexOf(dept);
    self.editArray[ind] = false;
    getDepartments();
  }

  self.editingDept = function (dept) {
    var ind = self.Departments.indexOf(dept);
    return self.editArray[ind];
  }

  self.editArray = [];

}]);