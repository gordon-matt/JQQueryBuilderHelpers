'use strict'

var ViewModel = function () {
    var self = this;

    self.apiUrl = "/odata/PersonApi";
    
    self.validator = false;

    self.id = ko.observable(0);
    self.familyName = ko.observable(null);
    self.givenNames = ko.observable(null);
    self.dateOfBirth = ko.observable(null);

    self.init = function () {
        currentSection = $("#grid-section");
        
        self.validator = $("#form-section-form").validate({
            rules: {
                FamilyName: { required: true, maxlength: 128 },
                GivenNames: { required: true, maxlength: 128 },
                DateOfBirth: { required: true, date: true }
            }
        });

        $("#grid").kendoGrid({
            data: null,
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: "/people/query",
                        type: "POST",
                        dataType: "json"
                    }
                },
                schema: {
                    data: 'data',
                    total: 'total',
                    model: {
                        id: 'id',
                        fields: {
                            familyName: { type: "string" },
                            givenNames: { type: "string" },
                            dateOfBirth: { type: "date" }
                        }
                    }
                },
                pageSize: 10,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: [
                    { field: "familyName", dir: "asc" },
                    { field: "givenNames", dir: "asc" }
                ]
            },
            dataBound: function (e) {
                var body = this.element.find("tbody")[0];
                if (body) {
                    ko.cleanNode(body);
                    ko.applyBindings(ko.dataFor(body), body);
                }
            },
            filterable: true,
            sortable: {
                allowUnsort: false
            },
            pageable: {
                refresh: true
            },
            scrollable: false,
            columns: [{
                field: "familyName",
                title: "Family Name"
            }, {
                field: "givenNames",
                title: "Given Name/s"
            }, {
                field: "dateOfBirth",
                title: "Date of Birth",
                type: "date",
                format: "{0:yyyy-MM-dd}",
                filterable: false,
                width: 200
            }, {
                field: "id",
                title: "&nbsp;",
                template:
                    '<div class="btn-group">' +
                        '<button type="button" data-bind="click: edit.bind($data,\'#=id#\')" class="btn btn-secondary btn-sm" title="Edit"><i class="fas fa-edit"></i></button>' +
                        '<button type="button" data-bind="click: remove.bind($data,\'#=id#\')" class="btn btn-danger btn-sm" title="Delete"><i class="fas fa-times"></i></button>' +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });

        $("#DateOfBirth").kendoDatePicker({
            start: "decade",
            format: "yyyy-MM-dd"
        });

        $.get('/people/get-query-config', function (json) {
            $('#query-builder').queryBuilder(json);

            $('#query-builder').on('getSQLFieldID.queryBuilder.filter', function (e) {
                switch (e.value) {
                    case "FamilyName": e.value = "family-name"; break;
                    case "GivenNames": e.value = "given-names"; break;
                    case "DateOfBirth": e.value = "date-of-birth"; break;
                    default: break;
                }
            });
        });
    };

    self.create = function () {
        self.id(0);
        self.familyName(null);
        self.givenNames(null);
        self.dateOfBirth(null);

        self.validator.resetForm();
        switchSection($("#form-section"));
        $("#form-section-legend").html("Create");
    };

    self.edit = function (id) {
        $.ajax({
            url: self.apiUrl + "(" + id + ")",
            type: "GET",
            dataType: "json",
            async: false
        })
        .done(function (json) {
            self.id(json.Id);
            self.familyName(json.FamilyName);
            self.givenNames(json.GivenNames);
            self.dateOfBirth(json.DateOfBirth);

            switchSection($("#form-section"));
            $("#form-section-legend").html("Edit");
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $.notify({ message: "Error when trying to retrieve record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            console.log(textStatus + ': ' + errorThrown);
        });
    };

    self.remove = function (id) {
        if (confirm("Are you sure that you want to delete this record?")) {
            $.ajax({
                url: self.apiUrl + "(" + id + ")",
                type: "DELETE",
                async: false
            })
            .done(function (json) {
                self.refreshGrid();
                $.notify({ message: "Successfully deleted record!", icon: 'fa fa-check' }, { type: 'success' });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify({ message: "Error when trying to delete record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    };

    self.save = function () {
        var isNew = (self.id() == 0);

        if (!$("#form-section-form").valid()) {
            return false;
        }

        var record = {
            Id: self.id(),
            FamilyName: self.familyName(),
            GivenNames: self.givenNames(),
            DateOfBirth: self.dateOfBirth()
        };

        if (isNew) {
            $.ajax({
                url: self.apiUrl,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.refreshGrid();
                switchSection($("#grid-section"));
                $.notify({ message: "Successfully inserted record!", icon: 'fa fa-check' }, { type: 'success' });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify({ message: "Error when trying to insert record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                console.log(textStatus + ': ' + errorThrown);
            });
        }
        else {
            $.ajax({
                url: self.apiUrl + "(" + self.id() + ")",
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.refreshGrid();
                switchSection($("#grid-section"));
                $.notify({ message: "Successfully updated record!", icon: 'fa fa-check' }, { type: 'success' });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify({ message: "Error when trying to update record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    };

    self.cancel = function () {
        switchSection($("#grid-section"));
    };

    self.refreshGrid = function () {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    };
    
    //=================================================================================================
    //  Query Builder
    //=================================================================================================

    self.runQuery = function () {
        var result = $('#query-builder').queryBuilder('getSQL', false);

        var grid = $('#grid').data('kendoGrid');
        grid.dataSource.transport.options.read.url = '/people/execute-query';
        grid.dataSource.transport.options.read.contentType = "application/json; charset=utf-8";
        grid.dataSource.transport.parameterMap = function (options, operation) {
            if (operation === "read") {
                var page = grid.dataSource.page();

                return JSON.stringify({
                    Query: result.sql,
                    Skip: (page - 1) * 10,
                    Take: 10
                });
            }
        };
        grid.dataSource.page(1);
    };

    self.resetQuery = function () {
        $('#query-builder').queryBuilder('reset');

        var grid = $('#grid').data('kendoGrid');
        grid.dataSource.transport.options.read.url = '/people/query';
        grid.dataSource.transport.options.read.contentType = 'application/x-www-form-urlencoded; charset=UTF-8';
        grid.dataSource.transport.parameterMap = function (options, operation) {
            return options;
        };
        grid.dataSource.page(1);
    };

    self.loadQuery = function () {
        var queryId = $('#SelectedQuery').val();
        $.get("/people/load-query/" + queryId, function (json) {
            $('#Query_Name').val(json.name);
            $('#query-builder').queryBuilder('setRulesFromSQL', json.query);
        });
    }

    self.openSaveQueryDialog = function () {
        $("#saveQueryModal").modal("show");
        $('#Query_Name').focus();
    };

    self.saveQuery = function () {
        var queryName = $('#Query_Name').val();

        var result = $('#query-builder').queryBuilder('getSQL', false);

        $.ajax({
            url: "/people/save-query",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                name: queryName,
                query: result.sql
            }),
            dataType: "json",
            async: false
        })
        .done(function (json) {
            if (!json || !json.success) {
                alert(json.message);
                return;
            }

            if (json.isNew) {
                $('#SelectedQuery')
                    .append($("<option></option>")
                        .attr("value", json.queryId)
                        .text(json.name));
            }
            alert('Saved!');
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus + ': ' + errorThrown);
        });

        $("#saveQueryModal").modal("hide");
    };

    //=================================================================================================
};