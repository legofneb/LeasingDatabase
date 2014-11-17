angular.module("scan")
  .config(function ($routeProvider, rootUrl) {

    $routeProvider.when("/scanning", {
      templateUrl: rootUrl + "NGScan/scanning"
    });

    $routeProvider.when("/summary", {
      templateUrl: rootUrl + "NGScan/summary"
    });

    $routeProvider.when("/loading", {
      templateUrl: rootUrl + "NGScan/loading"
    });

    $routeProvider.otherwise({
      templateUrl: rootUrl + "NGScan/home"
    });    
  })

  .controller("scanCtrl", function ($scope, $http, $location, $timeout, $window, rootUrl) {

    $http.get(rootUrl + "api/NewOrdersPreScan")
        .success(function (data) {
          $scope.newOrders = data;
        });

    $scope.nav = {
      orderNumber: undefined
    };

    $scope.gettingOrders = function () {
      return !angular.isDefined($scope.newOrders);
    }

    $scope.getOrderFromQuickView = function (sr) {
      $scope.nav.orderNumber = sr;
      console.log(sr);
      $scope.getOrder();
    }

    $scope.getOrder = function () {
      $location.path("/loading");
      $scope.errors = undefined;
      $http.get(rootUrl + "api/Scan/" + $scope.nav.orderNumber)
          .success(function (data) {
            $location.path("/scanning");

            $scope.SR = data.SR;
            $scope.order = data.orders;
            console.log(data);
            $scope.currentSystem = $scope.order[0];
            $scope.currentComponent = $scope.currentSystem.Components[0];
            $timeout(function () {
              $("#SerialNumber").focus();
            }, 1500);
          })
          .error(function (error) {
            $location.path("/home");
            $scope.errors = error;
          });

    };

    $scope.nextComponent = function () {
      if ($scope.hasNext()) {
        $scope.setNextComponent();
      } else {
        if (!$scope.lastComponent()) {
          $scope.setNextSystem();
        } else {
          // this is the last component available
          $scope.submit();
        }
      }
    };

    $scope.previousComponent = function () {
      if ($scope.hasPrevious()) {
        $scope.setPreviousComponent();
      } else {
        if (!$scope.firstComponent()) {
          $scope.setPreviousSystem();
        }
      }
    }

    $scope.setNextSystem = function () {
      $scope.currentSystem = $scope.order[$scope.order.indexOf($scope.currentSystem) + 1];
      $scope.currentComponent = $scope.currentSystem.Components[0];
    };

    $scope.setPreviousSystem = function () {
      $scope.currentSystem = $scope.order[$scope.order.indexOf($scope.currentSystem) - 1];
      $scope.currentComponent = $scope.currentSystem.Components[$scope.currentSystem.Components.length - 1];
    }

    $scope.setNextComponent = function () {
      var ind = $scope.currentSystem.Components.indexOf($scope.currentComponent);
      $scope.currentComponent = $scope.currentSystem.Components[ind + 1];
    };

    $scope.setPreviousComponent = function () {
      var ind = $scope.currentSystem.Components.indexOf($scope.currentComponent);
      $scope.currentComponent = $scope.currentSystem.Components[ind - 1];
    };

    $scope.hasPrevious = function () {
      return $scope.currentSystem.Components.indexOf($scope.currentComponent) > 0 ? true : false;
    };

    $scope.hasNext = function () {
      return $scope.currentSystem.Components.indexOf($scope.currentComponent) < ($scope.currentSystem.Components.length - 1) ? true : false;
    };

    // this is the last component of the entire object collection, not just the currentSystem
    $scope.lastComponent = function () {
      if (!angular.isDefined($scope.currentSystem)) {
        return false;
      }

      if (!$scope.hasNext() && ($scope.order.indexOf($scope.currentSystem) == ($scope.order.length - 1))) {
        return true;
      }
      else {
        return false;
      }
    }

    $scope.firstComponent = function () {
      if (!angular.isDefined($scope.currentSystem)) {
        return false;
      }

      if (!$scope.hasPrevious() && ($scope.order.indexOf($scope.currentSystem) == 0)) {
        return true;
      } else {
        return false;
      }
    }

    $scope.enterSerialNumber = function () {
      $("#LeaseTag").focus();
    };

    $scope.enterLeaseTag = function () {
      if (!$scope.lastComponent()) {
        $("#SerialNumber").focus();
        $scope.nextComponent();
      } else {
        $scope.submit();
      }
    }

    $scope.hasAllValues = function () {
      var hasValue = true;

      angular.forEach($scope.order, function (system, key) {
        angular.forEach(system.Components, function (component, compKey) {
          if ((!angular.isString(component.SerialNumber) || component.SerialNumber.length < 1) || (!angular.isString(component.LeaseTag) || component.LeaseTag.length < 1)) {
            hasValue = false;
            return hasValue;
          }
        });
      });

      return hasValue;
    }

    $scope.ComponentCount = function () {
      if (!angular.isDefined($scope.order)) {
        return 0;
      }

      var count = 0;

      angular.forEach($scope.order, function (system, key) {
        angular.forEach(system.Components, function (component, compKey) {
          count++;
        })
      });

      return count;
    };

    $scope.ComponentIndex = function () {
      if (!angular.isDefined($scope.order)) {
        return 0;
      }

      return ($scope.currentSystem.Components.indexOf($scope.currentComponent)) + 1 + ($scope.order.indexOf($scope.currentSystem) * $scope.currentSystem.Components.length);
    };

    $scope.clearOrder = function () {
      $scope.order = undefined;
      $scope.SR = undefined;
      $scope.currentSystem = undefined;
      $scope.currentComponent = undefined;
      $scope.nav.orderNumber = undefined;
    }

    $scope.submit = function () {
      $location.path("/loading");
      $http.post(rootUrl + "api/Scan/", $scope.order)
          .success(function (data) {
            window.location.href = rootUrl + "SR/Index?SRs=" + $scope.SR;
            $scope.clearOrder();
            $location.path("/home");
            $timeout(function () {
              $("#home").focus();
            }, 0);
          })
          .error(function (error) {
          });
    };

    $scope.cancel = function () {
      $scope.clearOrder();
      $location.path("/home");
      $timeout(function () {
        $("#home").focus();
      }, 1500);
    }

  })
.directive('ngEnter', function () {
  return function (scope, element, attrs) {
    element.bind("keydown keypress", function (event) {
      if (event.which === 13) {
        scope.$apply(function () {
          scope.$eval(attrs.ngEnter);
        });

        event.preventDefault();
      }
    });
  };
});