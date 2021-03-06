﻿angular.module("bfApp", [])
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
});