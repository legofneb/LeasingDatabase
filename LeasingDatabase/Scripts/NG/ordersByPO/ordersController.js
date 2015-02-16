angular.module('SROrders')
.config(function ($routeProvider, rootUrl) {

  $routeProvider.when("/System", {
    templateUrl: rootUrl + "NGOrdersByPO/System"
  });

  $routeProvider.when("/Billing", {
    templateUrl: rootUrl + "NGOrdersByPO/Billing"
  });

  $routeProvider.when("/Transfer", {
    templateUrl: rootUrl + "NGOrdersByPO/Transfer"
  });

  $routeProvider.otherwise({
    templateUrl: rootUrl + "NGOrdersByPO/Home"
  });
})
.controller('SRController', ['rootUrl', '$http', '$timeout','$location', 'searchOrdersByPOFilter', function (rootUrl, $http, $timeout, $location, searchOrdersByPOFilter) {
  var self = this;
  initialize();

  self.setSelectedOrder = function (order, index) {
    self.editingOrder = false;
    self.selectedSystem = undefined;
    self.selectedRow = undefined;
    self.globalOrderNumber = undefined;
    self.globalFOP = undefined;
    self.globalDepartmentName = undefined;
    $location.path('/');

    self.selectedOrder = angular.copy(order);
    self.backupOrder = angular.copy(order);
    self.selected = index;
  }

  self.editOrder = function () {
    self.backupOrder = angular.copy(self.selectedOrder);
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
    self.selectedOrder = self.backupOrder;
    self.users = self.backupUsers;
    self.depts = self.backupDepts;
    self.$apply;
  }

  self.setSelectedSystem = function (system) {
    $location.path('/System');
    self.selectedSystem = system;
  }

  self.setOrderNumber = function () {
    angular.forEach(self.selectedOrder.SystemGroups, function (systemGroup, key) {
      systemGroup.OrderNumber = self.globalOrderNumber;
    });
  }

  self.setFOP = function () {
    angular.forEach(self.selectedOrder.SystemGroups, function (systemGroup, key) {
      systemGroup.FOP = self.globalFOP;
    });
  }

  self.setDepartmentName = function () {
    angular.forEach(self.selectedOrder.SystemGroups, function (systemGroup, key) {
      systemGroup.DepartmentName = self.globalDepartmentName;
    });
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

      var found = false;
      angular.forEach(self.makes, function (make) {
        if (make.Name == value) {
          found = true;
        }
      });
      if (!found) {
        self.makes.push({ Name: value })
      }
      
      //alert(id);
      self.$apply;
      $timeout(function () {

        $("#" + id + " select").val(value);
        $(".componentType").trigger('chosen:updated'); // Performance improvement: should limit this to just id Type on update
        $("#" + id + " select").trigger('chosen:close');

      }, 0);
    }
  }

  self.addValueToModel = function (id) {
    if ($("#" + id).find('.highlighted').length == 0) {
      var value = $("#" + id).find('input[type="text"]').val();
      //alert(value);

      var found = false;
      angular.forEach(self.models, function (model) {
        if (model.Name == value) {
          found = true;
        }
      });
      if (!found) {
        self.models.push({ Name: value })
      }

      
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
    $http.get(rootUrl + 'api/PO?condition=new').success(function (data) {
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

  self.transferSystem = function () {
    $location.path('/Transfer');
    self.transfer = new Transfer();
    self.transfer.oldSR = self.selectedOrder.SR;
    self.transfer.id = self.selectedSystem.id;
    self.transfer.newSR = "";
  }

  function Transfer() {
    this.oldSR = "";
    this.id = 0;
    this.newSR = "";
  }

  self.transferSystemPOST = function () {
    $http.post(rootUrl + 'api/Transfer', self.transfer).success(function (data) {
      alert("yay");
    })
    .error(function (data) {
      alert("boo");
    })
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

  self.backToSystem = function () {
    $location.path("/System");
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

  self.indexForFiltered = function () {
    orders = searchOrdersByPOFilter(self.orders, self.searchTerm);
    var ind = -1;

    for (var i = 0; i < orders.length; i++) {
      if (orders[i].id != null && self.backupOrder != null && orders[i].id == self.backupOrder.id) {
        ind = i;
      }
    }
    return ind;
  }

  self.groupMatches = function (group) {
    if (self.searchTerm == null || self.searchTerm.length < 1) {
      return false;
    }

    var searchArray = self.searchTerm.split(" ");
    return groupMatches(group, searchArray);
  }

  self.orderHasAllSerials = function (order) {
    hasAllSerials = true;
    angular.forEach(order.SystemGroups, function (group) {
      angular.forEach(group.Components, function (component) {
        if (!angular.isDefined(component.SerialNumber) || component.SerialNumber == null || component.SerialNumber.length < 1) {
          hasAllSerials = false;
        }
      })
    });

    return hasAllSerials;
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

    $http.get(rootUrl + 'api/model').success(function (data) {
      self.models = data;
    });

    self.rateLevels = [{ Name: 'Base' }, { Name: 'Support' }];
    self.terms = [{ Name: 24 }, { Name: 36 }];


    self.selected = -1;
    self.selectedRow = -1;
    self.editingOrder = false;
    self.cart = [];

    self.addEOLSystem = { text: undefined };

    $location.path("/");
  };

  function getBillingRates() {
    $http.get(rootUrl + 'api/BillingRates').success(function (data) {
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
})
.filter('searchOrdersByPO', function () {
  return function (orders, searchTerm) {

    if (angular.isDefined(orders)) {
      var filteredOrders = [];

      angular.forEach(orders, function (order) {
        if (angular.isDefined(searchTerm) && searchTerm.length > 0) {

          var hasMatch = false;

          // check top level fields
          if (OrderContains(order, searchTerm)) {
            hasMatch = true;
          }

          if (hasMatch) {
            filteredOrders.push(order);
          }
        } else {
          filteredOrders.push(order);
        }
      });

      return filteredOrders;
    }
  };
});

function OrderContains(order, searchTerm) {
  var searchArray = searchTerm.split(" ");
  var result = true;

  for (var i = 0; i < searchArray.length; i++) {



    var fuzzySearchFindsMatch = Contains(order.SR, searchArray[i]) || Contains(order.OrdererGID, searchArray[i]) || Contains(order.OrdererBuilding, searchArray[i]) || Contains(order.OrdererRoom, searchArray[i]);
    
    if (angular.isDefined(order.Configuration)) {
      angular.forEach(order.Configuration, function(component) {
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(component.Type, searchArray[i]) || Contains(component.Make, searchArray[i]) || Contains(component.Model, searchArray[i]);
      });
    }

    if (angular.isDefined(order.SystemGroups)) {
      angular.forEach(order.SystemGroups, function (group) {
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.OrdererGID, searchArray[i]) || Contains(group.OrdererBuilding, searchArray[i]) || Contains(group.OrdererRoom, searchArray[i]);
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.StatementName, searchArray[i]) || Contains(group.GID, searchArray[i]) || Contains(group.DepartmentName, searchArray[i]);
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.FOP, searchArray[i]) || Contains(group.RateLevel, searchArray[i]) || Contains(group.Term, searchArray[i]);
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.Room, searchArray[i]) || Contains(group.Building, searchArray[i]) || Contains(group.OrderNumber, searchArray[i]);

        if (angular.isDefined(group.Components)) {
          angular.forEach(group.Components, function (comp) {
            fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(comp.SerialNumber, searchArray[i]) || Contains(comp.LeaseTag, searchArray[i]);
          });
        }

        if (angular.isDefined(group.EOLComponents)) {
          angular.forEach(group.EOLComponents, function (comp) {
            fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(comp.SerialNumber, searchArray[i]) || Contains(comp.LeaseTag, searchArray[i]);
          });
        };
      });
    }

    if (!fuzzySearchFindsMatch) {
      result = false;
    }
  }

  return result;
}

function groupMatches(group, searchArray) {
  var fuzzySearchFindsMatch = false;
  for (var i = 0; i < searchArray.length; i++) {
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.OrdererGID, searchArray[i]) || Contains(group.OrdererBuilding, searchArray[i]) || Contains(group.OrdererRoom, searchArray[i]);
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.StatementName, searchArray[i]) || Contains(group.GID, searchArray[i]) || Contains(group.DepartmentName, searchArray[i]);
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.FOP, searchArray[i]) || Contains(group.RateLevel, searchArray[i]) || Contains(group.Term, searchArray[i]);
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(group.Room, searchArray[i]) || Contains(group.Building, searchArray[i]) || Contains(group.OrderNumber, searchArray[i]);

    if (angular.isDefined(group.Components)) {
      angular.forEach(group.Components, function (comp) {
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(comp.SerialNumber, searchArray[i]) || Contains(comp.LeaseTag, searchArray[i]);
      });
    }

    if (angular.isDefined(group.EOLComponents)) {
      angular.forEach(group.EOLComponents, function (comp) {
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(comp.SerialNumber, searchArray[i]) || Contains(comp.LeaseTag, searchArray[i]);
      });
    }
  }

  return fuzzySearchFindsMatch;
}

function Contains(field, searchTerm) {
  return ((field != null) && (field.toString().toLowerCase().indexOf(searchTerm.toLowerCase()) > -1));
};