angular.module('Orders')
.config(function ($routeProvider, rootUrl) {

  $routeProvider.when("/System", {
    templateUrl: rootUrl + "NGOrders/System"
  });

  $routeProvider.otherwise({
    templateUrl: rootUrl + "NGOrders/Home"
  });
})

.controller('OrdersController', ['rootUrl', '$http', '$timeout', '$location', 'searchOrdersFilter', function (rootUrl, $http, $timeout, $location,searchOrdersFilter) {
  var self = this;
  initialize();

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

  self.setSelectedOrder = function (order) {
    self.selectedOrder = angular.copy(order);
    self.backupOrder = angular.copy(order); // meant to denote a "backup" of the selectedOrder
    self.backToMain();
  }

  self.createNewOrder = function () {
    if (self.editingOrder) {
      return;
    }

    newComponent = {
      Configuration: [{ Type: null, Make: null, Model: null}],
      Components: []
    };

    self.selectedOrder = pushElementOnArray(newComponent, self.orders);
    self.backupOrder = null;
    self.editingOrder = true;
  }

  self.submitCart = function () {
    $http.post(rootUrl + 'api/PO', { cart: self.cart, newSR: self.newSR }).
      success(function (data, status, headers, config) {
        initialize();
        self.selectedOrder = undefined;
      }).
      error(function (data, status, headers, config) {

      });
  }

  self.removeSystem = function (system) {
    self.selectedOrder.Components.splice(self.selectedOrder.Components.indexOf(system), 1);
    self.backToMain();
  }

  self.addToCart = function (order) {
    var fieldCheck = true;

    angular.forEach(self.selectedOrder.Components, function (component) {
      if (component.RateLevel == null || component.FOP == null || component.Term == null || component.RateLevel.length < 1 || component.FOP.length < 1) {
        fieldCheck = false;
      }
    });

    if (fieldCheck) {
      self.cart.push(order.id);
    } else {
      alert("The order must have RateLevel, FOP, and Term before adding to an SR.");
    }
  }

  self.clearCart = function () {
    self.cart = [];
    self.newSR = "";
  }

  self.editOrder = function () {
    self.backupOrder = angular.copy(self.selectedOrder);
    self.editingOrder = true;

  }

  self.saveOrder = function () {
    self.editingOrder = false;

    var selectedOrderId = self.selectedOrder.id;
    var selectedSystemId = self.selectedSystem.id;
    var currentPath = $location.path();

    $http.post(rootUrl + 'api/NewOrders', self.selectedOrder).
      success(function (data, status, headers, config) {

        $http.get(rootUrl + 'api/NewOrders').success(function (data) {
          self.orders = data;
          setDefaults();

          var orderIndex = -1;
          if (selectedOrderId == undefined || selectedOrderId == null) {
          } else {
            angular.forEach(self.orders, function (order) {
              if (order.id == selectedOrderId) {
                self.setSelectedOrder(order);
                orderIndex = self.orders.indexOf(order);
              }
            });

            if (currentPath == "/System" && orderIndex >= 0) {
              angular.forEach(self.selectedOrder.Components, function (component) {
                if (component.id == selectedSystemId) {
                  self.componentDetails(component);
                }
              });
            }
          }
        });
      }).
      error(function (data, status, headers, config) {
      });

  }

  self.cancelOrder = function () {
    self.editingOrder = false;
    self.$apply;

    if (self.collapseSidebar) { self.toggleSidebar(); }

    if (!angular.isDefined(self.selectedOrder) || (!angular.isDefined(self.selectedOrder.id))) {
      self.orders.pop();
      self.selectedOrder = null;
    }
    else
    {
      self.selectedOrder = self.backupOrder;
    }

    $location.path("/");
  }

  self.deleteOrder = function () {
    $http.delete(rootUrl + 'api/NewOrders/' + self.selectedOrder.id).
      success(function () {
        initialize();
      }).
      error(function () {

      });
  }

  self.addNewComponent = function (selectedOrder) {
    if (selectedOrder.Configuration.length == 0 || selectedOrder.Configuration[0].Type == null) {
      if (selectedOrder.Configuration.length == 0) {
        selectedOrder.Configuration.push({ Type: "", Make: "", Model: "" });
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
    self.componentDetails(pushElementOnArray({}, self.selectedOrder.Components));
  }

  self.generateSR = function () {
    $http.get(rootUrl + 'api/PO?condition=new').success(function (data) {
      self.newSR = data.split("\"").join(""); //removing leading and trailing quotes
    });
  }

  self.addValueToMake = function (id) {
    console.log(id);
    if ($("#" + id).find('.highlighted').length == 0) {
      var value = $("#" + id).find('input[type="text"]').val();

      var found = false;
      angular.forEach(self.makes, function (make) {
        if (make.Name == value) {
          found = true;
        }
      });
      if (!found) {
        self.makes.push({ Name: value })
      }

      self.$apply;
      $timeout(function () {

        $("#" + id + " select").val(value);
        $(".componentType").trigger('chosen:updated'); // Proposed Performance improvement: should limit this to just id Type on update
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

  self.componentDetails = function (component) {
    $location.path("/System");
    self.selectedSystem = component;
  }

  self.backToMain = function () {
    $location.path("/");
  }

  self.indexForFiltered = function () {
    orders = searchOrdersFilter(self.orders, self.searchTerm);
    var ind = -1;

    for (var i = 0; i < orders.length; i++) {
      if (orders[i].id != null && self.backupOrder != null && orders[i].id == self.backupOrder.id) {
        ind = i;
      }
    }
    return ind;
  }

  self.incrementSystem = function () {
    var currentIndex = self.selectedOrder.Components.indexOf(self.selectedSystem);
    currentIndex++;
    if (currentIndex >= self.selectedOrder.Components.length) {
      currentIndex = 0;
    }

    self.selectedSystem = self.selectedOrder.Components[currentIndex];
  }

  self.decrementSystem = function () {
    var currentIndex = self.selectedOrder.Components.indexOf(self.selectedSystem);
    currentIndex--;
    if (currentIndex < 0) {
      currentIndex = self.selectedOrder.Components.length - 1;
    }

    self.selectedSystem = self.selectedOrder.Components[currentIndex];
  }

  self.setSystem = function (index) {
    self.selectedSystem = self.selectedOrder.Components[index];
  }

  self.setFOP = function () {
    $timeout(function () {
      angular.forEach(self.selectedOrder.Components, function (system) {
        system.FOP = self.globalFOP;
      })
    }, 0);
  }

  self.setNotes = function () {
    $timeout(function () {
      angular.forEach(self.selectedOrder.Components, function (system) {
        system.Notes = self.selectedOrder.Notes;
      })
    }, 0);
  }

  self.setRateLevel = function (rateLevel) {
    angular.forEach(self.selectedOrder.Components, function (system) {
      system.RateLevel = rateLevel;
    })
  }

  function pushElementOnArray(element, arr) {
    arr.push(element);
    return arr[arr.length - 1];
  }

  function initialize() {
    $http.get(rootUrl + 'api/NewOrders').success(function (data) {
      self.orders = data;
    });

    $http.get(rootUrl + 'api/make').success(function (data) {
      self.makes = data;
      console.log(self.makes);
    });

    $http.get(rootUrl + 'api/type').success(function (data) {
      self.types = data;
    });

    $http.get(rootUrl + 'api/model').success(function (data) {
      self.models = data;
    });

    $http.get(rootUrl + 'api/operatingsystem').success(function (data) {
      self.operatingSystems = data;
    });

    setDefaults();
  };

  function setDefaults() {
    self.rateLevels = [{ Name: 'Base' }, { Name: 'Support' }];
    self.terms = [{ Name: 24 }, { Name: 36 }];

    $location.path("/");

    self.selectedOrder = undefined;
    self.backupOrder = undefined;
    self.selected = -1;
    self.editingOrder = false;
    self.cart = [];
    self.collapseSidebar = false;
    self.newSR = "";
  }

}])
.filter('searchOrders', function () {
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



    var fuzzySearchFindsMatch = Contains(order.OrdererGID, searchArray[i]) || Contains(order.OrdererGID, searchArray[i]) || Contains(order.OrdererBuilding, searchArray[i]) || Contains(order.OrdererRoom, searchArray[i]) || Contains(order.OrdererFirstName, searchArray[i]) || Contains(order.OrdererLastName, searchArray[i]);

    if (angular.isDefined(order.Components)) {
      angular.forEach(order.Components, function (component) {
        fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(component.FirstName, searchArray[i]) || Contains(component.LastName, searchArray[i]) || Contains(component.GID, searchArray[i]) || Contains(component.StatementName, searchArray[i]) || Contains(component.DepartmentName, searchArray[i]);
      });
    }

    if (!fuzzySearchFindsMatch) {
      result = false;
    }
  }

  return result;
}

function Contains(field, searchTerm) {
  return ((field != null) && (field.toLowerCase().indexOf(searchTerm.toLowerCase()) > -1));
}