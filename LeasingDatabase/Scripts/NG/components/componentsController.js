angular.module('Components')
.config(function ($routeProvider, rootUrl) {

  $routeProvider.when("/Component", {
    templateUrl: rootUrl + "NGComponents/Component"
  });

  $routeProvider.otherwise({
    templateUrl: rootUrl + "NGComponents/Home"
  });
})
.controller('ComponentsController', ['rootUrl', '$http', '$timeout','$location', function (rootUrl, $http, $timeout, $location) {
  var self = this;
  initialize();

  self.setSelectedOrder = function (order, index) {
    self.editingOrder = false;
    self.selectedSystem = undefined;
    self.selectedRow = undefined;
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

  self.setSelectedComponent = function (component) {
    $location.path('/Component');
    self.selectedComponent = component;
    self.getBilling();
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

  self.backToMain = function () {
    $location.path("/");
  }

  self.backToSystem = function () {
    $location.path("/System");
  }

  self.retrieveNextPage = function () {
    if (self.requestInAction == 0) {
      self.requestInAction = 1;
      var currentPageNumber = self.orders.length / 100;

      $http.get(rootUrl + 'api/Components/?lastPageNumber=' + currentPageNumber + '&filteredTerms=' + encodeURIComponent(self.searchTerm)).success(function (data) {
        self.orders = self.orders.concat(data);
        self.requestInAction = 0;
      });
    }
  }

  self.requestInAction = 0;

  self.searchForComponents = function () {
    if (self.requestInAction == 0) {
      self.requestInAction = 1;
      $http.get(rootUrl + 'api/Components/?lastPageNumber=0&filteredTerms=' + encodeURIComponent(self.searchTerm)).success(function (data) {
        self.orders = data;
        self.requestInAction = 0;
      });
    }
  }

  self.getBilling = function () {
    $http.get(rootUrl + 'api/ComponentBilling/' + self.selectedComponent.id).success(function (data) {
      self.billingData = data;
    });
  }

  self.loading = function () {
    return !angular.isObject(self.orders);
  }

  self.componentMatches = function (component) {
    if (self.searchTerm == null || self.searchTerm.length < 1) {
      return false;
    }

    var searchArray = self.searchTerm.split(" ");
    return componentMatches(component, searchArray);
  }

  function initialize() {
    $http.get(rootUrl + 'api/Components').success(function (data) {
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

    self.selected = -1;
    self.selectedRow = -1;
    self.editingOrder = false;
    self.cart = [];

    self.addEOLSystem = { text: undefined };

    $location.path("/");
  };

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
.directive('scrollTrigger', function ($window) {
  return {
    link: function (scope, element, attrs) {
      var offset = parseInt(attrs.threshold) || 0;
      var e = jQuery(element[0]);
      var doc = jQuery(document);
      angular.element(document).bind('scroll', function () {
        if (doc.scrollTop() + $window.innerHeight + offset > e.offset().top) {
          scope.$apply(attrs.scrollTrigger);
        }
      });
    }
  };
});

function componentMatches(component, searchArray) {
  var fuzzySearchFindsMatch = false;
  for (var i = 0; i < searchArray.length; i++) {
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(component.StatementName, searchArray[i]) || Contains(component.GID, searchArray[i]) || Contains(component.DepartmentName, searchArray[i]);
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(component.FOP, searchArray[i]) || Contains(component.RateLevel, searchArray[i]) || Contains(component.Term, searchArray[i]);
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(component.Room, searchArray[i]) || Contains(component.Building, searchArray[i]) || Contains(component.OrderNumber, searchArray[i]);
    fuzzySearchFindsMatch = fuzzySearchFindsMatch || Contains(component.SerialNumber, searchArray[i]) || Contains(component.LeaseTag, searchArray[i]);
  }

  return fuzzySearchFindsMatch;
}

function Contains(field, searchTerm) {
  return ((field != null) && (field.toString().toLowerCase().indexOf(searchTerm.toLowerCase()) > -1));
}