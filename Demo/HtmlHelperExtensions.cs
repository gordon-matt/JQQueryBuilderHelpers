using System.Linq;
using Demo.Data;
using Demo.Data.Domain;
using Extenso.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo
{
    public static class HtmlHelperExtensions
    {
        public static ThisApp<TModel> ThisApp<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return new ThisApp<TModel>(htmlHelper);
        }
    }

    public class ThisApp<TModel>
    {
        private readonly IHtmlHelper<TModel> html;

        internal ThisApp(IHtmlHelper<TModel> html)
        {
            this.html = html;
        }

        public IHtmlContent SavedQueryDropDownList(string name, SavedQueryEntityType entityType, string selectedValue = null, object htmlAttributes = null, string emptyText = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(Startup.Configuration.GetConnectionString("DefaultConnection"));
            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                var queries = context.SavedQueries.Where(x => x.Type == entityType).ToList();

                var selectList = queries
                    .Select(x => new { x.Id, x.Name })
                    .ToSelectList(
                        value => value.Id,
                        text => text.Name,
                        selectedValue,
                        emptyText);

                return html.DropDownList(name, selectList, htmlAttributes);
            }
        }
    }
}