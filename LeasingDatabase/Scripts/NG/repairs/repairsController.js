
angular.module("repairApp")

  .config(function ($routeProvider, rootUrl) {
    $routeProvider.when('/', {
      templateUrl: rootUrl + "NGRepair/Home"
    });

    $routeProvider.when('/New', {
      templateUrl: rootUrl + "NGRepair/New"
    });

  })

  .controller("repairCtrl", function ($scope, $location, Repair) {
    'use strict';

    var self = this;
    this.showCompleted = false;
    self.newRepair = {};

    self.repairSelected = repairSelected;
    self.toggleShowCompleted = toggleShowCompleted;
    self.addNewRepair = addNewRepair;
    self.submitNewRepair = submitNewRepair;
    self.clearNewRepair = clearNewRepair;
    self.completeRepair = completeRepair;
    self.editRepair = editRepair;
    self.getRepairData = getRepairData;

    self.getRepairData();

    function repairSelected(repair) {
      self.selectedRepair = repair;
    }

    function toggleShowCompleted() {
      if (self.showCompleted) {
        self.showCompleted = false;
      } else {
        self.showCompleted = true;
      }
    }

    function addNewRepair() {
      $location.path('/New');
      self.clearNewRepair();  
    }

    function submitNewRepair() {
      if (angular.isDefined(self.newRepair.id)) {
        var id = self.newRepair.id;
        Repair.update({ id: id }, self.newRepair, function success() {
          self.getRepairData();
        });
      } else {
        self.newRepair.$save()
          .then(self.getRepairData());
      }

      $location.path('/');
    }

    function clearNewRepair() {
      self.newRepair = new Repair();
    }

    function editRepair(selectedRepair) {
      self.newRepair = selectedRepair;
      $location.path('/New');
    }

    function completeRepair(selectedRepair) {
      selectedRepair.Completed = true;
      var id = selectedRepair.id;
      Repair.update({ id: id }, selectedRepair, function success() {
        self.getRepairData();
      });
      $location.path('/');
    }

    function getRepairData() {
      Repair.query(function (data) {
        self.repairs = data;
        self.selectedRepair = undefined;
      });
    }
  })
  .factory("Repair", function ($resource, rootUrl) {
    return $resource(rootUrl + "api/Repairs/:id", null, 
      {
        'update': { method: 'PUT'}
      });
  })
  .directive("typeIcon", function (rootUrl) {
    return {
      restrict: 'E',
      scope: {
        repair: '=repair'
      },
      link: function (scope, elem, attrs) {

        scope.$watch('repair', function (newVal) {
          if (angular.isDefined(newVal)) {
            scope.type = newVal.Type;
          }
        });

      },
      templateUrl: rootUrl + "Common/Type"
    };
  })

  .filter('CompletedRepairs', function () {
    return function (repairs, showCompleted) {
      var resultArr = [];

      angular.forEach(repairs, function (repair) {
        if (repair.Completed === false || showCompleted) {
          resultArr.push(repair);
        }
      });

      return resultArr;
    };
  });