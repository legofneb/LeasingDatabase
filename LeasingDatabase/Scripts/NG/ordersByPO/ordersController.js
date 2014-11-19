angular.module('SROrders')
.controller('SRController', ['rootUrl', '$http', '$timeout', function (rootUrl, $http, $timeout) {
  var self = this;
  initialize();
 

  self.selected = -1;
  self.selectedRow = -1;
  self.editingOrder = false;
  self.cart = [];

  self.addEOLSystem = { text: undefined };

  self.setSelectedOrder = function (order, index) {
    self.editingOrder = false;
    self.selectedSystem = undefined;
    self.selectedRow = undefined;

    self.selectedOrder = angular.copy(order);
    self.selected = index;

  }

  self.addToCart = function (order) {
    self.cart.push(order);
  }

  self.clearCart = function () {
    self.cart = [];
    self.newSR = undefined;
  }

  self.editOrder = function () {
    self.oldOrder = angular.copy(self.selectedOrder);
    self.backupUsers = angular.copy(self.users);
    self.backupDepts = angular.copy(self.depts);
    self.editingOrder = true;
  }

  self.saveOrder = function () {
    self.editingOrder = false;
  }

  self.cancelOrder = function () {
    self.editingOrder = false;
    self.selectedOrder = self.oldOrder;
    self.users = self.backupUsers;
    self.depts = self.backupDepts;
    self.$apply;
  }

  self.setSelectedSystem = function (component, index) {
    self.selectedSystem = component;
    self.selectedRow = index;
  }

  self.AddEOL = function () {
    self.selectedSystem.EOLComponents.push({ SerialNumber: "", LeaseTag: "" });
  }

  self.AddComponent = function () {
    self.selectedOrder.Configuration.push({ Type: "Monitor", Make: "Dell", Model: "P2414" });
    console.log(self.selectedOrder);
  }

  self.NewUserForComponent = function (component) {
    var newUser = { GID: "", Phone: "" };
    self.users.push(newUser);

    component.User = newUser;
  }

  self.NewFOPForComponent = function (component) {
    var newFOP = { DepartmentName: "", FOP: "" };
    self.depts.push(newFOP);

    component.Department = newFOP;
  }

  self.FindEOLSystem = function () {
    if (self.addEOLSystem.text.length > 3) {
      var text = self.addEOLSystem.text;
      $http.get('api/EOLSystem?text=' + text).success(function (data) {
        $.each(data, function (index, value) {
          self.selectedSystem.EOLComponents.push(value);
        })

        self.addEOLSystem.text = undefined;
      });
    }
  }

  self.addValueToMake = function (id) {
    if ($("#" + id).find('.highlighted').length == 0) {
      var value = $("#" + id).find('input[type="text"]').val();
      //alert(value);
      self.makes.push({ Name: value })
      //alert(id);
      self.$apply;
      $timeout(function () {

        $("#" + id + " select").val(value);
        $(".componentType").trigger('chosen:updated'); // Performance improvement: should limit this to just id Type on update
        $("#" + id + " select").trigger('chosen:close');

      }, 0);
    }
  }

  self.generateSR = function () {
    $http.get('api/PO?condition=new').success(function (data) {
      self.newSR = data.split("\"").join(""); //removing leading and trailing quotes
    });
  }

  function initialize() {
    $http.get('api/NewOrdersByPO').success(function (data) {
      self.orders = data;
      console.log(data);
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