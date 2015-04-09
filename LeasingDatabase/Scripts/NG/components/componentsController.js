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
    self.selected = self.orders.indexOf(order);
    self.editingOrder = false;
    self.selectedSystem = undefined;

    self.selectedOrder = angular.copy(order);
    self.backupOrder = angular.copy(order);
    
    console.log(self.selectedOrder);
    var EndDate = self.selectedOrder.SystemGroups[0].Components[0].BillingData[0].EndDate; // Grab First End Date to reference the default values

    self.globalChangeFOPDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    self.globalBuyOutEndLeaseDate = new Date(EndDate.getFullYear(), EndDate.getMonth(), 0);
    self.globalBuyOutDate = new Date(EndDate.getFullYear(), EndDate.getMonth(), 0);
    self.globalBuyOutAmount = 0;
    self.globalBuyOutArray = [];
    self.globalExtendMonths = 1;
    self.globalFOP = "";
    self.globalCharge = "";
    self.globalBillingAction = 'FOP';

    $location.path('/');
  }

  self.editOrder = function () {
    self.editingOrder = true;
  }

  self.saveOrder = function () {
    self.editingOrder = false;

    var order = convertDatesToTicksForSelectedOrder(self.selectedOrder);

    $http.post(rootUrl + 'api/Components', order).
      success(function (data, status, headers, config) {
        alert("The Order Has Been Saved!");
        self.searchForComponents();
      }).
      error(function (data, status, headers, config) {
      })
  }

  self.cancelOrder = function () {
    self.editingOrder = false;
    self.selectedOrder = angular.copy(self.backupOrder);
    $location.path('/');
  }

  self.unfinalizeOrder = function (id) {
    $http.delete(rootUrl + 'api/Components/' + id).success(function (data) {
      self.selectedOrder = undefined;
      self.selected = -1;
      self.searchForComponents();
    });
  }

  self.setSelectedSystem = function (system) {
    $location.path('/System');
    self.selectedSystem = system;;
  }

  self.setSelectedComponent = function (component) {
    $location.path('/Component');
    self.selectedComponent = component;
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

  self.addBillingItem = function (billingData) {
    billingData.unshift({
      BeginDate: billingData[0].BeginDate.getMonth() < 11 ? new Date(billingData[0].EndDate.getFullYear(), billingData[0].EndDate.getMonth()+ 1, 1) : new Date(billingData[0].EndDate.getFullYear() + 1, 0, 1),
      EndDate: billingData[0].EndDate.getMonth() < 11 ? new Date(billingData[0].EndDate.getFullYear(), billingData[0].EndDate.getMonth() + 2, 0) : new Date(billingData[0].EndDate.getFullYear() + 1, 1, 0),
      StatementName: billingData[0].StatementName,
      ContractNumber: billingData[0].ContractNumber,
      FOP: billingData[0].FOP,
      RateLevel: billingData[0].RateLevel,
      MonthlyCharge: billingData[0].MonthlyCharge
    });
  }

  self.removeBillingItem = function (billingData, billingItem) {
    if (billingData.length == 1) {
      alert("Illegal Operation: Must have at least 1 Billing Item");
      return;
    }

    var ind = billingData.indexOf(billingItem);
    billingData.splice(ind, 1);
  }

  self.setBillingAction = function (action) {
    self.globalBillingAction = action;
    console.log(self.globalBillingAction);
  }

  self.isBillingAction = function (action) {
    return action == self.globalBillingAction;
  }

  self.setContractNumber = function (order, contractNumber) {
    order.SystemGroups.forEach(function (system) {
      system.Components.forEach(function (component) {
        component.BillingData.forEach(function (bill) {
          bill.ContractNumber = contractNumber;
        })
      })
    })
  }

  self.changeFOPForComponent = function (component, FOP, EffectiveDate) {
    component.BillingData.unshift({
      BeginDate: EffectiveDate,
      EndDate: component.BillingData[0].EndDate,
      StatementName: component.BillingData[0].StatementName,
      ContractNumber: component.BillingData[0].ContractNumber,
      FOP: FOP,
      RateLevel: component.BillingData[0].RateLevel,
      MonthlyCharge: component.BillingData[0].MonthlyCharge
    });

    component.BillingData[1].EndDate = new Date(EffectiveDate.getFullYear(), EffectiveDate.getMonth(), 0);
  }

  self.changeFOPForSystem = function (system, FOP, EffectiveDate) {
    system.Components.forEach(function (component) {
      self.changeFOPForComponent(component, FOP, EffectiveDate);
    })
  }

  self.changeFOPForOrder = function (order, FOP, EffectiveDate) {
    order.SystemGroups.forEach(function (system) {
      self.changeFOPForSystem(system, FOP, EffectiveDate);
    });
  }
  
  self.buyOutComponent = function (component, buyOutAmount, lastLeaseDate, buyOutDate, FOP) {

    component.BillingData[0].EndDate = lastLeaseDate;

    component.BillingData.unshift({
      BeginDate: new Date(buyOutDate.getFullYear(), buyOutDate.getMonth(), 1),
      EndDate: buyOutDate,
      StatementName: component.BillingData[0].StatementName,
      ContractNumber: component.BillingData[0].ContractNumber,
      FOP: (FOP != null && FOP.length > 5) ? FOP : component.BillingData[0].FOP,
      RateLevel: component.BillingData[0].RateLevel,
      MonthlyCharge: buyOutAmount
    });

    component.ReturnDate = null;
  }

  self.buyOutSystem = function (system, buyOutAmount, lastLeaseDate, buyOutDate, FOP) {
    for (var i = 0; i < system.Components.length; i++) {
      self.buyOutComponent(system.Components[i], buyOutAmount[i], lastLeaseDate, buyOutDate, FOP)
    }
  }

  self.buyOutOrder = function (order, buyOutAmount, lastLeaseDate, buyOutDate, FOP) {
    order.SystemGroups.forEach(function (system) {
      self.buyOutSystem(system, buyOutAmount, lastLeaseDate, buyOutDate, FOP);
    });
  }

  self.extendComponent = function(component, ExtendByMonths) {
    var years = Math.floor(ExtendByMonths/ 12 );
    var months = ExtendByMonths % 12;

    component.BillingData.unshift({
      BeginDate: component.BillingData[0].BeginDate.getMonth() < 11 ? new Date(component.BillingData[0].EndDate.getFullYear(), component.BillingData[0].EndDate.getMonth()+ 1, 1) : new Date(component.BillingData[0].EndDate.getFullYear() + 1, 0, 1),
      EndDate: component.BillingData[0].EndDate.getMonth() + months < 12 ? new Date(component.BillingData[0].EndDate.getFullYear() + years, component.BillingData[0].EndDate.getMonth() + 1 + months, 0) : new Date(component.BillingData[0].EndDate.getFullYear() + 1 + years, months + component.BillingData[0].EndDate.getMonth() - 11, 0),
      StatementName: component.BillingData[0].StatementName,
      ContractNumber: component.BillingData[0].ContractNumber,
      FOP: component.BillingData[0].FOP,
      RateLevel: component.BillingData[0].RateLevel,
      MonthlyCharge: component.BillingData[0].MonthlyCharge
    });

    component.ReturnDate = component.BillingData[0].EndDate.getMonth() < 10 ? new Date(component.BillingData[0].EndDate.getFullYear(), component.BillingData[0].EndDate.getMonth() + 2, 0) : new Date(component.BillingData[0].EndDate.getFullYear() + 1, component.BillingData[0].EndDate.getMonth() - 10  , 0)
  }

  self.extendSystem = function (system, ExtendByMonths) {
    system.Components.forEach(function (component) {
      self.extendComponent(component, ExtendByMonths);
    })
  }

  self.extendOrder = function (order, ExtendByMonths) {
    order.SystemGroups.forEach(function (system) {
      self.extendSystem(system, ExtendByMonths);
    });
  }

  self.backToMain = function () {
    $location.path("/");
  }

  self.backToSystem = function () {
    $location.path("/System");
  }

  self.getForm = function (SR) {
    window.location.href = rootUrl + "SR/Index?SRs=" + SR;
  }

  self.moreDetails = function (id) {
    var URL = rootUrl + 'Component/BillingPopUp' + "?Id=" + id;
    window.open(URL, "Summary", 'height=500,width=500');
  }

  self.retrieveNextPage = function () {
    if (self.requestInAction == 0) {
      self.requestInAction = 1;
      var currentPageNumber = self.orders.length / 100;

      $http.get(rootUrl + 'api/Components/?lastPageNumber=' + currentPageNumber + '&filteredTerms=' + encodeURIComponent(self.searchTerm)).success(function (data) {
        data = convertTicksToDatesForData(data);

        self.orders = self.orders.concat(data);
        self.requestInAction = 0;
      });
    }
  }

  self.requestInAction = 0;

  self.searchForComponents = function () {
    self.editingOrder = false;
    self.selectedOrder = undefined;

    if (self.requestInAction == 0) {
      self.orders = undefined;
      self.requestInAction = 1;
      $http.get(rootUrl + 'api/Components/?lastPageNumber=0&filteredTerms=' + encodeURIComponent(self.searchTerm)).success(function (data) {
        data = convertTicksToDatesForData(data);

        self.orders = data;
        self.requestInAction = 0;
      });
    }
  };

  self.loading = function () {
    return !angular.isObject(self.orders);
  }

  self.componentMatches = function (component) {
    if (self.searchTerm == null || self.searchTerm.length < 1) {
      return false;
    }

    searchArray = self.searchTerm.match(/\w+|"(?:\\"|[^"])+"/g);

    for (var i = 0; i < searchArray.length; i++) {
      searchArray[i] = searchArray[i].replace(/['"]+/g, '');
    }

    return componentMatches(component, searchArray);
  }

  function initialize() {
    $http.get(rootUrl + 'api/Components').success(function (data) {
      data = convertTicksToDatesForData(data);

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

    self.editingOrder = false;

    $location.path("/");
  };

  function convertTicksToDatesForData(data) {
    if (data.length > 0) {
      data.forEach(function (SR) {
        SR.SystemGroups.forEach(function (group) {
          group.Components.forEach(function (component) {
            if (component.ReturnDate != null) {
              var returnDate = new Date(component.ReturnDate);
              component.ReturnDate = new Date(returnDate.getFullYear(), returnDate.getMonth(), returnDate.getDate()); // trying to ignore UTC variations
            }
            component.BillingData.forEach(function (bill) {
              var beginDate = new Date(bill.BeginDate)
              bill.BeginDate = new Date(beginDate.getFullYear(), beginDate.getMonth(), beginDate.getDate());
              var endDate = new Date(bill.EndDate);
              bill.EndDate = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate());
            });
          });
        });
      });
    }

    return data;
  }

  function convertDatesToTicksForSelectedOrder(order) {
    // we don't want to modify the objects, angular might throw an error on one of the date objects
    var returnOrder = angular.copy(order);

    returnOrder.SystemGroups.forEach(function (group) {
      group.Components.forEach(function (component) {
        if (component.ReturnDate != null) {
          component.ReturnDate = (component.ReturnDate.getTime() * 10000) + 621355968000000000;
        }
        component.BillingData.forEach(function (bill) {
          bill.BeginDate = (bill.BeginDate.getTime() * 10000) + 621355968000000000; // https://stackoverflow.com/questions/7966559/how-to-convert-javascript-date-object-to-ticks
          bill.EndDate = (bill.EndDate.getTime() * 10000) + 621355968000000000;
        });
      });
    });

    return returnOrder;
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