using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace WebApplication.Controllers
{
    public class GenerateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateRepo()
        {
            var releaseController = new ReleaseController();
            //var result = ((ObjectResult)await releaseController.CreateRepo("PaulTest", "Enterprise%20Service%20Bus", "AlertApi")).Value;
            //var repo = (GitRepository)result;

            var build = await releaseController.CloneBuild(48, "Enterprise%20Service%20Bus");

            return new OkObjectResult("Yay");
        }

        public async Task<IActionResult> PerformMassRelease()
        {
            var releaseController = new ReleaseController();
            var result = ((ObjectResult)await releaseController.Get()).Value;
            var pipelines = (List<IO.Swagger.Models.ReleaseInfo>)result;
            var fromEnv = "SDTTEST";
            var toEnv = "SDTUAT";
            return new OkObjectResult("Disabled");
            foreach (var pipeline in pipelines)
            {

                var fromEnvironment = pipeline.Environments.SingleOrDefault(x => x.Name == fromEnv);
                var toEnvironment = pipeline.Environments.SingleOrDefault(x => x.Name == toEnv);

                if (pipeline.ProjectName == "Shared" || fromEnvironment == null || toEnvironment == null) continue;

                await releaseController.PerformMassRelease("Enterprise%20Service%20Bus", fromEnvironment.ReleaseId, toEnvironment.DefinitionEnvironmentId);
            }

            return new OkObjectResult($"Release from {fromEnv} to {toEnv} Triggered");
        }

        public async Task<IActionResult> CreateStoryDoneAPI()
        {
            var releaseController = new ReleaseController();
            var result = ((ObjectResult)await releaseController.CreateStoryDoneAPI("Enterprise%20Service%20Bus")).Value;


            return new OkObjectResult("Yay");
        }
    }
}