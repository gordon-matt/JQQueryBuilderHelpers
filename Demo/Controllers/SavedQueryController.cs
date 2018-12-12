using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.Data.Domain;
using Extenso;
using Extenso.Data.Entity;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("saved-queries")]
    public class SavedQueryController : Controller
    {
        private readonly Lazy<IRepository<SavedQuery>> repository;

        public SavedQueryController(Lazy<IRepository<SavedQuery>> repository)
        {
            this.repository = repository;
        }

        [Route("")]
        public IActionResult Index(string entityType)
        {
            ViewBag.EntityType = entityType;
            return View("Index");
        }

        [HttpPost]
        [Route("query/{entityType}")]
        public IActionResult Query(string entityType, [FromBody]KendoGridMvcRequest request)
        {
            using (var connection = repository.Value.OpenConnection())
            {
                var type = EnumExtensions.Parse<SavedQueryEntityType>(entityType);

                var query = connection.Query(x => x.Type == type)
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Name);

                var grid = new KendoGrid<SavedQuery>(request, query);
                return Json(grid);
            }
        }

        [HttpGet]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await repository.Value.DeleteAsync(x => x.Id == id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await repository.Value.FindOneAsync(id);
            return View(product);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(SavedQuery model)
        {
            var product = await repository.Value.FindOneAsync(model.Id);
            product.Name = model.Name;
            //product.Query = model.Query;

            await repository.Value.UpdateAsync(product);

            return Json(new { status = true, redirectUrl = Url.Action("Index") });
        }
    }
}