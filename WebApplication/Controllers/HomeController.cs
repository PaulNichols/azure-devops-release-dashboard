using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using static WebApplication.Models.ReleaseEnvironmentInfo;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string environment = null, int? definitionId = null)
        {
            var releaseController = new ReleaseController();
            var result = (ObjectResult)await releaseController.Get(environment, definitionId);
            var dashboard = ToDashboard(result);

            Response.Headers.Add("Refresh", "30");
            return View(dashboard);
        }

        public async Task<IActionResult> Compare(string from = "preprod", string to = "prod")
        {
            var releaseController = new ReleaseController();
            var result = (ObjectResult)await releaseController.Compare(from, to);
            var dashboard = ToDashboard(result);

            Response.Headers.Add("Refresh", "30");
            return View(dashboard);
        }

        private Dashboard ToDashboard(ObjectResult result)
        {
            var releases = (List<IO.Swagger.Models.ReleaseInfo>)result.Value;

            var dashboard = new Dashboard() { Projects = new List<Project>() };

            foreach (var release in releases)
            {
                dashboard.Projects.Add(new Project
                {
                    ProjectName = release.ProjectName,
                    Environments = release.Environments.ConvertAll(x =>
                    new ReleaseEnvironmentInfo
                    {
                        Name = x.Name,
                        Status = x.Status == null ? StatusEnum.Undefined : (StatusEnum)Enum.Parse(typeof(StatusEnum), x.Status.ToString()),
                        Version = x.Version,
                        When = x.When,
                        Who = x.Who,
                        Rank = x.Rank,
                        ReleaseId = x.ReleaseId,
                    }),
                });
            }

            dashboard.Projects = dashboard.Projects.OrderBy(x => x.ProjectName).ToList();

            return dashboard;
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
