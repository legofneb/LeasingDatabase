angular.module("bfApp", [])
.directive('auSelect', function ($timeout) {
  return {
    restrict: 'AE',
    template: '<select ng-Model="ngModel" class="componentType"><option ng-repeat="n in ngList" ng-selected="ngModel === n.Name" value="{{n.Name}}">{{n.Name}}</option><option></option></select>',
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
.directive("checkbox", function ($timeout) {
  return {
    restrict: 'AE',
    template: '<input ng-Model="ngModel" type="checkbox" data-toggle="checkbox" class="custom-checkbox" />',
    link: function ($scope, el, attr) {
      $timeout(function () {
        $('.custom-checkbox').radiocheck();
      }, 0);
    },
    scope: {
      ngModel: '='
    }
  }
})
.directive("typeIcon", function (rootUrl) {
  return {
    restrict: 'E',
    scope: {
      component: '=component' // component here is the string "CPU", "Laptop" etc...
    },
    link: function (scope, elem, attrs) {
      scope.$watch('component', function (newVal) {
        if (angular.isDefined(newVal)) {
          scope.type = newVal;
        }
      });

    },
    templateUrl: rootUrl + "Common/Type"
  };
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
})
.directive('editInput', function (){
  return {
    restrict: 'E',
    require: 'ngModel',
    scope: {
      editing: '=editing',
      bindModel: '=ngModel'
    },
    template: '<span ng-show="!editing">{{bindModel}}</span><span ng-show="editing"><input type="text" class="form-control" ng-model="bindModel" /></span>'
  }
})
.directive('editTextarea', function () {
  return {
    restrict: 'E',
    scope: {
      editing: '=editing',
      bindModel: '=ngModel',
      rows: '=rows'
    },
    template: '<span ng-show="!editing">{{bindModel}}</span><span ng-show="editing"><textarea rows="{{rows}}" type="text" class="form-control" ng-model="bindModel" /></span>'
  }
})
.directive('editCheckbox', function () {
  return {
    restrict: 'E',
    scope: {
      editing: '=editing',
      bindModel: '=ngModel',
    },
    template: '<span ng-show="!editing"><span ng-if="bindModel" class="glyphicon glyphicon-ok"></span><span ng-if="!bindModel" class="glyphicon glyphicon-remove"></span></span><span ng-show="editing"><input type="checkbox" ng-model="bindModel" /></span>'
  }
})
.directive('editSelect', function ($timeout) {
  return {
    restrict: 'E',
    template: '<select ng-Model="bindModel" class="editSelect"><option ng-repeat="n in ngList" ng-selected="bindModel == n.Name" value="{{n.Name}}">{{n.Name}}</option></select>',
    link: function ($scope, el, attr) {
      $timeout(function () {
        $(".editSelect").chosen({
          width: "100%",
          disable_search: true
        });
      }, 0);
    },
    scope: {
      bindModel: '=ngModel',
      ngList: '=',
    }
  }
})
.directive('editDate', function (dateFilter) {
  function link(scope, element, attrs) {
    var date;
    var startDay = scope.startDay; // use start-day="0" for end of the month

    scope.increment = function increment() {
      if (startDay == 0) {
        if (scope.ngModel.getMonth() < 11) {
          scope.ngModel = new Date(scope.ngModel.getFullYear(), scope.ngModel.getMonth() + 2, startDay);
        } else {
          scope.ngModel = new Date(scope.ngModel.getFullYear() + 1, 1, startDay);
        }
      } else {
        if (scope.ngModel.getMonth() < 11) {
          scope.ngModel = new Date(scope.ngModel.getFullYear(), scope.ngModel.getMonth() + 1, startDay);
        } else {
          scope.ngModel = new Date(scope.ngModel.getFullYear() + 1, 0, startDay);
        }
      }

      scope.date = dateFilter(scope.ngModel, 'longDate');
      
    }

    scope.decrement = function decrement() {
      if (startDay == 0) {
        scope.ngModel = new Date(scope.ngModel.getFullYear(), scope.ngModel.getMonth(), startDay);
      } else {
        if (scope.ngModel.getMonth() > 0) {
          scope.ngModel = new Date(scope.ngModel.getFullYear(), scope.ngModel.getMonth() - 1, startDay);
        } else {
          scope.ngModel = new Date(scope.ngModel.getFullYear() - 1, 11, startDay);
        }
      }

      scope.date = dateFilter(scope.ngModel, 'longDate');
      
    }

    scope.$watch(function () {
      return scope.ngModel;
    }, function (value) {
      scope.date = dateFilter(value, 'longDate');
    });

    scope.date = dateFilter(scope.ngModel, 'longDate');
  }

  return {
    restrict: 'E',
    template: '<div class="input-group"><span class="input-group-btn"><button type="button" class="btn" ng-click="decrement()"><i class="fa fa-minus"></i></button></span><span class="form-control text-center">{{date}}</span><span class="input-group-btn"><button type="button" class="btn" ng-click="increment()"><i class="fa fa-plus"></i></button></span></div>',
    link: link,
    scope: {
      ngModel: '=ngModel',
      startDay: '=startDay'
    }
  };
})
.directive('ngConfirmClick', [
  function () {
    return {
      link: function (scope, element, attr) {
        var msg = attr.ngConfirmClick || "Are you sure?";
        var clickAction = attr.confirmedClick;
        element.bind('click', function (event) {
          if (window.confirm(msg)) {  
            scope.$eval(clickAction);
          }
        });
      }
    };
  }]);