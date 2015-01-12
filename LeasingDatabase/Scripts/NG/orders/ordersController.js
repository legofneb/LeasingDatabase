angular.module('Orders')
.config(function ($routeProvider, rootUrl) {

  $routeProvider.when("/System", {
    templateUrl: rootUrl + "NGOrders/System"
  });

  $routeProvider.otherwise({
    templateUrl: rootUrl + "NGOrders/Home"
  });
})

.controller('OrdersController', ['rootUrl', '$http', '$timeout', '$location', function (rootUrl, $http, $timeout, $location) {
  var self = this;
  initialize();

  self.selected = -1;
  self.editingOrder = false;
  self.cart = [];
  self.collapseSidebar = false;
  self.newSR = "";

  self.toggleSidebar = function () {
    if (self.collapseSidebar) {
      // expand Sidebar
      $(".sidebar").removeClass("hidden");
      $("#mainContent").addClass("col-md-9");
      $("#mainContent").addClass("col-md-offset-3");
      $("#mainContent").removeClass("col-md-12");
      $("#cartBar").addClass("col-md-9");
      $("#cartBar").addClass("col-md-offset-3");
      $("#cartBar").removeClass("col-md-12");
      $("#toggleSidebar").removeClass("active");
      $("#toggleSidebar > i").removeClass("fa-caret-square-o-right").addClass("fa-caret-square-o-left");
      self.collapseSidebar = false;
    } else {
      // collapse Sidebar
      $(".sidebar").addClass("hidden");
      $("#mainContent").removeClass("col-md-9");
      $("#mainContent").removeClass("col-md-offset-3");
      $("#mainContent").addClass("col-md-12");
      $("#cartBar").removeClass("col-md-9");
      $("#cartBar").removeClass("col-md-offset-3");
      $("#cartBar").addClass("col-md-12");
      $("#toggleSidebar").addClass("active");
      $("#toggleSidebar > i").removeClass("fa-caret-square-o-left").addClass("fa-caret-square-o-right");
      self.collapseSidebar = true;
    }
  }

  self.setSelectedOrder = function (order, index) {
    self.selectedOrder = angular.copy(order);
    self.selected = index;
    console.log(index);
  }

  self.createNewOrder = function () {
    newComponent = {
      Configuration: [{ Type: null, Make: null, Model: null}],
      Components: []
    };

    self.selectedOrder = pushElementOnArray(newComponent, self.orders);
    self.selected = self.orders.length - 1;
    self.editingOrder = true;
  }

  self.submitCart = function () {
    $http.post(rootUrl + 'api/PO', self.cart).
      success(function (data, status, headers, config) {

      }).
      error(function (data, status, headers, config) {

      });
  }

  self.addToCart = function (order) {
    self.cart.push(order.id);
  }

  self.clearCart = function () {
    self.cart = [];
    self.newSR = "";
  }

  self.editOrder = function () {
    self.oldOrder = angular.copy(self.selectedOrder);
    self.editingOrder = true;

  }

  self.saveOrder = function () {
    $http.post(rootUrl + 'api/NewOrders', self.selectedOrder).
      success(function (data, status, headers, config) {
        alert("yaaaa");
      }).
      error(function (data, status, headers, config) {
      });

    console.log(self.selectedOrder);
    self.editingOrder = false;
    var ind = -1;
    angular.forEach(self.orders, function (value, key) {
      if (value.id == self.selectedOrder.id) {
        ind = self.orders.indexOf(value);
      }
    });
    
    if (ind >= 0) {
      self.orders[ind] = angular.copy(self.selectedOrder);
      self.setSelectedOrder(self.orders[ind], ind);
    }

  }

  self.cancelOrder = function () {
    self.editingOrder = false;
    self.selectedOrder = self.oldOrder;
    self.$apply;

    if (self.collapseSidebar) { self.toggleSidebar(); }

    if (!angular.isDefined(self.selectedOrder.id)) {
      self.orders.pop(); self.selected = -1;
    }

    $location.path("/");
  }

  self.addNewComponent = function (selectedOrder) {
    if (selectedOrder.Configuration.length == 0 || selectedOrder.Configuration[0].Type == null) {
      if (selectedOrder.Configuration.length == 0) {
        selectedOrder.Configuration.push({})
      }

      selectedOrder.Configuration[0].Type = "Monitor";
      console.log(selectedOrder);
    } else {
      selectedOrder.Configuration.push({ Type: "Monitor", Make: "", Model: "" });
    }
  }

  self.removeComponent = function (component) {
    var ind = self.selectedOrder.Configuration.indexOf(component);
    self.selectedOrder.Configuration.splice(ind, 1);
  }

  self.addNewSystem = function (selectedOrder) {
    self.selectedSystem = pushElementOnArray({}, self.selectedOrder.Components);
    $location.path("/System");
  }

  self.generateSR = function () {
    $http.get('api/PO?condition=new').success(function (data) {
      self.newSR = data.split("\"").join(""); //removing leading and trailing quotes
    });
  }

  self.addValueToMake = function (id) {
    console.log(id);
    if ($("#" + id).find('.highlighted').length == 0) {
      var value = $("#" + id).find('input[type="text"]').val();
      self.makes.push({ Name: value });
      self.$apply;
      $timeout(function () {

        $("#" + id + " select").val(value);
        $(".componentType").trigger('chosen:updated'); // Proposed Performance improvement: should limit this to just id Type on update
        $("#" + id + " select").trigger('chosen:close');

      }, 0);
    }
  }

  self.componentDetails = function (component) {
    $location.path("/System");
    self.selectedSystem = component;
  }

  self.backToMain = function () {
    $location.path("/");
  }

  function pushElementOnArray(element, arr) {
    // pushes an element onto an array and returns the object
    arr.push(element);
    return arr[arr.length - 1];

  }

  function initialize() {
    $http.get(rootUrl + 'api/NewOrders').success(function (data) {
      self.orders = data;
    });

    $http.get(rootUrl + 'api/make').success(function (data) {
      self.makes = data;
    });

    $http.get(rootUrl + 'api/type').success(function (data) {
      self.types = data;
    });

    $http.get(rootUrl + 'api/model').success(function (data) {
      self.models = data;
    });
  };

}]);
