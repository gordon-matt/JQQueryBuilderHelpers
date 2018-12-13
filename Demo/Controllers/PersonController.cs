using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Data.Dapper;
using Demo.Data.Domain;
using Extenso.Data.Entity;
using JQQueryBuilderHelpers;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Demo.Controllers
{
    [Route("people")]
    public class PersonController : Controller, IJQQueryBuilderController
    {
        private readonly Lazy<IRepository<Person>> personRepository;
        private readonly Lazy<IRepository<SavedQuery>> savedQueryRepository;
        private readonly Lazy<IDapperRepository<Person, int>> personDapperRepository;

        public PersonController(Lazy<IRepository<Person>> personRepository,
            Lazy<IRepository<SavedQuery>> savedQueryRepository,
            Lazy<IDapperRepository<Person, int>> personDapperRepository)
        {
            this.personRepository = personRepository;
            this.savedQueryRepository = savedQueryRepository;
            this.personDapperRepository = personDapperRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [Route("query")]
        public IActionResult Query(KendoGridMvcRequest request)
        {
            using (var connection = personRepository.Value.OpenConnection())
            {
                var query = connection.Query()
                    .OrderBy(x => x.FamilyName)
                    .ThenBy(x => x.GivenNames);

                var grid = new KendoGrid<Person>(request, query);
                return Json(grid);
            }
        }

        [HttpPost]
        [Route("execute-query")]
        public IActionResult ExecuteQuery([FromBody] JQQueryBuilderRawQuery query)
        {
            try
            {
                int count = personDapperRepository.Value.Count(query.Query);
                if (count > 0)
                {
                    var data = personDapperRepository.Value.Find(query.Query, query.Skip, query.Take);
                    var grid = new KendoGrid<Person>(data, count);
                    return Json(grid);
                }
                else
                {
                    return Json(new KendoGrid<Person>(Enumerable.Empty<Person>(), 0));
                }
            }
            catch (Exception x)
            {
                return Json(new { Success = false, Message = x.GetBaseException().Message });
            }
        }

        #region IJQQueryBuilderController Members

        [HttpGet]
        [Route("get-query-config")]
        public IActionResult GetQueryConfig()
        {
            var jqQueryBuilderConfig = new JQQueryBuilderConfig
            {
                //Plugins = JQQueryBuilderConfig.DefaultPlugins.Value,
                Plugins = new Dictionary<string, object>
                {
                    { "sortable", null },
                    //{ "filter-description", null },
                    { "unique-filter", null },
                    //{ "bt-tooltip-errors", null },
                    //{ "bt-selectpicker", null },
                    //{ "bt-checkbox", null },
                    { "invert", null },
                    { "not-group", null },
                    { "sql-support", JObject.FromObject(new { boolean_as_integer = false }) }
                },
                Filters = new List<JQQueryBuilderFilter>
                {
                    new JQQueryBuilderFilter { Id = "family-name", Field= "\"FamilyName\"" , Label = "Family Name", Operators = JQQueryBuilderConfig.ShortTextOperatorTypes.Value},
                    new JQQueryBuilderFilter { Id = "given-names", Field = "\"GivenNames\"", Label = "Given Name/s", Operators = JQQueryBuilderConfig.ShortTextOperatorTypes.Value },

                    new JQQueryBuilderFilter
                    {
                        Id = "date-of-birth",
                        Field = "\"DateOfBirth\"",
                        Label = "Date of Birth",
                        Operators = JQQueryBuilderConfig.DateTimeOperatorTypes.Value,
                        Plugin = "datepicker",
                        PluginConfig = new
                        {
                            format = "yyyy-mm-dd",
                            todayBtn = "linked",
                            todayHighlight = true,
                            autoclose = true
                        }
                    }
                }
            };
            return Json(jqQueryBuilderConfig);
        }

        [HttpGet]
        [Route("load-query/{key}")]
        public async Task<IActionResult> LoadQuery(Guid key)
        {
            var savedQuery = await savedQueryRepository.Value.FindOneAsync(key);
            return Json(savedQuery);
        }

        [HttpPost]
        [Route("save-query")]
        public async Task<IActionResult> SaveQuery([FromBody] JQQueryBuilderSavedQuery query)
        {
            try
            {
                var savedQueryEntityType = SavedQueryEntityType.Person;

                var savedQuery = await savedQueryRepository.Value.FindOneAsync(x => x.Type == savedQueryEntityType && x.Name == query.Name);

                var isNew = false;
                if (savedQuery == null)
                {
                    isNew = true;
                    savedQuery = new SavedQuery
                    {
                        Type = savedQueryEntityType,
                        Name = query.Name,
                        Query = query.Query,
                    };
                    await savedQueryRepository.Value.InsertAsync(savedQuery);
                }
                else
                {
                    savedQuery.Query = query.Query;
                    await savedQueryRepository.Value.UpdateAsync(savedQuery);
                }

                return Json(new { Success = true, QueryId = savedQuery.Id, Name = savedQuery.Name, IsNew = isNew });
            }
            catch (Exception x)
            {
                return Json(new { Success = false, Message = x.GetBaseException().Message });
            }
        }

        #endregion IJQQueryBuilderController Members
    }
}