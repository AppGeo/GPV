//  Copyright 2012 Applied Geographics, Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

// requires Extensions.js

// loads data in the following JSON format:
//
//   {
//     headers: [],     list of column name strings, converted to TH elements, optional
//     rows: [          list of row objects, converted to TR elements
//       {
//         id: {},      each row object must have an ID, can be any type, returned as selection value
//         v: []        list of row values, any type, converted to TD elements, numbers right-aligned
//       },
//       ...
//     ]
//   }
//

(function ($) {
  $.fn.dataGrid = function (method) {
    var key = "dataGrid";
    var clickHandle = null;

    // =====  private functions (shared)  =====

    function init($target, options) {
      var settings = {
        alternateClass: null,
        multiSelect: false,
        rowClass: "__DataGridRow",
        selectedClass: "__DataGridSelect",
        selectionChanged: [],
        stringCompare: function (a, b) { return a < b ? -1 : a > b ? 1 : 0; }
      };

      if (options) {
        $.extend(settings, options);

        if (typeof settings.selectionChanged == "function") {
          settings.selectionChanged = [settings.selectionChanged];
        }
      }

      settings.staticHeader = $target.find("thead").length > 0;
      settings.lastSort = -1;

      if (!settings.staticHeader) {
        $target.append("<thead><tr/></thead>");
      }

      $target.find("thead").on("click", "th", sortRows);
      $target.append("<tbody>");
      $target.data(key, settings);
    }

    function bindRowData($tr, data) {
      $tr.empty().on("mousedown", preventDefault);

      $.each(data, function (i, v) {
        var right = false;

        if (v == null) {
          v = "";
        }
        else {
          var type = v.getDate ? "date" : typeof v;

          switch (type) {
            case "string":
              var d = Date.fromJson(v);

              if (d) {
                data[i] = d;
                v = d.format();
              }
              else {
                right = v.isNumeric();

                if (!right) {
                  v = v.replace("\n", "<br/>");
                }
              }
              break;

            case "number": right = true; break;
            case "date": v = v.format(); break;
          }
        }

        var align = right ? " align='right'" : "";
        $("<td" + align + " unselectable='on'/>").text(v).appendTo($tr);
      });
    }

    function clearSelection($target) {
      var selectedClass = $target.data(key).selectedClass;
      $target.find("tbody tr." + selectedClass).removeClass(selectedClass);
    }

    function deleteSelection($target) {
      var settings = $target.data(key);
      $target.find("tbody tr." + settings.selectedClass).remove();
      setAlternating($target);
    }

    function empty($target) {
      if (!$target.data(key).staticHeader) {
        $target.find("thead tr").empty();
      }

      $target.find("tbody").empty();
    }

    function getAlternateClass(settings) {
      return settings.alternateClass ? settings.alternateClass : settings.rowClass;
    }

    function getData($target, id) {
      var $tr = getRowById($target, id);
      return $tr == null ? null : $tr.data(key).v;
    }

    function getIds($target) {
      return $target.find("tbody tr").map(function () { return $(this).data(key).id }).get();
    }

    function getRowById($target, id) {
      var $trs = $target.find("tbody tr");

      for (var i = 0; i < $trs.length; ++i) {
        var $tr = $trs.eq(i);

        if ($tr.data(key).id == id) {
          return $tr;
        }
      }

      return null;
    }

    function getSelectedRows($target) {
      return $target.find("tbody tr." + $target.data(key).selectedClass);
    }

    function getSelection($target) {
      var selection = [];

      getSelectedRows($target).each(function () {
        selection.push($(this).data(key).id);
      });

      return selection;
    }

    function load($target, data) {
      var settings = $target.data(key);
      var altClass = getAlternateClass(settings);

      if (!settings.staticHeader) {
        var $head = $target.find("thead tr").empty();

        if (data.headers && data.rows.length) {
          $.each(data.headers, function () {
            $("<th/>").text(this).appendTo($head);
          });
        }
      }

      var $tbody = $target.find("tbody").empty();

      $.each(data.rows, function (i, row) {
        var $tr = $("<tr/>").appendTo($tbody).data(key, row).addClass(i % 2 == 0 ? altClass : settings.rowClass).on("click dblclick", rowClick);
        bindRowData($tr, row.v);
      });
    }

    function onSelectionChanged($target, dblClick) {
      clickHandle = null;

      $.each($target.data(key).selectionChanged, function (i, fn) {
        fn(dblClick);
      });
    }

    function preventDefault(e) {
      e.preventDefault();
    }

    function rowClick(e) {
      var $row = $(this).closest("tr");
      var $target = $row.closest("table");
      var settings = $target.data(key);

      if (e.type == "click") {
        if (!clickHandle) {
          var changed = true;
          var $selectedRows = getSelectedRows($target);

          if (settings.multiSelect && e.shiftKey && $selectedRows.length > 0) {
            var cur = $row.index(), startMin, startMax;
            var min = startMin = $selectedRows.first().index();
            var max = startMax = $selectedRows.last().index();

            if (cur <= min) {
              min = cur;
            }
            else if (cur >= max) {
              max = cur;
            }
            else if (cur - min <= max - cur) {
              min = cur;
            }
            else {
              max = cur;
            }

            changed = min != startMin || max != startMax || max + 1 - min != $selectedRows.length;

            if (changed) {
              clearSelection($target);
              $target.find("tbody tr").slice(min, max + 1).addClass(settings.selectedClass);
            }
          }
          else if (settings.multiSelect && e.ctrlKey) {
            $row.toggleClass(settings.selectedClass);
          }
          else {
            changed = $selectedRows.length != 1 || !$row.hasClass(settings.selectedClass);

            if (changed) {
              clearSelection($target);
              $row.addClass(settings.selectedClass);
            }
          }

          if (changed) {
            clickHandle = setTimeout(function () {
              if (clickHandle) {
                onSelectionChanged($target, false);
              }
            }, 500);
          }
        }
      }
      else {
        onSelectionChanged($target, true);
      }
    }

    function selectedChanged($target, fn) {
      $target.data(key).selectedChanged.push(fn);
    }

    function setAlternating($target) {
      var settings = $target.data(key);
      var altClass = getAlternateClass(settings);
      var $rows = $target.find("tbody tr").removeClass(settings.rowClass + " " + altClass);
      $rows.filter(":even").addClass(altClass);
      $rows.filter(":odd").addClass(settings.rowClass);
    }

    function setData($target, id, data) {
      var $tr = getRowById($target, id);

      if ($tr) {
        $tr.data(key).v = data;
        bindRowData($tr, data);
      }
    }

    function setSelection($target, fn) {
      var settings = $target.data(key);

      $target.find("tbody tr").removeClass(settings.selectedClass).each(function () {
        var $row = $(this);

        if (fn($row.data(key).id)) {
          $row.addClass(settings.selectedClass);
        }
      });
    }

    function sortRows() {
      var $th = $(this);
      var column = $th.index();
      var $target = $th.parents("table").eq(0);
      var $tbody = $target.find("tbody");
      var settings = $target.data(key);
      var dir = 1;

      if (column == settings.lastSort) {
        var dir = -1;
        settings.lastSort = -1;
      }
      else {
        settings.lastSort = column;
      }

      var rows = $tbody.find("tr").detach().map(function () {
        return { data: $(this).data(key), elem: this };
      });

      rows.sort(function (a, b) {
        var aValue = a.data.v[column];
        var bValue = b.data.v[column];
        var aType = Object.getType(aValue);
        var bType = Object.getType(bValue);

        if (aType == "null" || bType == "null") {
          return aType == "null" ? (bType == "null" ? 0 : dir) : -dir;
        }

        if (aType != bType) {
          return aType < bType ? -dir : dir;
        }

        if (aType == "string") {
          return dir * settings.stringCompare(aValue, bValue);
        }
        else {
          return aValue < bValue ? -dir : aValue > bValue ? dir : 0;
        }
      });

      $.each(rows, function () {
        $tbody.append(this.elem);
      });

      setAlternating($target);
    }

    // =====  public methods  =====

    var methods = {
      init: function (options) {
        return this.each(function () { init($(this), options); });
      },

      clearSelection: function () {
        return this.each(function () { clearSelection($(this)); });
      },

      deleteSelection: function () {
        return this.each(function () { deleteSelection($(this)); });
      },

      empty: function () {
        return this.each(function () { empty($(this)); });
      },

      getData: function (id) {
        return getData($(this), id);
      },

      getIds: function () {
        return getIds($(this));
      },

      getSelection: function () {
        return getSelection($(this));
      },

      load: function (data) {
        return this.each(function () { load($(this), data); });
      },

      selectionChanged: function (fn) {
        return this.each(function () { selectionChanged($(this), fn); });
      },

      setData: function (id, data) {
        return setData($(this), id, data);
      },

      setSelection: function (fn) {
        return this.each(function () { setSelection($(this), fn); });
      }
    };

    // =====  initialization and method execution  =====

    if (methods[method]) {
      return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
    }
    else if (typeof method === "object" || !method) {
      return methods.init.apply(this, arguments);
    }
    else {
      $.error("Method " + method + " does not exist in " + key);
    }
  };
})(jQuery);