angular.module('SROrders', [])
.controller('SRController', ['$scope', '$http', '$timeout', function ($scope, $http, $timeout) {
  var self = this;
  initialize();
 

  $scope.selected = -1;
  $scope.selectedRow = -1;
  $scope.editingOrder = false;
  $scope.cart = [];

  $scope.addEOLSystem = { text: undefined };

  $scope.setSelectedOrder = function (order, index) {
    $scope.editingOrder = false;
    $scope.selectedSystem = undefined;
    $scope.selectedRow = undefined;

    $scope.selectedOrder = angular.copy(order);
    $scope.selected = index;

  }

  $scope.addToCart = function (order) {
    $scope.cart.push(order);
  }

  $scope.clearCart = function () {
    $scope.cart = [];
    $scope.newSR = undefined;
  }

  $scope.editOrder = function () {
    $scope.oldOrder = angular.copy($scope.selectedOrder);
    $scope.backupUsers = angular.copy($scope.users);
    $scope.backupDepts = angular.copy($scope.depts);
    $scope.editingOrder = true;
  }

  $scope.saveOrder = function () {
    $scope.editingOrder = false;
  }

  $scope.cancelOrder = function () {
    $scope.editingOrder = false;
    $scope.selectedOrder = $scope.oldOrder;
    $scope.users = $scope.backupUsers;
    $scope.depts = $scope.backupDepts;
    $scope.$apply;
  }

  $scope.setSelectedSystem = function (component, index) {
    $scope.selectedSystem = component;
    $scope.selectedRow = index;
  }

  $scope.AddEOL = function () {
    $scope.selectedSystem.EOLComponents.push({ SerialNumber: "", LeaseTag: "" });
  }

  $scope.AddComponent = function () {
    $scope.selectedOrder.Configuration.push({ Type: "Monitor", Make: "Dell", Model: "P2414" });
    console.log($scope.selectedOrder);
  }

  $scope.NewUserForComponent = function (component) {
    var newUser = { GID: "", Phone: "" };
    $scope.users.push(newUser);

    component.User = newUser;
  }

  $scope.NewFOPForComponent = function (component) {
    var newFOP = { DepartmentName: "", FOP: "" };
    $scope.depts.push(newFOP);

    component.Department = newFOP;
  }

  $scope.FindEOLSystem = function () {
    if ($scope.addEOLSystem.text.length > 3) {
      var text = $scope.addEOLSystem.text;
      $http.get('api/EOLSystem?text=' + text).success(function (data) {
        $.each(data, function (index, value) {
          $scope.selectedSystem.EOLComponents.push(value);
        })

        $scope.addEOLSystem.text = undefined;
      });
    }
  }

  $scope.addValueToMake = function (id) {
    if ($("#" + id).find('.highlighted').length == 0) {
      var value = $("#" + id).find('input[type="text"]').val();
      //alert(value);
      $scope.makes.push({ Name: value })
      //alert(id);
      $scope.$apply;
      $timeout(function () {

        $("#" + id + " select").val(value);
        $(".componentType").trigger('chosen:updated'); // Performance improvement: should limit this to just id Type on update
        $("#" + id + " select").trigger('chosen:close');

      }, 0);
    }
  }

  $scope.generateSR = function () {
    $http.get('api/PO?condition=new').success(function (data) {
      $scope.newSR = data.split("\"").join(""); //removing leading and trailing quotes
    });
  }

  function initialize() {
    $http.get('api/NewOrdersByPO').success(function (data) {
      self.orders = data;
    });

    $http.get('api/make').success(function (data) {
      self.makes = data;
    });

    $http.get('api/type').success(function (data) {
      self.types = data;
    });

    $http.get('api/model').success(function (data) {
      self.models = data;
    });
  };

}])
.directive('auSelect', function ($timeout) {
  return {
    restrict: 'AE',
    template: '<select ng-Model="ngModel" class="componentType"><option ng-repeat="n in ngList" ng-selected="ngModel === n.Name" value="{{n.Name}}">{{n.Name}}</option></select>',
    link: function ($scope, el, attr) {
      $timeout(function () {
        $(".componentType").chosen({
          width: "100%",
          no_results_text: "Press Enter to add ",
        });
      }, 0);
    },
    scope: {
      ngModel: '=',
      ngList: '='
    }
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