angular.module('SROrders')
.config(function ($routeProvider, rootUrl) {

  $routeProvider.when("/System", {
    templateUrl: rootUrl + "NGOrdersByPO/System"
  });

  $routeProvider.when("/Billing", {
    templateUrl: rootUrl + "NGOrdersByPO/Billing"
  });

  $routeProvider.otherwise({
    templateUrl: rootUrl + "NGOrdersByPO/Home"
  });
})
.controller('SRController', ['rootUrl', '$http', '$timeout','$location', function (rootUrl, $http, $timeout, $location) {
  var self = this;
  initialize();

  self.setSelectedOrder = function (order, index) {
    self.editingOrder = false;
    self.selectedSystem = undefined;
    self.selectedRow = undefined;
    $location.path('/');

    self.selectedOrder = angular.copy(order);
    self.selected = index;
  }

  self.editOrder = function () {
    self.oldOrder = angular.copy(self.selectedOrder);
    self.backupUsers = angular.copy(self.users);
    self.backupDepts = angular.copy(self.depts);
    self.editingOrder = true;
  }

  self.saveOrder = function () {
    self.editingOrder = false;

    $http.post(rootUrl + 'api/NewOrdersByPO', self.selectedOrder).
      success(function (data, status, headers, config) {
        alert("yaaaa");
      }).
      error(function (data, status, headers, config) {
      })

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
    self.users = self.backupUsers;
    self.depts = self.backupDepts;
    self.$apply;
  }

  self.setSelectedSystem = function (system) {
    $location.path('/System');
    self.selectedSystem = system;
  }

  self.AddEOL = function () {
    self.selectedSystem.EOLComponents.push({ SerialNumber: "", LeaseTag: "" });
  }

  self.AddComponent = function () {
    self.selectedOrder.Configuration.push({ Type: "Monitor", Make: "Dell", Model: "P2414" });
    console.log(self.selectedOrder);
  }

  self.AddEOLComponent = function () {
    self.selectedSystem.EOLComponents.push({});
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

  self.validateEOLComponent = function (component, $index) {
    if (self.inputTimeout) { $timeout.cancel(self.inputTimeout);}

    self.inputTimeout = $timeout(function () {
      $http.post(rootUrl + 'api/NewOrdersEOL', component).
        success(function (data, status, headers, config) {
          if (status != 200)
          {
            // No Component found
          }
          else
          {
            component = data;
            self.selectedSystem.EOLComponents[$index] = component;
          }
        }).
        error(function (data, status, headers, config) {
          // Some other error occurred
        });
    }, 1000);
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

  self.decrementBillingIndex = function () {
    if (self.billingIndex > 0) {
      self.billingIndex--;
    }
  }

  self.setBillingIndex = function ($index) {
    self.billingIndex = $index;
  }

  self.incrementBillingIndex = function () {
    if (self.billingIndex < self.selectedOrder.SystemGroups.length -1) {
      self.billingIndex++;
    }
  }

  self.usePreviousBillingRate = function () {
    self.billingRate = "previous";
  }

  self.useCurrentBillingRate = function () {
    self.billingRate = "current";
  }

  self.processBilling = function () {
    var billingData = {
      SR: self.selectedOrder.SR,
      useCurrentRates: self.billingRate == "current",
      usePreviousRates: self.billingRate == "previous",
      costList: self.costList,
      insurance: self.insurance,
      warrantyOrShipping: self.warrantyOrShipping,
      beginBillDate: self.beginBillDate,
      billingNotes: self.billingNotes,
      suppressEmail: self.suppressEmail,
      confirmed: false
    };
    
    $http.post(rootUrl + 'api/Billing', billingData).success(function (data) {
      self.billingSummary = data;
    })
    .error(function (data) {
      alert("failure");
    });

    console.log(billingData);
  }

  self.confirmBilling = function () {
    var billingData = {
      SR: self.selectedOrder.SR,
      useCurrentRates: self.billingRate == "current",
      usePreviousRates: self.billingRate == "previous",
      costList: self.costList,
      insurance: self.insurance,
      warrantyOrShipping: self.warrantyOrShipping,
      beginBillDate: self.beginBillDate,
      billingNotes: self.billingNotes,
      suppressEmail: self.suppressEmail,
      confirmed: true
    };

    $http.post(rootUrl + 'api/Billing', billingData).success(function (data) {
      alert("TODO: Remove This billing data from FrontEnd array");
      $location.path("/");
    })
    .error(function (data) {
      alert("failure");
    });

  }

  self.backToMain = function () {
    $location.path("/");
  }

  self.navigateToBilling = function () {
    self.billingIndex = 0;
    self.billingRate = "current";
    self.costList = [];
    self.insurance = 0.00;
    self.warrantyOrShipping = 0.00;
    self.billingNotes = undefined;
    self.suppressEmail = false;
    self.beginBillDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    self.incrementBillDate();
    $location.path("/Billing");

    getBillingRates();
  }

  self.cancelBilling = function () {
    $location.path("/");
  }

  self.incrementBillDate = function () {
    if (self.beginBillDate.getMonth() < 11) {
      self.beginBillDate = new Date(self.beginBillDate.getFullYear(), self.beginBillDate.getMonth() + 1, 1);
    } else {
      self.beginBillDate = new Date(self.beginBillDate.getFullYear() + 1, 0, 1);
    }
  }

  self.decrementBillDate = function () {
    if (self.beginBillDate.getMonth() > 0) {
      self.beginBillDate = new Date(self.beginBillDate.getFullYear(), self.beginBillDate.getMonth() - 1, 1);
    } else {
      self.beginBillDate = new Date(self.beginBillDate.getFullYear() - 1, 11, 1);
    }
  }

  function initialize() {
    $http.get(rootUrl + 'api/NewOrdersByPO').success(function (data) {
      self.orders = data;
    });

    $http.get(rootUrl + 'api/make').success(function (data) {
      self.makes = data;
    });

    $http.get(rootUrl + 'api/type').success(function (data) {
      self.types = data;
    });

    $http.get('api/model').success(function (data) {
      self.models = data;
    });

    self.selected = -1;
    self.selectedRow = -1;
    self.editingOrder = false;
    self.cart = [];

    self.addEOLSystem = { text: undefined };

    $location.path("/");
  };

  function getBillingRates() {
    $http.get('api/BillingRates').success(function (data) {
      self.billingRates = data;
      console.log(self.billingRates);
    });
  }

}])
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